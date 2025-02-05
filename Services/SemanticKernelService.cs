using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Newtonsoft.Json;
using NJsonSchema;
using OpenAI.Chat;

namespace ArchitectNow.SemanticKernelDemo.Services;

public interface ISemanticKernelService
{
    Task<string> GenerateCompletionAsync(string systemPrompt, string userPrompt,
        string aiDeployment = "");
    
    Task<ChatHistory> GenerateCompletionAsync(ChatHistory chatHistory, string userPrompt,
        string aiDeployment = "");

    Task<T> GenerateDataAsync<T>(string prompt,
        string aiDeployment = "");
}

public class SemanticKernelService : ISemanticKernelService
{
    private readonly Kernel _kernel;
    private readonly string _aiDeployment;
    private readonly IChatCompletionService _chatCompletionService;

    private readonly OpenAIPromptExecutionSettings _settings = new()
    {
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
        Temperature = .1
    };

    public SemanticKernelService(Kernel kernel, IConfiguration configuration)
    {
        _kernel = kernel;
        _settings.Temperature = configuration.GetValue<double>("OpenAi:Temperature");
        _aiDeployment = configuration.GetValue<string>("OpenAi:OpenAILatestGptDeployment");
        _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
    }

    public async Task<string> GenerateCompletionAsync(string systemPrompt, string userPrompt,
        string aiDeployment = "")
    {
        ArgumentException.ThrowIfNullOrEmpty(systemPrompt);
        ArgumentException.ThrowIfNullOrEmpty(userPrompt);

        if (string.IsNullOrEmpty(aiDeployment))
        {
            aiDeployment = _aiDeployment;
        }

        var chatHistory = new ChatHistory(systemPrompt);

        chatHistory.AddUserMessage(userPrompt);

        var completionResults = await _chatCompletionService.GetChatMessageContentAsync(chatHistory, _settings, _kernel);
        var answer = completionResults.Content;

        return answer!;
    }

    public async Task<ChatHistory> GenerateCompletionAsync(ChatHistory chatHistory, string userPrompt,
        string aiDeployment = "")
    {
        if (chatHistory == null)
        {
            chatHistory =
                new ChatHistory("You are a helpful assistant in an AI demo showcasing the .NET semantic kernel");
        }
        
        chatHistory.AddUserMessage(userPrompt);
        
        if (string.IsNullOrEmpty(aiDeployment))
        {
            aiDeployment = _aiDeployment;
        }
        
        var completionResults = await _chatCompletionService.GetChatMessageContentAsync(chatHistory, _settings, _kernel);

        if (completionResults.Metadata != null && completionResults.Metadata.Any())
        {
            // Debug.WriteLine($"Tokens: {completionResults.}");
        };
        chatHistory.AddAssistantMessage(completionResults.Content);
        
        return chatHistory;
    }

    [Experimental("SKEXP0010")]
    public async Task<T> GenerateDataAsync<T>(string prompt,
        string aiDeployment = "")
    {
        var schema = JsonSchema.FromType<T>();
        var schemaJson = schema.ToJson();
        
        ChatResponseFormat chatResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
            jsonSchemaFormatName: "object",
            jsonSchema: BinaryData.FromString(schemaJson),
            jsonSchemaIsStrict: false);
        
        var executionSettings = new OpenAIPromptExecutionSettings
        {
            ResponseFormat = chatResponseFormat
        };
        
        var functionResult = await _kernel.InvokePromptAsync(prompt, new(executionSettings));
        
        var jsonResult = functionResult.GetValue<string>();
        
        return JsonConvert.DeserializeObject<T>(jsonResult);
    }
}
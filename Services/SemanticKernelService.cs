using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace ArchitectNow.SemanticKernelDemo.Services;

public interface ISemanticKernelService
{
    Task<string> GenerateCompletionAsync(string systemPrompt, string userPrompt,
        string aiDeployment = "");
    
    Task<ChatHistory> GenerateCompletionAsync(ChatHistory chatHistory, string userPrompt,
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
        
        chatHistory.AddAssistantMessage(completionResults.Content);
        
        return chatHistory;
    }
}
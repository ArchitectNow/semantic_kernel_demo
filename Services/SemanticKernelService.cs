using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace ArchitectNow.SemanticKernelDemo.Services;

public interface ISemanticKernelService
{
    Task<string> GenerateCompletionAsync(string systemPrompt, string userPrompt,
        string aiDeployment = "");
}

public class SemanticKernelService : ISemanticKernelService
{
    private readonly Kernel _kernel;
    private readonly string _aiDeployment;

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
        
        var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

        var chatHistory = new ChatHistory(systemPrompt);

        chatHistory.AddUserMessage(userPrompt);

        var completionResults = await chatCompletionService.GetChatMessageContentAsync(chatHistory, _settings, _kernel);
        var answer = completionResults.Content;

        return answer!;
    }
}
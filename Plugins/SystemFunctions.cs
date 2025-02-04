using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace ArchitectNow.SemanticKernelDemo.Plugins;

public interface ISystemFunctions
{
    Task ClearCache();
}

public class SystemFunctions : ISystemFunctions
{
    
    [KernelFunction]
    [Description("Call this method to clear all items currently cached by the application.")]
    public async Task ClearCache()
    {
        //TODO:  Clear cache....
    }
    
    
    
}
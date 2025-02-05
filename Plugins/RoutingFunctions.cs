using System.ComponentModel;
using System.Reflection;
using Microsoft.AspNetCore.Components;
using Microsoft.SemanticKernel;

namespace ArchitectNow.SemanticKernelDemo.Plugins;

public interface IRoutingFunctions
{
    string GetCurrentRoute();
    void NavigateTo(string url);
}

public class RoutingFunctions : IRoutingFunctions
{
    private readonly NavigationManager _navigationManager;

    public RoutingFunctions(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
    }

    [KernelFunction]
    [Description(
        "Returns the URL for the current page that the user is viewing within the application.  When asked about 'this screen' or 'this page' use this function to get the route")]
    public string GetCurrentRoute()
    {
        return _navigationManager.Uri;
    }

    [KernelFunction]
    [Description("Navigates the application to a new page")]
    public void NavigateTo(string url)
    {
        _navigationManager.NavigateTo(url);
    }

    [KernelFunction]
    [Description("Gets a list of all routes registered in this application")]
    public List<string> GetRoutes()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        var routes = new List<string>();

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(ComponentBase)) && t.GetCustomAttribute<RouteAttribute>() != null);

            foreach (var type in types)
            {
                var routeAttributes = type.GetCustomAttributes<RouteAttribute>();
                foreach (var route in routeAttributes)
                {
                    routes.Add(route.Template);
                }
            }
        }

        return routes;
    }
}
// See https://aka.ms/new-console-template for more information

using ArchitectNow.SemanticKernelDemo.Components;
using ArchitectNow.SemanticKernelDemo.Plugins;
using ArchitectNow.SemanticKernelDemo.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using MudBlazor;
using MudBlazor.Services;

namespace ArchitectNow.SemanticKernelDemo;

public class Program
{
    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);
        
        builder.WebHost.UseStaticWebAssets();
        builder.Services.AddOptions();
        builder.Services.AddLogging();
        builder.Services.AddRazorPages();

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        
        builder.Services.AddMudServices(config =>
        {
            config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;

            config.SnackbarConfiguration.PreventDuplicates = false;
            config.SnackbarConfiguration.NewestOnTop = false;
            config.SnackbarConfiguration.ShowCloseIcon = true;
            config.SnackbarConfiguration.VisibleStateDuration = 10000;
            config.SnackbarConfiguration.HideTransitionDuration = 500;
            config.SnackbarConfiguration.ShowTransitionDuration = 500;
            config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
        });
        
        builder.Services.AddScoped<Kernel>(serviceProvider =>
        {
            Kernel k = new(serviceProvider);

            var chartingPlugin = serviceProvider.GetRequiredService<IChartFunctions>();
            k.Plugins.AddFromObject(chartingPlugin);

            var systemFunctions = serviceProvider.GetRequiredService<ISystemFunctions>();
            k.Plugins.AddFromObject(systemFunctions);
            
            var routingFunctions = serviceProvider.GetRequiredService<IRoutingFunctions>();
            k.Plugins.AddFromObject(routingFunctions);

            return k;
        });

        builder.Services.AddAzureOpenAIChatCompletion(builder.Configuration["OpenAI:OpenAILatestGptDeployment"],
            builder.Configuration["OpenAI:OpenAIEndpoint"], builder.Configuration["OpenAI:OpenAIKey"]);
        
        builder.Services.AddScoped<ISemanticKernelService, SemanticKernelService>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IChartFunctions, ChartFunctions>();
        builder.Services.AddScoped<ISystemFunctions, SystemFunctions>();
        builder.Services.AddScoped<IRoutingFunctions, RoutingFunctions>();

        var app = builder.Build();

        app.UseStaticFiles();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }
        


        app.UseHttpsRedirection();
        app.MapControllers();
        app.UseAntiforgery();

        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
        
    }

}
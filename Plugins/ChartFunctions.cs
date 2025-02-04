using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace ArchitectNow.SemanticKernelDemo.Plugins;

public interface IChartFunctions
{
    Task<string> GetChartImageUrl(string jsonConfig);
}

public class ChartFunctions : IChartFunctions
{
    [KernelFunction]
    [Description(
        "This method is used to generate a url to a chart generated via QuickChart.io. All configuration of the chart and data will be sent in via a Json string.  The returned Url can be rendered like an image. When rendering a chart image via the Url into chat wrap it in an anchor tag that opens the chart in a new window if clicked. When returning markdown render all charts as images wrapped in anchor tags that open the resulting image in a new tab. Wrap all chart images in anchor tags that open the resulting image in a new tab.")]
    public async Task<string> GetChartImageUrl(string jsonConfig)
    {
        // Construct the QuickChart URL with the chart configuration as a URL-encoded parameter
        return "https://quickchart.io/chart?c={Uri.EscapeDataString(jsonConfig)}\";";
    }
}
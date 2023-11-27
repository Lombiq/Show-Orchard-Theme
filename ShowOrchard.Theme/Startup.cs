using Lombiq.HelpfulExtensions.Extensions.OrchardRecipeMigration.Services;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Modules;
using ShowOrchard.Theme.Services;

namespace ShowOrchard.Theme;

public class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IOrchardContentConverter, WebsiteOrchardContentConverter>();
        services.AddScoped<IOrchardContentConverter, CategoryOrchardContentConverter>();
        services.AddScoped<IOrchardExportConverter, CategoryOrchardExportConverter>();
        services.AddScoped<IOrchardExportConverter, ScreenshotOrchardExportConverter>();
        services.AddScoped<IOrchardExportConverter, CreatedUtcAdjustingOrchardExportConverter>();
    }
}

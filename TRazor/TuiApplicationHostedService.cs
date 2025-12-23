using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Hosting;
using Terminal.Gui.App;
using TRazor.Core;

namespace TRazor;

public class TuiApplicationHostedService<TAppComponent> : IHostedService
    where TAppComponent : IComponent
{
    private readonly IApplication _tuiApp;
    private readonly TuiRenderer _renderer;
    private readonly IHostApplicationLifetime _appLifetime;

    public TuiApplicationHostedService(IApplication tuiApp, TuiRenderer renderer, IHostApplicationLifetime appLifetime)
    {
        _tuiApp = tuiApp;
        _renderer = renderer;
        _appLifetime = appLifetime;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _tuiApp.Init();
        await _renderer.StartApplication<TAppComponent>();
        _appLifetime.StopApplication();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // _tuiApp.RequestStop();
        _tuiApp.Dispose();

        return Task.CompletedTask;
    }
}

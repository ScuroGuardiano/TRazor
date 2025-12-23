using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Terminal.Gui.App;
using TRazor.Core;

namespace TRazor;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public void AddTRazor<TAppComponent>()
            where TAppComponent : IComponent
        {
            IApplication tuiApp = Application.Create();
            services.TryAddSingleton(Dispatcher.CreateDefault());
            services.AddSingleton(tuiApp);
            services.AddSingleton<TuiRenderer>();

            services.AddHostedService<TuiApplicationHostedService<TAppComponent>>();
        }
    }
}

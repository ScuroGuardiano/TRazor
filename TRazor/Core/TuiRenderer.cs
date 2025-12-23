using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.Extensions.Logging;
using Terminal.Gui.App;
using Terminal.Gui.Views;
using IComponent = Microsoft.AspNetCore.Components.IComponent;
using Window = TRazor.Components.Window;

namespace TRazor.Core;

#pragma warning disable BL0006
public class TuiRenderer : Renderer
{
    private int? _root;
    private readonly Dictionary<int, TuiComponentAdapter> _components = new();
    private readonly IApplication _tuiApp;

    protected override RendererInfo RendererInfo { get; } = new RendererInfo("TuiRenderer", true);

    public TuiRenderer(
        IServiceProvider serviceProvider,
        ILoggerFactory loggerFactory,
        Dispatcher dispatcher,
        IApplication tuiApp) : base(serviceProvider, loggerFactory)
    {
        Dispatcher = dispatcher;
        _tuiApp = tuiApp;
    }

    public async Task<TRootComponent> StartApplication<TRootComponent>() where TRootComponent : IComponent
    {
        return await Dispatcher.InvokeAsync(async () =>
        {
            if (_root is not null)
            {
                throw new InvalidOperationException("Application is already started");
            }

            var root = (Window)InstantiateComponent(typeof(Window));
            var cid = AssignRootComponentId(root);

            var windowAdapter = new TuiComponentAdapter(this, null, root)
            {
                Name = "Root Window"
            };

            RegisterComponent(cid, windowAdapter);
            _root = cid;

            var parameters = new Dictionary<string, object?>();


            var app = (TRootComponent)InstantiateComponent(typeof(TRootComponent));

            parameters["ChildContent"] = (RenderFragment)Rf;

            RenderRootComponentAsync(cid, ParameterView.FromDictionary(parameters));
            _tuiApp.Run(root.Win);

            return app;

            void Rf(RenderTreeBuilder builder)
            {
                builder.OpenComponent(0, typeof(TRootComponent));
                builder.CloseComponent();
            }
        });
    }

    protected override void HandleException(Exception exception)
    {
        MessageBox.ErrorQuery(_tuiApp, "Exception thrown", exception.ToString(), 0);
    }

    protected override Task UpdateDisplayAsync(in RenderBatch renderBatch)
    {
        for (int i = 0; i < renderBatch.UpdatedComponents.Count; i++)
        {
            var update = renderBatch.UpdatedComponents.Array[i];

            if (update.Edits.Count > 0)
            {
                var adapter = _components[update.ComponentId];
                adapter.ApplyEdits(update.ComponentId, update.Edits, in renderBatch);
            }
        }

        return Task.CompletedTask;
    }


    public void RegisterComponent(int componentId, TuiComponentAdapter component)
    {
        _components[componentId] = component;
    }

    public void RemoveRootComponent()
    {
        if (_root is not null)
        {
            RemoveRootComponent(_root.Value);
            _root = null;
        }
    }

    public void RemoveComponent(int componentId)
    {
        _components.Remove(componentId);
    }

    public override Dispatcher Dispatcher { get; }
}
#pragma warning restore BL0006

using System.ComponentModel;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Terminal.Gui.App;
using Terminal.Gui.Views;
using IComponent = Microsoft.AspNetCore.Components.IComponent;

namespace BlazorTuiTests.Core;

#pragma warning disable BL0006
public class TuiRenderer : Renderer
{
    private int? _root;
    private readonly Dictionary<int, TuiComponentAdapter> _components = new();
    private IApplication _tuiApp;

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

    public async Task<TComponent> AddRootComponent<TComponent>(Dictionary<string, object>? parameters = null) where TComponent : IComponent
    {
        return await Dispatcher.InvokeAsync(async () =>
        {
            if (_root is not null)
            {
                throw new InvalidOperationException("Root component already exists");
            }

            var c = (TComponent)InstantiateComponent(typeof(TComponent));
            var cid = AssignRootComponentId(c);

            var cAdapter = new TuiComponentAdapter(this, null, c)
            {
                Name = "Root"
            };

            RegisterComponent(cid, cAdapter);
            _root = cid;
            await RenderRootComponentAsync(cid,
                parameters?.Count > 0 ? ParameterView.FromDictionary(parameters) : ParameterView.Empty);
            return c;
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

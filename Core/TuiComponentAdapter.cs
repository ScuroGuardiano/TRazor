using Microsoft.AspNetCore.Components.RenderTree;
using IComponent = Microsoft.AspNetCore.Components.IComponent;

namespace BlazorTuiTests.Core;

#pragma warning disable BL0006

public class TuiComponentAdapter
{
    public TuiComponentAdapter(TuiRenderer renderer, TuiComponentAdapter? parent, IComponent component)
    {
        Renderer = renderer;
        Parent = parent;
        _component = component;
    }

    public string Name { get; internal set; } = "TuiComponentAdapter";

    public int DeepLevel { get; init; }

    protected TuiComponentAdapter? Parent { get; private set; }

    protected TuiRenderer Renderer { get; private set; }

    protected List<TuiComponentAdapter> Children { get; } = [];

    private IComponent _component;

    public void ApplyEdits(int componentId, ArrayBuilderSegment<RenderTreeEdit> edits, in RenderBatch renderBatch)
    {
        foreach (var e in edits)
        {
            HandleEdit(renderBatch, e);
        }
    }

    private void HandleEdit(in RenderBatch renderBatch, RenderTreeEdit edit)
    {
        switch (edit.Type)
        {
            case RenderTreeEditType.PrependFrame:
                HandlePrependFrame(in renderBatch, edit);
                break;

            case RenderTreeEditType.UpdateText:
                HandleUpdateText(in renderBatch, edit);
                break;

            case RenderTreeEditType.RemoveFrame:
                HandleRemoveFrame(in renderBatch, edit);
                break;

            default:
                throw new NotImplementedException();
        }

        // Console.WriteLine($" Sibling Index: {edit.SiblingIndex}, Move to sibling idx {edit.MoveToSiblingIndex}");
    }

    private void HandleRemoveFrame(in RenderBatch renderBatch, RenderTreeEdit edit)
    {
        // Console.WriteLine($"Removing component: {Children[edit.SiblingIndex]}");
        var c = Children[edit.SiblingIndex];
        Children.RemoveAt(edit.SiblingIndex);

        if (_component is TuiControlComponentBase tc && c._component is TuiControlComponentBase tcc)
        {
            tc.RemoveChild(tcc);
        }
    }

    private void HandlePrependFrame(in RenderBatch renderBatch, in RenderTreeEdit edit)
    {
        var r = renderBatch.ReferenceFrames.Array[edit.ReferenceFrameIndex];
        switch (r.FrameType)
        {
            case RenderTreeFrameType.Component:
                // Console.WriteLine($"Adding Component: {r.Component.GetType().FullName} ({r})");

                var newC = new TuiComponentAdapter(Renderer, this, r.Component)
                {
                    Name = "XD",
                    DeepLevel = DeepLevel + 1
                };

                Children.Insert(edit.SiblingIndex, newC);
                Renderer.RegisterComponent(r.ComponentId, newC);

                if (_component is TuiControlComponentBase tc && r.Component is TuiControlComponentBase tcc)
                {
                    tc.AddChild(tcc);
                }

                break;
            case RenderTreeFrameType.Text:
                // Console.WriteLine($"Adding Text: {r.TextContent} ({r})");
                HandleUpdateText(in renderBatch, edit);
                break;
            case RenderTreeFrameType.Markup:
                // Console.WriteLine($"Adding Markup: {r.MarkupContent} ({r})");
                HandleUpdateText(in renderBatch, edit);
                break;

            default:
                throw new NotImplementedException();
        }
    }

    protected void HandleUpdateText(in RenderBatch renderBatch, in RenderTreeEdit edit)
    {
        if (_component is ITuiTextControl tc)
        {
            tc.HandleTextUpdate(renderBatch.ReferenceFrames.Array[edit.ReferenceFrameIndex].TextContent);
        }
        else
        {
            Children.Insert(edit.SiblingIndex, null!);
        }
    }
}

using Microsoft.AspNetCore.Components;
using Terminal.Gui.ViewBase;

namespace BlazorTuiTests.Core;

public abstract class TuiControlComponentBase : ComponentBase, ITuiContainer, IDisposable
{
    public abstract View View { get; }

    [Parameter]
    public Pos X
    {
        get => View.X;
        set => View.X = value;
    }

    [Parameter]
    public Pos Y
    {
        get => View.Y;
        set => View.Y = value;
    }

    [Parameter]
    public Dim Width
    {
        get => View.Width;
        set => View.Width = value;
    }

    [Parameter]
    public Dim Height
    {
        get => View.Height;
        set => View.Height = value;
    }

    public void AddChild(TuiControlComponentBase child)
    {
        View.Add(child.View);
    }

    public void RemoveChild(TuiControlComponentBase child)
    {
        View.Remove(child.View);
    }

    public virtual void Dispose()
    {
        View.Dispose();
    }

    protected async Task HandleEventAsync(Func<Task> action)
    {
        await InvokeAsync(action);
    }

    protected void HandleEvent(Func<Task> action)
    {
        AwaitVoid(HandleEventAsync(action));
    }

    protected async void AwaitVoid(Task task)
    {
        try
        {
            await task;
        }
        catch (Exception e)
        {
            HandleException(e);
        }
    }

    private void HandleException(Exception ex)
    {
        DispatchExceptionAsync(ex);
    }
}

using Terminal.Gui.ViewBase;

namespace BlazorTuiTests.Views;

public interface ITuiViewContainer
{
    public void AddChild(View child);
    public void RemoveChild(View child);
}

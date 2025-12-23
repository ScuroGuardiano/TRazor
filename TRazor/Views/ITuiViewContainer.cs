using Terminal.Gui.ViewBase;

namespace TRazor.Views;

public interface ITuiViewContainer
{
    public void AddChild(View child);
    public void RemoveChild(View child);
}

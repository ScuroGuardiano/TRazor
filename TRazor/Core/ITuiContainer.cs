namespace TRazor.Core;

public interface ITuiContainer
{
    public void AddChild(TuiControlComponentBase child);
    public void RemoveChild(TuiControlComponentBase child);
}

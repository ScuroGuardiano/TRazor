using Terminal.Gui.ViewBase;

namespace BlazorTuiTests.Views;

public class FlowLayoutView : View
{
    public override void BeginInit()
    {
        base.BeginInit();
        Width = Dim.Fill();
        Height = Dim.Fill();
        CanFocus = true;
    }

    protected override void OnSubViewLayout(LayoutEventArgs args)
    {
        base.OnSubViewLayout(args);
        RecalculateLayout();
    }

    private void RecalculateLayout()
    {
        View? prev = null;
        foreach (var child in SubViews)
        {
            child!.Y = prev is not null ? Pos.Bottom(prev) : 0;
            prev = child;
        }
    }
}

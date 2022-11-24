using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using binstarjs03.AerialOBJ.WpfAppNew.Components.Interfaces;
using binstarjs03.AerialOBJ.WpfAppNew.ViewModel;

namespace binstarjs03.AerialOBJ.WpfAppNew.View;

public partial class ViewportControl : UserControl, IViewportView
{
    public ViewportControl()
    {
        InitializeComponent();
        DataContext = new ViewportControlVM(this);
    }

    public new void Focus()
    {
        ViewportPanel.Focus();
        Keyboard.Focus(ViewportPanel);
    }

    public void AddToCanvas(UIElement uiElement)
    {
        ViewportCanvas.Children.Add(uiElement);
    }

    public bool HasCanvasItem(UIElement uiElement)
    {
        return ViewportCanvas.Children.Contains(uiElement);
    }

    public void RemoveFromCanvas(UIElement uiElement)
    {
        ViewportCanvas.Children.Remove(uiElement);
    }

    public void UpdateCanvasItemPosition(UIElement uiElement, Point newPosition)
    {
        Canvas.SetLeft(uiElement, newPosition.X);
        Canvas.SetTop(uiElement, newPosition.Y);
    }

    public Size GetScreenSize()
    {
        return new Size(ViewportPanel.ActualWidth, ViewportPanel.ActualHeight);
    }
}

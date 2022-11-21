using System.Windows;

namespace binstarjs03.AerialOBJ.WpfAppNew.Components.Interfaces;

public interface IViewportView
{
    void Focus();
    void AddToCanvas(UIElement uiElement);
    void RemoveFromCanvas(UIElement uiElement);
    bool HasCanvasItem(UIElement uiElement);
    void UpdateCanvasItemPosition(UIElement uiElement, Point newPosition);
}

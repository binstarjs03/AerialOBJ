using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace binstarjs03.CubeOBJ.WpfApp.UIElements.Controls;
public class ViewportControlVM : ViewModelBase<ViewportControlVM, ViewportControl>
{
    public ViewportControlVM(ViewportControl control) : base(control) { }

    
    
    #region Data Binders

    private int _exportArea1X;
    private int _exportArea1Y;
    private int _exportArea1Z;
    public int ExportArea1X
    {
        get => _exportArea1X;
        set => SetAndNotifyPropertyChanged(value, ref _exportArea1X);
    }
    public int ExportArea1Y
    {
        get => _exportArea1Y;
        set => SetAndNotifyPropertyChanged(value, ref _exportArea1Y);
    }
    public int ExportArea1Z
    {
        get => _exportArea1Z;
        set => SetAndNotifyPropertyChanged(value, ref _exportArea1Z);
    }



    private int _exportArea2X;
    private int _exportArea2Y;
    private int _exportArea2Z;
    public int ExportArea2X
    {
        get => _exportArea2X;
        set => SetAndNotifyPropertyChanged(value, ref _exportArea2X);
    }
    public int ExportArea2Y
    {
        get => _exportArea2Y;
        set => SetAndNotifyPropertyChanged(value, ref _exportArea2Y);
    }
    public int ExportArea2Z
    {
        get => _exportArea2Z;
        set => SetAndNotifyPropertyChanged(value, ref _exportArea2Z);
    }



    private double _cameraPosX;
    private double _cameraPosZ;
    public double CameraPosX
    {
        get => _cameraPosX;
        set => SetAndNotifyPropertyChanged(value, ref _cameraPosX);
    }
    public double CameraPosZ
    {
        get => _cameraPosZ;
        set => SetAndNotifyPropertyChanged(value, ref _cameraPosZ);
    }



    private int _zoomLevel = 2;
    public int ZoomLevel
    {
        get => _zoomLevel;
        set => SetAndNotifyPropertyChanged(value, ref _zoomLevel);
    }

    #endregion
}

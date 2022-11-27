using System;

namespace binstarjs03.AerialOBJ.Core.Visualization;
internal interface INotifyPropertyChanged
{
    event Action<string> PropertyChanged;

}

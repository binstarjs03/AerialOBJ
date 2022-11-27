using System;

namespace binstarjs03.AerialOBJ.Core;
internal interface INotifyPropertyChanged
{
    event Action<string> PropertyChanged;

}

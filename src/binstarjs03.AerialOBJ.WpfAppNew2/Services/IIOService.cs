using System;

namespace binstarjs03.AerialOBJ.WpfAppNew2.Services;
public interface IIOService
{
    bool WriteText(string path, string content, out Exception? e);
}

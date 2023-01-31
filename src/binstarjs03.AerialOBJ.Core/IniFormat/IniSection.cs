using System.Collections.Generic;

namespace binstarjs03.AerialOBJ.Core.IniFormat;
public class IniSection
{
    public IniSection()
    {
        Properties = new Dictionary<string, string>();
    }

    public Dictionary<string, string> Properties { get; }
}
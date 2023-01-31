using System.Collections.Generic;

namespace binstarjs03.AerialOBJ.Core.IniFormat;

public class IniDocument
{
    public IniDocument()
    {
        RootSection = new IniSection();
        Subsections = new Dictionary<string, IniSection>();
    }

    public IniSection RootSection { get; }
    public Dictionary<string, IniSection> Subsections { get; }
}

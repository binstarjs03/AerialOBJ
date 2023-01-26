using System.IO;

namespace binstarjs03.AerialOBJ.Core.IniFormat;

public static class IniSerializing
{
    public static string Serialize(IniDocument document)
    {
        StringWriter writer = new();
        IniSection root = document.RootSection;
        WriteRootSection(root, writer);
        foreach ((string sectionName, IniSection section) in document.Subsections)
            WriteSection(section, writer, sectionName);
        return writer.ToString();
    }

    private static void WriteRootSection(IniSection section, StringWriter writer)
    {
        WriteSectionProperties(section, writer);
        if (section.Properties.Count != 0)
            writer.WriteLine();
    }

    private static void WriteSectionProperties(IniSection section, StringWriter writer)
    {
        foreach ((string key, string value) in section.Properties)
        {
            writer.Write(key);
            writer.Write(" = ");
            writer.WriteLine(value);
        }
    }

    private static void WriteSection(IniSection section, StringWriter writer, string sectionName)
    {
        writer.Write('[');
        writer.Write(sectionName);
        writer.WriteLine(']');
        WriteSectionProperties(section, writer);
        writer.WriteLine();
    }
}
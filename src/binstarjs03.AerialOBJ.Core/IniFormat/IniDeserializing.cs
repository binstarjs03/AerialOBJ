﻿using System;
using System.IO;
using System.Linq;

namespace binstarjs03.AerialOBJ.Core.IniFormat;

public static class IniDeserializing
{
    public static IniDocument Deserialize(string input)
    {
        StringReader reader = new(input);
        IniDocument document = new();
        IniSection currentSection = document.RootSection;

        while (reader.Peek() != -1)
        {
            string? line = reader.ReadLine();
            if (line is null)
                break;

            line = StripComment(line).TrimEnd();
            if (string.IsNullOrWhiteSpace(line))
                continue;

            if (line[0] == '[') // section declaration
            {
                string sectionName = ReadSectionDefinition(line);
                IniSection section = new();
                document.Subsections.Add(sectionName, section);
                currentSection = section;
            }
            else
            {
                (string key, string value) = ReadKeyValueDefinition(line);
                currentSection.Properties.Add(key, value);
            }
        }

        return document;
    }

    private static string ReadSectionDefinition(string line)
    {
        if (line[^1] != ']')
            throw new IniInvalidSectionException();
        string identifier = line[1..(line.Length - 1)];
        ValidateString(identifier);
        return identifier;
    }

    private static (string key, string value) ReadKeyValueDefinition(string line)
    {
        int equalSignCount = line.Count(ch => ch == '=');
        if (equalSignCount != 1)
            throw new IniInvalidKeyValueException();
        string[] keyValue = line.Split('=', StringSplitOptions.TrimEntries);
        ValidateString(keyValue[0]);
        ValidateString(keyValue[1], true);
        return (keyValue[0], keyValue[1]);
    }

    private static void ValidateString(string stringval, bool isKeyValue = false)
    {
        int length = stringval.Length;
        for (int i = 0; i < length; i++)
        {
            char ch = stringval[i];
            if (i == 0 || i == length - 1)
            {
                if (!isKeyValue)
                {
                    if (!char.IsLetter(ch)
                        && !char.IsDigit(ch))
                        throw new IniInvalidStringException();
                }
                else
                    if (!char.IsLetter(ch)
                        && ch != '.'
                        && ch != ' '
                        && ch != '-'
                        && ch != '_'
                        && !char.IsDigit(ch))
                    throw new IniInvalidStringException();
            }
            if (!char.IsLetter(ch)
                && ch != '.'
                && ch != ' '
                && ch != '-'
                && ch != '_'
                && !char.IsDigit(ch))
                throw new IniInvalidStringException();
        }
    }

    private static string StripComment(string line)
    {
        ReadOnlySpan<char> lineSpan = line.AsSpan();
        int commentPos = lineSpan.IndexOfAny(';', '#');
        if (commentPos == -1)
            return line;
        return line[..commentPos];
    }
}
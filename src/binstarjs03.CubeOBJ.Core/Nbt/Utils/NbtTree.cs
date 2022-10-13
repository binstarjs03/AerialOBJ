using System.Collections.Generic;
using System.Text;

namespace binstarjs03.AerialOBJ.Core.Nbt.Utils;

// TODO: use stack based connectors that will be shared between all Compile call.
// Current implementation is to return new List<string of Connector> each successive Compile call
// Issue: this will cause excessive GC pressure and deallocation a lot, excessive heap allocation
public class NbtTree
{
    private readonly StringBuilder _sb;
    private readonly NbtBase _nbt;
    //private readonly Stack<ConnectorType> _connectors;
    private bool _hasCompiled;

    private enum ConnectorType
    {
        Connect,
        Branch,
        Last,
        Space,
    }

    private class Connector
    {
        public static readonly string Connect = "│  ";
        public static readonly string Branch = "├──";
        public static readonly string Last = "└──";
        public static readonly string Space = "   ";
    }

    public NbtTree(NbtBase nbt)
    {
        _sb = new();
        _nbt = nbt;
        //_connectors = new();
        _hasCompiled = false;
    }

    public string Compile()
    {
        if (!_hasCompiled)
        {
            Compile(_nbt, new(), string.Empty, true, false, true);
            _hasCompiled = true;
        }
        return _sb.ToString();
    }

    private void Compile(NbtBase nbt, List<string> otherConnectors, string connector, bool isParentLast, bool isInsideList, bool isRoot = false)
    {
        foreach (string otherConnector in otherConnectors)
            _sb.Append(otherConnector);
        _sb.Append($"{connector}{DotOrDash(nbt, isRoot)}");
        if (!isInsideList)
        {
            _sb.Append($"<{nbt.NbtType}> {nbt.Name} = ");
        }

        switch (nbt.NbtTypeBase)
        {
            case NbtTypeBase.NbtContainerType:
                compileContainerType(this, (NbtContainerType)nbt, otherConnectors, isParentLast, isRoot);
                break;
            case NbtTypeBase.NbtArrayType:
                compileArrayType(this, (NbtArrayType)nbt, otherConnectors, isParentLast, isRoot);
                break;
            case NbtTypeBase.NbtSingleValueType:
                compileSingleValueType(this, (NbtSingleValueType)nbt);
                break;
            default:
                break;
        }

        static void compileContainerType(NbtTree tree, NbtContainerType nbt, List<string> otherConnectors, bool isParentLast, bool isRoot)
        {
            if (nbt is NbtCompound)
                tree._sb.AppendLine($"{nbt.ValueCount} tag(s)");
            if (nbt is NbtList nbtList)
            {
                NbtType? listType = nbtList.ListType;
                if (listType is null)
                    tree._sb.AppendLine($"{nbtList.ValueCount} tag (empty list tag)");
                else
                    tree._sb.AppendLine($"{nbtList.ValueCount} tag(s) of {listType}");
            }

            List<string> childOtherConnectors = new(otherConnectors);
            NbtBase[] childs = nbt.Tags;
            int childLength = nbt.ValueCount;
            string childConnector;
            bool childIsParentLast;
            bool childIsInsideList;

            if (!isRoot)
                childOtherConnectors.Add(isParentLast ? Connector.Space : Connector.Connect);
            childIsInsideList = nbt.NbtType == NbtType.NbtList;

            for (int i = 0; i < nbt.ValueCount; i++)
            {
                if (i == childLength - 1)
                {
                    childConnector = Connector.Last;
                    childIsParentLast = true;
                }
                else
                {
                    childConnector = Connector.Branch;
                    childIsParentLast = false;
                }
                tree.Compile(childs[i], childOtherConnectors, childConnector, childIsParentLast, childIsInsideList);
            }
        }

        static void compileArrayType(NbtTree tree, NbtArrayType nbt, List<string> otherConnectors, bool isParentLast, bool isRoot)
        {
            string[] values = nbt.ValuesStringized;
            tree._sb.AppendLine($"{values.Length} value(s)");

            string childConnector = "";
            if (!isRoot)
                childConnector = isParentLast ? Connector.Space : Connector.Connect;
            for (int i = 0; i < values.Length; i++)
            {
                foreach (string otherConnector in otherConnectors)
                    tree._sb.Append(otherConnector);
                string connector = i == values.Length - 1 ? Connector.Last : Connector.Branch;
                tree._sb.AppendLine($"{childConnector}{connector}{values[i]}");
            }
        }

        static void compileSingleValueType(NbtTree tree, NbtSingleValueType nbt)
        {
            tree._sb.AppendLine(nbt.ValueStringized);
        }
    }

    private static string DotOrDash(NbtBase nbt, bool isRoot)
    {
        if (nbt is NbtMultipleValueType nbtMultipleValueType && nbtMultipleValueType.ValueCount > 0)
            return "●"; // dot
        else
        {
            if (isRoot)
                return "";
            return "─"; // dash
        }
    }
}

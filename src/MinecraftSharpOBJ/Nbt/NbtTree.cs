using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using binstarjs03.MinecraftSharpOBJ.Nbt.Abstract;
using binstarjs03.MinecraftSharpOBJ.Nbt.Concrete;
namespace binstarjs03.MinecraftSharpOBJ.Nbt;

public class NbtTree {
    private readonly StringBuilder _sb;
    private readonly NbtBase _nbt;
    //private readonly Stack<ConnectorType> _connectors;
    private bool _hasCompiled;

    private enum ConnectorType {
        Connect,
        Branch,
        Last,
        Space,
    }

    private class Connector {
        public static readonly string Connect = "│  ";
        public static readonly string Branch = "├──";
        public static readonly string Last = "└──";
        public static readonly string Space = "   ";
    }

    public NbtTree(NbtBase nbt) {
        _sb = new();
        _nbt = nbt;
        //_connectors = new();
        _hasCompiled = false;
    }

    public string Compile() {
        if (!_hasCompiled) {
            Compile(_nbt, new(), string.Empty, true, false, true);
            _hasCompiled = true;
        }
        return _sb.ToString();
    }

    private void Compile(NbtBase nbt, List<string> otherConnectors, string connector, bool isParentLast, bool isInsideList, bool isRoot=false) {
        foreach (string otherConnector in otherConnectors)
            _sb.Append(otherConnector);
        _sb.Append($"{connector}{DotOrDash(nbt, isRoot)}");
        if (!isInsideList) {
            _sb.Append($"<{nbt.NbtTypeName}> {nbt.Name} = ");
        }

        switch (nbt.NbtTypeBase) {
            case NbtTypeBase.NbtContainerType:
                CompileContainerType(_sb, (NbtContainerType)nbt, otherConnectors, isParentLast, isRoot);
                break;
            case NbtTypeBase.NbtArrayType:
                CompileArrayType(_sb, (NbtArrayType)nbt, otherConnectors, isParentLast, isRoot);
                break;
            case NbtTypeBase.NbtSingleValueType:
                CompileSingleValueType(_sb, (NbtSingleValueType)nbt);
                break;
            default:
                break;
        }

        void CompileContainerType(StringBuilder sb, NbtContainerType nbt, List<string> otherConnectors, bool isParentLast, bool isRoot) {
            if (nbt is NbtCompound)
                sb.AppendLine($"{nbt.ValueCount} tag(s)");
            if (nbt is NbtList nbtList) {
                NbtType? listType = nbtList.ListType;
                if (listType is null)
                    sb.AppendLine($"{nbtList.ValueCount} tag(s)");
                else
                    sb.AppendLine($"{nbtList.ValueCount} tag(s) of {NbtTypeName.FromEnum((NbtType)listType)}");
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

            for (int i = 0; i < nbt.ValueCount; i++) {
                if (i == childLength - 1) {
                    childConnector = Connector.Last;
                    childIsParentLast = true;
                }
                else {
                    childConnector = Connector.Branch;
                    childIsParentLast = false;
                }
                Compile(childs[i], childOtherConnectors, childConnector, childIsParentLast, childIsInsideList);
            }
        }

        void CompileArrayType(StringBuilder sb, NbtArrayType nbt, List<string> otherConnectors, bool isParentLast, bool isRoot) {
            string[] values = nbt.ValuesStringized;
            sb.AppendLine($"{values.Length} value(s)");

            string childConnector = "";
            if (!isRoot)
                childConnector = isParentLast ? Connector.Space : Connector.Connect;
            for (int i = 0; i < values.Length; i++) {
                foreach (string otherConnector in otherConnectors)
                    sb.Append(otherConnector);
                string connector = i == values.Length - 1 ? Connector.Last : Connector.Branch;
                sb.AppendLine($"{childConnector}{connector}{values[i]}");
            }
        }

        void CompileSingleValueType(StringBuilder sb, NbtSingleValueType nbt) {
            sb.AppendLine(nbt.ValueStringized);
        }
    }

    private string DotOrDash(NbtBase nbt, bool isRoot) {
        if (nbt is NbtMultipleValueType nbtMultipleValueType && nbtMultipleValueType.ValueCount > 0)
            return "●"; // dot
        else {
            if (isRoot)
                return "";
            return "─"; // dash
        }
    }
}

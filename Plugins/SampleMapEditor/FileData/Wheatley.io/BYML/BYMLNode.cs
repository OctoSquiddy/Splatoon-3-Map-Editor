using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Wheatley.io.BYML
{
    /*-------------------------------------------------------------------------------*/
    /*                             BYML's Node Types                                 */
    /*-------------------------------------------------------------------------------*/

    public enum BymlNodeType
    {
        String = 0xA0,
        ArrayNode = 0xC0,
        DictionaryNode = 0xC1,
        StringListNode = 0xC2,
        Boolean = 0xD0,
        Int32 = 0xD1,
        Single = 0xD2,
        UInt32 = 0xD3,
        Int64 = 0xD4,
        UInt64 = 0xD5,
        Double = 0xD6,
        NULL = 0xFF,
    }

    /*-------------------------------------------------------------------------------*/
    /*                                  Comparers                                    */
    /*-------------------------------------------------------------------------------*/

    // Sorts the Byaml nodes by Name
    public class BymlNodeComparer : IComparer<BymlNode>
    {
        public int Compare(BymlNode mBymlNode00, BymlNode mBymlNode01) => string.CompareOrdinal(mBymlNode00.Name, mBymlNode01.Name);
    }

    // Sorts the HashTable
    public class HashTableOrder : IComparer<string>
    {
        public int Compare(string Name00, string Name01) => string.CompareOrdinal(Name00, Name01);
    }

    /*-------------------------------------------------------------------------------*/
    /*                                BYML's Nodes                                   */
    /*-------------------------------------------------------------------------------*/

    public abstract class BymlNode
    {
        public uint Address { get; set; }
        public string Name { get; set; }

        BymlNode Parent = null;
        public List<BymlNode> Nodes = new List<BymlNode>();

        // String Tables
        public static List<string> HashTable = new List<string>();
        public static List<string> StringTable = new List<string>();

        public abstract BymlNodeType Type { get; }
        public static bool IsBigEndian;

        public BymlNode this[int key]
        {
            get => Nodes[key];
            set => Nodes[key] = value;
        }

        public BymlNode this[string key]
        {
            get
            {
                foreach (BymlNode Node in Nodes)
                {
                    if (Node.Name == key) return Node;
                }

                return null;
            }
            set => Nodes[0] = value;
        }

        /*-------------------------------------------------------------------------------*/
        /*                                  Node Types                                   */
        /*-------------------------------------------------------------------------------*/

        public class String : BymlNode // 0xA0
        {
            public override BymlNodeType Type => BymlNodeType.String;
            public string Text;

            public String(string mName, string mText) { Name = mName; Text = mText; }
            public String(string mText) { Text = mText; }
            public String() { }

            public String(NitroFile mFile)
            {
                Address = mFile.Position;
                Text = StringTable.ElementAt(mFile.ReadInt32());
                Name = Text;
            }
        }

        public class ArrayNode : BymlNode // 0xC0
        {
            public override BymlNodeType Type => BymlNodeType.ArrayNode;
            public uint NodeOffset;

            public ArrayNode() { }

            public ArrayNode(NitroFile mFile)
            {
                Address = mFile.Position;
                GetArray(mFile);
            }

            public ArrayNode(BymlNode mNode, uint mPosition)
            {
                NodeOffset = mPosition;
                Nodes = ((ArrayNode)mNode).Nodes;
            }

            private void GetArray(NitroFile mFile)
            {
                Nodes.Clear();
                int mNodeName = 0;

                if (Address == 0 || mFile.ReadByte() != (int)BymlNodeType.ArrayNode)
                    throw new Exception("The Node is not of an Array!");

                uint Entries = mFile.ReadUInt24();
                byte[] Types = mFile.ReadBytes((int)Entries);

                while (mFile.Position % 4 != 0)
                    mFile.Position += 1;

                uint mBaseAddress = mFile.Position;

                for (int i = 0; i < Entries; i++)
                {
                    switch ((BymlNodeType)Types[i])
                    {
                        case BymlNodeType.String:
                            Nodes.Add(new String(mFile));
                            break;
                        case BymlNodeType.ArrayNode:
                            mFile.Position = mFile.ReadUInt32();
                            Nodes.Add(new ArrayNode(mFile) { Name = mNodeName++.ToString() });
                            break;
                        case BymlNodeType.DictionaryNode:
                            mFile.Position = mFile.ReadUInt32();
                            Nodes.Add(new DictionaryNode(mFile) { Name = mNodeName++.ToString() });
                            break;
                        case BymlNodeType.Boolean:
                            Nodes.Add(new Boolean(mFile));
                            break;
                        case BymlNodeType.Int32:
                            Nodes.Add(new Int32(mFile));
                            break;
                        case BymlNodeType.Single:
                            Nodes.Add(new Single(mFile));
                            break;
                        case BymlNodeType.UInt32:
                            Nodes.Add(new UInt32(mFile));
                            break;
                        case BymlNodeType.Int64:
                            mFile.Position = mFile.ReadUInt32();
                            Nodes.Add(new Int64(mFile));
                            break;
                        case BymlNodeType.UInt64:
                            mFile.Position = mFile.ReadUInt32();
                            Nodes.Add(new UInt64(mFile));
                            break;
                        case BymlNodeType.Double:
                            mFile.Position = mFile.ReadUInt32();
                            Nodes.Add(new Double(mFile));
                            break;
                        case BymlNodeType.NULL:
                            Nodes.Add(new NULL(mFile));
                            break;
                        default:
                            throw new Exception($"This Node Type is Unsupported! \n Node: 0x{mFile.ReadByte():X}");
                    }

                    Nodes[i].Parent = this;
                    mFile.Position = (uint)(mBaseAddress + (i + 1) * 4);
                }
            }

            public void WriteArray(NitroFile mFile)
            {
                mFile.Position = NodeOffset;
                mFile.WriteByte((byte)Type);

                int Entries = Nodes.Count;
                mFile.WriteInt24(Entries);

                if (Nodes.Any() == false)
                    return;

                foreach (BymlNode mNode in Nodes)
                    mFile.WriteByte((byte)mNode.Type);

                while (mFile.Position % 4 != 0)
                    mFile.WriteByte(0);

                uint BaseAddress = mFile.Position;
                int CurrentNode = 1;
                uint NodeAddress = 0;

                mFile.WriteBytes(new byte[4 * Entries]);
                NodeAddress = mFile.Position;
                mFile.Position = BaseAddress;

                foreach (BymlNode mNode in Nodes)
                {
                    switch (mNode.Type)
                    {
                        case BymlNodeType.String:
                            mFile.WriteInt32(StringTable.IndexOf(((String)mNode).Text));
                            break;
                        case BymlNodeType.ArrayNode:
                            mFile.WriteUInt32(NodeAddress);

                            ((ArrayNode)mNode).NodeOffset = NodeAddress;
                            ((ArrayNode)mNode).WriteArray(mFile);
                            NodeAddress = mFile.Position;
                            break;
                        case BymlNodeType.DictionaryNode:
                            mFile.WriteUInt32(NodeAddress);

                            ((DictionaryNode)mNode).NodeOffset = NodeAddress;
                            ((DictionaryNode)mNode).WriteDictionary(mFile);
                            NodeAddress = mFile.Position;
                            break;
                        case BymlNodeType.Boolean:
                            byte[] boolval = new byte[4];
                            if (((Boolean)mNode).Value)
                                boolval[0] = 1;
                            mFile.WriteBytes(boolval);
                            break;
                        case BymlNodeType.Int32:
                            mFile.WriteInt32(((Int32)mNode).Value);
                            break;
                        case BymlNodeType.Single:
                            mFile.WriteSingle(((Single)mNode).Value);
                            break;
                        case BymlNodeType.UInt32:
                            mFile.WriteUInt32(((UInt32)mNode).Value);
                            break;
                        case BymlNodeType.Int64:
                            mFile.WriteUInt32((uint)mFile.mFile.Count);
                            mFile.Position = (uint)mFile.mFile.Count;
                            mFile.WriteInt64(((Int64)mNode).Value);
                            NodeAddress = mFile.Position;
                            break;
                        case BymlNodeType.UInt64:
                            mFile.WriteUInt32((uint)mFile.mFile.Count);
                            mFile.Position = (uint)mFile.mFile.Count;
                            mFile.WriteUInt64(((UInt64)mNode).Value);
                            NodeAddress = mFile.Position;
                            break;
                        case BymlNodeType.Double:
                            mFile.WriteUInt32((uint)mFile.mFile.Count);
                            mFile.Position = (uint)mFile.mFile.Count;
                            mFile.WriteDouble(((Double)mNode).Value);
                            NodeAddress = mFile.Position;
                            break;
                        case BymlNodeType.NULL:
                            mFile.WriteBytes(new byte[4]);
                            break;
                    }

                    mFile.Position = (uint)(BaseAddress + 4 * CurrentNode++);
                }

                mFile.Position = (uint)mFile.mFile.Count;
            }

            public void AddNode(object Value)
            {
                Type type = Value.GetType();
                Type nullableType = Nullable.GetUnderlyingType(type);
                if (nullableType != null && nullableType.GetTypeInfo().IsEnum)
                    type = nullableType;
                if (type.IsEnum)
                    type = Enum.GetUnderlyingType(type);

                if (type == typeof(bool)) this.Nodes.Add(new BymlNode.Boolean((bool)Value));
                else if (type == typeof(string)) this.Nodes.Add(new BymlNode.String((string)Value));
                else if (type == typeof(int)) this.Nodes.Add(new BymlNode.Int32((int)Value));
                else if (type == typeof(uint)) this.Nodes.Add(new BymlNode.UInt32((uint)Value));
                else if (type == typeof(long)) this.Nodes.Add(new BymlNode.Int64((long)Value));
                else if (type == typeof(ulong)) this.Nodes.Add(new BymlNode.UInt64((ulong)Value));
                else if (type == typeof(float)) this.Nodes.Add(new BymlNode.Single((float)Value));
                else if (type == typeof(double)) this.Nodes.Add(new BymlNode.Double((double)Value));
            }
        }

        public class DictionaryNode : BymlNode // 0xC1
        {
            public override BymlNodeType Type => BymlNodeType.DictionaryNode;
            
            public uint NodeOffset;

            public DictionaryNode() { }

            public DictionaryNode(NitroFile mFile)
            {
                Address = mFile.Position;
                GetDictionary(mFile);
            }

            public DictionaryNode(string mName)
            {
                Name = mName;
            }

            public DictionaryNode(BymlNode mNode, uint mPosition)
            {
                NodeOffset = mPosition;
                Nodes = ((DictionaryNode)mNode).Nodes;
            }

            private void GetDictionary(NitroFile mFile)
            {
                Nodes.Clear();

                if (Address == 0 || mFile.ReadByte() != (int)BymlNodeType.DictionaryNode)
                    throw new Exception($"The Node is not of a Dictionary!");

                uint Entries = mFile.ReadUInt24();
                uint mBaseAddress = (uint)mFile.Position;

                for (int i = 0; i < Entries; i++)
                {
                    uint mNodeNameIndex = mFile.ReadUInt24();

                    switch ((BymlNodeType)mFile.ReadByte())
                    {
                        case BymlNodeType.String:
                            Nodes.Add(new String(mFile) { Name = HashTable.ElementAt((int)mNodeNameIndex) });
                            break;
                        case BymlNodeType.ArrayNode:
                            mFile.Position = mFile.ReadUInt32();
                            Nodes.Add(new ArrayNode(mFile) { Name = HashTable.ElementAt((int)mNodeNameIndex) });
                            break;
                        case BymlNodeType.DictionaryNode:
                            mFile.Position = mFile.ReadUInt32();
                            Nodes.Add(new DictionaryNode(mFile) { Name = HashTable.ElementAt((int)mNodeNameIndex) });
                            break;
                        case BymlNodeType.Boolean:
                            Nodes.Add(new Boolean(mFile) { Name = HashTable.ElementAt((int)mNodeNameIndex) });
                            break;
                        case BymlNodeType.Int32:
                            Nodes.Add(new Int32(mFile) { Name = HashTable.ElementAt((int)mNodeNameIndex) });
                            break;
                        case BymlNodeType.Single:
                            Nodes.Add(new Single(mFile) { Name = HashTable.ElementAt((int)mNodeNameIndex) });
                            break;
                        case BymlNodeType.UInt32:
                            Nodes.Add(new UInt32(mFile) { Name = HashTable.ElementAt((int)mNodeNameIndex) });
                            break;
                        case BymlNodeType.Int64:
                            mFile.Position = mFile.ReadUInt32();
                            Nodes.Add(new Int64(mFile) { Name = HashTable.ElementAt((int)mNodeNameIndex) });
                            break;
                        case BymlNodeType.UInt64:
                            mFile.Position = mFile.ReadUInt32();
                            Nodes.Add(new UInt64(mFile) { Name = HashTable.ElementAt((int)mNodeNameIndex) });
                            break;
                        case BymlNodeType.Double:
                            mFile.Position = mFile.ReadUInt32();
                            Nodes.Add(new Double(mFile) { Name = HashTable.ElementAt((int)mNodeNameIndex) });
                            break;
                        case BymlNodeType.NULL:
                            Nodes.Add(new NULL(mFile) { Name = HashTable.ElementAt((int)mNodeNameIndex) });
                            break;
                        default:
                            mFile.Position -= 1;
                            throw new Exception($"This Node Type is Unsupported! \n Node: 0x{mFile.ReadByte():X}");
                    }

                    Nodes[i].Parent = this;
                    mFile.Position = (uint)(mBaseAddress + (i + 1) * 8); // i + 1 because this line becomes useful on the 2nd node!
                }
            }

            public void WriteDictionary(NitroFile mFile)
            {
                mFile.Position = NodeOffset;
                mFile.WriteByte((byte)Type);

                int Entries = Nodes.Count;
                mFile.WriteInt24(Entries);

                if (Entries == 0)
                    return;

                // Setting up the up-coming mess
                uint BaseAddress = mFile.Position;
                int CurrentNode = 1;

                mFile.WriteBytes(new byte[0x8 * Entries]);

                uint NodeAddress = mFile.Position;
                mFile.Position = BaseAddress;

                Nodes.Sort(new BymlNodeComparer());
                foreach (BymlNode mNode in Nodes)
                {
                    mFile.WriteInt24(HashTable.IndexOf(mNode.Name));
                    mFile.WriteByte((byte)mNode.Type);

                    switch (mNode.Type)
                    {
                        case BymlNodeType.String:
                            mFile.WriteInt32(StringTable.IndexOf(((String)mNode).Text));
                            break;
                        case BymlNodeType.ArrayNode:
                            mFile.WriteUInt32(NodeAddress);

                            ((ArrayNode)mNode).NodeOffset = NodeAddress;
                            ((ArrayNode)mNode).WriteArray(mFile);
                            NodeAddress = mFile.Position;
                            break;
                        case BymlNodeType.DictionaryNode:
                            mFile.WriteUInt32(NodeAddress);

                            ((DictionaryNode)mNode).NodeOffset = NodeAddress;
                            ((DictionaryNode)mNode).WriteDictionary(mFile);
                            NodeAddress = mFile.Position;
                            break;
                        case BymlNodeType.Boolean:
                            byte[] boolval = new byte[4];
                            if (((Boolean)mNode).Value)
                                boolval[0] = 1;
                            mFile.WriteBytes(boolval);
                            break;
                        case BymlNodeType.Int32:
                            mFile.WriteInt32(((Int32)mNode).Value);
                            break;
                        case BymlNodeType.Single:
                            mFile.WriteSingle(((Single)mNode).Value);
                            break;
                        case BymlNodeType.UInt32:
                            mFile.WriteUInt32(((UInt32)mNode).Value);
                            break;
                        case BymlNodeType.Int64:
                            mFile.WriteUInt32((uint)mFile.mFile.Count);
                            mFile.Position = (uint)mFile.mFile.Count;
                            mFile.WriteInt64(((Int64)mNode).Value);
                            NodeAddress = mFile.Position;
                            break;
                        case BymlNodeType.UInt64:
                            mFile.WriteUInt32((uint)mFile.mFile.Count);
                            mFile.Position = (uint)mFile.mFile.Count;
                            mFile.WriteUInt64(((UInt64)mNode).Value);
                            NodeAddress = mFile.Position;
                            break;
                        case BymlNodeType.Double:
                            mFile.WriteUInt32((uint)mFile.mFile.Count);
                            mFile.Position = (uint)mFile.mFile.Count;
                            mFile.WriteDouble(((Double)mNode).Value);
                            NodeAddress = mFile.Position;
                            break;
                        case BymlNodeType.NULL:
                            mFile.WriteBytes(new byte[4]);
                            break;
                    }

                    mFile.Position = (uint)(BaseAddress + 0x8 * CurrentNode++);
                }

                mFile.Position = (uint)mFile.mFile.Count;
            }

            public BymlNode get(string mName)
            {
                for (int i = 0; i < Nodes.Count; i++)
                {
                    if (Nodes[i].Name == mName)
                        return Nodes[i];
                }

                return null;
            }

            public bool exists(string mName)
            {
                for (int i = 0; i < Nodes.Count; i++)
                {
                    if (Nodes[i].Name == mName)
                        return true;
                }

                return false;
            }

            public void AddNode(string Name, object Value)
            {
                Type type = Value.GetType();
                Type nullableType = Nullable.GetUnderlyingType(type);
                if (nullableType != null && nullableType.GetTypeInfo().IsEnum)
                    type = nullableType;
                if (type.IsEnum)
                    type = Enum.GetUnderlyingType(type);

                if (type == typeof(bool)) this.Nodes.Add(new BymlNode.Boolean(Name, (bool)Value));
                else if (type == typeof(string)) this.Nodes.Add(new BymlNode.String(Name, (string)Value));
                else if (type == typeof(int)) this.Nodes.Add(new BymlNode.Int32(Name, (int)Value));
                else if (type == typeof(uint)) this.Nodes.Add(new BymlNode.UInt32(Name, (uint)Value));
                else if (type == typeof(long)) this.Nodes.Add(new BymlNode.Int64(Name, (long)Value));
                else if (type == typeof(ulong)) this.Nodes.Add(new BymlNode.UInt64(Name, (ulong)Value));
                else if (type == typeof(float)) this.Nodes.Add(new BymlNode.Single(Name, (float)Value));
                else if (type == typeof(double)) this.Nodes.Add(new BymlNode.Double(Name, (double)Value));
            }
        }

        public class StringListNode : BymlNode // 0xC2
        {
            private uint NodeOffsetPosition = 0;
            private uint NodeOffset;
            public override BymlNodeType Type => BymlNodeType.StringListNode;
            public List<string> mStringTable = new List<string>();

            public StringListNode(NitroFile mFile)
            {
                Address = mFile.Position;
                GetStringTable(mFile);
            }

            public StringListNode(uint mNodeOffset, uint mNodeOffsetPosition, List<string> mStringTable00)
            {
                NodeOffset = mNodeOffset;
                NodeOffsetPosition = mNodeOffsetPosition;
                mStringTable = mStringTable00;
            }

            private void GetStringTable(NitroFile mFile)
            {
                StringTable.Clear();
                mFile.Position = Address;

                if (mFile.ReadByte() != (int)Type)
                    throw new Exception("The Node is not of a string table!");

                uint Entries = mFile.ReadUInt24();

                uint[] Offsets = new uint[Entries + 0x1];

                for (int i = 0; i < Entries + 0x1; i++)
                    Offsets[i] = mFile.ReadUInt32();

                for (int i = 0; i < Entries; i++)
                {
                    mStringTable.Add(mFile.ReadUTF8String((int)(Offsets[i + 1] - Offsets[i] - 0x1))); // -1 for the space in-betwixt strings
                    mFile.Position += 0x1;
                }
            }

            public void WriteStringTable(NitroFile mFile)
            {
                // Writes the offset of the String Table
                mFile.Position = NodeOffsetPosition;
                mFile.WriteUInt32(NodeOffset);
                mFile.Position = NodeOffset;

                mFile.WriteByte((byte)Type);

                // Writes StringTable's number of items
                int HashTableCount = mStringTable.Count;
                mFile.WriteInt24(HashTableCount);

                int StringLenSum = 0;
                for (int i = 0; i < HashTableCount + 1; i++)
                {
                    mFile.WriteInt32(0x4 * (HashTableCount + 2) + StringLenSum);
                    if (i < HashTableCount)
                        StringLenSum += Encoding.GetEncoding(65001).GetBytes(mStringTable.ElementAt(i)).Length + 0x1;
                }
                for (int i = 0; i < HashTableCount; i++)
                {
                    mFile.WriteString(mStringTable.ElementAt(i));
                    mFile.WriteByte(0);
                }
            }
        }

        public class Boolean : BymlNode // 0xD0
        {
            public override BymlNodeType Type => BymlNodeType.Boolean;
            public bool Value;

            public Boolean(bool mValue) { Value = mValue; }
            public Boolean(string mName, bool mValue) { Name = mName; Value = mValue; }
            public Boolean() { }

            public Boolean(NitroFile mFile)
            {
                Address = mFile.Position;
                Value = mFile.ReadBoolean();
                Name = ToString();
            }

            public override string ToString() => Value ? "True" : "False";
        }

        public class Int32 : BymlNode // 0xD1
        {
            public override BymlNodeType Type => BymlNodeType.Int32;
            public int Value;

            public Int32(int mValue) { Value = mValue; }
            public Int32(string mName, int mValue) { Name = mName; Value = mValue; }
            public Int32() { }

            public Int32(NitroFile mFile)
            {
                Address = mFile.Position;
                Value = mFile.ReadInt32();
                Name = ToString();
            }

            public override string ToString() => Value.ToString();
        }

        public class Single : BymlNode // 0xD2
        {
            public override BymlNodeType Type => BymlNodeType.Single;
            public float Value = 0.0f;

            public Single(float mValue) { Value = mValue; }
            public Single(string mName, float mValue) { Name = mName; Value = mValue; }
            public Single() { }

            public Single(NitroFile mFile)
            {
                Address = mFile.Position;
                Value = mFile.ReadSingle();
                Name = ToString();
            }

            public override string ToString() => Value.ToString();
        }

        public class UInt32 : BymlNode // 0xD3
        {
            public override BymlNodeType Type => BymlNodeType.UInt32;
            public uint Value;

            public UInt32(string mName, uint mValue) { Name = mName; Value = mValue; }
            public UInt32(uint mValue) { Value = mValue; }
            public UInt32() { }

            public UInt32(NitroFile mFile)
            {
                Address = mFile.Position;
                Value = mFile.ReadUInt32();
                Name = ToString();
            }

            public override string ToString() => Value.ToString();
        }

        public class Int64 : BymlNode // 0xD4
        {
            public override BymlNodeType Type => BymlNodeType.Int64;
            public long Value;

            public Int64(string mName, long mValue) { Name = mName; Value = mValue; }
            public Int64(long mValue) { Value = mValue; }
            public Int64() { }

            public Int64(NitroFile mFile)
            {
                Address = mFile.Position;
                Value = mFile.ReadInt64();

                Name = ToString();
            }

            public override string ToString() => Value.ToString();
        }

        public class UInt64 : BymlNode // 0xD5
        {
            public override BymlNodeType Type => BymlNodeType.UInt64;
            public ulong Value;

            public UInt64(string mName, ulong mValue) { Name = mName; Value = mValue; }
            public UInt64(ulong mValue) { Value = mValue; }
            public UInt64() { }

            public UInt64(NitroFile mFile)
            {
                Address = mFile.Position;
                Value = mFile.ReadUInt64();

                Name = ToString();
            }

            public override string ToString() => Value.ToString();
        }

        public class Double : BymlNode // 0xD6
        {
            public override BymlNodeType Type => BymlNodeType.Double;
            public double Value;

            public Double(string mName, double mValue) { Name = mName; Value = mValue; }
            public Double(double mValue) { Value = mValue; }
            public Double() { }

            public Double(NitroFile mFile)
            {
                Address = mFile.Position;
                Value = mFile.ReadDouble();

                Name = ToString();
            }

            public override string ToString() => Value.ToString();
        }

        public class NULL : BymlNode // 0xFF
        {
            public override BymlNodeType Type => BymlNodeType.NULL;

            public NULL() { }

            public NULL(NitroFile mFile)
            {
                Address = mFile.Position;
                mFile.Position += 4;
                Name = "NULL";
            }

            public override string ToString() => "<NULL>";
        }
    }
}

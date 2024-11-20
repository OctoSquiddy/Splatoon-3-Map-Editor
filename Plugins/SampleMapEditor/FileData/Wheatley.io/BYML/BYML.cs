using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Wheatley.io.BYML
{
    public class BYML
    {
        // Fields
        public string FileName = "";
        public bool IsBigEndian = false;  // Big or Little endian flag (determined at 0x0)
        public Int16 FileVersion = 0;     // File Version (at 0x2)

        // String Tables
        public List<string> HashTable = new List<string>();
        public List<string> StringTable = new List<string>();

        // Treeview
        public BymlNode Root = null;

        public BYML() 
        {
            Root = new BymlNode.DictionaryNode();
        }

        public BYML(NitroFile mFile)
        {
            Read(mFile);
            FileName = mFile.FileName;
        }

        private void Read(NitroFile mFile)
        {
            this.IsBigEndian = mFile.ReadUTF8String(2) == "BY";
            mFile.IsBigEndian = IsBigEndian;

            FileVersion = mFile.ReadInt16();

            uint HashTableOffset = mFile.ReadUInt32();
            uint StringTableOffset = mFile.ReadUInt32();
            uint RootNodeOffset = mFile.ReadUInt32();

            // String Tables
            HashTable = new List<string>();
            StringTable = new List<string>();

            /*---------------------------String Table Nodes------------------------------*/

            if (HashTableOffset != 0)
            {
                mFile.Position = HashTableOffset;
                HashTable.AddRange(new BymlNode.StringListNode(mFile).mStringTable);

                BymlNode.HashTable = HashTable;
            }

            if (StringTableOffset != 0)
            {
                mFile.Position = StringTableOffset;
                StringTable.AddRange(new BymlNode.StringListNode(mFile).mStringTable);

                BymlNode.StringTable = StringTable;
            }

            /*------------------------------Actual Nodes---------------------------------*/

            mFile.Position = RootNodeOffset;
            int RootType = mFile.ReadByte();
            mFile.Position -= 1;

            if (RootType == (int)BymlNodeType.DictionaryNode) // if root is 0xC1
                Root = new BymlNode.DictionaryNode(mFile) { Name = Path.GetFileName(mFile.FileName) };
            else if (RootType == (int)BymlNodeType.ArrayNode)
                Root = new BymlNode.ArrayNode(mFile) { Name = Path.GetFileName(mFile.FileName) };
        }

        public NitroFile Save()
        {
            NitroFile mFile = new NitroFile() { FileName = this.FileName, IsBigEndian = this.IsBigEndian };
            Write(mFile);

            return mFile;
        }

        private void Write(NitroFile mFile)
        {
            if (IsBigEndian)
                mFile.WriteString("BY");
            else
                mFile.WriteString("YB");

            // Set up the Byml Node Data
            BymlNode.IsBigEndian = IsBigEndian;
            BymlNode.HashTable = HashTable;
            BymlNode.StringTable = StringTable;

            mFile.WriteInt16(FileVersion);
            mFile.WriteBytes(new byte[0xC]);

            // Writes the HashTable if needed
            if (HashTable.Any())
            {
                BymlNode.StringListNode newStringNode = new BymlNode.StringListNode(mFile.Position, 0x4, HashTable);
                newStringNode.WriteStringTable(mFile);

                while (mFile.Position % 4 != 0)
                    mFile.WriteByte(0);
            }

            // Writes the StringTable if needed
            if (StringTable.Any())
            {
                BymlNode.StringListNode newStringNode = new BymlNode.StringListNode(mFile.Position, 0x8, StringTable);
                newStringNode.WriteStringTable(mFile);

                while (mFile.Position % 4 != 0)
                    mFile.WriteByte(0);
            }

            // Writes the node offset
            mFile.Position = 0xC;
            mFile.WriteInt32(mFile.mFile.Count);
            mFile.Position = (uint)mFile.mFile.Count;


            // Writes the TreeView
            if (Root.Type == BymlNodeType.DictionaryNode)
            {
                BymlNode.DictionaryNode mRootNode = new BymlNode.DictionaryNode(Root, mFile.Position);
                mRootNode.WriteDictionary(mFile);
            }
            else
            {
                BymlNode.ArrayNode mRootNode = new BymlNode.ArrayNode(Root, mFile.Position);
                mRootNode.WriteArray(mFile);
            }
        }
    }
}



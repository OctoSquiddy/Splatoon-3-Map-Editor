using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Wheatley.io
{
    public class NitroFile
    {
        // Fields
        public List<byte> mFile = new List<byte>();

        const int INT16_LENGTH = 0x2;
        const int INT32_LENGTH = 0x4;
        const int INT64_LENGTH = 0x8;

        public string FileName = "";

        public string Description = "";
        public string[] FileExtensions = new string[0];

        public uint Position = 0;
        public bool IsBigEndian = true;

        // Constructors
        public NitroFile() { }

        public NitroFile(string mFileName)
        {
            mFile.AddRange(File.ReadAllBytes(mFileName));
            FileName = mFileName;
            GetDescriptionAndFileExtensionFromFileData();
        }

        public NitroFile(byte[] mNewFile, string mFileName)
        {
            mFile.AddRange(mNewFile);
            FileName = mFileName;
            GetDescriptionAndFileExtensionFromFileData();
        }

        public NitroFile(byte[] mNewFile)
        {
            mFile.AddRange(mNewFile);
            FileName = "Untitled.unk";
            GetDescriptionAndFileExtensionFromFileData();
        }

        /*----------------------------------------------------------------------------*/
        /*                                  Save                                      */
        /*----------------------------------------------------------------------------*/

        public void Save() => File.WriteAllBytes(FileName, mFile.ToArray());

        /*----------------------------------------------------------------------------*/
        /*                                  Read                                      */
        /*----------------------------------------------------------------------------*/

        public string ReadUTF8String(int StrLen)
        {
            byte[] str = new byte[StrLen];

            for (int i = 0; i < StrLen; i++)
            {
                str[i] = mFile.ElementAt((int)Position);
                Position++;
            }

            return Encoding.GetEncoding(65001).GetString(str, 0, StrLen);
        }

        public string ReadNullTerminatedUTF8String()
        {
            byte _char = ReadByte();
            List<byte> FileName = new List<byte>();

            while (_char != 0)
            {
                FileName.Add(_char);
                _char = ReadByte();
            }

            return Encoding.GetEncoding(65001).GetString(FileName.ToArray(), 0, FileName.Count);
        }

        public byte ReadByte()
        {
            byte Value = mFile.ElementAt((int)Position);
            Position++;

            return Value;
        }

        public byte[] ReadBytes(int ArrayLen)
        {
            byte[] arr = new byte[ArrayLen];

            for (int i = 0; i < ArrayLen; i++)
            {
                arr[i] = mFile.ElementAt((int)Position);
                Position++;
            }

            return arr;
        }

        public UInt16 ReadUInt16()
        {
            byte[] Value = new byte[INT16_LENGTH];

            for (int i = 0; i < INT16_LENGTH; i++)
            {
                Value[i] = mFile.ElementAt((int)Position);
                Position++;
            }

            if (IsBigEndian)
                Array.Reverse(Value);

            return BitConverter.ToUInt16(Value, 0);
        }

        public Int16 ReadInt16()
        {
            byte[] Value = new byte[INT16_LENGTH];

            for (int i = 0; i < INT16_LENGTH; i++)
            {
                Value[i] = mFile.ElementAt((int)Position);
                Position++;
            }

            if (IsBigEndian)
                Array.Reverse(Value);

            return BitConverter.ToInt16(Value, 0);
        }

        public bool ReadBoolean()
        {
            byte[] Value = new byte[INT32_LENGTH];

            for (int i = 0; i < INT32_LENGTH; i++)
            {
                Value[i] = mFile.ElementAt((int)Position);
                Position++;
            }

            if (IsBigEndian)
                Array.Reverse(Value);

            return BitConverter.ToBoolean(Value, 0);
        }

        public float ReadSingle()
        {
            byte[] Value = new byte[INT32_LENGTH];

            for (int i = 0; i < INT32_LENGTH; i++)
            {
                Value[i] = mFile.ElementAt((int)Position);
                Position++;
            }

            if (IsBigEndian)
                Array.Reverse(Value);

            return BitConverter.ToSingle(Value, 0);
        }

        public double ReadDouble()
        {
            byte[] Value = new byte[INT64_LENGTH];

            for (int i = 0; i < INT64_LENGTH; i++)
            {
                Value[i] = mFile.ElementAt((int)Position);
                Position++;
            }

            if (IsBigEndian)
                Array.Reverse(Value);

            return BitConverter.ToDouble(Value, 0);
        }

        public Int32 ReadInt24()
        {
            byte[] Value = new byte[0x3];

            for (int i = 0; i < 0x3; i++)
            {
                Value[i] = mFile.ElementAt((int)Position);
                Position++;
            }

            if (IsBigEndian)
                Array.Reverse(Value);

            return Value[2] << 16 | Value[1] << 8 | Value[0];
        }

        public UInt32 ReadUInt24()
        {
            byte[] Value = new byte[0x3];

            for (int i = 0; i < 0x3; i++)
            {
                Value[i] = mFile.ElementAt((int)Position);
                Position++;
            }

            if (IsBigEndian)
                Array.Reverse(Value);

            return (uint)Value[2] << 16 | (uint)Value[1] << 8 | (uint)Value[0];
        }

        public UInt32 ReadUInt32()
        {
            byte[] Value = new byte[INT32_LENGTH];

            for (int i = 0; i < INT32_LENGTH; i++)
            {
                Value[i] = mFile.ElementAt((int)Position);
                Position++;
            }

            if (IsBigEndian)
                Array.Reverse(Value);

            return BitConverter.ToUInt32(Value, 0);
        }

        public Int32 ReadInt32()
        {
            byte[] Value = new byte[INT32_LENGTH];

            for (int i = 0; i < INT32_LENGTH; i++)
            {
                Value[i] = mFile.ElementAt((int)Position);
                Position++;
            }

            if (IsBigEndian)
                Array.Reverse(Value);

            return BitConverter.ToInt32(Value, 0);
        }

        public ulong ReadUInt64()
        {
            byte[] Value = new byte[INT64_LENGTH];

            for (int i = 0; i < INT64_LENGTH; i++)
            {
                Value[i] = mFile.ElementAt((int)Position);
                Position++;
            }

            if (IsBigEndian)
                Array.Reverse(Value);

            return BitConverter.ToUInt64(Value, 0);
        }

        public long ReadInt64()
        {
            byte[] Value = new byte[INT64_LENGTH];

            for (int i = 0; i < INT64_LENGTH; i++)
            {
                Value[i] = mFile.ElementAt((int)Position);
                Position++;
            }

            if (IsBigEndian)
                Array.Reverse(Value);

            return BitConverter.ToInt64(Value, 0);
        }

        /*----------------------------------------------------------------------------*/
        /*                                  Write                                     */
        /*----------------------------------------------------------------------------*/

        private void Write2mFile(byte[] Data)
        {
            for (int i = 0; i < Data.Length; i++)
            {
                if (Position < mFile.Count)
                {
                    mFile[(int)Position] = Data[i];
                    Position++;
                }
                else
                {
                    mFile.Add(Data[i]);
                    Position++;
                }
            }
        }

        public void WriteByte(byte Value)
        {
            byte[] Data = new byte[] { Value };

            Write2mFile(Data);
        }

        public void WriteBytesNoReverse(byte[] Data) => Write2mFile(Data);

        public void WriteBytes(byte[] Data)
        {
            if (IsBigEndian)
                Array.Reverse(Data);

            Write2mFile(Data);
        }

        public void WriteString(string str)
        {
            byte[] Data = Encoding.GetEncoding(65001).GetBytes(str);

            Write2mFile(Data);
        }

        public void WriteUInt16(UInt16 Value)
        {
            byte[] Data = BitConverter.GetBytes(Value);

            if (IsBigEndian)
                Array.Reverse(Data);

            Write2mFile(Data);
        }

        public void WriteInt16(Int16 Value)
        {
            byte[] Data = BitConverter.GetBytes(Value);

            if (IsBigEndian)
                Array.Reverse(Data);

            Write2mFile(Data);
        }

        public void WriteUInt24(uint Value)
        {
            byte[] Number = BitConverter.GetBytes(Value);
            byte[] Data = new byte[3];
            int len = Number.Length <= 0x3 ? Number.Length : 0x3;

            for (int i = 0; i < len; i++)
                Data[i] = Number[i];

            if (IsBigEndian)
                Array.Reverse(Data);

            Write2mFile(Data);
        }

        public void WriteInt24(int Value)
        {
            byte[] Number = BitConverter.GetBytes(Value);
            byte[] Data = new byte[3];
            int len = Number.Length <= 0x3 ? Number.Length : 0x3;

            for (int i = 0; i < len; i++)
                Data[i] = Number[i];

            if (IsBigEndian)
                Array.Reverse(Data);

            Write2mFile(Data);
        }

        public void WriteUInt32(UInt32 Value)
        {
            byte[] Data = BitConverter.GetBytes(Value);

            if (IsBigEndian)
                Array.Reverse(Data);

            Write2mFile(Data);
        }

        public void WriteInt32(Int32 Value)
        {
            byte[] Data = BitConverter.GetBytes(Value);

            if (IsBigEndian)
                Array.Reverse(Data);

            Write2mFile(Data);
        }

        public void WriteUInt64(UInt64 Value)
        {
            byte[] Data = BitConverter.GetBytes(Value);

            if (IsBigEndian)
                Array.Reverse(Data);

            Write2mFile(Data);
        }

        public void WriteInt64(Int64 Value)
        {
            byte[] Data = BitConverter.GetBytes(Value);

            if (IsBigEndian)
                Array.Reverse(Data);

            Write2mFile(Data);
        }

        public void WriteSingle(float Value)
        {
            byte[] Data = BitConverter.GetBytes(Value);

            if (IsBigEndian)
                Array.Reverse(Data);

            Write2mFile(Data);
        }

        public void WriteDouble(double Value)
        {
            byte[] Data = BitConverter.GetBytes(Value);

            if (IsBigEndian)
                Array.Reverse(Data);

            Write2mFile(Data);
        }

        /*----------------------------------------------------------------------------*/
        /*                          Extension Guesser                                      */
        /*----------------------------------------------------------------------------*/

        public void GetDescriptionAndFileExtensionFromFileData()
        {
            // If the file is empty
            if (mFile.Count == 0 || Path.GetExtension(FileName) == "")
            {
                Description = "File";
                FileExtensions = new string[] { "unk" };
                return;
            }

            // magic with 2 characters
            string Magic = ReadUTF8String(0x2);
            Position = 0;

            if (Magic == "BY" || Magic == "YB")
            {
                Description = "Binary Data-Serialization";
                FileExtensions = new string[] { ".byaml", ".byml", ".bprm", ".sbyml" };
                return;
            }

            // If the loaded file isn't supported by Switch_ToolBox, then assign default extension
            string Type = Path.GetExtension(FileName).Remove(0, 1); // Removes the . given by the function
            Description = Type.ToUpper() + " File"; // Adds EXT File to the Archive description
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FactorioServerAPI
{
    public static class Reader
    {
        public static short ReadVarShort(BinaryReader stream)
        {
            int ansval;
            if ((ansval = (int)(stream.ReadByte() & 0xFF)) == 255)
                ansval = stream.ReadInt16();
            return (short)ansval;
        }
        public static void WriteVarShort(BinaryWriter stream, short data)
        {
            if (data > 0xFF)
            {
                stream.Write((byte)(0xFF));
                stream.Write(data);
            }
            else
            {
                stream.Write((byte)(data));
            }
        }
        public static int ReadVarInt(BinaryReader stream)
        {
            int ansval;
            if ((ansval = (int)(stream.ReadByte() & 0xFF)) == 255)
                ansval = stream.ReadInt32();
            return ansval;
        }
        public static void WriteVarInt(BinaryWriter stream, int data)
        {
            if (data > 0xFF)
            {
                stream.Write((byte)(0xFF));
                stream.Write(data);
            }
            else
            {
                stream.Write((byte)data);
            }
        }
        public static string ReadString(BinaryReader stream)
        {
            short strln = ReadVarShort(stream);
            byte[] data = new byte[strln];
            stream.Read(data, 0, strln);
            return Encoding.UTF8.GetString(data);
        }
        public static void WriteString(BinaryWriter stream, string data)
        {
            byte[] dt = Encoding.UTF8.GetBytes(data);
            WriteVarShort(stream, (short)dt.Length);
            stream.Write(dt);
        }
        public static string ReadComplexString(BinaryReader stream)
        {
            int strln = ReadVarInt(stream);
            byte[] data = new byte[strln];
            stream.Read(data, 0, strln);
            return Encoding.UTF8.GetString(data);
        }
        public static void WriteComplexString(BinaryWriter stream, string data)
        {
            byte[] dt = Encoding.UTF8.GetBytes(data);
            WriteVarInt(stream, dt.Length);
            stream.Write(dt);
        }
    }
}

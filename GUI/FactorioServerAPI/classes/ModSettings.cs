using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FactorioServerAPI
{
    public struct ModSetting
    {
        public byte type;
        public string name;
        public object value;

        public ModSetting(byte type, object value) : this()
        {
            this.type = type;
            this.value = value;
        }

        public static ModSetting Read(BinaryReader stream)
        {
            ModSetting ret = new ModSetting();
            ret.type = stream.ReadByte();
            switch (ret.type)
            {
                case 1: goto case 4;
                case 2: goto case 4;
                case 3: goto case 4;
                case 4:
                    ret.name = Reader.ReadComplexString(stream);
                    break;
                default:
                    throw new IOException("Wrong setting type");
            }
            switch (ret.type)
            {
                case 1:
                    ret.value = stream.ReadBoolean();
                    break;
                case 2:
                    ret.value = stream.ReadDouble();
                    break;
                case 3:
                    ret.value = stream.ReadInt64();
                    break;
                case 4:
                    ret.value = Reader.ReadComplexString(stream);
                    break;
                default:
                    throw new IOException("Wrong setting type");
            }
            return ret;
        }
        public void Write(BinaryWriter stream)
        {
            throw new NotImplementedException();
        }
    }
}

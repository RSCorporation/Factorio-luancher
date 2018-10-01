using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FactorioServerAPI
{
    public struct ModInfo
    {
        public string name;
        public Version version;
        public int crc;

        public ModInfo(string name, Version version, int crc)
        {
            this.name = name;
            this.version = version;
            this.crc = crc;
        }
        public static ModInfo Read(BinaryReader stream)
        {
            ModInfo ret = new ModInfo();
            ret.name = Reader.ReadString(stream);
            ret.version = Version.Read(stream);
            ret.crc = stream.ReadInt32();
            return ret;
        }
        public void Write(BinaryWriter stream)
        {
            Reader.WriteString(stream, name);
            version.Write(stream);
            stream.Write(crc);
        }
        public override string ToString()
        {
            return name + " " + version;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FactorioServerAPI
{
    public struct ModInfo
    {
        public string Name { get; set; }
        public Version Version { get; set; }
        public int crc;

        public ModInfo(string name, Version version, int crc)
        {
            this.Name = name;
            this.Version = version;
            this.crc = crc;
        }
        public static ModInfo Read(BinaryReader stream)
        {
            ModInfo ret = new ModInfo();
            ret.Name = Reader.ReadString(stream);
            ret.Version = Version.Read(stream);
            ret.crc = stream.ReadInt32();
            return ret;
        }
        public void Write(BinaryWriter stream)
        {
            Reader.WriteString(stream, Name);
            Version.Write(stream);
            stream.Write(crc);
        }
        public override string ToString()
        {
            return Name + " " + Version;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ModInfo))
            {
                return false;
            }

            var info = (ModInfo)obj;
            return Name == info.Name &&
                   EqualityComparer<Version>.Default.Equals(Version, info.Version);
        }

        public override int GetHashCode()
        {
            var hashCode = 1545369197;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<Version>.Default.GetHashCode(Version);
            return hashCode;
        }

        public static bool operator ==(ModInfo mod1, ModInfo mod2)
        {
            return mod1.Name == mod2.Name && mod1.Version == mod2.Version;
        }
        public static bool operator !=(ModInfo mod1, ModInfo mod2)
        {
            return !(mod1 == mod2);
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FactorioServerAPI
{
    public struct Version
    {
        public short majorVersion;
        public short minorVersion;
        public short subVersion;

        public Version(short majorVersion, short minorVersion, short subVersion)
        {
            this.majorVersion = majorVersion;
            this.minorVersion = minorVersion;
            this.subVersion = subVersion;
        }

        public static Version Read(BinaryReader stream)
        {
            Version ret = new Version();
            ret.majorVersion = Reader.ReadVarShort(stream);
            ret.minorVersion = Reader.ReadVarShort(stream);
            ret.subVersion = Reader.ReadVarShort(stream);
            return ret;
        }
        public void Write(BinaryWriter stream)
        {
            Reader.WriteVarShort(stream, majorVersion);
            Reader.WriteVarShort(stream, minorVersion);
            Reader.WriteVarShort(stream, subVersion);
        }
        public override string ToString()
        {
            return majorVersion + "." + minorVersion + "." + subVersion;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Version))
            {
                return false;
            }

            var version = (Version)obj;
            return majorVersion == version.majorVersion &&
                   minorVersion == version.minorVersion &&
                   subVersion == version.subVersion;
        }

        public override int GetHashCode()
        {
            var hashCode = -2132793737;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + majorVersion.GetHashCode();
            hashCode = hashCode * -1521134295 + minorVersion.GetHashCode();
            hashCode = hashCode * -1521134295 + subVersion.GetHashCode();
            return hashCode;
        }

        public static bool operator == (Version ver1, Version ver2)
        {
            return ver1.majorVersion == ver2.majorVersion && ver1.minorVersion == ver2.minorVersion && ver1.subVersion == ver2.subVersion;
        }
        public static bool operator != (Version ver1, Version ver2)
        {
            return !(ver1 == ver2);
        }
    }
}

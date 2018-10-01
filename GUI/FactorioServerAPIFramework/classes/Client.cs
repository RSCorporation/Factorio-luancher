using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FactorioServerAPI
{
    public struct Client
    {
        public short peerId;
        public string Username { get; set; }
        public byte droppingProgrss;
        public byte mapSavingProgress;
        public byte mapDownloadingProgress;
        public byte mapLoadingProgress;
        public byte tryingToCatchUpProgress;

        public static Client Read(BinaryReader stream)
        {
            Client ret = new Client();
            ret.peerId = Reader.ReadVarShort(stream);
            ret.Username = Reader.ReadString(stream);
            byte flags = stream.ReadByte();
            if ((flags & 0x01) > 0)
                ret.droppingProgrss = stream.ReadByte();
            if ((flags & 0x02) > 0)
                ret.mapSavingProgress = stream.ReadByte();
            if ((flags & 0x04) > 0)
                ret.mapDownloadingProgress = stream.ReadByte();
            if ((flags & 0x08) > 0)
                ret.mapLoadingProgress = stream.ReadByte();
            if ((flags & 0x10) > 0)
                ret.tryingToCatchUpProgress = stream.ReadByte();
            return ret;
        }
        public void Write(BinaryWriter stream)
        {
            throw new NotImplementedException();
        }
        public override string ToString()
        {
            return "peerId: " + peerId + "; username: " + Username;
        }
    }
}

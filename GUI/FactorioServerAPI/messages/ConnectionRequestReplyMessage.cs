using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FactorioServerAPI
{
    class ConnectionRequestReplyMessage : ServerMessage
    {
        public Version version;
        public short buildVersion;
        public int connectionRequestIDGeneratedOnClient;
        public int connectionRequestIDGeneratedOnServer;
        public override void FromBytes(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            BinaryReader stream = new BinaryReader(ms);

            version = Version.Read(stream);
            buildVersion = stream.ReadInt16();
            connectionRequestIDGeneratedOnClient = stream.ReadInt32();
            connectionRequestIDGeneratedOnServer = stream.ReadInt32();
        }

        public override byte GetMessageType()
        {
            return 3;
        }
    }
}

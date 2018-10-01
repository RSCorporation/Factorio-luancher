using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FactorioServerAPI
{
    class ConnectionRequestMessage : ClientMessage
    {
        public Version version;
        public short buildVersion;
        public int connectionRequestIDGeneratedOnClient;
        public override byte[] GetBytes()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter stream = new BinaryWriter(ms);
            version.Write(stream);
            stream.Write(buildVersion);
            stream.Write(connectionRequestIDGeneratedOnClient);
            return ms.ToArray();
        }

        public override byte GetMessageType()
        {
            return 2;
        }
    }
}

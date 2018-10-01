using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FactorioServerAPI
{
    class ConnectionRequestReplyConfirmMessage : ClientMessage
    {
        public int connectionRequestIDGeneratedOnClient;
        public int connectionRequestIDGeneratedOnServer;
        public int instanceID;
        public string username = "factoriolauncher";
        public string passwordHash = "";
        public string serverKey = "";
        public string serverKeyTimestamp = "";
        public int coreChecksum;
        public ModInfo[] mods = new ModInfo[0];
        public ModSetting[] modSettings = new ModSetting[0];
        public override byte[] GetBytes()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter stream = new BinaryWriter(ms);

            stream.Write(connectionRequestIDGeneratedOnClient);
            stream.Write(connectionRequestIDGeneratedOnServer);
            stream.Write(instanceID);
            Reader.WriteString(stream, username);
            Reader.WriteString(stream, passwordHash);
            Reader.WriteString(stream, serverKey);
            Reader.WriteString(stream, serverKeyTimestamp);
            stream.Write(coreChecksum);
            Reader.WriteVarInt(stream, mods.Length);
            foreach (ModInfo mod in mods)
                mod.Write(stream);
            Reader.WriteVarInt(stream, modSettings.Length);
            foreach (ModSetting modSetting in modSettings)
                modSetting.Write(stream);
            return ms.ToArray();
        }

        public override byte GetMessageType()
        {
            return 4;
        }
    }
}

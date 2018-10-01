using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FactorioServerAPI
{
    public class ConnectionAcceptOrDenyMessage : ServerMessage
    {
        public bool online;

        public int clientrequestId;
        public byte status;
        public string gameName;
        public string serverHash;
        public string description;
        public byte latency;
        public int gameId;

        public string serverUsername;
        public byte mapSavingProgress;
        public short var0;
        public Client[] clients;

        public int firstSequenceNumberToExpect;
        public int firstSequenceNumberToSend;
        public short newPeerId;

        public ModInfo[] mods;
        public ModSetting[] modSettings;

        public short pausedBy;

        public int lanGameId;
        public string name;
        public Version applicationVersion;
        public short buildVersion;
        public string serverDescription;
        public short maxPlayers;
        public int gameTimeElapsed;
        public bool hasPassword;
        public string hostAddress;

        public string[] tags;

        public string serverUsername1;
        public int autosaveInterval;
        public int autosaveSlots;
        public int AFKAutoKickInterval;
        public bool allowCommands;
        public int maxUploadInKilobytesPerSecond;
        public byte minimumLatencyInTicks;
        public bool ignorePlayerLimitForReturnongPlayers;
        public bool onlyAdminsCanPauseTheGame;
        public bool requireUserVerification;

        public string[] admins;

        public ListItem[] whitelist;
        public AddressToUsernameMapping[] whitelistMappings;

        public ListItem[] banlist;
        public AddressToUsernameMapping[] banlistMappings;

        public override void FromBytes(byte[] data)
        {
            MemoryStream ms = new MemoryStream(data);
            BinaryReader stream = new BinaryReader(ms);

            clientrequestId = stream.ReadInt32();
            status = stream.ReadByte();
            gameName = Reader.ReadString(stream);
            serverHash = Reader.ReadString(stream);
            description = Reader.ReadString(stream);
            latency = stream.ReadByte();
            gameId = stream.ReadInt32();

            serverUsername = Reader.ReadString(stream);
            mapSavingProgress = stream.ReadByte();
            var0 = Reader.ReadVarShort(stream);
            short clientscnt = Reader.ReadVarShort(stream);
            clients = new Client[clientscnt];
            for (int i = 0; i < clientscnt; i++)
                clients[i] = Client.Read(stream);

            firstSequenceNumberToExpect = stream.ReadInt32();
            firstSequenceNumberToSend = stream.ReadInt32();
            newPeerId = stream.ReadInt16();

            short modscnt = Reader.ReadVarShort(stream);
            mods = new ModInfo[modscnt];
            for (int i = 0; i < modscnt; i++)
                mods[i] = ModInfo.Read(stream);
            short modsettingscnt = Reader.ReadVarShort(stream);
            modSettings = new ModSetting[modsettingscnt];
            for (int i = 0; i < modsettingscnt; i++)
                modSettings[i] = ModSetting.Read(stream);

            pausedBy = stream.ReadInt16();

            lanGameId = stream.ReadInt32();
            name = Reader.ReadString(stream);
            applicationVersion = Version.Read(stream);
            buildVersion = stream.ReadInt16();
            serverDescription = Reader.ReadString(stream);
            maxPlayers = stream.ReadInt16();
            gameTimeElapsed = stream.ReadInt32();
            hasPassword = stream.ReadBoolean();
            int hostaddrln = stream.ReadInt32();
            byte[] hostaddrstrb = new byte[hostaddrln];
            stream.Read(hostaddrstrb, 0, hostaddrln);
            hostAddress = Encoding.UTF8.GetString(hostaddrstrb);

            int tagscnt = Reader.ReadVarInt(stream);
            tags = new string[tagscnt];
            for (int i = 0; i < tagscnt; i++)
                tags[i] = Reader.ReadString(stream);
            serverUsername1 = Reader.ReadString(stream);
            autosaveInterval = stream.ReadInt32();
            autosaveSlots = stream.ReadInt32();
            AFKAutoKickInterval = stream.ReadInt32();
            allowCommands = stream.ReadBoolean();
            maxUploadInKilobytesPerSecond = stream.ReadInt32();
            minimumLatencyInTicks = stream.ReadByte();
            ignorePlayerLimitForReturnongPlayers = stream.ReadBoolean();
            onlyAdminsCanPauseTheGame = stream.ReadBoolean();
            requireUserVerification = stream.ReadBoolean();

            int adminscnt = Reader.ReadVarInt(stream);
            admins = new string[adminscnt];
            for (int i = 0; i < adminscnt; i++)
                admins[i] = Reader.ReadString(stream);

            int whitelistcnt = Reader.ReadVarInt(stream);
            whitelist = new ListItem[whitelistcnt];
            for (int i = 0; i < whitelistcnt; i++)
                whitelist[i] = ListItem.Read(stream);
            int whitelistmappingcnt = Reader.ReadVarInt(stream);
            whitelistMappings = new AddressToUsernameMapping[whitelistmappingcnt];
            for (int i = 0; i < whitelistmappingcnt; i++)
                whitelistMappings[i] = AddressToUsernameMapping.Read(stream);

            int banlistcnt = Reader.ReadVarInt(stream);
            banlist = new ListItem[banlistcnt];
            for (int i = 0; i < banlistcnt; i++)
                banlist[i] = ListItem.Read(stream);
            int banlistmappingscnt = Reader.ReadVarInt(stream);
            banlistMappings = new AddressToUsernameMapping[banlistmappingscnt];
            for (int i = 0; i < banlistmappingscnt; i++)
                banlistMappings[i] = AddressToUsernameMapping.Read(stream);

            online = true;
        }

        public override byte GetMessageType()
        {
            return 5;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FactorioServerAPI
{
    public class FactorioNetMessage
    {
        public byte[] packetBytes;

        public byte type;
        public short messageId;
        public bool isFragmented;
        public short fragmentId;

        public bool isLastfragment;

        public short currentMessageId;
        public short lastServerMessageId;

        public FactorioNetMessage(byte[] packet)
        {
            BinaryReader stream = new BinaryReader(new MemoryStream(packet));
            int offset = 0;

            type = stream.ReadByte();
            offset++;
            type = (byte)(type & 0b11011111);
            isFragmented = (type & 0b01000000) > 0;
            isLastfragment = (type & 0b10000000) > 0;
            type = (byte)(type & 0b00011111);
            if ((type >= 2 && type <= 5) || isFragmented || isLastfragment)
            {
                messageId = stream.ReadInt16();
                offset += 2;
                if(isLastfragment)
                {
                    if((fragmentId = stream.ReadByte()) == 0xFF)
                    {
                        fragmentId = stream.ReadInt16();
                        offset += 2;
                    }
                    offset++;
                }
            }
            if((messageId & 0b1000000000000000) > 0)
            {
                stream.ReadByte();
                stream.ReadInt32();
                offset += 5;
            }
            messageId = (short)(messageId & 0b0111111111111111);
            packetBytes = new byte[packet.Length - offset];
            stream.Read(packetBytes, 0, packetBytes.Length);
        }

        public FactorioNetMessage(byte type, byte[] data)
        {
            packetBytes = data;
            this.type = type;
        }

        public byte[] GetPacket()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter stream = new BinaryWriter(ms);
            bool isReliable = (type >= 2 && type <= 5);
            if (isFragmented)
                type |= 0b01000000;
            if (isLastfragment)
                type |= 0b10000000;
            stream.Write(type);
            if(isReliable || isFragmented)
            {
                messageId = currentMessageId;
                stream.Write((short)(type | (lastServerMessageId > 0 ? 0x8000 : 0)));
                if(isFragmented)
                {
                    Reader.WriteVarShort(stream, fragmentId);
                }
            }
            if(lastServerMessageId > 0)
            {
                stream.Write((byte)1);
                stream.Write((int)lastServerMessageId);
            }
            stream.Write(packetBytes);
            return ms.ToArray();
        }
        public FactorioNetMessage() { }
    }
}

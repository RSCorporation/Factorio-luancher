using System;
using System.Collections.Generic;
using System.Text;

namespace FactorioServerAPI
{
    public abstract class ServerMessage
    {
        public abstract void FromBytes(byte[] data);
        public abstract byte GetMessageType();
    }
}

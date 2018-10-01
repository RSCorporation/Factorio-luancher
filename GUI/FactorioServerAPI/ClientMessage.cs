using System;
using System.Collections.Generic;
using System.Text;

namespace FactorioServerAPI
{
    public abstract class ClientMessage
    {
        public abstract byte[] GetBytes();
        public abstract byte GetMessageType();
    }
}

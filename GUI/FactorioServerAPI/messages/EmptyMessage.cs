using System;
using System.Collections.Generic;
using System.Text;

namespace FactorioServerAPI
{
    class EmptyMessage : ClientMessage
    {
        public override byte[] GetBytes()
        {
            return new byte[0];
        }
        public override byte GetMessageType()
        {
            return 0x12;
        }
    }
}

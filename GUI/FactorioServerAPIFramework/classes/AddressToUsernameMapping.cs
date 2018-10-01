using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FactorioServerAPI
{
    public struct AddressToUsernameMapping
    {
        public string username;
        public string address;

        public static AddressToUsernameMapping Read(BinaryReader stream)
        {
            AddressToUsernameMapping ret = new AddressToUsernameMapping();
            ret.username = Reader.ReadComplexString(stream);
            ret.username = Reader.ReadComplexString(stream);
            return ret;
        }
        public void Write(BinaryWriter stream)
        {
            throw new NotImplementedException();
        }
    }
}

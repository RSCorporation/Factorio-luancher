using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace FactorioServerAPI
{
    public struct ListItem
    {
        public string username;
        public string reason;
        public string address;

        public static ListItem Read(BinaryReader stream)
        {
            ListItem i = new ListItem();
            i.username = Reader.ReadString(stream);
            i.reason = Reader.ReadString(stream);
            i.address = Reader.ReadString(stream);
            return i;
        }
        public static void Write(BinaryWriter stream)
        {
            throw new NotImplementedException();
        }
        public override string ToString()
        {
            return username + ":" + reason + ":" + address;
        }
    }
}

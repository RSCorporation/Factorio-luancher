using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace FactorioServerAPI
{
    public struct ListItem
    {
        public string Username { get; set; }
        public string Reason { get; set; }
        public string address;

        public static ListItem Read(BinaryReader stream)
        {
            ListItem i = new ListItem();
            i.Username = Reader.ReadString(stream);
            i.Reason = Reader.ReadString(stream);
            i.address = Reader.ReadString(stream);
            return i;
        }
        public static void Write(BinaryWriter stream)
        {
            throw new NotImplementedException();
        }
        public override string ToString()
        {
            return Username + ":" + Reason + ":" + address;
        }
    }
}

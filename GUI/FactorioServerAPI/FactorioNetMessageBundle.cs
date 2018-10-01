using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FactorioServerAPI
{
    class FactorioNetMessageBundle
    {
        SortedList<short, FactorioNetMessage> bundleMessages = new SortedList<short, FactorioNetMessage>();
        int needCount = -1;
        public bool HandleBundleMessage(FactorioNetMessage message)
        {
            if (needCount == bundleMessages.Count) return true;
            if(message.isLastfragment)
                needCount = message.fragmentId + 1;
            bundleMessages.Add(message.fragmentId, message);
            return needCount == bundleMessages.Count();
        }

        public FactorioNetMessage GetOverallMessage()
        {
            if (needCount != bundleMessages.Count) return null;
            int overallsize = 0;
            foreach(FactorioNetMessage netmsg in bundleMessages.Values)
            {
                overallsize += netmsg.packetBytes.Length;
            }
            byte[] bytes = new byte[overallsize];
            int arrayptr = 0;
            foreach(FactorioNetMessage netmsg in bundleMessages.Values)
            {
                Array.Copy(netmsg.packetBytes, 0, bytes, arrayptr, netmsg.packetBytes.Length);
                arrayptr += netmsg.packetBytes.Length;
            }
            return new FactorioNetMessage(bundleMessages.First().Value.type, bytes);
        }
    }
}

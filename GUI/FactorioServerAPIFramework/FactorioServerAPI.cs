using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Linq;

namespace FactorioServerAPI
{
    public static class FactorioServerAPI
    {
        static Socket socket;

        static int state = 0;
        static bool working = false;
        static int lastServerMessageId;
        static short currentMessageId;
        static ConnectionAcceptOrDenyMessage serverInfo;
        static Dictionary<short, FactorioNetMessageBundle> fragmentedpackets = new Dictionary<short, FactorioNetMessageBundle>();

        public static ConnectionAcceptOrDenyMessage GetServerInfo(string address)
        {
            string[] substrs = address.Split(':');
            if (substrs.Length > 2) throw new FormatException();
            string ip = substrs[0];
            int port = (substrs.Length == 2 ? int.Parse(substrs[1]) : 34197);
            IPAddress serverIp = Dns.GetHostAddresses(ip)[0];
            IPEndPoint endPoint = new IPEndPoint(serverIp, port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.ReceiveTimeout = 1000;
            socket.Connect(endPoint);
            
            Thread listener = new Thread(() =>
            {
                while (working)
                {
                    if (socket == null) continue;
                    byte[] packet = new byte[1024];
                    try
                    {
                        int length = socket.Receive(packet);
                        HandlePacket(packet.Take(length).ToArray());
                    }
                    catch (ThreadAbortException)
                    {
                        break;
                    }
                    catch (SocketException)
                    {

                    }
                    catch (Exception ex)
                    {
                        state = 2;
                    }
                }
            });

            working = true;
            listener.IsBackground = true;
            listener.Start();
            ClientMessage message = new ConnectionRequestMessage();
            SendMessage(message);
            while(state == 0)
            {
                Thread.Sleep(1);
            }
            working = false;
            listener.Abort();
            return serverInfo;
        }
        static void SendMessage(ClientMessage message)
        {
            currentMessageId++;
            byte[] messagedata = message.GetBytes();
            FactorioNetMessage[] messages;
            if (messagedata.Length > 500)
            {
                messages = new FactorioNetMessage[messagedata.Length / 500 + 1];
                for (int i = 0; i < messagedata.Length / 500 + 1; i++)
                {
                    messages[i] = new FactorioNetMessage();
                    messages[i].isFragmented = true;
                    messages[i].isLastfragment = (i == messagedata.Length / 500);
                    messages[i].fragmentId = (short)(i + 1);
                    messages[i].type = message.GetMessageType();
                    if (messages[i].isLastfragment)
                        messages[i].packetBytes = new byte[messagedata.Length % 500];
                    else
                        messages[i].packetBytes = new byte[500];
                    Array.Copy(messagedata, messages[i].packetBytes, messages[i].packetBytes.Length);
                }
            }
            else
            {
                messages = new FactorioNetMessage[1];
                messages[0] = new FactorioNetMessage();
                messages[0].type = message.GetMessageType();
                messages[0].packetBytes = message.GetBytes();
            }
            foreach (FactorioNetMessage msg in messages)
            {
                msg.currentMessageId = currentMessageId;
                msg.lastServerMessageId = (short)lastServerMessageId;
                socket.Send(msg.GetPacket());
            }
        }
        static void HandlePacket(byte[] packet)
        {
            FactorioNetMessage netmsg = new FactorioNetMessage(packet);
            lastServerMessageId = netmsg.messageId;
            if(netmsg.isFragmented)
            {
                if(!fragmentedpackets.ContainsKey(netmsg.messageId))
                {
                    fragmentedpackets.Add(netmsg.messageId, new FactorioNetMessageBundle());
                }
                if (fragmentedpackets[netmsg.messageId].HandleBundleMessage(netmsg))
                {
                    HandleMessage(netmsg.type, fragmentedpackets[netmsg.messageId].GetOverallMessage().packetBytes);
                    fragmentedpackets.Remove(netmsg.messageId);
                }
            }
            else
            {
                HandleMessage(netmsg.type, netmsg.packetBytes);
            }
        }
        static void HandleMessage(byte type, byte[] data)
        {
            switch (type)
            {
                case 3:
                    ConnectionRequestReplyConfirmMessage message = new ConnectionRequestReplyConfirmMessage();
                    ConnectionRequestReplyMessage tmsg = new ConnectionRequestReplyMessage();
                    tmsg.FromBytes(data);
                    message.connectionRequestIDGeneratedOnServer = tmsg.connectionRequestIDGeneratedOnServer;
                    SendMessage(message);
                    break;
                case 5:
                    type = 5;
                    if(state == 0)
                    {
                        ConnectionAcceptOrDenyMessage msg = new ConnectionAcceptOrDenyMessage();
                        msg.FromBytes(data);
                        serverInfo = msg;
                    }
                    state = 1;
                    break;
            }
        }
    }
}

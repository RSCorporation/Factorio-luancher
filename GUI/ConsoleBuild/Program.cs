using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FactorioServerAPI;
namespace ConsoleBuild
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionAcceptOrDenyMessage sinf = args.Length > 0 ? FactorioServerAPI.FactorioServerAPI.GetServerInfo(args[0]) ?? new ConnectionAcceptOrDenyMessage() : FactorioServerAPI.FactorioServerAPI.GetServerInfo(Console.ReadLine()) ?? new ConnectionAcceptOrDenyMessage();
            if (!sinf.online)
            {
                Console.WriteLine("OFFLINE");
                if (args.Length == 0) Console.ReadKey();
                return;
            }
            Console.WriteLine("ONLINE");
            Console.WriteLine("\nServer options:");
            Console.WriteLine("|========================|========================|");
            Console.WriteLine("|Option                  |                   Value|");
            Console.WriteLine("|------------------------|------------------------|");
            WriteConsoleRaw("Name", sinf.name);
            WriteConsoleRaw("Description", sinf.serverDescription);
            WriteConsoleRaw("Latency", sinf.latency + "ms");
            WriteConsoleRaw("Paused By", (from i in sinf.clients where i.peerId == sinf.pausedBy select i.Username).FirstOrDefault());
            WriteConsoleRaw("Game Version", sinf.applicationVersion);
            WriteConsoleRaw("Maximum players", sinf.maxPlayers);
            WriteConsoleRaw("Game time", new TimeSpan(sinf.gameTimeElapsed * 10000000L));
            WriteConsoleRaw("Has password", sinf.hasPassword);
            WriteConsoleRaw("Autosave interval", sinf.autosaveInterval + "min");
            WriteConsoleRaw("AFK autokick interval", sinf.AFKAutoKickInterval == 0 ? (object)"Disabled" : (object)sinf.AFKAutoKickInterval);
            WriteConsoleRaw("Can everybody use lua", sinf.allowCommands);
            WriteConsoleRaw("Can everybody pause game", !sinf.onlyAdminsCanPauseTheGame);
            WriteConsoleRaw("Needs authorization", sinf.requireUserVerification);
            Console.WriteLine("|========================|========================|");

            Console.WriteLine("\nMods:");
            Console.WriteLine("|========================|========================|");
            Console.WriteLine("|Mod                     |                 Version|");
            Console.WriteLine("|------------------------|------------------------|");
            foreach (var i in sinf.mods)
            {
                WriteConsoleRaw(i.Name, i.Version);
            }
            Console.WriteLine("|========================|========================|");

            Console.WriteLine("\nWhitelist:");
            if (sinf.whitelist.Length == 0) Console.WriteLine("Disabled");
            foreach (var i in sinf.whitelist)
            {
                Console.WriteLine(i.Username);
            }

            Console.WriteLine("\nBanlist:");
            if (sinf.banlist.Length == 0) Console.WriteLine("Empty");
            foreach (var i in sinf.banlist)
            {
                Console.WriteLine(i.Username + "; reason: " + i.Reason);
            }

            Console.WriteLine("\nAdmins: ", sinf.admins.Length);
            foreach (var i in sinf.admins)
            {
                Console.WriteLine(i);
            }

            Console.WriteLine("\n{0} players online:", sinf.clients.Length);
            foreach (var i in sinf.clients)
            {
                Console.WriteLine(i.Username);
            }
            if (args.Length == 0) Console.ReadKey();
        }
        static bool color1 = true;
        static void WriteConsoleRaw(string param, object value)
        {
            Console.ForegroundColor = color1 ? ConsoleColor.Yellow : ConsoleColor.Red;
            color1 = !color1;
            string val;
            if (value == null) val = "";
            else val = value is bool ? (bool)value ? "Yes" : "No" : value.ToString();
            string[] param_s = new string[param.Length / 24 + 1 - (param.Length % 24 == 0 ? 1 : 0)];
            for (int i = 0; i < param_s.Length; i++)
            {
                param_s[i] = param.Substring(i * 24, Math.Min(24, param.Length - i * 24));
            }
            string[] vals = new string[val.Length / 24 + 1 - (val.Length % 24 == 0 ? 1 : 0)];
            for (int i = 0; i < vals.Length; i++)
            {
                vals[i] = val.Substring(i * 24, Math.Min(24, val.Length - i * 24));
            }
            for (int i = 0; i < Math.Max(param_s.Length, vals.Length); i++)
            {
                string op1 = i < param_s.Length ? param_s[i] : "";
                string op2 = i < vals.Length ? vals[i] : "";
                Console.WriteLine("|{0,-24}|{1,24}|", op1, op2);
            }
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}

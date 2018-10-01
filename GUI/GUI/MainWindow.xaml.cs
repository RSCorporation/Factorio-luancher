using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.Serialization.Json;
using FactorioServerAPI;

namespace GUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        delegate void UIUpdateDelegate();
        List<ModInfo> installedmods;
        DirectoryInfo mods;
        WebClient client = new WebClient();
        bool __GLOBALOKTRANSFER = false;
        public MainWindow()
        {
            InitializeComponent();
        }
        private void __loadmods()
        {
            installedmods = new List<ModInfo>();
            mods = new DirectoryInfo("../../mods");
            if (!mods.Exists) mods = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/Factorio/mods");
            var mods_struct = mods.GetFiles().Where(i => i.Extension == ".zip");
            Dispatcher.Invoke(new UIUpdateDelegate(() =>
            {
                LoadingProgress.Maximum = mods_struct.Count();
            }), null);
            foreach (var i in mods_struct)
            {
                Dispatcher.Invoke(new UIUpdateDelegate(() =>
                {
                    InfoLabel.Content = "Loading mods... " + i.Name;
                }), null);
                var _ = new FileStream(i.FullName, FileMode.Open, FileAccess.Read);
                ZipArchive mod = new ZipArchive(_, ZipArchiveMode.Read);
                ZipArchiveEntry obj = null;
                foreach (var j in mod.Entries)
                {
                    if (j.Name == "info.json")
                    {
                        obj = j;
                        break;
                    }
                }
                if (obj == null)
                {
                    Dispatcher.Invoke(new UIUpdateDelegate(() =>
                    {
                        LoadingProgress.Maximum--;
                    }), null);
                    continue;
                }
                StreamReader modinfo = new StreamReader(obj.Open());
                ModInfo currmod = new ModInfo();
                while (!modinfo.EndOfStream)
                {
                    string[] line = modinfo.ReadLine().Split(':');
                    if (line.Length != 2) continue;
                    line[0] = line[0].Trim(' ', '\t', '\n', '\r', ',');
                    line[1] = line[1].Trim(' ', '\t', '\n', '\r', ',');
                    if (line[0] == "\"name\"")
                    {
                        currmod.Name = line[1].Trim('\"');
                    }
                    else if (line[0] == "\"version\"")
                    {
                        string[] vernums = line[1].Trim('\"').Split('.');
                        currmod.Version = new FactorioServerAPI.Version(short.Parse(vernums[0]), short.Parse(vernums[1]), short.Parse(vernums[2]));
                    }
                }
                modinfo.Close();
                if (currmod.Name != null && currmod.Version.minorVersion + currmod.Version.majorVersion + currmod.Version.subVersion > 0)
                {
                    installedmods.Add(currmod);
                    Dispatcher.Invoke(new UIUpdateDelegate(() =>
                    {
                        LoadingProgress.Value++;
                    }), null);
                }
                else
                {
                    Dispatcher.Invoke(new UIUpdateDelegate(() =>
                    {
                        LoadingProgress.Maximum--;
                    }), null);
                }
                _.Close();
            }
            Dispatcher.Invoke(new UIUpdateDelegate(() =>
            {
                OverlayGrid.Visibility = Visibility.Hidden;
            }), null);
        }
        private void CheckServerButton_Click(object sender, RoutedEventArgs e)
        {
            CheckServerButton.IsEnabled = false;
            CheckServerButton.Opacity = 0.3;
            OnlineListView.Items.Clear();
            ModsListView.Items.Clear();
            AdminsListView.Items.Clear();
            WhiteListView.Items.Clear();
            BanListView.Items.Clear();
            TopStatusLabel.Content = "";
            Thread DEMON = new Thread(new ParameterizedThreadStart(__checkserverbuttonclick));
            DEMON.IsBackground = true;
            DEMON.Start(AddressInputField.Text);
        }
        private void __checkserverbuttonclick(object address)
        {
            Exception e = null;
            ConnectionAcceptOrDenyMessage sinf;
            try
            {
                sinf = FactorioServerAPI.FactorioServerAPI.GetServerInfo(address as string);
            }
            catch (System.Net.Sockets.SocketException)
            {
                MessageBox.Show("Invalid ip");
                Dispatcher.Invoke(new UIUpdateDelegate(() =>
                {
                    CheckServerButton.IsEnabled = true;
                    CheckServerButton.Opacity = 1;
                }), new object[0]);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Dispatcher.Invoke(new UIUpdateDelegate(() =>
                {
                    CheckServerButton.IsEnabled = true;
                    CheckServerButton.Opacity = 1;
                }), new object[0]);
                return;
            }
            Dispatcher.Invoke(new UIUpdateDelegate(() =>
            {
                if (e != null)
                {
                    TopStatusLabel.Content = e;
                }
                else if (sinf.online)
                {
                    TopStatusLabel.Content = sinf.name + " is online";
                }
                else
                {
                    TopStatusLabel.Content = address as string + " is offline";
                }
            }), new object[0]);
            if (e != null || !sinf.online)
            {
                Dispatcher.Invoke(new UIUpdateDelegate(() =>
                {
                    CheckServerButton.IsEnabled = true;
                    CheckServerButton.Opacity = 1;
                }), new object[0]);
                return;
            }
            Dispatcher.Invoke(new UIUpdateDelegate(() =>
            {
                foreach (var i in sinf.clients)
                {
                    OnlineListView.Items.Add(i);
                }
                foreach (var i in sinf.mods)
                {
                    ModsListView.Items.Add(i);
                }
                foreach (var i in sinf.admins)
                {
                    AdminsListView.Items.Add(i);
                }
                foreach (var i in sinf.whitelist)
                {
                    WhiteListView.Items.Add(i);
                }
                foreach (var i in sinf.banlist)
                {
                    BanListView.Items.Add(i);
                }
                LeftInfoTextBlock.Text += "Server name: " + sinf.name + "\n";
                LeftInfoTextBlock.Text += "Description: " + sinf.serverDescription + "\n";
                LeftInfoTextBlock.Text += "Game version: " + sinf.applicationVersion.ToString() + "\n";
                LeftInfoTextBlock.Text += "Maximum players: " + (sinf.maxPlayers == 0 ? "Unlimited" : sinf.maxPlayers.ToString()) + "\n";
                LeftInfoTextBlock.Text += "Game time: " + new TimeSpan(sinf.gameTimeElapsed * 10000000L).ToString() + "\n";
                LeftInfoTextBlock.Text += "Has password: " + (sinf.hasPassword ? "Yes" : "No") + "\n";
                LeftInfoTextBlock.Text += "Autosave interval: " + (sinf.autosaveInterval == 0 ? "Disabled" : sinf.autosaveInterval.ToString() + "min") + "\n";
                LeftInfoTextBlock.Text += "AFK autokick interval: " + (sinf.AFKAutoKickInterval == 0 ? "Disabled" : sinf.AFKAutoKickInterval.ToString() + "min") + "\n";
                LeftInfoTextBlock.Text += "Can everybody use commands: " + (sinf.allowCommands ? "Yes" : "No") + "\n";
                LeftInfoTextBlock.Text += "Only admins can pause: " + (sinf.onlyAdminsCanPauseTheGame ? "Yes" : "No") + "\n";
                CheckServerButton.IsEnabled = true;
                CheckServerButton.Opacity = 0.9;
            }), new object[0]);
        }
        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            Thread DEMON = new Thread(new ParameterizedThreadStart(__connectbuttonclick));
            DEMON.IsBackground = true;
            DEMON.Start(AddressInputField.Text);
        }
        private void __connectbuttonclick(object address)
        {
            __syncmodsbuttonclick(address);
            if (!__GLOBALOKTRANSFER) return;
            try
            {
                Process factorioprocess = Process.Start("factorio.exe", "--mp-connect " + address as string);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void SyncModsButton_Click(object sender, RoutedEventArgs e)
        {
            Thread DEMON = new Thread(new ParameterizedThreadStart(__syncmodsbuttonclick));
            DEMON.IsBackground = true;
            DEMON.Start(AddressInputField.Text);
        }
        private void __syncmodsbuttonclick(object address)
        {
            bool ok = true;
            ConnectionAcceptOrDenyMessage sinf;
            try
            {
                sinf = FactorioServerAPI.FactorioServerAPI.GetServerInfo(address as string);
            }
            catch (System.Net.Sockets.SocketException)
            {
                MessageBox.Show("Invalid ip");
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }
            {
                Exception e = null;
                try
                {
                    sinf = FactorioServerAPI.FactorioServerAPI.GetServerInfo(address as string);
                }
                catch (Exception ex)
                {
                    e = ex;
                }
                if (e != null || !sinf.online)
                {
                    ok = false;
                }
            }
            if (!ok)
            {
                MessageBox.Show("Something is wrong with server", "FL info", MessageBoxButton.OK);
                Dispatcher.Invoke(new UIUpdateDelegate(() =>
                {
                    OverlayGrid.Visibility = Visibility.Hidden;
                }), null);
                return;
            }
            var needstoinstall = (from i in sinf.mods where !installedmods.Contains(i) && i.Name != "base" select i);
            Dispatcher.Invoke(new UIUpdateDelegate(() =>
            {
                LoadingProgress.Maximum = needstoinstall.Count();
                LoadingProgress.Value = 0;
                OverlayGrid.Visibility = Visibility.Visible;
            }), null);
            foreach (var i in needstoinstall)
            {
                Dispatcher.Invoke(new UIUpdateDelegate(() =>
                {
                    InfoLabel.Content = "Downloading mods... " + i.ToString();
                }), null);
                ok = __downloadmod(i);
                if (!ok)
                {
                    MessageBox.Show("Error while downloading mods!");
                    __GLOBALOKTRANSFER = false;
                    break;
                }
                List<ModInfo> modstoremove = new List<ModInfo>();
                foreach (var j in installedmods)
                {
                    if (j.Name == i.Name)
                    {
                        File.Delete(mods.FullName + "/" + j.Name + "_" + j.Version.majorVersion + "." + j.Version.minorVersion + "." + j.Version.subVersion + ".zip");
                        modstoremove.Add(j);
                    }
                }
                installedmods.Add(i);
                foreach(var j in modstoremove)
                {
                    installedmods.Remove(j);
                }
                Dispatcher.Invoke(new UIUpdateDelegate(() =>
                {
                    LoadingProgress.Value++;
                }), null);
            }
            Dispatcher.Invoke(new UIUpdateDelegate(() =>
            {
                OverlayGrid.Visibility = Visibility.Hidden;
            }), null);
            string jsonstring = "{\n  \"mods\": [\n    {\n      \"name\": \"base\",\n      \"enabled\": true\n    }";
            foreach (var i in installedmods)
            {
                jsonstring += ",\n    {\n      \"name\": \"" + i.Name + "\",\n      \"enabled\": ";
                if (sinf.mods.Contains(i))
                {
                    jsonstring += "true\n    }";
                }
                else
                {
                    jsonstring += "false\n    }";
                }
            }
            jsonstring += "\n  ]\n}";
            StreamWriter sw = new StreamWriter(mods.FullName + "/mod-list.json");
            sw.WriteLine(jsonstring);
            sw.Close();
            __GLOBALOKTRANSFER = true;
        }
        private bool __downloadmod(ModInfo mod)
        {
            try
            {
                client.DownloadFile("http://185.63.188.9/mods/" + mod.Name + "/" + mod.Version.majorVersion + "." + mod.Version.minorVersion + "." + mod.Version.subVersion + ".zip", mods.FullName + "/" + mod.Name + "_" + mod.Version.majorVersion + "." + mod.Version.minorVersion + "." + mod.Version.subVersion + ".zip");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Thread DEMON = new Thread(new ThreadStart(__loadmods));
            DEMON.IsBackground = true;
            DEMON.Start();
        }
    }
}

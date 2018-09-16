using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FactorioServerAPI;

namespace GUI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        delegate void UIUpdateDelegate();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CheckServerButton_Click(object sender, RoutedEventArgs e)
        {
            PlayersList.Items.Clear();
            ModsList.Items.Clear();
            Thread DEMON = new Thread(new ParameterizedThreadStart(__checkserverbuttonclick));
            DEMON.IsBackground = true;
            DEMON.Start(AddressInputField.Text);
        }
        private void __checkserverbuttonclick(object address)
        {
            Exception e = null;
            ServerInfo sinf = new ServerInfo();
            try
            {
                sinf = FactorioServerAPI.FactorioServerAPI.GetServerInfo(address as string);
            }
            catch (Exception ex)
            {
                e = ex;
            }
            Dispatcher.Invoke(new UIUpdateDelegate(() =>
            {
                if (e != null)
                {
                    TopLabel.Content = e;
                }
                else if (sinf.state)
                {
                    TopLabel.Content = address as string + " is online";
                }
                else
                {
                    TopLabel.Content = address as string + " is offline";
                }
            }), new object[0]);
            if (e != null) return;
            var players = (from i in sinf.admins select new { Name = i, IsAdmin = "Admin" }).Union(from i in sinf.players select new { Name = i, IsAdmin = "" });
            var modsadapted = from i in sinf.mods select new { Name = i.name, Version = i.MajorVersion + "." + i.MinorVersion + "." + i.AssemblyVersion };
            Dispatcher.Invoke(new UIUpdateDelegate(() =>
            {
                foreach(var i in players)
                {
                    PlayersList.Items.Add(i);
                }
                foreach(var i in modsadapted)
                {
                    ModsList.Items.Add(i);
                }
            }), new object[0]);
        }
    }
}

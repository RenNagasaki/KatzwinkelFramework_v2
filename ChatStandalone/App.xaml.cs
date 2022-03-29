using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace ChatStandalone
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static List<string> Smileys = new List<string>();
        public static ReceiveClient Client { get; set; }
        public static bool Kicked = false;
        public static Framework.ChatUser User { get; set; }
        
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow main = new MainWindow();

            if (main.chat != null)
            {
                main.Show();
            }
        }
    }
}

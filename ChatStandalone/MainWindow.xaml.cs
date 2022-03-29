using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace ChatStandalone
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TaskbarIcon tbi = new TaskbarIcon();
        internal ChatControl chat = null;

        public MainWindow()
        {
            InitializeComponent();
            LoginWindow login = new LoginWindow();
            login.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            bool res = Convert.ToBoolean(login.ShowDialog());

            if (res)
            {
                chat = new ChatControl(this);
                grid_main.Children.Add(chat);
            }
            else
            {
                this.Close();
            }
        }

        internal void ShowNotification(string title, string message)
        {
            if (!this.IsActive)
            {
                tbi.ShowBalloonTip(title, message, BalloonIcon.Info);
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            chat.UpdateRect();
        }
    }
}

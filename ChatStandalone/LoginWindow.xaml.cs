using ChatStandalone.Functions;
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
using System.Windows.Shapes;

namespace ChatStandalone
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            App.Client = new ReceiveClient();
            App.Client.SetUp(App.Client);
        }

        private void label_forgot_MouseUp(object sender, MouseButtonEventArgs e)
        {
            AlertBox.Show("Haha!", "Pech gehabt!", this);
        }

        private void btn_login_Click(object sender, RoutedEventArgs e)
        {
            Framework.ChatUser res = App.Client.Login(text_username.Text, text_password.Password);

            if (res.LoggedIn)
            {
                App.User = res;
                DialogResult = true;
                this.Hide();
            }
            else
            {
                AlertBox.Show("Login failed", "Login failed, wrong Credentials.", this);
            }
        }

        private void btn_register_Click(object sender, RoutedEventArgs e)
        {
            RegisterWindow reg = new RegisterWindow(this);
            bool res = Convert.ToBoolean(reg.ShowDialog());

            if (res)
            {
                AlertBox.Show("Register success", "You successfully registered.\r\nYou may now proceed to Login.", this);
            }
        }
    }
}

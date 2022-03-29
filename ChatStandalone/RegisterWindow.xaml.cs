using ChatStandalone.Functions;
using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChatStandalone
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public RegisterWindow(Window window)
        {
            InitializeComponent();

            this.Owner = window;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        private void btn_register_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(text_username.Text))
            {
                if (!String.IsNullOrEmpty(text_email.Text))
                {
                    if (!String.IsNullOrEmpty(text_fullname.Text))
                    {
                        if (!String.IsNullOrEmpty(text_password.Password))
                        {
                            if (!String.IsNullOrEmpty(text_repeatpassword.Password))
                            {
                                if (text_password.Password == text_repeatpassword.Password)
                                {
                                    RegisterResult res = App.Client.Register(text_username.Text, text_email.Text, text_fullname.Text, text_password.Password);

                                    if (res.Success)
                                    {
                                        DialogResult = true;
                                        this.Hide();
                                    }
                                    else
                                    {
                                        AlertBox.Show("Registering not successfull.", res.Errormessage, this);
                                    }
                                }
                                else
                                {
                                    AlertBox.Show("Passwords do not match.", "The entered passwords do not match.", this);
                                    text_password.Password = "";
                                    text_repeatpassword.Password = "";
                                }
                            }
                            else
                            {
                                AlertBox.Show("Field not filled.", "You did not enter a value for 'Repeat password'.", this);
                            }
                        }
                        else
                        {
                            AlertBox.Show("Field not filled.", "You did not enter a value for 'Password'.", this);
                        }
                    }
                    else
                    {
                        AlertBox.Show("Field not filled.", "You did not enter a value for 'Full Name'.", this);
                    }
                }
                else
                {
                    AlertBox.Show("Field not filled.", "You did not enter a value for 'E-Mail'.", this);
                }
            }
            else
            {
                AlertBox.Show("Field not filled.", "You did not enter a value for 'Username'.", this);
            }
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Hide();
        }
    }
}

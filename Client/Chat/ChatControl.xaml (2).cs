using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;

namespace Client.Chat
{
    /// <summary>
    /// Interaction logic for ChatControl.xaml
    /// </summary>
    public partial class ChatControl : UserControl
    {
        internal ReceiveClient rc = null;
        internal Dictionary<string, ChatUser> lstClients = new Dictionary<string, ChatUser>();
        internal ChatUser activeUser = null;
        private MainWindow main = null;

        public ChatControl(MainWindow main)
        {
            InitializeComponent();

            this.main = main;

            rc = new ReceiveClient();
            rc.Start(rc, MainWindow.username);

            rc.NewNames += new GotNames(rc_NewNames);
            rc.ReceiveMsg += new ReceivedMessage(rc_ReceiveMsg);

            Dispatcher.Invoke(new Action(() => { UpdateRect(); }), DispatcherPriority.ContextIdle, null);
        }

        ~ChatControl()
        {
            rc.Stop(MainWindow.username);
        }

        internal void UpdateRect()
        {
            scroll_chat.Minimum = 0;
            cliprect.Rect = new Rect(0, 0, grid_chatwindow.ActualWidth, grid_chatwindow.ActualHeight);
            scroll_chat.Maximum = grid_chat.ActualHeight - grid_chatwindow.ActualHeight;
        }
        
        void rc_ReceiveMsg(string sender, string msg)
        {
            ChatUser user = lstClients[sender];

            if (user != null)
            {
                user.AddMessage(false, msg);
                string subtext = msg.Length >= 30 ? msg.Replace(Environment.NewLine, " ").Substring(0, 30) + "..." : msg;
                main.ShowNotification("New message!", String.Format("New message from: {0}{2}{1}", sender, subtext, Environment.NewLine));
            }
        }

        void rc_NewNames(object sender, List<string> names)
        {
            List<string> notFound = new List<string>();

            foreach (string user in lstClients.Keys)
            {
                if (!names.Contains(user))
                {
                    notFound.Add(user);
                }
            }

            foreach (string user in notFound)
            {
                ChatUser userc = lstClients[user];

                if (userc != null)
                {
                    if (activeUser == userc)
                    {
                        grid_chat.Children.Clear();
                        text_chatsend.IsEnabled = false;
                        btn_send.IsEnabled = false;
                    }
                    grid_users.Children.Remove(userc);
                    lstClients.Remove(user);
                }
            }

            foreach (string name in names)
            {
                if (!lstClients.ContainsKey(name) && name != MainWindow.username)
                {
                    ChatUser user = new ChatUser(this, name);
                    grid_users.Children.Add(user);
                    lstClients.Add(name, user);
                }
            }

            SetPositions();
        }

        void SetPositions()
        {
            int i = 0;
            ChatUser user = new ChatUser(this, "");
            for (i = 0; i < grid_users.Children.Count; i++)
            {
                user = grid_users.Children[i] as ChatUser;
                if (user != null)
                {
                    user.Margin = new Thickness(5, (60 + 5) * (i) + 5, 5, 5);
                    user.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
                    user.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                }
            }

            //height = ((i - 1) * (app.Height + 5)) + app.Height;
            //scroll_apps.Minimum = 0;
            //scroll_apps.Maximum = height - grid_appointments.ActualHeight;
        }

        private void btn_send_Click(object sender, RoutedEventArgs e)
        {
            String text = new TextRange(text_chatsend.Document.ContentStart, text_chatsend.Document.ContentEnd).Text;
            if (activeUser != null && !String.IsNullOrEmpty(text))
            {
                rc.SendMessage(text, MainWindow.username, activeUser.text_username.Content.ToString());
                text_chatsend.Document.Blocks.Clear();
                activeUser.AddMessage(true, text);
            }
        }

        private void text_chatsend_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift) && e.Key == Key.Enter)
            {
                btn_send_Click(null, null);
                e.Handled = true;
            }
        }

        private void ScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            grid_chat.Margin = new Thickness(grid_chat.Margin.Left, 0, grid_chat.Margin.Right, scroll_chat.Value * -1);
        }

        private void UserControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            scroll_chat.Value += e.Delta * 2;
        }
    }
}

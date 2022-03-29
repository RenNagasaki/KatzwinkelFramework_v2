using ChatStandalone.Functions;
using Framework;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ChatStandalone
{
    /// <summary>
    /// Interaction logic for ChatControl.xaml
    /// </summary>
    public partial class ChatControl : UserControl
    {
        internal Dictionary<string, ChatUserControl> lstClients = new Dictionary<string, ChatUserControl>();
        internal ChatUserControl activeUser = null;
        private MainWindow main = null;

        public ChatControl(MainWindow main)
        {
            InitializeComponent();

            this.main = main;

            LoadSmileyData();

            main.Title = App.User.FullName + " - " + main.Title;
            image_user.Source = App.User.GetProfilePic();
            text_username.Text = App.User.UserName;
            text_desc.AppendText(App.User.Desc);

            App.Client.NewNames += new GotNames(rc_NewNames);
            App.Client.ReceiveMsg += new ReceivedMessage(rc_ReceiveMsg);
            App.Client.Disconnect += new Disconnect(rc_Disconnect);

            App.Client.Start(App.User.UserName);

            Dispatcher.Invoke(new Action(() => { UpdateRect(); }), DispatcherPriority.ContextIdle, null);
        }

        ~ChatControl()
        {
            App.Client.Stop(App.User.UserName);
        }

        void LoadSmileyData()
        {
            img_smileys.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "Resources\\Smileys\\amazing.png"));

            string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + "Resources\\Smileys\\");

            foreach (string file in files)
            {
                if (System.IO.Path.GetExtension(file).ToLower() == ".jpg" || System.IO.Path.GetExtension(file).ToLower() == ".png")
                {
                    BitmapImage img = new BitmapImage(new Uri(file));
                    System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                    image.Source = img;
                    image.Width = 30;
                    image.Margin = new Thickness(0, 0, 0, 0);

                    Button toggle = new Button();
                    toggle.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    toggle.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    toggle.Margin = new Thickness(5, 5, 5, 5);
                    toggle.Content = image;
                    App.Smileys.Add(System.IO.Path.GetFileNameWithoutExtension(file));
                    toggle.Tag = System.IO.Path.GetFileNameWithoutExtension(file);
                    toggle.Height = 30;
                    toggle.Width = 30;
                    toggle.BorderThickness = new Thickness(0);
                    toggle.Background = Brushes.Transparent;
                    toggle.Click += btn_smiley_Clicked;
                    toggle.Focusable = false;

                    grid_smileys.Children.Add(toggle);
                }
            }

            int x = 0;
            int y = 0;
            foreach (Button toggle in grid_smileys.Children)
            {
                toggle.Margin = new Thickness(toggle.Margin.Left + (x * (toggle.Width + 5)), toggle.Margin.Top + (y * (toggle.Width + 5)), toggle.Margin.Right, toggle.Margin.Bottom);

                x++;

                if (x > 4)
                {
                    x = 0;
                    y++;
                }
            }
        }

        void btn_smiley_Clicked(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            text_chatsend.CaretPosition.InsertTextInRun("[" + button.Tag.ToString() + "]");
            btn_smileys.IsChecked = false;
        }

        internal void UpdateRect()
        {
            scroll_chat.Minimum = 0;
            cliprect.Rect = new Rect(0, 0, grid_chatwindow.ActualWidth, grid_chatwindow.ActualHeight);
            scroll_chat.Maximum = grid_chat.ActualHeight - grid_chatwindow.ActualHeight;
        }

        void rc_Disconnect()
        {
            App.Kicked = true;
            AlertBox.Show("Kicked", "You got kicked from chat", main);
            main.Close();
        }
        
        void rc_ReceiveMsg(Framework.ChatMessage msg)
        {
            ChatUserControl user = lstClients[msg.Sender];

            if (user != null)
            {
                user.AddMessage(false, msg);
                string subtext = msg.Message.Length >= 30 ? msg.Message.Replace(Environment.NewLine, " ").Substring(0, 30) + "..." : msg.Message;
                main.ShowNotification("New message!", String.Format("New message from: {0}{2}{1}", msg.Sender, subtext, Environment.NewLine));
            }
        }

        void rc_NewNames(object sender, List<ChatUser> names)
        {
            List<string> notFound = new List<string>();

            foreach (string user in lstClients.Keys)
            {
                if (names.Find(u => u.UserName == user) == null)
                {
                    notFound.Add(user);
                }
            }

            foreach (string user in notFound)
            {
                ChatUserControl userc = lstClients[user];

                if (userc != null)
                {
                    if (activeUser == userc)
                    {
                        grid_chat.Children.Clear();
                        text_chatsend.IsEnabled = false;
                        btn_send.IsEnabled = false;
                        btn_smileys.IsEnabled = false;
                        btn_smileys.IsChecked = false;
                    }
                    grid_users.Children.Remove(userc);
                    lstClients.Remove(user);
                }
            }

            foreach (ChatUser name in names)
            {
                if (!lstClients.ContainsKey(name.UserName) && name.UserName != App.User.UserName)
                {
                    ChatUserControl user = new ChatUserControl(this, name);
                    grid_users.Children.Add(user);
                    lstClients.Add(name.UserName, user);
                }
            }

            SetPositions();
        }

        void SetPositions()
        {
            int i = 0;
            ChatUserControl user = new ChatUserControl(this, null);
            for (i = 0; i < grid_users.Children.Count; i++)
            {
                user = grid_users.Children[i] as ChatUserControl;
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
                Framework.ChatMessage message = App.Client.SendMessage(text, App.User.UserName, activeUser.text_username.Content.ToString());
                message.Message = text;

                text_chatsend.Document.Blocks.Clear();
                activeUser.AddMessage(true, message);
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

        private void btn_update_Click(object sender, RoutedEventArgs e)
        {
            App.User.SetProfilePic(image_user.Source as BitmapImage);
            App.User.Desc = new TextRange(text_desc.Document.ContentStart, text_desc.Document.ContentEnd).Text;

            App.Client.UpdateUser(App.User);
        }

        private void btn_image_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Please select the image";
            dlg.Filter = "jpeg files (*.jpg)|*.jpg|png files (*.png)|*.png";
            dlg.Multiselect = false;

            bool res = Convert.ToBoolean(dlg.ShowDialog());

            if (res)
            {
                image_user.Source = new BitmapImage(new Uri(dlg.FileName));
                btn_update_Click(null, null);
            }
        }
    }
}

using Framework.Classes;
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
using System.Windows.Threading;

namespace ChatStandalone
{
    /// <summary>
    /// Interaction logic for ChatUser.xaml
    /// </summary>
    public partial class ChatUser : UserControl
    {
        private Dictionary<int, ChatMessage> chatHistory = new Dictionary<int, ChatMessage>();
        ChatMessage lastMsg = null;
        ChatControl chat = null;

        public ChatUser(ChatControl chat, string username)
        {
            InitializeComponent();
            this.chat = chat;
            this.text_username.Content = username;
            if (!String.IsNullOrEmpty(username))
            {
                GetHistory();
            }
        }

        private void GetHistory()
        {
            Framework.ChatMessage[] history = App.Client.GetHistory(App.User.UserName, text_username.Content.ToString());

            foreach (Framework.ChatMessage message in history)
            {
                if (message.Sender == App.User.UserName)
                {
                    AddMessage(true, message.Message);
                }
                else
                {
                    AddMessage(false, message.Message);
                }
            }
        }

        internal void AddMessage(bool sent, string msg)
        {
            ChatMessage message = new ChatMessage(msg);

            if (sent)
            {
                message.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                message.Margin = new Thickness(5, 5, 0, 5);
                message.border_msg.Background = Brushes.LightBlue;
            }
            else
            {
                message.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                message.Margin = new Thickness(0, 5, 5, 5);
                message.border_msg.Background = Brushes.MediumVioletRed;
            }

            PositionMsg(message);

            chatHistory.Add(chatHistory.Keys.Count, message);
        }

        private void PositionMsg(ChatMessage msg)
        {
            if (lastMsg != null)
            {
                msg.Margin = new Thickness(msg.Margin.Left, lastMsg.Margin.Top + lastMsg.ActualHeight + 5, msg.Margin.Right, msg.Margin.Bottom);
            }

            if (chat.activeUser == this)
            {
                chat.grid_chat.Children.Add(msg);
                Dispatcher.Invoke(new Action(() => { chat.grid_chat.Height += msg.ActualHeight + 5; }), DispatcherPriority.ContextIdle, null);                
            }

            Dispatcher.Invoke(new Action(() => { chat.UpdateRect(); }), DispatcherPriority.ContextIdle, null);
            lastMsg = msg; 
        }

        private void grid_background_MouseUp(object sender, MouseButtonEventArgs e)
        {
            foreach (ChatUser user in chat.lstClients.Values)
            {
                user.Background = Brushes.Transparent;
            }

            Background = Brushes.LightBlue;
            chat.btn_send.IsEnabled = true;
            chat.text_chatsend.IsEnabled = true;
            chat.activeUser = this;

            lastMsg = null;
            chat.grid_chat.Children.Clear();
            chat.grid_chat.Height = 10;
            for (int i = 0; i < chatHistory.Count; i++)
            {
                PositionMsg(chatHistory[i]);
            }
        }
    }
}

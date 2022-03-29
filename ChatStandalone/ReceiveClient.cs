using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ChatStandalone
{
    public delegate void ReceivedMessage(Framework.ChatMessage msg);
    public delegate void GotNames(object sender, List<ChatUser> names);
    public delegate void Disconnect();

    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ReceiveClient : ChatService.ISendChatServiceCallback
    {
        string CryptionPassPhrase = "Katzwinkel_Framework2017@Julian-The|Master@ChatService";
        public event ReceivedMessage ReceiveMsg;
        public event GotNames NewNames;
        public event Disconnect Disconnect;

        InstanceContext inst = null;
        ChatService.SendChatServiceClient chatClient = null;

        public void SetUp(ReceiveClient rc)
        {
            inst = new InstanceContext(rc);
            chatClient = new ChatService.SendChatServiceClient(inst);
        }

        public RegisterResult Register(string username, string email, string fullname, string password)
        {
            ChatUser user = new ChatUser();
            user.UserName = CryptionManager.Encrypt(username, CryptionPassPhrase);
            user.EMail = CryptionManager.Encrypt(email, CryptionPassPhrase);
            user.FullName = CryptionManager.Encrypt(fullname, CryptionPassPhrase);
            user.Password = CryptionManager.Encrypt(password, CryptionPassPhrase);

            return chatClient.Register(user);
        }

        public void UpdateUser(ChatUser user)
        {
            user.Desc = CryptionManager.Encrypt(user.Desc, CryptionPassPhrase);

            chatClient.UpdateUser(user);
        }

        public Framework.ChatUser Login(string name, string password)
        {
            name = CryptionManager.Encrypt(name, CryptionPassPhrase);
            password = CryptionManager.Encrypt(password, CryptionPassPhrase);

            ChatUser user = chatClient.Login(name, password);

            user.UserName = CryptionManager.Decrypt(user.UserName, CryptionPassPhrase);
            user.FullName = CryptionManager.Decrypt(user.FullName, CryptionPassPhrase);
            user.Desc = CryptionManager.Decrypt(user.Desc, CryptionPassPhrase);

            return user;
        }

        public void Start(string name)
        {
            name = CryptionManager.Encrypt(name, CryptionPassPhrase);
            chatClient.Start(name);
        }

        public Framework.ChatMessage SendMessage(string msg, string sender, string receiver)
        {
            receiver = CryptionManager.Encrypt(receiver, CryptionPassPhrase);
            msg = CryptionManager.Encrypt(msg, CryptionPassPhrase);
            sender = CryptionManager.Encrypt(sender, CryptionPassPhrase);

            Framework.ChatMessage message = new Framework.ChatMessage(sender) { Receiver = receiver, Time = DateTime.Now, Message = msg };

            chatClient.SendMessage(message);

            return message;
        }

        public void Stop(string name)
        {
            try
            {
                if (!App.Kicked)
                {
                    name = CryptionManager.Encrypt(name, CryptionPassPhrase);
                    chatClient.Stop(name);
                }
            }
            catch (Exception ex)
            { }
        }

        void ChatService.ISendChatServiceCallback.ReceiveMessage(Framework.ChatMessage message)
        {
            message.Sender = CryptionManager.Decrypt(message.Sender, CryptionPassPhrase);
            message.Message = CryptionManager.Decrypt(message.Message, CryptionPassPhrase);
            if (ReceiveMsg != null)
                ReceiveMsg(message);
        }

        public Framework.ChatMessage[] GetHistory(string Sender, string Receiver)
        {
            Sender = CryptionManager.Encrypt(Sender, CryptionPassPhrase);
            Receiver = CryptionManager.Encrypt(Receiver, CryptionPassPhrase);

            Framework.ChatMessage[] history = chatClient.GetHistory(Sender, Receiver);

            foreach (Framework.ChatMessage message in history)
            {
                message.Message = CryptionManager.Decrypt(message.Message, CryptionPassPhrase);
                message.Sender = CryptionManager.Decrypt(message.Sender, CryptionPassPhrase);
                message.Receiver = CryptionManager.Decrypt(message.Receiver, CryptionPassPhrase);
            }

            return history;
        }

        public void SendNames(ChatUser[] names)
        {
            foreach (ChatUser user in names)
            {
                user.UserName = CryptionManager.Decrypt(user.UserName, CryptionPassPhrase);
                user.FullName = CryptionManager.Decrypt(user.FullName, CryptionPassPhrase);
                user.Desc = CryptionManager.Decrypt(user.Desc, CryptionPassPhrase);
            }

            if (NewNames != null)
                NewNames(this, names.ToList());
        }

        public void DisconnectClient()
        {
            if (Disconnect != null)
                Disconnect();
        }
    }
}
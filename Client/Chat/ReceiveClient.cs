using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client.Chat
{
    public delegate void ReceivedMessage(string sender, string message);
    public delegate void GotNames(object sender, List<string> names);

    class ReceiveClient : ChatService.ISendChatServiceCallback
    {
        string CryptionPassPhrase = "Katzwinkel_Framework2017@Julian-The|Master@ChatService";
        public event ReceivedMessage ReceiveMsg;
        public event GotNames NewNames;

        InstanceContext inst = null;
        ChatService.SendChatServiceClient chatClient = null;

        public void Start(ReceiveClient rc, string name, string password)
        {
            name = CryptionManager.Encrypt(name, CryptionPassPhrase);
            inst = new InstanceContext(rc);
            chatClient = new ChatService.SendChatServiceClient(inst);
            chatClient.Start(name, password);
        }

        public void SendMessage(string msg, string sender, string receiver)
        {
            receiver = CryptionManager.Encrypt(receiver, CryptionPassPhrase);
            msg = CryptionManager.Encrypt(msg, CryptionPassPhrase);
            sender = CryptionManager.Encrypt(sender, CryptionPassPhrase);
            chatClient.SendMessage(msg, sender, receiver);
        }

        public void Stop(string name)
        {
            try
            {
                name = CryptionManager.Encrypt(name, CryptionPassPhrase);
                chatClient.Stop(name);
            }
            catch (Exception ex)
            { }
        }

        void ChatService.ISendChatServiceCallback.ReceiveMessage(string msg, string receiver)
        {
            receiver = CryptionManager.Decrypt(receiver, CryptionPassPhrase);
            msg = CryptionManager.Decrypt(msg, CryptionPassPhrase);
            if (ReceiveMsg != null)
                ReceiveMsg(receiver, msg);
        }

        public void SendNames(string[] names)
        {
            if (NewNames != null)
                NewNames(this, names.ToList());
        }
    }
}
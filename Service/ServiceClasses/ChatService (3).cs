﻿using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ChatService : ISendChatService
    {
        string CryptionPassPhrase = "Katzwinkel_Framework2017@Julian-The|Master@ChatService";
        Dictionary<string, IReceiveChatService> names = new Dictionary<string, IReceiveChatService>();

        public static event ListOfNames ChatListOfNames;

        IReceiveChatService callback = null;

        public ChatService() { }

        public void Close()
        {
            callback = null;
            names.Clear();
        }

        public void Start(string Name, string Password)
        {
            try
            {
                Name = CryptionManager.Decrypt(Name, CryptionPassPhrase);
                if (!names.ContainsKey(Name) && )
                {
                    Console.WriteLine(String.Format("User: {0} logged in.", Name));
                    callback = OperationContext.Current.GetCallbackChannel<IReceiveChatService>();
                    AddUser(Name, callback);
                    SendNamesToAll();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Stop(string Name)
        {
            try
            {
                Name = CryptionManager.Decrypt(Name, CryptionPassPhrase);
                if (names.ContainsKey(Name))
                {
                    Console.WriteLine(String.Format("User: {0} logged out.", Name));
                    names.Remove(Name);
                    SendNamesToAll();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        void SendNamesToAll()
        {
            foreach (KeyValuePair<string, IReceiveChatService> name in names)
            {
                IReceiveChatService proxy = name.Value;
                proxy.SendNames(names.Keys.ToList());
            }

            if (ChatListOfNames != null)
                ChatListOfNames(names.Keys.ToList(), this);
        }

        void ISendChatService.SendMessage(string msg, string sender, string receiver)
        {
            receiver = CryptionManager.Decrypt(receiver, CryptionPassPhrase);
            if (names.ContainsKey(receiver))
            {
                callback = names[receiver];
                callback.ReceiveMessage(msg, sender);
            }
        }

        private void AddUser(string name, IReceiveChatService callback)
        {
            names.Add(name, callback);
            if (ChatListOfNames != null)
                ChatListOfNames(names.Keys.ToList(), this);

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Framework
{
    [DataContract]
    public class ChatMessage
    {
        [DataMember]
        public DateTime Time { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string Receiver { get; set; }

        [DataMember]
        public string Sender { get; set; }

        public ChatMessage(string sender)
        {
            Sender = sender;
        }

        [OperationContract]
        public bool NewerThan(DateTime compareTime)
        {
            if (Time > compareTime)
            {
                return true;
            }

            return false;
        }
    }
}

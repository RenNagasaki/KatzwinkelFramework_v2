using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    [ServiceContract(CallbackContract = typeof(IReceiveChatService))]
    public interface ISendChatService
    {
        [OperationContract(IsOneWay = true)]
        void SendMessage(ChatMessage message);
        [OperationContract]
        ChatMessage[] GetHistory(string sender, string receiver);
        [OperationContract]
        RegisterResult Register(ChatUser user);
        [OperationContract(IsOneWay = true)]
        void UpdateUser(ChatUser user);
        [OperationContract]
        ChatUser Login(string Name, string Password);
        [OperationContract(IsOneWay = true)]
        void Start(string Name);
        [OperationContract(IsOneWay = true)]
        void Stop(string Name);
    }
}

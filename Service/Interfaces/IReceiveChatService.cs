using Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IReceiveChatService
    {
        [OperationContract(IsOneWay = true)]
        void ReceiveMessage(Framework.ChatMessage message);
        [OperationContract(IsOneWay = true)]
        void SendNames(List<Framework.ChatUser> names);
        [OperationContract(IsOneWay = true)]
        void DisconnectClient();
    }
}

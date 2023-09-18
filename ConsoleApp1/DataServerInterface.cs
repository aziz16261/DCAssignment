using DatabaseLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Console1
{
    [ServiceContract]
    public interface DataServerInterface
    {
        [OperationContract]
        [FaultContract(typeof(ServerException))]
        bool CheckAccount(String username);

        [OperationContract]
        [FaultContract(typeof(ServerException))]
        string CreateChatRoom(string roomName);

        [OperationContract]
        [FaultContract(typeof(ServerException))]
        string JoinChatRoom(string roomName, string username);

        [OperationContract]
        [FaultContract(typeof(ServerException))]
        string LeaveChatRoom(string roomName, string username);

        [OperationContract]
        [FaultContract(typeof(ServerException))]
        List<string> CreateInitialChatRooms();


        [OperationContract]
        [FaultContract(typeof(ServerException))]
        void RemoveAccount(string username);
}
}

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
        List<ChatRoom> CreateChatRoom(string roomName, List<ChatRoom> chatRoomsList);

        [OperationContract]
        [FaultContract(typeof(ServerException))]
        List<ChatRoom> JoinChatRoom(string roomName, string username, List<ChatRoom> chatRoomsList);

        [OperationContract]
        [FaultContract(typeof(ServerException))]
        List<ChatRoom> LeaveChatRoom(string roomName, string username, List<ChatRoom> chatRoomsList);

        [OperationContract]
        [FaultContract(typeof(ServerException))]
        List<ChatRoom> CreateInitialChatRooms(List<ChatRoom> chatRoomsList);

        [OperationContract]
        [FaultContract(typeof(ServerException))]
        void RemoveAccount(string username);

      //  [OperationContract]
       // [FaultContract(typeof(ServerException))]
       // List<ChatRoom> GetChatRooms();

        [OperationContract]
        [FaultContract(typeof(ServerException))]
        ChatRoom GetChatRoom(string roomName, List<ChatRoom> chatRoomsList);

        [OperationContract]
        [FaultContract(typeof(ServerException))]
        List<ChatRoom> SendMessage(string sender, string roomName, string message, List<ChatRoom> chatRoomsList);
    }

}
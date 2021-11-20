using LUD.DataStructures;
using Mirage;
using Mirage.SocketLayer;

namespace LUD.Messages
{
    [NetworkMessage]
    public struct ServerAcceptAuthCode
    {
    }

    [NetworkMessage]
    public struct ServerAuthCode
    {
        public string AuthenticationCode;
        public ServerDataInfo ServerInfo;
    }
}

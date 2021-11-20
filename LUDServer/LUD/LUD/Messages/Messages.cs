using LUD.DataStructures;
using Mirage;

namespace LUD.Messages
{
    [NetworkMessage]
    public struct ServerAuthCode
    {
        public string AuthenticationCode;
        public ServerDataInfo ServerInfo;
    }

    [NetworkMessage]
    public struct ServerAcceptAuthCode
    {
    }
}

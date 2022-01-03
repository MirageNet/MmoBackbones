#if UNITY_SERVER || UNITY_EDITOR || DEVELOPMENT_BUILD
using LUD.DataStructures;
#endif
using Mirage;

namespace LUD.Messages
{
#if UNITY_SERVER || UNITY_EDITOR || DEVELOPMENT_BUILD

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

#endif
}

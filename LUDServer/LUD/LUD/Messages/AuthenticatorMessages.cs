using LUD.DataStructures;
using Mirage;

namespace LUD.Messages
{
    public class AuthenticatorMessages
    {
        [NetworkMessage]
        public struct ServerAuthCode
        {
            public string AuthenticationCode;
            public RegionDataInfo ServerInfo;
        }

        [NetworkMessage]
        public struct ServerRequestAuthCode
        {
        }
    }
}

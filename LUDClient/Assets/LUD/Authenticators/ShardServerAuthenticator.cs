#if UNITY_SERVER || UNITY_EDITOR || DEVELOPMENT_BUILD

using LUD.DataStructures;
using LUD.Messages;
using Mirage;
using UnityEngine;

namespace LUD.Authenticators
{
    public class ShardServerAuthenticator : NetworkAuthenticator
    {
        [Header("Server Setup")] [SerializeField, Tooltip("The secret key only the master server listener accepts.")]
        private string _secretKey = "G97Y&D^W*P_tuFQAAUzwu?ZkmX#_!9R9L#v*rJeLv$zW5EH%fdL7rxSJKxRt";

        [SerializeField, Tooltip("Name of server represent to clients.")]
        private string _serverName = "";

        [SerializeField, Tooltip("Unique id to represent which shard this server is for.")]
        private byte _shardId = 1;

        [SerializeField, Tooltip("Unique id to know which map this server is running.")]
        private byte _mapId = 1;

        public byte ShardId => _shardId;

        #region Overrides of NetworkAuthenticator

        public override void ServerSetup(NetworkServer server)
        {
            //NOOP
        }

        public override void ServerAuthenticate(INetworkPlayer player)
        {
            //NOOP
        }

        public override void ClientSetup(NetworkClient client)
        {
            client.MessageHandler.RegisterHandler<ServerAcceptAuthCode>((player, message) => ClientAccept(player));
        }

        public override void ClientAuthenticate(INetworkPlayer player)
        {
            player.Send(new ServerAuthCode
            {
                AuthenticationCode = _secretKey,
                ServerInfo = new ServerDataInfo { ShardId = _shardId, ServerName = _serverName, MapId = _mapId}
            });
        }

        #endregion
    }
}

#endif

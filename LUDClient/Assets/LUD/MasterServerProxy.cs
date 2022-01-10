#if UNITY_SERVER || UNITY_EDITOR || DEVELOPMENT_BUILD

using System;
using LUD.Authenticators;
using Mirage;
using UnityEngine;

namespace LUD
{
    [RequireComponent(typeof(NetworkClient), typeof(ShardServerAuthenticator))]
    public class MasterServerProxy : MonoBehaviour
    {
        #region Fields

        [Header("Master Server Setup")]

        [Tooltip("Main reference to the actual server object manager.")]
        [SerializeField] private ServerObjectManager _serverInstance;

        private NetworkClient _masterServerProxyClient;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _masterServerProxyClient = GetComponent<NetworkClient>();

            _serverInstance.NetIdGenerator = _serverInstance.GetComponent<NetIdGenerator>();
            _serverInstance.Server.Started.AddListener(OnServerStart);
            _serverInstance.Server.Stopped.AddListener(OnServerStopped);
        }

        #endregion

        #region Callback Listener's

        /// <summary>
        ///     Once server has finally started we will connect to our master server.
        /// </summary>
        private void OnServerStart()
        {
            _masterServerProxyClient.Connected.AddListener(OnConnected);
            _masterServerProxyClient.Connect();
        }

        /// <summary>
        ///     Once we connect up as a proxy client for master server / shard we need register
        ///     messages to allow the data to flow between them.
        /// </summary>
        /// <param name="proxyPlayer"></param>
        private void OnConnected(INetworkPlayer proxyPlayer)
        {
            _serverInstance.Server.AddConnection(_masterServerProxyClient.Player);

            _masterServerProxyClient.MessageHandler.RegisterHandler<SpawnMessage>(OnReceivedMessageToRelay);
            _masterServerProxyClient.MessageHandler.RegisterHandler<UpdateVarsMessage>(OnReceivedMessageToRelay);
            _masterServerProxyClient.MessageHandler.RegisterHandler<ObjectDestroyMessage>(OnReceivedMessageToRelay);
            _masterServerProxyClient.MessageHandler.RegisterHandler<ObjectHideMessage>(OnReceivedMessageToRelay);
            _masterServerProxyClient.MessageHandler.RegisterHandler<RemoveAuthorityMessage>(OnReceivedMessageToRelay);
            _masterServerProxyClient.MessageHandler.RegisterHandler<RemoveCharacterMessage>(OnReceivedMessageToRelay);
        }

        /// <summary>
        ///     We received a new message from other shards of the same world.
        ///     We don't need to actually handle it as this client because its relaying to other real clients.
        /// </summary>
        /// <param name="msg">The message details we are going to relay to clients.</param>
        private void OnReceivedMessageToRelay<T>(T msg)
        {
            // TODO We need a safe way to know that this spawn message is withing range
            // or the aoi system the current objects using will spawn it. For now we will just
            // send the spawn message to all clients regardless.

            //Type itemType = typeof(T);

            //if (itemType == typeof(SpawnMessage))
            //{
            //    foreach (INetworkPlayer player in _serverInstance.Server.Players)
            //    {
            //        if (_masterServerProxyClient.Player == player) continue;

            //        _serverInstance.SpawnVisibleObjects(player);
            //    }
            //}
            //else
            //{
                foreach (INetworkPlayer player in _serverInstance.Server.Players)
                {
                    if (_masterServerProxyClient.Player == player) continue;

                    player.Send(msg);
                }
            //}
        }

            /// <summary>
        ///     Once server has finally shut down we want to disconnect from the master server.
        /// </summary>
        private void OnServerStopped()
        {
            _masterServerProxyClient.Disconnect();
        }

        #endregion
    }
}

#endif

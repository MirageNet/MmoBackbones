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

        [Tooltip("Main reference to the actual server this instance will be proxy from.")]
        [SerializeField] private NetworkServer _serverInstance;

        [Tooltip("The address to connect to your master server.")]
        [SerializeField] private string _address = "localhost";

        [Tooltip("The port to use to connect to your master server.")]
        [SerializeField] private ushort _port = 7777;

        private NetworkClient _masterServerProxyClient;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _masterServerProxyClient = GetComponent<NetworkClient>();

            _serverInstance.Started.AddListener(OnServerStart);
            _serverInstance.Stopped.AddListener(OnServerStopped);
        }

        #endregion

        /// <summary>
        ///     Once we connect up as a proxy client for master server / shard we need register
        ///     messages to allow the data to flow between them.
        /// </summary>
        /// <param name="proxyPlayer"></param>
        private void OnConnected(INetworkPlayer proxyPlayer)
        {
        }

        #region Callback Listener's

        /// <summary>
        ///     Once server has finally started we will connect to our master server.
        /// </summary>
        private void OnServerStart()
        {
            _masterServerProxyClient.Connected.AddListener(OnConnected);
            _masterServerProxyClient.Connect(_address, _port);
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

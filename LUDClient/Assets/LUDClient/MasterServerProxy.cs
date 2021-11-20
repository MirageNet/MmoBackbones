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

        private NetworkClient _masterServerProxyClient;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _masterServerProxyClient = GetComponent<NetworkClient>();

            _serverInstance.Started.AddListener(OnServerStart);

            _serverInstance.StartServer();
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
        ///     Once server has finally started we will connect to our master server directly.
        /// </summary>
        private void OnServerStart()
        {
            _masterServerProxyClient.Connected.AddListener(OnConnected);
            _masterServerProxyClient.Connect();
        }

        #endregion
    }
}

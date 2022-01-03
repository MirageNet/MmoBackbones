using LUD.Authenticators;
using LUD.DataStructures;
using LUD.Messages;
using LUD.Redis;
using Mirage;
using Mirage.Sockets.Udp;
using UnityEngine;
using Microsoft.Extensions.Configuration;
using Mirage.Logging;
using LogFactory = LUD.Logging.LogFactory;
using LogType = LUD.Logging.LogType;

namespace LUD.Core
{
    public class Server
    {
        #region Fields

        private IConfiguration? _configuration;
        private MasterServerMessageHandler _serverMessageHandler;
        private NetworkServer? _server;
        private RedisManager? _redisManager;

        #endregion

        #region Callback Listener's

        /// <summary>
        /// 
        /// </summary>
        /// <param name="player"></param>
        private void OnServerDisconnected(INetworkPlayer player)
        {
            ServerDataInfo serverInfo = _serverMessageHandler.RegionServers
                .FirstOrDefault(x => x.Value.TryGetValue(player, out _)).Key;

            if (serverInfo.MapId <= 0) return;

            LogFactory.Log(
                $"[Master Server] - Regional server: {serverInfo.ServerName} shard: {serverInfo.ShardId} is disconnecting from master server.",
                LogType.Log);

            _serverMessageHandler.RegionServers[serverInfo].Remove(player);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="player"></param>
        private void OnServerHasAuthenticated(ServerAuthCode data, INetworkPlayer player)
        {
            if (!_serverMessageHandler.RegionServers.ContainsKey(data.ServerInfo))
            {
                _serverMessageHandler.RegionServers.Add(data.ServerInfo, new HashSet<INetworkPlayer>());

                _serverMessageHandler.RegionServers[data.ServerInfo].Add(player);

                LogFactory.Log(
                    $"[Master Server] - New server spun up. Server:{data.ServerInfo.ServerName} shard: {data.ServerInfo.ShardId} for map: {data.ServerInfo.MapId}",
                    LogType.Log);
            }
            else
            {
                _serverMessageHandler.RegionServers[data.ServerInfo].Add(player);

                LogFactory.Log(
                    $"[Master Server] - Adding new shard: {data.ServerInfo.ShardId} for map: {data.ServerInfo.MapId} on {data.ServerInfo.ServerName}",
                    LogType.Log);
            }
        }

        #endregion

        #region Class Specific

        /// <summary>
        ///     Create a new server.
        /// </summary>
        /// <param name="authenticator">The type of authenticator we want to use for this server.</param>
        public Server(RegionAuthenticator authenticator)
        {
            if (authenticator == null)
            {
                throw new ArgumentNullException(nameof(authenticator), "Authenticator passed in was null.");
            }

            Initialize(authenticator);
        }

        /// <summary>
        ///     Initializes a new server with configuration from appsettings.json file and run
        ///     internal mirage trigger to mimic unity firing on load.
        /// </summary>
        /// <param name="authenticator">The type of authenticator we want to use for this server.</param>
        private void Initialize(RegionAuthenticator authenticator)
        {
            LogFactory.Log("[Master Server] - Booting up.", LogType.Log);

            // Build configuration
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory)?.FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            Debug.unityLogger = new StandaloneLogger();

            // This allows us to fire up mirage internal code to mimic unity system.
            InitializeReadWrite.RunMethods();

            CreateServer(authenticator);

            _redisManager = new RedisManager();

            LogFactory.Log("[Master Server] - Listening for new region servers.", LogType.Log);

            Update();
        }

        /// <summary>
        ///     Creates the necessary server and authenticators and starts up the server.
        /// </summary>
        /// <param name="authenticator">The type of authenticator we want to use for this server.</param>
        private void CreateServer(RegionAuthenticator authenticator)
        {
            var socketFactory = new UdpSocketFactory
            {
                Port = Convert.ToUInt16(_configuration?.GetSection("Network:Port").Value)
            };

            authenticator.ServerHasAuthenticated += OnServerHasAuthenticated;

            _server = new NetworkServer { SocketFactory = socketFactory, authenticator = authenticator };
            _server.Disconnected.AddListener(OnServerDisconnected);

            _serverMessageHandler = new MasterServerMessageHandler(_server.World, true);
            _server.StartServer(_serverMessageHandler);
        }

        /// <summary>
        ///     Process server data.
        /// </summary>
        private async void Update()
        {
            while (true)
            {
                try
                {
                    _server?.Update();
                }
                catch (Exception ex)
                {
                    LogFactory.Log(ex.Message, LogType.Exception);
                }

                await Task.Delay(5);
            }
        }

        /// <summary>
        ///     Shutdown server.
        /// </summary>
        public void Shutdown()
        {
            LogFactory.Log("[Master Server] - Shutting down.", LogType.Log);
        }

        #endregion
    }
}

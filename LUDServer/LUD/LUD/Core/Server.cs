using System.Reflection;
using LUD.Authenticators;
using LUD.DataStructures;
using LUD.Logging;
using LUD.Messages;
using LUD.Redis;
using Mirage;
using Mirage.Sockets.Udp;
using UnityEngine;
using Microsoft.Extensions.Configuration;
using LogType = LUD.Logging.LogType;

namespace LUD.Core
{
    public class Server
    {
        #region Fields

        private IConfiguration? _configuration;
        private readonly MasterServerMessageHandler _serverMessageHandler = new MasterServerMessageHandler(true);
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
            ServerDataInfo serverInfo = _serverMessageHandler.RegionServers.FirstOrDefault(x => x.Value.TryGetValue(player, out player)).Key;

            if (serverInfo.ServerId <= 0) return;

                LogFactory.Log($"[Master Server] Regional server: {serverInfo.ServerName} shard: {serverInfo.ServerId} is disconnecting from master server.", LogType.Log);

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
            }
            else
            {
                _serverMessageHandler.RegionServers[data.ServerInfo].Add(player);
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
            LogFactory.Log("[Master Server] Booting up.", LogType.Log);

            // Build configuration
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory)?.FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            // This allows us to fire up mirage internal code to mimic unity system.
            RunInitializeMethods();

            CreateServer(authenticator);

            _redisManager = new RedisManager();

            LogFactory.Log("[Master Server] Listening for new region servers.", LogType.Log);

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
            _server.Disconnected += OnServerDisconnected;

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
                    Console.WriteLine(ex);
                }

                await Task.Delay(5);
            }
        }

        /// <summary>
        ///     Initializes mirage to run.
        /// </summary>
        private void RunInitializeMethods()
        {
            var asm = Assembly.GetExecutingAssembly();

            MethodInfo[] methods = asm.GetTypes()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false).Length > 0)
                .ToArray();

            foreach (MethodInfo method in methods)
            {
                method.Invoke(null, null);
            }
        }

        /// <summary>
        ///     Shutdown server.
        /// </summary>
        public void Shutdown()
        {
            LogFactory.Log("[Master Server] Shutting down.", LogType.Log);
        }

        #endregion
    }
}

using LUD.Logging;
using LUD.Messages;
using Microsoft.Extensions.Configuration;
using Mirage;

namespace LUD.Authenticators
{
    public class RegionAuthenticator : NetworkAuthenticator
    {
        #region Fields

        private IConfiguration? _configuration;

        public Action<ServerAuthCode, INetworkPlayer> ServerHasAuthenticated;

        #endregion

        #region Class Specific

        /// <summary>
        ///     Regional server has request to connected to master server.
        /// </summary>
        /// <param name="player">The server that sent the correct message back.</param>
        /// <param name="message">The data being sent from regional server to allow connecting.</param>
        private void OnRegionServerRequestConnection(INetworkPlayer player, ServerAuthCode message)
        {
            bool passed = message.AuthenticationCode.Equals(_configuration?.GetSection("ServerAuthenticate:SecretKey").Value);

            if (passed)
            {
                ServerHasAuthenticated.Invoke(message, player);

                ServerAccept(player);

                player.Send(new ServerAcceptAuthCode());
            }
            else
                ServerReject(player);

            LogFactory.Log($"[Master Server] Regional server {message.ServerInfo.ServerName} requesting connection. Correct authentication code: {passed}", LogType.Log);
        }

        #endregion

        #region Overrides of NetworkAuthenticator

        /// <summary>
        /// Used to set up authenticator on server
        /// <para>Can be used to register message handlers before any players connect</para>
        /// </summary>
        public override void ServerSetup(NetworkServer server)
        {
            // Build configuration
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory)?.FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            server.MessageHandler.RegisterHandler<ServerAuthCode>(OnRegionServerRequestConnection);
        }

        /// <summary>
        /// Authenticate the player on the Server.
        /// <para>Called by the server when new client connects</para>
        /// </summary>
        /// <param name="player"></param>
        public override void ServerAuthenticate(INetworkPlayer player)
        {
            // NOOP
        }

        /// <summary>
        /// Used to set up authenticator on client
        /// <para>Can be used to register message handlers before any player connects</para>
        /// </summary>
        public override void ClientSetup(NetworkClient client)
        {
            //NOOP
        }

        /// <summary>
        /// Authenticate the player on the Client.
        /// <para>Called by the client after connected to the server</para>
        /// </summary>
        /// <param name="player"></param>
        public override void ClientAuthenticate(INetworkPlayer player)
        {
            //NOOP
        }

        #endregion
    }
}

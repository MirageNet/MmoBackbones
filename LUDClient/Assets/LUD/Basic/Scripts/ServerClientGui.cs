using Mirage;
using UnityEngine;

namespace LUD.Examples
{
    public class ServerClientGui : MonoBehaviour
    {
        #region Fields

        private bool showStatus = false;
        private NetworkServer _server;
        private NetworkClient _client;

        #endregion

        #region Unity Methods

        private void Awake()
        {
            _server = GetComponent<NetworkServer>();
            _client = GetComponent<NetworkClient>();
        }

        private void Update()
        {
            showStatus = _server.Active | _client.Active;
        }

        private void OnGUI()
        {
            if(!showStatus)
            {
                if (GUILayout.Button("Client Connect"))
                {
                    _client.authenticator = null;
                    _client.Connect();
                }
                else if (GUILayout.Button("Server Start"))
                {
                    _server.StartServer();
                }
            }
            else
            {
                if (!showStatus) return;

                if (_server.Active)
                {
                    GUILayout.Label("Server Running.");
                    if (GUILayout.Button("Shutdown"))
                    {
                        _server.Stop();
                    }
                }
                else if (_client.Active)
                    GUILayout.Label("Client Running.");
            }
        }

        #endregion
    }
}

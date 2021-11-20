using Mirage;
using UnityEngine;

public class ConnectClientGui : MonoBehaviour
{
    private void OnGUI()
    {
        if (GUILayout.Button("Client Connect"))
        {
            GetComponent<NetworkClient>().Connect();
        }
    }
}

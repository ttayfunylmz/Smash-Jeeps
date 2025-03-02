using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkClient : IDisposable
{
    private NetworkManager _networkManager;

    public NetworkClient(NetworkManager networkManager)
    {
        _networkManager = networkManager;

        networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if(clientId != 0 && clientId != _networkManager.LocalClientId) { return; }

        if(SceneManager.GetActiveScene().name != Consts.SceneNames.MAIN_MENU_SCENE)
        {
            SceneManager.LoadScene(Consts.SceneNames.MAIN_MENU_SCENE);
        }

        if(_networkManager.IsConnectedClient)
        {
            _networkManager.Shutdown();
        }
    }

    public void Dispose()
    {
        if(_networkManager == null) { return; }

        _networkManager.OnClientDisconnectCallback -= OnClientDisconnect;

        if(_networkManager.IsListening)
        {
            _networkManager.Shutdown();
        }
    }
}

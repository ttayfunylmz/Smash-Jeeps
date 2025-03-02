using System;
using System.Text;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientManager : IDisposable
{
    private JoinAllocation _joinAllocation;
    private NetworkClient _networkClient;

    public async UniTask<bool> InitAsync()
    {
        await UnityServices.InitializeAsync();

        _networkClient = new NetworkClient(NetworkManager.Singleton);

        AuthenticationState authenticationState = await AuthenticationHandler.DoAuth();

        if(authenticationState == AuthenticationState.Authenticated)
        {
            return true;
        }

        return false;
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(Consts.SceneNames.MAIN_MENU_SCENE);
    }

    public async UniTask StartClientAsync(string joinCode)
    {
        try
        {
            _joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch(Exception exception)
        {
            Debug.LogError(exception);
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetRelayServerData(AllocationUtils.ToRelayServerData(_joinAllocation, "dtls"));

        UserData userData = new UserData
        {
            UserName = PlayerPrefs.GetString(Consts.PlayerData.PLAYER_NAME, "Noname"),
            UserAuthId = AuthenticationService.Instance.PlayerId
        };
        string payload = JsonUtility.ToJson(userData);
        byte[] payloadBytes = Encoding.UTF8.GetBytes(payload);
        NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadBytes; 

        NetworkManager.Singleton.StartClient();
    }

    public void Dispose()
    {
        _networkClient?.Dispose();
    }
}

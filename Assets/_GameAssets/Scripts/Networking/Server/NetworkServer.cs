using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Netcode;
using UnityEngine;

public class NetworkServer : IDisposable
{
    private NetworkManager _networkManager;

    private Dictionary<ulong, string> _clientIdToAuthDictionary = new Dictionary<ulong, string>();
    private Dictionary<string, UserData> _authIdToUserDataDictionary = new Dictionary<string, UserData>();

    public NetworkServer(NetworkManager networkManager)
    {
        _networkManager = networkManager;

        networkManager.ConnectionApprovalCallback += ApprovalCheck;
        networkManager.OnServerStarted += OnNetworkReady;
    }

    private void ApprovalCheck(
        NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        string payload = System.Text.Encoding.UTF8.GetString(request.Payload);
        UserData userData = JsonUtility.FromJson<UserData>(payload);

        _clientIdToAuthDictionary[request.ClientNetworkId] = userData.UserAuthId;
        _authIdToUserDataDictionary[userData.UserAuthId] = userData;

        response.Approved = true;
        // response.Position = SpawnPoint.GetRandomSpawnPosition();
        // response.Rotation = SpawnPoint.GetSpawnRotation();
        response.CreatePlayerObject = true;
    }

    private void OnNetworkReady()
    {
        _networkManager.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if(_clientIdToAuthDictionary.TryGetValue(clientId, out string authId))
        {
            _clientIdToAuthDictionary.Remove(clientId);
            _authIdToUserDataDictionary.Remove(authId);
        }
    }

    public UserData GetUserDataByClientId(ulong clientId)
    {
        if(_clientIdToAuthDictionary.TryGetValue(clientId, out string authId))
        {
            if(_authIdToUserDataDictionary.TryGetValue(authId, out UserData userData))
            {
                return userData;
            }

            return null;
        }

        return null;
    }

    public void Dispose()
    {
        if(_networkManager == null) { return; }

        _networkManager.ConnectionApprovalCallback -= ApprovalCheck;
        _networkManager.OnServerStarted -= OnNetworkReady;
        _networkManager.OnClientDisconnectCallback -= OnClientDisconnect;

        if(_networkManager.IsListening)
        {
            _networkManager.Shutdown();
        }
    }
}

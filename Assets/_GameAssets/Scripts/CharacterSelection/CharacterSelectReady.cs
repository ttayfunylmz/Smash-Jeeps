using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectReady : NetworkBehaviour
{
    public static CharacterSelectReady Instance { get; private set; }

    public event Action OnReadyChanged;
    public event Action OnUnreadyChanged;
    public event Action OnAllPlayersReady;

    private Dictionary<ulong, bool> _playerReadyDictionary;

    private void Awake()
    {
        Instance = this;
        _playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    public void SetPlayerReady()
    {
        SetPlayerReadyRpc();
    }

    public void SetPlayerUnready()
    {
        SetPlayerUnreadyRpc();
    }

    [Rpc(SendTo.Server)]
    private void SetPlayerReadyRpc(RpcParams rpcParams = default)
    {
        SetPlayerReadyToAllRpc(rpcParams.Receive.SenderClientId);

        _playerReadyDictionary[rpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!_playerReadyDictionary.ContainsKey(clientId) || !_playerReadyDictionary[clientId])
            {
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            OnAllPlayersReady?.Invoke();
        }
    }

    [Rpc(SendTo.Server)]
    private void SetPlayerUnreadyRpc(RpcParams rpcParams = default)
    {
        SetPlayerUnreadyToAllRpc(rpcParams.Receive.SenderClientId);

        if (_playerReadyDictionary.ContainsKey(rpcParams.Receive.SenderClientId))
        {
            _playerReadyDictionary[rpcParams.Receive.SenderClientId] = false;
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetPlayerReadyToAllRpc(ulong clientId)
    {
        _playerReadyDictionary[clientId] = true;
        OnReadyChanged?.Invoke();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetPlayerUnreadyToAllRpc(ulong clientId)
    {
        _playerReadyDictionary[clientId] = false;
        OnReadyChanged?.Invoke();
        OnUnreadyChanged?.Invoke();
    }

    public bool AreAllPlayersReady()  
    {  
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)  
        {  
            if (!_playerReadyDictionary.ContainsKey(clientId) || !_playerReadyDictionary[clientId])  
            {  
                return false;  
            }  
        }  
        return true;  
    }

    public bool IsPlayerReady(ulong clientId)
    {
        return _playerReadyDictionary.ContainsKey(clientId) && _playerReadyDictionary[clientId];
    }
}

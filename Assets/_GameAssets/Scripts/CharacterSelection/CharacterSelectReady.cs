using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectReady : NetworkBehaviour
{
    public static CharacterSelectReady Instance { get; private set; }

    public event Action OnReadyChanged;

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
            NetworkManager.Singleton.SceneManager.LoadScene
                (Consts.SceneNames.GAME_SCENE, LoadSceneMode.Single);
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
    }

    public bool IsPlayerReady(ulong clientId)
    {
        return _playerReadyDictionary.ContainsKey(clientId) && _playerReadyDictionary[clientId];
    }
}

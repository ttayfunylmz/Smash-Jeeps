using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerGameManager : NetworkBehaviour
{
    public static MultiplayerGameManager Instance { get; private set; }

    public event Action OnPlayerDataNetworkListChanged;

    [SerializeField] private List<Color> _playerColorList;

    private NetworkList<PlayerDataSerializable> _playerDataNetworkList;

    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);

        _playerDataNetworkList = new NetworkList<PlayerDataSerializable>();
        _playerDataNetworkList.OnListChanged += PlayerDataNetworkList_OnListChanged;
    }

    public override void OnNetworkSpawn()
    {
        if(IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;

        }
    }

    private void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        _playerDataNetworkList.Add(new PlayerDataSerializable
        {
            ClientId = clientId,
            ColorId = GetFirstUnusedColorId()
        }); 
    }

    private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerDataSerializable> changeEvent)
    {
        OnPlayerDataNetworkListChanged?.Invoke();
    }

    public bool IsPlayerIndexConnected(int playerIndex)
    {
        return playerIndex < _playerDataNetworkList.Count;
    }

    public PlayerDataSerializable GetPlayerDataFromClientId(ulong clientId)
    {
        foreach(PlayerDataSerializable playerData in _playerDataNetworkList)
        {
            if(playerData.ClientId == clientId)
            {
                return playerData;
            }
        }

        return default;
    }

    public int GetPlayerDataIndexFromClientId(ulong clientId)
    {
        for(int i = 0; i < _playerDataNetworkList.Count; ++i)
        {
            if(_playerDataNetworkList[i].ClientId == clientId)
            {
                return i;
            }
        }

        return -1;
    }

    public PlayerDataSerializable GetPlayerData()
    {
        return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
    }

    public PlayerDataSerializable GetPlayerDataFromPlayerIndex(int playerIndex)
    {
        return _playerDataNetworkList[playerIndex];
    }

    public Color GetPlayerColor(int colorId)
    {
        return _playerColorList[colorId];
    }

    public void ChangePlayerColor(int colorId)
    {
        ChangePlayerColorRpc(colorId);
    }

    [Rpc(SendTo.Server)]
    private void ChangePlayerColorRpc(int colorId, RpcParams rpcParams = default)
    {
        if(!IsColorAvailable(colorId))
        {
            // COLOR NOT AVAILABLE
            return;
        }

        int playerDataIndex = GetPlayerDataIndexFromClientId(rpcParams.Receive.SenderClientId);
        PlayerDataSerializable playerData = _playerDataNetworkList[playerDataIndex];
        playerData.ColorId = colorId;
        _playerDataNetworkList[playerDataIndex] = playerData;
    }

    private bool IsColorAvailable(int colorId)
    {
        foreach(PlayerDataSerializable playerData in _playerDataNetworkList)
        {
            if(playerData.ColorId == colorId)
            {
                // ALREADY USING
                return false;
            }
        }

        return true;
    }

    private int GetFirstUnusedColorId()
    {
        for(int i = 0; i < _playerColorList.Count; ++i)
        {
            if(IsColorAvailable(i))
            {
                return i;
            }
        }

        return -1;
    }
}

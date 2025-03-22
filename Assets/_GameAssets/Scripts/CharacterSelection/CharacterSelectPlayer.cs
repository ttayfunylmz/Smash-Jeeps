using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : NetworkBehaviour
{
    [SerializeField] private int _playerIndex;
    [SerializeField] private TMP_Text _playerNameText;
    [SerializeField] private GameObject _readyGameObject;
    [SerializeField] private CharacterSelectVisual _characterSelectVisual;
    [SerializeField] private Button _kickButton;

    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();

    private void Awake()
    {
        _kickButton.onClick.AddListener(OnKickButtonClicked);
    }

    private void Start()
    {
        MultiplayerGameManager.Instance.OnPlayerDataNetworkListChanged
            += MultiplayerGameManager_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChanged += CharacterSelectReady_OnReadyChanged;

        UpdatePlayer();

        HandlePlayerNameChanged(string.Empty, PlayerName.Value);
        PlayerName.OnValueChanged += HandlePlayerNameChanged;
    }

    private void HandlePlayerNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
    {
        _playerNameText.text = newName.ToString();
    }

    private void OnKickButtonClicked()
    {
        PlayerDataSerializable playerData = MultiplayerGameManager.Instance.GetPlayerDataFromPlayerIndex(_playerIndex);
        MultiplayerGameManager.Instance.KickPlayer(playerData.ClientId);
    }

    private void CharacterSelectReady_OnReadyChanged()
    {
        UpdatePlayer();
    }

    private void MultiplayerGameManager_OnPlayerDataNetworkListChanged()
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        if (MultiplayerGameManager.Instance.IsPlayerIndexConnected(_playerIndex))
        {
            Show();

            PlayerDataSerializable playerData
                = MultiplayerGameManager.Instance.GetPlayerDataFromPlayerIndex(_playerIndex);
            _readyGameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.ClientId));
            _characterSelectVisual.SetPlayerColor(MultiplayerGameManager.Instance.GetPlayerColor(playerData.ColorId));
            HideKickButtonOnHost(playerData);
            SetOwner(playerData.ClientId);
            UpdatePlayerNameRpc();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void HideKickButtonOnHost(PlayerDataSerializable playerData)
    {
        _kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer && playerData.ClientId != NetworkManager.Singleton.LocalClientId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdatePlayerNameRpc()
    {
        if(IsServer)
        {
            UserData userData = HostSingleton.Instance.HostManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            PlayerName.Value = userData.UserName;
        }
    }

    private void SetOwner(ulong clientId)
    {
        if (IsServer)
        {
            var networkObject = GetComponent<NetworkObject>();
            
            if(networkObject.OwnerClientId != clientId)
            {
                networkObject.ChangeOwnership(clientId);
            }
        }
    }

    public override void OnDestroy()
    {
        MultiplayerGameManager.Instance.OnPlayerDataNetworkListChanged
            -= MultiplayerGameManager_OnPlayerDataNetworkListChanged;
        CharacterSelectReady.Instance.OnReadyChanged -= CharacterSelectReady_OnReadyChanged;
        PlayerName.OnValueChanged -= HandlePlayerNameChanged;
    }
    
}

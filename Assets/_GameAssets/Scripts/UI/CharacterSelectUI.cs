using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    public static CharacterSelectUI Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _readyButton;
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _copyButton;
    [SerializeField] private TMP_Text _readyText;
    [SerializeField] private TMP_Text _joinCodeText;
    [SerializeField] private Image _copiedImage;

    [Header("Settings")]
    [SerializeField] private Sprite _tickSprite;
    [SerializeField] private Sprite _crossSprite;
    [SerializeField] private Sprite _greenButtonSprite;
    [SerializeField] private Sprite _redButtonSprite;

    private bool _isPlayerReady;

    private void Awake()
    {
        Instance = this;

        _mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        _readyButton.onClick.AddListener(OnReadyButtonClicked);
        _startButton.onClick.AddListener(OnStartButtonClicked);
        _copyButton.onClick.AddListener(OnCopyButtonClicked);
    }

    private void Start()
    {
        _startButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);
        _mainMenuButton.gameObject.SetActive(!NetworkManager.Singleton.IsServer);
        SetStartButtonInteractable(false);

        _copiedImage.sprite = _crossSprite;

        CharacterSelectReady.Instance.OnAllPlayersReady += CharacterSelectReady_OnAllPlayersReady;
        CharacterSelectReady.Instance.OnUnreadyChanged += CharacterSelectReady_OnUnreadyChanged;
        MultiplayerGameManager.Instance.OnPlayerDataNetworkListChanged += MultiplayerGameManager_OnPlayerDataNetworkListChanged;
    }

    private void MultiplayerGameManager_OnPlayerDataNetworkListChanged()
    {
        if (CharacterSelectReady.Instance.AreAllPlayersReady())
        {
            SetStartButtonInteractable(true);
        }
        else
        {
            SetStartButtonInteractable(false);
        }
    }

    private void OnEnable()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            _joinCodeText.text = HostSingleton.Instance.HostManager.GetJoinCode();
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            _joinCodeText.text = ClientSingleton.Instance.ClientManager.GetJoinCode();
        }
    }

    private void CharacterSelectReady_OnUnreadyChanged()
    {
        SetStartButtonInteractable(false);
    }

    private void CharacterSelectReady_OnAllPlayersReady()
    {
        SetStartButtonInteractable(true);
    }

    private async void OnStartButtonClicked()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            try
            {
                await LobbyService.Instance.UpdateLobbyAsync(HostSingleton.Instance.HostManager.GetLobbyId(), new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject>
                    {
                        {
                            "GameStarted", new DataObject(
                                visibility: DataObject.VisibilityOptions.Public,
                                value: "true")
                        }
                    }
                });

                await LobbyService.Instance.DeleteLobbyAsync(HostSingleton.Instance.HostManager.GetLobbyId());
            }
            catch (LobbyServiceException lobbyServiceException)
            {
                Debug.LogError($"Failed to update or delete lobby: {lobbyServiceException}");
            }

            NetworkManager.Singleton.SceneManager.LoadScene(Consts.SceneNames.GAME_SCENE, LoadSceneMode.Single);
        }
    }

    private void OnCopyButtonClicked()
    {
        _copiedImage.sprite = _tickSprite;
        GUIUtility.systemCopyBuffer = _joinCodeText.text;
    }

    private void SetStartButtonInteractable(bool isActive)
    {
        if(_startButton != null)
        {
            _startButton.interactable = isActive;
        }
    }

    private void OnReadyButtonClicked()
    {
        _isPlayerReady = !_isPlayerReady;

        if (_isPlayerReady)
        {
            SetPlayerReady();
        }
        else
        {
            SetPlayerUnready();
        }
    }

    private void SetPlayerReady()
    {
        CharacterSelectReady.Instance.SetPlayerReady();
        _readyText.text = "Ready";
        _readyButton.image.sprite = _greenButtonSprite;
    }

    private void SetPlayerUnready()
    {
        CharacterSelectReady.Instance.SetPlayerUnready();
        _readyText.text = "Not Ready";
        _readyButton.image.sprite = _redButtonSprite;
    }

    private void OnMainMenuButtonClicked()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            HostSingleton.Instance.HostManager.Shutdown();
        }

        ClientSingleton.Instance.ClientManager.Disconnect();
    }

    public bool IsPlayerReady()
    {
        return _isPlayerReady;
    }
}

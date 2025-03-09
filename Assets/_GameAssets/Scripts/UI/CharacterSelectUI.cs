using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
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
        _mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        _readyButton.onClick.AddListener(OnReadyButtonClicked);
        _startButton.onClick.AddListener(OnStartButtonClicked);
        _copyButton.onClick.AddListener(OnCopyButtonClicked);
    }

    private void Start()
    {
        _startButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);
        _startButton.interactable = false;

        CharacterSelectReady.Instance.OnAllPlayersReady += CharacterSelectReady_OnAllPlayersReady;
        CharacterSelectReady.Instance.OnUnreadyChanged += CharacterSelectReady_OnUnreadyChanged;
        MultiplayerGameManager.Instance.OnPlayerDataNetworkListChanged += MultiplayerGameManager_OnPlayerDataNetworkListChanged;
    }

    private void MultiplayerGameManager_OnPlayerDataNetworkListChanged()
    {
        if(CharacterSelectReady.Instance.AreAllPlayersReady())
        {
            _startButton.interactable = true;
        }
        else
        {
            _startButton.interactable = false;
        }
    }

    private void OnEnable()
    {
        if(NetworkManager.Singleton.IsHost)
        {
            _joinCodeText.text = HostSingleton.Instance.HostManager.GetJoinCode();
        }
        else if(NetworkManager.Singleton.IsClient)
        {
            _joinCodeText.text = ClientSingleton.Instance.ClientManager.GetJoinCode();
        }
    }

    private void CharacterSelectReady_OnUnreadyChanged()
    {
        _startButton.interactable = false;   
    }

    private void CharacterSelectReady_OnAllPlayersReady()
    {
        _startButton.interactable = true;
    }

    private void OnStartButtonClicked()
    {
        NetworkManager.Singleton.SceneManager.LoadScene(Consts.SceneNames.GAME_SCENE, LoadSceneMode.Single);
    }

    private void OnCopyButtonClicked()
    {
        _copiedImage.sprite = _tickSprite;
        GUIUtility.systemCopyBuffer = _joinCodeText.text;
    }

    private void OnReadyButtonClicked()
    {
        _isPlayerReady = !_isPlayerReady;

        if(_isPlayerReady)
        {
            CharacterSelectReady.Instance.SetPlayerReady();
            _readyText.text = "Ready";
            _readyButton.image.sprite = _greenButtonSprite;
        }
        else
        {
            CharacterSelectReady.Instance.SetPlayerUnready();
            _readyText.text = "Not Ready";
            _readyButton.image.sprite = _redButtonSprite;
        }
    }

    private void OnMainMenuButtonClicked()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            HostSingleton.Instance.HostManager.Shutdown();
        }

        ClientSingleton.Instance.ClientManager.Disconnect();
    }
}

using System;
using DG.Tweening;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _volumeButton;

    [Header("Settings Menu")]
    [SerializeField] private RectTransform _settingsMenuTransform;
    [SerializeField] private Image _blackBackgroundImage;
    [SerializeField] private Button _vsyncButton;
    [SerializeField] private GameObject _vsyncTick;
    [SerializeField] private Button _soundButton;
    [SerializeField] private GameObject _soundTick;
    [SerializeField] private Button _musicButton;
    [SerializeField] private GameObject _musicTick;
    [SerializeField] private Button _leaveGameButton;
    [SerializeField] private Button _keepPlayingButton;
    [SerializeField] private Button _copyCodeButton;
    [SerializeField] private Image _copiedImage;
    [SerializeField] private TMP_Text _joinCodeText;

    [Header("Sprites")]
    [SerializeField] private Sprite _audioOnSprite;
    [SerializeField] private Sprite _audioOffSprite;
    [SerializeField] private Sprite _tickSprite;
    [SerializeField] private Sprite _crossSprite;

    [Header("Settings")]
    [SerializeField] private float _animationDuration = 0.75f;

    private Image _volumeImage;
    private bool _isAnimating;

    private bool _isVsyncActive;
    private bool _isSoundActive = true;
    private bool _isMusicActive = true;
    private bool _isCopiedJoinCode;

    private void Awake()
    {
        _volumeImage = _volumeButton.GetComponent<Image>();

        _vsyncButton.onClick.AddListener(OnVsyncButtonClicked);
        _soundButton.onClick.AddListener(OnSoundButtonClicked);
        _musicButton.onClick.AddListener(OnMusicButtonClicked);
        _settingsButton.onClick.AddListener(OnSettingsButtonClicked);
        _volumeButton.onClick.AddListener(OnVolumeButtonClicked);
        _leaveGameButton.onClick.AddListener(OnLeaveGameButtonClicked);
        _keepPlayingButton.onClick.AddListener(OnKeepPlayingButtonClicked);
        _copyCodeButton.onClick.AddListener(OnCopyCodeButtonClicked);
    }

    private void OnCopyCodeButtonClicked()
    {
        if (_isCopiedJoinCode) { return; }

        _isCopiedJoinCode = true;
        _copiedImage.sprite = _tickSprite;
        GUIUtility.systemCopyBuffer = _joinCodeText.text;
    }

    private void OnMusicButtonClicked()
    {
        _isMusicActive = !_isMusicActive;
        _musicTick.SetActive(_isMusicActive);
    }

    private void OnSoundButtonClicked()
    {
        SetVolume();
    }

    private void OnVsyncButtonClicked()
    {
        _isVsyncActive = !_isVsyncActive;
        QualitySettings.vSyncCount = _isVsyncActive ? 1 : 0;
        _vsyncTick.SetActive(_isVsyncActive);
    }

    private void Start()
    {
        _settingsMenuTransform.localScale = Vector3.zero;
        _settingsMenuTransform.gameObject.SetActive(false);
    }

    private void OnKeepPlayingButtonClicked()
    {
        if (_isAnimating) { return; }

        _isAnimating = true;
        _blackBackgroundImage.raycastTarget = false;
        _blackBackgroundImage.DOFade(0f, _animationDuration);
        _settingsMenuTransform.DOScale(0f, _animationDuration).SetEase(Ease.InBack).OnComplete(() =>
        {
            _settingsMenuTransform.gameObject.SetActive(false);
            _isAnimating = false;
            _isCopiedJoinCode = false;
            _copiedImage.sprite = _crossSprite;
        });
    }

    private void OnLeaveGameButtonClicked()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            HostSingleton.Instance.HostManager.Shutdown();
        }

        ClientSingleton.Instance.ClientManager.Disconnect();
    }

    private void OnVolumeButtonClicked()
    {
        SetVolume();
    }

    private void OnSettingsButtonClicked()
    {
        if (_isAnimating) { return; }

        _joinCodeText.text = HostSingleton.Instance.HostManager.GetJoinCode();
        _isAnimating = true;
        _blackBackgroundImage.raycastTarget = true;
        _settingsMenuTransform.gameObject.SetActive(true);
        _blackBackgroundImage.DOFade(.8f, _animationDuration);
        _settingsMenuTransform.DOScale(1f, _animationDuration).SetEase(Ease.OutBack).OnComplete(() =>
        {
            _isAnimating = false;
        });
    }

    private void SetVolume()
    {
        _isSoundActive = !_isSoundActive;
        _volumeImage.sprite = _isSoundActive ? _audioOnSprite : _audioOffSprite;
        _soundTick.SetActive(_isSoundActive);
    }
}

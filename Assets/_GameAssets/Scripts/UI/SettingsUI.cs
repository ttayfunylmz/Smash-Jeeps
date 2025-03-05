using System;
using DG.Tweening;
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
    [SerializeField] private Button _leaveGameButton;
    [SerializeField] private Button _keepPlayingButton;

    [Header("Sprites")]
    [SerializeField] private Sprite _audioOnSprite;
    [SerializeField] private Sprite _audioOffSprite;

    [Header("Settings")]
    [SerializeField] private float _animationDuration = 0.75f;

    private Image _volumeImage;
    private bool _isAudioActive = true;
    private bool _isAnimating;

    private void Awake()
    {
        _volumeImage = _volumeButton.GetComponent<Image>();

        _settingsButton.onClick.AddListener(OnSettingsButtonClicked);
        _volumeButton.onClick.AddListener(OnVolumeButtonClicked);
        _leaveGameButton.onClick.AddListener(OnLeaveGameButtonClicked);
        _keepPlayingButton.onClick.AddListener(OnKeepPlayingButtonClicked);
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
        });
    }

    private void OnLeaveGameButtonClicked()
    {
        if(NetworkManager.Singleton.IsHost)
        {
            HostSingleton.Instance.HostManager.Shutdown();
        }

        ClientSingleton.Instance.ClientManager.Disconnect();
    }

    private void OnVolumeButtonClicked()
    {
        _isAudioActive = !_isAudioActive;
        _volumeImage.sprite = _isAudioActive ? _audioOnSprite : _audioOffSprite;
    }

    private void OnSettingsButtonClicked()
    {
        if (_isAnimating) { return; }

        _isAnimating = true;
        _blackBackgroundImage.raycastTarget = true;
        _settingsMenuTransform.gameObject.SetActive(true);
        _blackBackgroundImage.DOFade(.8f, _animationDuration);
        _settingsMenuTransform.DOScale(1f, _animationDuration).SetEase(Ease.OutBack).OnComplete(() =>
        {
            _isAnimating = false;
        });
    }
}

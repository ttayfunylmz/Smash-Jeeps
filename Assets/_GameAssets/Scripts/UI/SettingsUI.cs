using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _volumeButton;

    [Header("Sprites")]
    [SerializeField] private Sprite _audioOnSprite;
    [SerializeField] private Sprite _audioOffSprite;

    private Image _volumeImage;
    private bool _isAudioActive = true;

    private void Awake()
    {
        _volumeImage = _volumeButton.GetComponent<Image>();

        _settingsButton.onClick.AddListener(OnSettingsButtonClicked);
        _volumeButton.onClick.AddListener(OnVolumeButtonClicked);
    }

    private void OnVolumeButtonClicked()
    {
        _isAudioActive = !_isAudioActive;
        _volumeImage.sprite = _isAudioActive ? _audioOnSprite : _audioOffSprite;
    }

    private void OnSettingsButtonClicked()
    {
        
    }
}

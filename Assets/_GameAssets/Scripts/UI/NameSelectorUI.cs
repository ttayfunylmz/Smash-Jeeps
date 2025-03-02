using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NameSelectorUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_InputField _nameInputField;
    [SerializeField] private Button _connectButton;
    [SerializeField] private TMP_Text _warningText;

    [Header("Settings")]
    [SerializeField] private float _animationDuration;
    [SerializeField] private float _fadeDuration;

    private RectTransform _warningTransform;

    private bool _isAnimating;

    private void Awake()
    {
        _warningTransform = _warningText.GetComponent<RectTransform>();

        _connectButton.onClick.AddListener(OnConnectButtonClicked);
    }

    private void Start()
    {
        if(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
        {
            SceneManager.LoadScene(Consts.SceneNames.LOADING_SCENE);
            return;
        }

        _nameInputField.text = PlayerPrefs.GetString(Consts.PlayerData.PLAYER_NAME, string.Empty);

        _warningText.text = string.Empty;
        _warningText.alpha = 0f;
        _warningTransform.gameObject.SetActive(false);
    }

    private void OnConnectButtonClicked()
    {
        if(_nameInputField.text == string.Empty)
        {
            _warningText.text = "Name cannot be empty!";
            AnimateWarningText();
            return;
        }

        if(_nameInputField.text.Contains(" "))
        {
            _warningText.text = "Name cannot contain spaces!";
            AnimateWarningText();
            return;
        }

        PlayerPrefs.SetString(Consts.PlayerData.PLAYER_NAME, _nameInputField.text);
        SceneManager.LoadScene(Consts.SceneNames.LOADING_SCENE);
    }

    private void AnimateWarningText()
    {
        if(_isAnimating) { return; }

        _isAnimating = true;
        _warningTransform.gameObject.SetActive(true);
        _warningText.DOFade(1f, _fadeDuration);
        _warningTransform.DOAnchorPosY(200f, _animationDuration).SetEase(Ease.OutSine).OnComplete(() =>
        {
            _warningText.DOFade(0f, _fadeDuration).OnComplete(() =>
            {
                _warningTransform.anchoredPosition = Vector3.zero;
                _warningTransform.gameObject.SetActive(false);
                _isAnimating = false;
            });
        });
    }
}

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenuUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LobbiesListUI _lobbiesListUI;
    [SerializeField] private Button _hostButton;
    [SerializeField] private Button _clientButton;
    [SerializeField] private Button _lobbiesButton;
    [SerializeField] private Button _closeButton;
    [SerializeField] private TMP_InputField _joinCodeInputField;
    [SerializeField] private GameObject _lobbiesParentGameObject;
    [SerializeField] private RectTransform _lobbiesBackgroundTransform;
    [SerializeField] private TMP_Text _welcomeText;
    [SerializeField] private TMP_Text _warningText;

    [Header("Settings")]
    [SerializeField] private float _animationDuration = 1f;
    [SerializeField] private float _textAnimationDuration;
    [SerializeField] private float _fadeDuration;

    private RectTransform _warningTransform;
    private bool _isAnimating;

    private void Awake()
    {
        _warningTransform = _warningText.GetComponent<RectTransform>();

        _hostButton.onClick.AddListener(StartHost);
        _clientButton.onClick.AddListener(StartClient);
        _lobbiesButton.onClick.AddListener(OpenLobbies);
        _closeButton.onClick.AddListener(CloseLobbies);
    }

    private void Start()
    {
        _warningText.text = string.Empty;
        _warningText.alpha = 0f;
        _warningTransform.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        var playerName = PlayerPrefs.GetString(Consts.PlayerData.PLAYER_NAME, string.Empty);
        _welcomeText.text = $"welcome, <color=yellow>{playerName}</color>";
    }

    private async void StartHost()
    {
        _hostButton.interactable = false;
        await HostSingleton.Instance.HostManager.StartHostAsync();
    }

    private async void StartClient()
    {
        if(_joinCodeInputField.text == string.Empty || _joinCodeInputField.text.Contains(" "))
        {
            _warningText.text = "Enter a valid Join Code!";
            AnimateWarningText();
            return;
        }

        await ClientSingleton.Instance.ClientManager.StartClientAsync(_joinCodeInputField.text);
    }

    private void OpenLobbies()
    {
        _lobbiesParentGameObject.SetActive(true);
        _lobbiesBackgroundTransform.DOAnchorPosX(-650f, _animationDuration).SetEase(Ease.OutBack);

        _lobbiesListUI.RefreshList();
    }

    private void CloseLobbies()
    {
        _lobbiesBackgroundTransform.DOAnchorPosX(900f, _animationDuration).SetEase(Ease.InBack).OnComplete(() =>
        {
            _lobbiesParentGameObject.SetActive(false);
        });
    }

    private void AnimateWarningText()
    {
        if(_isAnimating) { return; }

        _isAnimating = true;
        _warningTransform.gameObject.SetActive(true);
        _warningText.DOFade(1f, _fadeDuration);
        _warningTransform.DOAnchorPosY(200f, _textAnimationDuration).SetEase(Ease.OutSine).OnComplete(() =>
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

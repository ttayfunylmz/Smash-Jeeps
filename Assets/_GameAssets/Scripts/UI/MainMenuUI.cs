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

    [Header("Settings")]
    [SerializeField] private float _animationDuration = 1f;

    private void Awake()
    {
        _hostButton.onClick.AddListener(StartHost);
        _clientButton.onClick.AddListener(StartClient);
        _lobbiesButton.onClick.AddListener(OpenLobbies);
        _closeButton.onClick.AddListener(CloseLobbies);
    }

    private void OnEnable()
    {
        var playerName = PlayerPrefs.GetString(Consts.PlayerData.PLAYER_NAME, string.Empty);
        _welcomeText.text = $"welcome, <color=yellow>{playerName}</color>";
    }

    private async void StartHost()
    {
        await HostSingleton.Instance.HostManager.StartHostAsync();
    }

    private async void StartClient()
    {
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
}

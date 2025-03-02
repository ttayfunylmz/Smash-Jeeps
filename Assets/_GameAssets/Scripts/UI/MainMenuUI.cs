using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button _hostButton;
    [SerializeField] private Button _clientButton;
    [SerializeField] private Button _lobbiesButton;
    [SerializeField] private TMP_InputField _joinCodeInputField;
    [SerializeField] private GameObject _lobbiesParentGameObject;

    private void Awake()
    {
        _hostButton.onClick.AddListener(StartHost);
        _clientButton.onClick.AddListener(StartClient);
        _lobbiesButton.onClick.AddListener(() =>
        {
            _lobbiesParentGameObject.SetActive(true);
        });
    }

    private async void StartHost()
    {
        await HostSingleton.Instance.HostManager.StartHostAsync();
    }

    private async void StartClient()
    {
        await ClientSingleton.Instance.ClientManager.StartClientAsync(_joinCodeInputField.text);
    }
}

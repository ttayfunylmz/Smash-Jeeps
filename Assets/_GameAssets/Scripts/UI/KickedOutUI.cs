using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KickedOutUI : MonoBehaviour
{
    [SerializeField] private Button _mainMenuButton;

    private void Awake()
    {
        _mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
    }

    private void Start() 
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;

        gameObject?.SetActive(false);    
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if(clientId == NetworkManager.ServerClientId)
        {
            // SERVER IS SHUTTING DOWN
            gameObject?.SetActive(true);
        }
    }

    private void OnMainMenuButtonClicked()
    {
        SceneManager.LoadScene(Consts.SceneNames.MAIN_MENU_SCENE);
    }

    private void OnDestroy()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;
    }
}

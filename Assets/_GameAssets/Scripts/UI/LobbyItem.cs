using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyItem : MonoBehaviour
{
    [SerializeField] private TMP_Text _lobbyNameText;
    [SerializeField] private TMP_Text _lobbyPlayersText;
    [SerializeField] private Button _joinButton;

    private LobbiesListUI _lobbiesListUI;
    private Lobby _lobby;

    private void Awake()
    {
        _joinButton.onClick.AddListener(OnJoinButtonClicked);
    }

    public void SetupLobbyItem(LobbiesListUI lobbiesListUI, Lobby lobby)
    {
        _lobbiesListUI = lobbiesListUI;
        _lobby = lobby;

        _lobbyNameText.text = lobby.Name;
        _lobbyPlayersText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
    }

    public void OnJoinButtonClicked()
    {
        _lobbiesListUI.JoinAsync(_lobby);
    }
}

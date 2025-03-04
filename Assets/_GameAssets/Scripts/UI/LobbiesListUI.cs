using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbiesListUI : MonoBehaviour
{
    [SerializeField] private Transform _lobbyItemParent;
    [SerializeField] private LobbyItem _lobbyItemPrefab;
    [SerializeField] private Button _refreshButton;

    private bool _isJoining;
    private bool _isRefreshing;

    private void Awake()
    {
        _refreshButton.onClick.AddListener(RefreshList);
    }

    public async void RefreshList()
    {
        if (_isRefreshing) { return; }

        _isRefreshing = true;

        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions();
            queryLobbiesOptions.Count = 25;
            queryLobbiesOptions.Filters = new List<QueryFilter>()
            {
                new QueryFilter
                (
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0"
                ),
                new QueryFilter
                (
                    field: QueryFilter.FieldOptions.IsLocked,
                    op: QueryFilter.OpOptions.EQ,
                    value: "0"
                )
            };

            QueryResponse lobbies = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            foreach(Transform child in _lobbyItemParent)
            {
                Destroy(child.gameObject);
            }

            foreach (Lobby lobby in lobbies.Results)
            {
                LobbyItem lobbyItem = Instantiate(_lobbyItemPrefab, _lobbyItemParent);
                lobbyItem.SetupLobbyItem(this, lobby);
            }
        }
        catch (LobbyServiceException lobbyServiceException)
        {
            Debug.LogError(lobbyServiceException);
        }



        _isRefreshing = false;
    }

    public async void JoinAsync(Lobby lobby)
    {
        if (_isJoining) { return; }

        _isJoining = true;

        try
        {
            Lobby joiningLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joiningLobby.Data["JoinCode"].Value;

            await ClientSingleton.Instance.ClientManager.StartClientAsync(joinCode);
        }
        catch (LobbyServiceException lobbyServiceException)
        {
            Debug.LogError(lobbyServiceException);
        }

        _isJoining = false;
    }
}

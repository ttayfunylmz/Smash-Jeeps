using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Smooth;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class SpawnerManager : NetworkBehaviour
{
    public event Action OnPlayerRespawned;

    public static SpawnerManager Instance { get; private set; }

    [SerializeField] private List<Transform> _spawnPointTransformList;
    [SerializeField] private List<Transform> _respawnPointTransformList;
    [SerializeField] private GameObject _playerPrefab;

    private List<int> _availableSpawnIndexList = new List<int>();

    private void Awake()
    {
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if(!IsServer) { return; }

        for(int i = 0; i < _spawnPointTransformList.Count; ++i)
        {
            _availableSpawnIndexList.Add(i);
        }


        SpawnAllPlayers();

        // CLIENT CONNECTED CALLBACK IS WORKING FOR NEWLY JOINING CLIENTS
        // if(IsHost)
        // {
        //     SpawnPlayer(NetworkManager.LocalClientId);
        // }
    }

    public void SpawnAllPlayers()
    {
        if (!IsServer) return;

        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            SpawnPlayer(client.ClientId);
        }
    }

    private void SpawnPlayer(ulong clientId)
    {
        if(_availableSpawnIndexList.Count == 0)
        {
            Debug.LogError("No available Spawn Points!");
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, _availableSpawnIndexList.Count);
        int spawnIndex = _availableSpawnIndexList[randomIndex];
        _availableSpawnIndexList.RemoveAt(randomIndex);

        Transform spawnPointTransform = _spawnPointTransformList[spawnIndex];
        GameObject playerInstance = Instantiate(_playerPrefab, spawnPointTransform.position, spawnPointTransform.rotation);
        playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }

    public void RespawnPlayer(int respawnTimer, ulong clientId)
    {
        StartCoroutine(RespawnPlayerCoroutine(respawnTimer, clientId));
    }

    private IEnumerator RespawnPlayerCoroutine(int respawnTimer, ulong clientId)
    {
        yield return new WaitForSeconds(respawnTimer);

        if (_respawnPointTransformList.Count == 0)
        {
            Debug.LogError("No available Respawn Points!");
            yield break;
        }

        if (!NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId))
        {
            Debug.LogError($"Client {clientId} not found!");
            yield break;
        }

        int randomIndex = UnityEngine.Random.Range(0, _respawnPointTransformList.Count);
        Transform respawnPoint = _respawnPointTransformList[randomIndex];

        NetworkObject playerNetworkObject = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;

        if (playerNetworkObject == null)
        {
            Debug.LogError($"Player object for Client {clientId} not found!");
            yield break;
        }

        if (playerNetworkObject.TryGetComponent<Rigidbody>(out var playerRigidbody))
        {
            playerRigidbody.isKinematic = true;
        }

        if(playerNetworkObject.TryGetComponent<NetworkTransform>(out var playerNetworkTransform))
        {
            playerNetworkTransform.Interpolate = false;
        }

        if(playerNetworkObject.TryGetComponent<PlayerVehicleVisualController>(out var playerVehicleVisualController))
        {
            playerVehicleVisualController.SetVehicleVisualActive(0.1f);
        }

        playerNetworkObject.transform.SetPositionAndRotation(respawnPoint.position, respawnPoint.rotation);

        yield return new WaitForSeconds(0.1f);

        playerRigidbody.isKinematic = false;
        playerNetworkTransform.Interpolate = true;

        OnPlayerRespawned?.Invoke();
    }
}

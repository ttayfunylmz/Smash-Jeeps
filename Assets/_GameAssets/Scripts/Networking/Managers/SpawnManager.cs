using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Smooth;
using Unity.Netcode;
using UnityEngine;

public class SpawnManager : NetworkBehaviour
{
    public event Action OnPlayerRespawned;

    public static SpawnManager Instance { get; private set; }

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

        NetworkManager.OnClientConnectedCallback += SpawnPlayer;
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

        OnPlayerRespawned?.Invoke();

        playerNetworkObject.transform.GetComponent<Rigidbody>().isKinematic = true;

        yield return null;

        playerNetworkObject.transform.SetPositionAndRotation(respawnPoint.position, respawnPoint.rotation);
        playerNetworkObject.transform.GetComponent<Rigidbody>().isKinematic = false;
    }

}

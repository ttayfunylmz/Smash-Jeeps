using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Smooth;
using Unity.Netcode;
using UnityEngine;

public class SpawnManager : NetworkBehaviour
{
    [SerializeField] private List<Transform> _spawnPointTransformList;
    [SerializeField] private GameObject _playerPrefab;

    private List<int> _availableSpawnIndexList = new List<int>();

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
}

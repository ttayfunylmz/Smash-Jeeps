using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkController : NetworkBehaviour
{
    [SerializeField] private CinemachineCamera _playerCinemachineCamera;

    public override void OnNetworkSpawn()
    {
        _playerCinemachineCamera.gameObject.SetActive(IsOwner);
    }

    
}

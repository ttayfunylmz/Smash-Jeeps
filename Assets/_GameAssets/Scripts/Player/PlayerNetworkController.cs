using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkController : NetworkBehaviour
{
    [SerializeField] private CinemachineCamera _playerCinemachineCamera;
    [SerializeField] private MeshRenderer _baseMeshRenderer;

    public override void OnNetworkSpawn()
    {
        _playerCinemachineCamera.gameObject.SetActive(IsOwner);
    }

    public void SetBaseColor(Color color)
    {
        _baseMeshRenderer.material.color = color;
    }
}

using Unity.Netcode;
using UnityEngine;

public class RocketController : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Collider _rocketCollider;

    [Header("Settings")]
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _rotationSpeed;

    private bool _isSpawned;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            SetOwnerVisualsRpc();
        }
    }

    private void OnEnable()
    {
        _isSpawned = true;
    }

    private void Update()
    {
        if(_isSpawned)
        {
            MovementRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void MovementRpc()
    {
        transform.position += _movementSpeed * Time.deltaTime * transform.forward;
        transform.Rotate(Vector3.forward, _rotationSpeed * Time.deltaTime, Space.Self);
    }

    [Rpc(SendTo.Owner)]
    public void SetOwnerVisualsRpc()
    {
        _rocketCollider.enabled = false;
    }
}

using Unity.Netcode;
using UnityEngine;

public class RocketController : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Collider _rocketCollider;

    [Header("Settings")]
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _rotationSpeed;

    private bool _isMoving = false;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            SetOwnerVisualsRpc();
            RequestStartMovementFromServerRpc();
        }
    }

    [Rpc(SendTo.Owner)]
    public void SetOwnerVisualsRpc()
    {
        _rocketCollider.enabled = false;
    }

    private void Update()
    {
        if (IsServer && _isMoving)
        {
            MoveRocket();
        }
    }

    [Rpc(SendTo.Server)]
    private void RequestStartMovementFromServerRpc()
    {
        _isMoving = true;
    }

    private void MoveRocket()
    {
        transform.position += _movementSpeed * Time.deltaTime * transform.forward;
        transform.Rotate(Vector3.forward, _rotationSpeed * Time.deltaTime, Space.Self);
    }
}

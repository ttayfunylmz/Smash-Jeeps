using Unity.Netcode;
using UnityEngine;

public class RocketController : NetworkBehaviour
{
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _rotationSpeed;

    private bool _isSpawned;

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
}

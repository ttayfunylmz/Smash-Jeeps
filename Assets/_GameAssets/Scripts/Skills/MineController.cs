using Unity.Netcode;
using UnityEngine;

public class MineController : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Collider _mineCollider;

    [Header("Settings")]
    [SerializeField] private float _fallSpeed = 5f;
    [SerializeField] private float _raycastDistance = 0.5f;
    [SerializeField] private LayerMask _groundLayer;

    private bool _hasLanded;
    private Vector3 _lastSentPosition;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            SetOwnerVisualsRpc();
        }
    }

    private void Update()
    {
        if (!IsServer || _hasLanded) return;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _raycastDistance, _groundLayer))
        {
            _hasLanded = true;
            transform.position = hit.point;
            if (_lastSentPosition != transform.position)
            {
                SyncPositionClientRpc(transform.position);
                _lastSentPosition = transform.position;
            }
        }
        else
        {
            transform.position += _fallSpeed * Time.deltaTime * Vector3.down;

            if (_lastSentPosition != transform.position)
            {
                SyncPositionClientRpc(transform.position);
                _lastSentPosition = transform.position;
            }
        }
    }

    [ClientRpc]
    private void SyncPositionClientRpc(Vector3 newPosition)
    {
        if (IsServer) return;
        transform.position = newPosition;
    }

    [Rpc(SendTo.Owner)]
    public void SetOwnerVisualsRpc()
    {
        _mineCollider.enabled = false;
    }
}

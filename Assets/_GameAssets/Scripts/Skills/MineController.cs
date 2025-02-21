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
            SyncPositionClientRpc(transform.position);
        }
        else
        {
            transform.position += Vector3.down * _fallSpeed * Time.deltaTime;
            SyncPositionClientRpc(transform.position);
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

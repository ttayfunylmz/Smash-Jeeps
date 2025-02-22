using Unity.Netcode;
using UnityEngine;

public class SpikeController : NetworkBehaviour
{
    [SerializeField] private Collider _spikeCollider;

    public override void OnNetworkSpawn()
    {
        PlayerSkillController.OnTimerFinished += PlayerSkillController_OnTimerFinished;

        if (IsOwner)
        {
            SetOwnerVisualsRpc();
        }
    }

    private void PlayerSkillController_OnTimerFinished()
    {
        DestroyRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DestroyRpc()
    {
        if(IsServer)
        {
            Destroy(gameObject);
        }
    }

    [Rpc(SendTo.Owner)]
    public void SetOwnerVisualsRpc()
    {
        _spikeCollider.enabled = false;
    }

    public override void OnNetworkDespawn()
    {
        PlayerSkillController.OnTimerFinished -= PlayerSkillController_OnTimerFinished;
    }
}

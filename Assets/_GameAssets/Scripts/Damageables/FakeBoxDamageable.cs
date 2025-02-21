using Unity.Netcode;
using UnityEngine;

public class FakeBoxDamageable : NetworkBehaviour, IDamageable
{
    public void Damage(PlayerVehicleController playerVehicleController)
    {
        playerVehicleController.CrashVehicle();
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
}

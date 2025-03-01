using Unity.Netcode;
using UnityEngine;

public class RocketDamageable : NetworkBehaviour, IDamageable
{
    [SerializeField] private MysteryBoxSkillsSO _mysteryBoxSkill;
    [SerializeField] private GameObject _explosionParticlesPrefab;

    private bool _isDestroyed;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(OwnerClientId, out var client))
            {
                NetworkObject ownerNetworkObject = client.PlayerObject;
                PlayerVehicleController playerVehicleController = ownerNetworkObject.GetComponent<PlayerVehicleController>();
                playerVehicleController.OnVehicleCrashed += PlayerVehicleController_OnVehicleCrashed;
            }
        }
    }

    private void PlayerVehicleController_OnVehicleCrashed()
    {
        DestroyRpc();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ShieldController shieldController))
        {
            DestroyRpc();
        }

        if (other.TryGetComponent(out HittableWall hittableWall))
        {
            DestroyRpc();
        }
    }

    public void Damage(PlayerVehicleController playerVehicleController)
    {
        playerVehicleController.CrashVehicle();
        KillScreenUI.Instance.SetSmashedUI("Tayfun", _mysteryBoxSkill.SkillData.RespawnTimer);
        DestroyRpc();
    }

    public int GetRespawnTimer()
    {
        return _mysteryBoxSkill.SkillData.RespawnTimer;
    }

    [Rpc(SendTo.Server)]
    private void DestroyRpc()
    {
        GameObject explosionParticlesInstance = Instantiate(_explosionParticlesPrefab, transform.position, Quaternion.identity);
        explosionParticlesInstance.GetComponent<NetworkObject>().Spawn();

        GetComponent<NetworkObject>().Despawn();
    }

    public ulong GetKillerClientId()
    {
        return OwnerClientId;
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            if (NetworkManager.Singleton.ConnectedClients.TryGetValue(OwnerClientId, out var client))
            {
                NetworkObject ownerNetworkObject = client.PlayerObject;
                PlayerVehicleController playerVehicleController = ownerNetworkObject.GetComponent<PlayerVehicleController>();
                playerVehicleController.OnVehicleCrashed -= PlayerVehicleController_OnVehicleCrashed;
            }
        }
    }

    public int GetDamageAmount()
    {
        return _mysteryBoxSkill.SkillData.DamageAmount;
    }
}
using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class FakeBoxDamageable : NetworkBehaviour, IDamageable
{
    [SerializeField] private MysteryBoxSkillsSO _mysteryBoxSkill;
    [SerializeField] private GameObject _explosionParticlesPrefab;

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
    }

    public void Damage(PlayerVehicleController playerVehicleController)
    {
        playerVehicleController.CrashVehicle();
        KillScreenUI.Instance.SetSmashedUI("Tayfun", _mysteryBoxSkill.SkillData.RespawnTimer);
        DestroyRpc();
    }

    [Rpc(SendTo.Server)]
    private void DestroyRpc()
    {
        GameObject explosionParticlesInstance = Instantiate(_explosionParticlesPrefab, transform.position, Quaternion.identity);
        explosionParticlesInstance.GetComponent<NetworkObject>().Spawn();

        GetComponent<NetworkObject>().Despawn();
    }

    public int GetRespawnTimer()
    {
        return _mysteryBoxSkill.SkillData.RespawnTimer;
    }

    public ulong GetKillerClientId()
    {
        return OwnerClientId;
    }

    public int GetDamageAmount()
    {
        return _mysteryBoxSkill.SkillData.DamageAmount;
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
}
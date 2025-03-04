using Unity.Netcode;
using UnityEngine;

public class MineDamageable : NetworkBehaviour, IDamageable
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

    public void Damage(PlayerVehicleController playerVehicleController, string playerName)
    {
        playerVehicleController.CrashVehicle();

        KillScreenUI.Instance.SetSmashedUI(playerName, _mysteryBoxSkill.SkillData.RespawnTimer);
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

    public int GetDamageAmount()
    {
        return _mysteryBoxSkill.SkillData.DamageAmount;
    }

    public string GetKillerName()
    {
        ulong killerClientId = GetKillerClientId();

        if(NetworkManager.Singleton.ConnectedClients.TryGetValue(killerClientId, out var killerClient))
        {
            string playerName = killerClient.PlayerObject.GetComponent<PlayerNetworkController>().PlayerName.Value.ToString();
            return playerName;
        }

        return string.Empty;
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

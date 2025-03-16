using System.Collections;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class SpikeDamageable : NetworkBehaviour, IDamageable
{
    [SerializeField] private MysteryBoxSkillsSO _mysteryBoxSkill;

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
        DelayedDestroy();
    }

    private async void DelayedDestroy()
    {
        await UniTask.DelayFrame(5);
        DestroyRpc();
    }

    public void Damage(PlayerVehicleController playerVehicleController, string playerName)
    {
        playerVehicleController.CrashVehicle();

        KillScreenUI.Instance.SetSmashedUI(playerName, _mysteryBoxSkill.SkillData.RespawnTimer);
    }

    public int GetRespawnTimer()
    {
        return _mysteryBoxSkill.SkillData.RespawnTimer;
    }

    [Rpc(SendTo.Server)]
    private void DestroyRpc()
    {
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

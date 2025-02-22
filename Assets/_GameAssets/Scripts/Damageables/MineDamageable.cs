using Unity.Netcode;
using UnityEngine;

public class MineDamageable : NetworkBehaviour, IDamageable
{
    [SerializeField] private MysteryBoxSkillsSO _mysteryBoxSkill;

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out ShieldController shieldController))
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

    [Rpc(SendTo.ClientsAndHost)]
    private void DestroyRpc()
    {
        if(IsServer)
        {
            Destroy(gameObject);
        }
    }
}

using Unity.Netcode;
using UnityEngine;

public class MysteryBoxCollectible : NetworkBehaviour, ICollectible
{
    [Header("References")]
    [SerializeField] private MysteryBoxSkillsSO[] _mysteryBoxSkills;

    [Header("Settings")]
    [SerializeField] private float _respawnTimer;

    public void Collect(PlayerSkillController playerSkillController)
    {
        MysteryBoxSkillsSO skill = GetRandomSkill();
        SkillsUI.Instance.SetSkill(skill.SkillIcon, skill.SkillName);

        playerSkillController.ActivateSkill(skill);

        OnCollectRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void OnCollectRpc()
    {
        SetMysteryBoxActive(false);
        Invoke(nameof(SetMysteryBoxActiveTrueRpc), _respawnTimer);
    }

    private MysteryBoxSkillsSO GetRandomSkill()
    {
        int randomIndex = Random.Range(0, _mysteryBoxSkills.Length);
        return _mysteryBoxSkills[randomIndex];
    }

    private void SetMysteryBoxActiveTrueRpc()
    {
        SetMysteryBoxActive(true);
    }

    private void SetMysteryBoxActive(bool active)
    {
        gameObject.SetActive(active);
    }
}

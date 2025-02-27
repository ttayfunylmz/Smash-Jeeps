using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

public class MysteryBoxCollectible : NetworkBehaviour, ICollectible
{
    [Header("References")]
    [SerializeField] private MysteryBoxSkillsSO[] _mysteryBoxSkills;
    [SerializeField] private Animator _boxAnimator;
    [SerializeField] private Collider _collider;
    [SerializeField] private ParticleSystem _wowParticleSystem;

    [Header("Settings")]
    [SerializeField] private float _respawnTimer;
    [SerializeField] private float _animationDelay;

    public void Collect(PlayerSkillController playerSkillController)
    {
        if(playerSkillController.HasSkillAlready()) { return; }

        MysteryBoxSkillsSO skill = GetRandomSkill();
        SkillsUI.Instance.SetSkill(skill.SkillIcon, skill.SkillName, skill.SkillUsageType, skill.SkillData.SpawnAmountOrTimer);

        playerSkillController.SetupSkill(skill);
        _wowParticleSystem.Play();
        OnCollectRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void OnCollectRpc()
    {
        AnimateCollection();
        Invoke(nameof(Show), _respawnTimer);
    }

    private MysteryBoxSkillsSO GetRandomSkill()
    {
        int randomIndex = Random.Range(0, _mysteryBoxSkills.Length);
        return _mysteryBoxSkills[randomIndex];
    }

    private void AnimateCollection()
    {
        Invoke(nameof(Hide), _animationDelay);
        _boxAnimator.SetTrigger(Consts.BoxAnimations.IS_COLLECTED);
    }

    private void Show()
    {
        _boxAnimator.SetTrigger(Consts.BoxAnimations.IS_RESPAWNED);
        SetColliderEnabled(true);
    }

    private void Hide()
    {
        SetColliderEnabled(false);
    }

    private void SetColliderEnabled(bool enabled)
    {
        _collider.enabled = enabled;
    }
}

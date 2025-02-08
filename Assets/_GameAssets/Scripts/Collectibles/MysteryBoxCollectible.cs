using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

public class MysteryBoxCollectible : NetworkBehaviour, ICollectible
{
    [Header("References")]
    [SerializeField] private MysteryBoxSkillsSO[] _mysteryBoxSkills;

    [Header("Settings")]
    [SerializeField] private float _respawnTimer;
    [SerializeField] private float _rotationDuration;
    [SerializeField] private float _scaleDuration;

    public override void OnNetworkSpawn()
    {
        if(!IsHost) { return; }

        AnimateMysteryBox();
    }

    private void AnimateMysteryBox()
    {
        transform.DORotate(new Vector3(0f, 360f, 0f), _rotationDuration, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
    }

    public void Collect(PlayerSkillController playerSkillController)
    {
        if(playerSkillController.HasSkillAlready()) { return; }

        MysteryBoxSkillsSO skill = GetRandomSkill();
        SkillsUI.Instance.SetSkill(skill.SkillIcon, skill.SkillType.ToString());

        playerSkillController.SetupSkill(skill);

        OnCollectRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void OnCollectRpc()
    {
        transform.DOScale(0f, _scaleDuration).SetEase(Ease.InBack).OnComplete(() =>
        {
            SetMysteryBoxActive(false);
            Invoke(nameof(SetMysteryBoxActiveTrueRpc), _respawnTimer);
        });
    }

    private MysteryBoxSkillsSO GetRandomSkill()
    {
        int randomIndex = Random.Range(0, _mysteryBoxSkills.Length);
        return _mysteryBoxSkills[randomIndex];
    }

    private void SetMysteryBoxActiveTrueRpc()
    {
        SetMysteryBoxActive(true);
        transform.DOScale(1f, _scaleDuration).SetEase(Ease.OutBack);
    }

    private void SetMysteryBoxActive(bool active)
    {
        gameObject.SetActive(active);
    }
}

using Unity.Netcode;
using UnityEngine;

public class PlayerSkillController : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _rocketLauncherTransform;

    [Header("Settings")]
    [SerializeField] private bool _hasSkillAlready;

    private MysteryBoxSkillsSO _mysteryBoxSkill;
    private bool _isSkillUsed;

    private void Update() 
    {
        if(!IsOwner) { return; }
        if(!_hasSkillAlready) { return ; }

        if(Input.GetKeyDown(KeyCode.Space) && !_isSkillUsed)
        {
            ActivateSkill();
            _isSkillUsed = true;
        }
    }

    public void SetupSkill(MysteryBoxSkillsSO skill)
    {
        _mysteryBoxSkill = skill;

        if(_mysteryBoxSkill.SkillType == SkillType.Rocket)
        {
            SetRocketLauncherActiveRpc(true);
        }

        _isSkillUsed = false;
        _hasSkillAlready = true;
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetRocketLauncherActiveRpc(bool active)
    {
        _rocketLauncherTransform.gameObject.SetActive(active);
    }

    public void ActivateSkill()
    {
        if(!_hasSkillAlready) { return; }

        SkillManager.Instance.ActivateSkill(_mysteryBoxSkill.SkillType, transform, OwnerClientId);
        _hasSkillAlready = false;
    }

    public bool HasSkillAlready()
    {
        return _hasSkillAlready;
    }
}

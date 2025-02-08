using Unity.Netcode;
using UnityEngine;

public class PlayerSkillController : NetworkBehaviour
{
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
        _isSkillUsed = false;
        _hasSkillAlready = true;
    }

    public void ActivateSkill()
    {
        if(!_hasSkillAlready) { return; }
        SkillManager.Instance.ActivateSkill(_mysteryBoxSkill.SkillType, transform);
        _hasSkillAlready = false;
    }

    public bool HasSkillAlready()
    {
        return _hasSkillAlready;
    }
}

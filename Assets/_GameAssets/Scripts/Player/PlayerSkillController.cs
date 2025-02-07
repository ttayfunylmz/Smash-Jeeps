using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    public void ActivateSkill(MysteryBoxSkillsSO skill)
    {
        if(skill != null)
        {
            skill.SkillData.ActivateSkill(transform);
        }
        else
        {
            Debug.LogError("Skill not Assigned!");
        }
    }
}

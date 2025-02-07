using UnityEngine;

[CreateAssetMenu(fileName = "Shield Skill", menuName = "Scriptable Objects/Skills/Shield Skill")]
public class ShieldSkill : BaseSkill
{
    public override void ActivateSkill(Transform playerTransform)
    {
        base.ActivateSkill(playerTransform);
        Debug.Log("Shield Skill Activated! " + playerTransform.name);
    }
}

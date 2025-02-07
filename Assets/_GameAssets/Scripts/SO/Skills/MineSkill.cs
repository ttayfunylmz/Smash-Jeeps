using UnityEngine;

[CreateAssetMenu(fileName = "Mine Skill", menuName = "Scriptable Objects/Skills/Mine Skill")]
public class MineSkill : BaseSkill
{
    public override void ActivateSkill(Transform playerTransform)
    {
        base.ActivateSkill(playerTransform);
        Debug.Log("Mine Skill Activated! " + playerTransform.name);
    }
}

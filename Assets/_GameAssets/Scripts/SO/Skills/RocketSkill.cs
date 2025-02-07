using UnityEngine;

[CreateAssetMenu(fileName = "Rocket Skill", menuName = "Scriptable Objects/Skills/Rocket Skill")]
public class RocketSkill : BaseSkill
{
    public override void ActivateSkill(Transform playerTransform)
    {
        base.ActivateSkill(playerTransform);
        Debug.Log("Rocket Skill Activated! " + playerTransform.name);
    }
}

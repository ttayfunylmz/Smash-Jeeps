using UnityEngine;

[CreateAssetMenu(fileName = "Spike Skill", menuName = "Scriptable Objects/Skills/Spike Skill")]
public class SpikeSkill : BaseSkill
{
    public override void ActivateSkill(Transform playerTransform)
    {
        base.ActivateSkill(playerTransform);
        Debug.Log("Spike Skill Activated! " + playerTransform.name);
    }
}

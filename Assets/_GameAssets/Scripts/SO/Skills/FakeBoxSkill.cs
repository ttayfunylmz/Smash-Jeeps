using UnityEngine;

[CreateAssetMenu(fileName = "Fake Box Skill", menuName = "Scriptable Objects/Skills/Fake Box Skill")]
public class FakeBoxSkill : BaseSkill
{
    public override void ActivateSkill(Transform playerTransform)
    {
        base.ActivateSkill(playerTransform);
        Debug.Log("Fake Box Skill Activated! " + playerTransform.name);
    }
}

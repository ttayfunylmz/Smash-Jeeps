using UnityEngine;

[CreateAssetMenu(fileName = "Rocket Skill", menuName = "Scriptable Objects/Skills/Rocket Skill")]
public class RocketSkill : BaseSkill
{
    [SerializeField] private Transform _rocketLauncherPrefab;
    [SerializeField] private Vector3 _rocketLauncherOffset;
}

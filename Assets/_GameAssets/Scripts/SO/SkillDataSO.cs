using UnityEngine;

[CreateAssetMenu(fileName = "Skill Data", menuName = "Scriptable Objects/Skill Data")]
public class SkillDataSO : ScriptableObject
{
    [Header("Activate Settings")]
    [SerializeField] private Transform _skillPrefab;
    [SerializeField] private Vector3 _skillOffset;
    [SerializeField] private bool _shouldBeAttachedToParent;

    public Transform SkillPrefab => _skillPrefab;
    public Vector3 SkillOffset => _skillOffset;
    public bool ShouldBeAttachedToParent => _shouldBeAttachedToParent;
}

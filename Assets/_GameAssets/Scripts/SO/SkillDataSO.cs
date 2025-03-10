using UnityEngine;

[CreateAssetMenu(fileName = "Skill Data", menuName = "Scriptable Objects/Skill Data")]
public class SkillDataSO : ScriptableObject
{
    [Header("Activate Settings")]
    [SerializeField] private Transform _skillPrefab;
    [SerializeField] private Vector3 _skillOffset;
    [SerializeField] private int _spawnAmountOrTimer;
    [SerializeField] private bool _shouldBeAttachedToParent;
    [SerializeField] private int _respawnTimer;
    [SerializeField] private int _damageAmount;

    [Header("Fake Box Hill Offset")]
    [SerializeField] private Vector3 _fakeBoxHillOffset;
    
    public Transform SkillPrefab => _skillPrefab;
    public Vector3 SkillOffset => _skillOffset;
    public int SpawnAmountOrTimer => _spawnAmountOrTimer;
    public bool ShouldBeAttachedToParent => _shouldBeAttachedToParent;
    public int RespawnTimer => _respawnTimer;
    public int DamageAmount => _damageAmount;
    public Vector3 FakeBoxHillOffset => _fakeBoxHillOffset;
}

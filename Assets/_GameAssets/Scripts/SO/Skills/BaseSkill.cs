using UnityEngine;

public abstract class BaseSkill : ScriptableObject
{
    [SerializeField] private Transform _skillPrefab;
    [SerializeField] private Vector3 _spawnOffset;

    public Transform SkillPrefab => _skillPrefab;
    public Vector3 SpawnOffset => _spawnOffset;

}

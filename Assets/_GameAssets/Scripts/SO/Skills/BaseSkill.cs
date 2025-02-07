using UnityEngine;

public abstract class BaseSkill : ScriptableObject
{
    [SerializeField] private GameObject _skillPrefab;

    public GameObject SkillPrefab => _skillPrefab;

    public virtual void ActivateSkill(Transform playerTransform)
    {
        Instantiate(_skillPrefab, playerTransform);
    }
}

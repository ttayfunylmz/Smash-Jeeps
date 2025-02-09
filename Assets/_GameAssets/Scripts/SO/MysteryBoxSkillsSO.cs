using UnityEngine;

[CreateAssetMenu(fileName = "Mystery Box Skills", menuName = "Scriptable Objects/Mystery Box Skills")]
public class MysteryBoxSkillsSO : ScriptableObject
{
    [SerializeField] private SkillType _skillType;
    [SerializeField] private Sprite _skillIcon;
    [SerializeField] private SkillDataSO _skillData;

    public SkillType SkillType => _skillType;
    public Sprite SkillIcon => _skillIcon;
    public SkillDataSO SkillData => _skillData;
}

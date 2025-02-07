using UnityEngine;

[CreateAssetMenu(fileName = "Mystery Box Skills", menuName = "Scriptable Objects/Mystery Box Skills")]
public class MysteryBoxSkillsSO : ScriptableObject
{
    [SerializeField] private string _skillName;
    [SerializeField] private Sprite _skillIcon;
    [SerializeField] private BaseSkill _skillData;

    public string SkillName => _skillName;
    public Sprite SkillIcon => _skillIcon;
    public BaseSkill SkillData => _skillData;
}

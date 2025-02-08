using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SkillManager : NetworkBehaviour
{
    public static SkillManager Instance { get; private set; }

    [SerializeField] private MysteryBoxSkillsSO[] mysteryBoxSkills;

    private Dictionary<SkillType, MysteryBoxSkillsSO> skillsDictionary;

    private void Awake() 
    {
        Instance = this;

        skillsDictionary = new Dictionary<SkillType, MysteryBoxSkillsSO>();
        foreach (MysteryBoxSkillsSO skill in mysteryBoxSkills)
        {
            skillsDictionary[skill.SkillType] = skill;
        }
    }

    public void ActivateSkill(SkillType skillType, Transform playerTransform)
    {
        if (!skillsDictionary.TryGetValue(skillType, out MysteryBoxSkillsSO skillData))
        {
            Debug.LogError($"SkillManager: {skillType} not found!");
            return;
        }

        SkillTransformDataSerializable skillTransformData 
            = new SkillTransformDataSerializable(playerTransform.position, playerTransform.rotation, skillType);

        if (!IsServer)
        {
            RequestSpawnRpc(skillTransformData);
            return;
        }

        SpawnSkill(skillTransformData);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void RequestSpawnRpc(SkillTransformDataSerializable skillTransformDataSerializable)
    {
        SpawnSkill(skillTransformDataSerializable);
    }

    private void SpawnSkill(SkillTransformDataSerializable skillTransformDataSerializable)
    {
        if (!skillsDictionary.TryGetValue(skillTransformDataSerializable.SkillType, out MysteryBoxSkillsSO skillData))
        {
            Debug.LogError($"SpawnSkill: {skillTransformDataSerializable.SkillType} not found!");
            return;
        }

        Transform skillInstance = Instantiate(skillData.SkillData.SkillPrefab);
        skillInstance.SetPositionAndRotation(skillTransformDataSerializable.Position, skillTransformDataSerializable.Rotation);

        if(IsServer)
        {
            skillInstance.GetComponent<NetworkObject>().Spawn(true);
        }
    }
}

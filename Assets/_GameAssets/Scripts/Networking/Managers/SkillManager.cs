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

    public void ActivateSkill(SkillType skillType, Transform playerTransform, ulong spawnerClientId)
    {
        SkillTransformDataSerializable skillTransformData 
            = new SkillTransformDataSerializable(playerTransform.position, playerTransform.rotation, skillType, playerTransform.GetComponent<NetworkObject>());

        if (!IsServer)
        {
            RequestSpawnRpc(skillTransformData, spawnerClientId);
            return;
        }

        SpawnSkill(skillTransformData, spawnerClientId);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void RequestSpawnRpc(SkillTransformDataSerializable skillTransformDataSerializable, ulong spawnerClientId)
    {
        SpawnSkill(skillTransformDataSerializable, spawnerClientId);
    }

    private void SpawnSkill(SkillTransformDataSerializable skillTransformDataSerializable, ulong spawnerClientId)
    {
        if (!skillsDictionary.TryGetValue(skillTransformDataSerializable.SkillType, out MysteryBoxSkillsSO skillData))
        {
            Debug.LogError($"SpawnSkill: {skillTransformDataSerializable.SkillType} not found!");
            return;
        }

        if(IsServer)
        {
            Transform skillInstance = Instantiate(skillData.SkillData.SkillPrefab);
            skillInstance.SetPositionAndRotation(skillTransformDataSerializable.Position, skillTransformDataSerializable.Rotation);
            var networkObject = skillInstance.GetComponent<NetworkObject>();
            networkObject.Spawn(true);

            if(NetworkManager.Singleton.ConnectedClients.TryGetValue(spawnerClientId, out var client))
            {
                networkObject.TrySetParent(client.PlayerObject);

                skillInstance.transform.localPosition += skillData.SkillData.SkillOffset;

                if(!skillData.SkillData.ShouldBeAttachedToParent)
                {
                    networkObject.TryRemoveParent();
                }

                if(skillData.SkillType == SkillType.FakeBox)
                {
                    PositionDataSerializable positionDataSerializable 
                        = new PositionDataSerializable(new Vector3(
                            skillInstance.transform.position.x, 
                            0f + skillData.SkillData.SkillOffset.y,
                            skillInstance.transform.position.z));

                    UpdateFakeBoxPositionRpc(networkObject.NetworkObjectId, positionDataSerializable);
                }
            }
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void UpdateFakeBoxPositionRpc(ulong objectId, PositionDataSerializable positionDataSerializable)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out var networkObject))
        {
            networkObject.transform.position = positionDataSerializable.Position;
        }
    }
}

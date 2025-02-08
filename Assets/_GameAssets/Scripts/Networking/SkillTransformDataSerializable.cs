using Unity.Netcode;
using UnityEngine;

public struct SkillTransformDataSerializable : INetworkSerializable
{
    public Vector3 Position;
    public Quaternion Rotation;
    public SkillType SkillType;

    public SkillTransformDataSerializable(Vector3 position, Quaternion rotation, SkillType skillType)
    {
        Position = position;
        Rotation = rotation;
        SkillType = skillType;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Position);
        serializer.SerializeValue(ref Rotation);
        serializer.SerializeValue(ref SkillType);
    }
}

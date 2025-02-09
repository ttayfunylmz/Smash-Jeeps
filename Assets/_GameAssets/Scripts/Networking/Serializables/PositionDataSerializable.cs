using Unity.Netcode;
using UnityEngine;

public struct PositionDataSerializable : INetworkSerializeByMemcpy
{
    public Vector3 Position;
    
    public PositionDataSerializable(Vector3 position)
    {
        Position = position;
    }
}

using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct LeaderboardEntitiesSerializable : INetworkSerializeByMemcpy, IEquatable<LeaderboardEntitiesSerializable>
{
    public ulong ClientId;
    public FixedString32Bytes PlayerName;
    public int Score;

    public LeaderboardEntitiesSerializable(ulong clientId, FixedString32Bytes playerName, int score)
    {
        ClientId = clientId;
        PlayerName = playerName;
        Score = score;
    }

    public bool Equals(LeaderboardEntitiesSerializable other)
    {
        return ClientId == other.ClientId 
            && PlayerName.Equals(other.PlayerName)
            && Score == other.Score;
    }
}

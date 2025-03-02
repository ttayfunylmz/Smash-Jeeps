using Unity.Netcode;
using UnityEngine;

public class PlayerScoreController : NetworkBehaviour
{
    public NetworkVariable<int> PlayerScore = new NetworkVariable<int>
    (
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    public void AddScore(int amount)
    {
        PlayerScore.Value += amount;
    }
}

using Unity.Netcode;
using UnityEngine;

public class MysteryBoxCollectible : NetworkBehaviour, ICollectible
{
    [SerializeField] private float _respawnTimerTotal = 3f;
    [SerializeField] private float _respawnTimer;

    public override void OnNetworkSpawn()
    {
        _respawnTimer = _respawnTimerTotal;
    }

    public void Collect()
    {
        Debug.Log("Collected!");
        OnCollectRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void OnCollectRpc()
    {
        SetMysteryBoxActive(false);
        Invoke(nameof(SetMysteryBoxActiveTrueRpc), _respawnTimer);
    }

    private void SetMysteryBoxActiveTrueRpc()
    {
        SetMysteryBoxActive(true);
    }

    private void SetMysteryBoxActive(bool active)
    {
        gameObject.SetActive(active);
    }
}

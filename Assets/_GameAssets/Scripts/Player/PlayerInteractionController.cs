using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerInteractionController : NetworkBehaviour
{
    private PlayerVehicleController _playerVehicleController;
    private PlayerSkillController _playerSkillController;
    private PlayerHealthController _playerHealthController;

    private bool _isShieldActive;
    private bool _isCrashed;

    public override void OnNetworkSpawn()
    {
        if(!IsOwner) return;

        _playerVehicleController = GetComponent<PlayerVehicleController>();
        _playerSkillController = GetComponent<PlayerSkillController>();
        _playerHealthController = GetComponent<PlayerHealthController>();

        _playerVehicleController.OnVehicleCrashed += PlayerVehicleController_OnVehicleCrashed;
        SpawnManager.Instance.OnPlayerRespawned += SpawnManager_OnPlayerRespawned;
    }

    private void SpawnManager_OnPlayerRespawned()
    {
        enabled = true;
        _isCrashed = false;
        _playerHealthController.RestartHealth();
    }

    private void PlayerVehicleController_OnVehicleCrashed()
    {
        enabled = false;
        _isCrashed = true;
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(!IsOwner) return;
        if(_isCrashed) return;

        if(other.gameObject.TryGetComponent(out ICollectible collectible))
        {
            collectible.Collect(_playerSkillController);
        }

        if(other.gameObject.TryGetComponent(out IDamageable damageable))
        {
            if(_isShieldActive)
            {
                Debug.Log("Shield Active: Damage Blocked");
                return;
            }

            SetKillerUIClientRpc(damageable.GetKillerClientId());
            damageable.Damage(_playerVehicleController);
            _playerHealthController.TakeDamage(damageable.GetDamageAmount());
            int respawnTimer = damageable.GetRespawnTimer();
            SpawnManager.Instance.RespawnPlayer(respawnTimer, OwnerClientId);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetKillerUIClientRpc(ulong killerClientId)
    {
        if(IsOwner) return;

        if(NetworkManager.Singleton.ConnectedClients.TryGetValue(killerClientId, out var killerClient))
        {
            KillScreenUI.Instance.SetSmashUI("Tayfs");
        }
    }

    public void SetShieldActive(bool isActive) => _isShieldActive = isActive;

}
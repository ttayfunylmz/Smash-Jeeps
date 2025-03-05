using System;
using Unity.Collections;
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
        SpawnerManager.Instance.OnPlayerRespawned += SpawnerManager_OnPlayerRespawned;
    }

    private void SpawnerManager_OnPlayerRespawned()
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
        CheckCollision(other);
    }

    private void OnTriggerStay(Collider other)
    {
        CheckCollision(other);
    }

    private void CheckCollision(Collider other)
    {
        if(!IsOwner) { return; }
        if(_isCrashed) { return; }
        if(GameManager.Instance.GetGameState() != GameState.Playing) { return; }

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

            var playerName = GetComponent<PlayerNetworkController>().PlayerName.Value;

            SetKillerUIClientRpc(damageable.GetKillerClientId(), playerName.ToString());
            damageable.Damage(_playerVehicleController, damageable.GetKillerName());
            _playerHealthController.TakeDamage(damageable.GetDamageAmount());
            int respawnTimer = damageable.GetRespawnTimer();
            SpawnerManager.Instance.RespawnPlayer(respawnTimer, OwnerClientId);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetKillerUIClientRpc(ulong killerClientId, FixedString32Bytes playerName)
    {
        if(IsOwner) return;

        if(NetworkManager.Singleton.ConnectedClients.TryGetValue(killerClientId, out var killerClient))
        {
            KillScreenUI.Instance.SetSmashUI(playerName.ToString());
            killerClient.PlayerObject.GetComponent<PlayerScoreController>().AddScore(1);
        }
    }

    public void SetShieldActive(bool isActive) => _isShieldActive = isActive;

    public override void OnNetworkDespawn()
    {
        if(!IsOwner) { return;}

        _playerVehicleController.OnVehicleCrashed -= PlayerVehicleController_OnVehicleCrashed;
        SpawnerManager.Instance.OnPlayerRespawned -= SpawnerManager_OnPlayerRespawned;
    }
}
using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerInteractionController : NetworkBehaviour
{
    [SerializeField] private CameraShake _cameraShake;

    private PlayerVehicleController _playerVehicleController;
    private PlayerSkillController _playerSkillController;
    private PlayerHealthController _playerHealthController;
    private PlayerDodgeController _playerDodgeController;

    private bool _isShieldActive;
    private bool _isSpikeActive;
    private bool _isCrashed;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        _playerVehicleController = GetComponent<PlayerVehicleController>();
        _playerSkillController = GetComponent<PlayerSkillController>();
        _playerHealthController = GetComponent<PlayerHealthController>();
        _playerDodgeController = GetComponent<PlayerDodgeController>();

        _playerVehicleController.OnVehicleCrashed += PlayerVehicleController_OnVehicleCrashed;
        _playerDodgeController.OnDodgeStarted += PlayerDodgeController_OnDodgeStarted;
        _playerDodgeController.OnDodgeFinished += PlayerDodgeController_OnDodgeFinished;
    }

    private void PlayerDodgeController_OnDodgeFinished()
    {
        enabled = true;
    }

    private void PlayerDodgeController_OnDodgeStarted()
    {
        enabled = false;
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
        if (!IsOwner) { return; }
        if (_isCrashed) { return; }
        if (GameManager.Instance.GetGameState() != GameState.Playing) { return; }

        CheckCollectibleCollision(other);
        CheckDamageableCollision(other);
    }

    private void CheckCollectibleCollision(Collider other)
    {
        if (other.gameObject.TryGetComponent(out ICollectible collectible))
        {
            collectible.Collect(_playerSkillController, _cameraShake);
        }
    }

    private void CheckDamageableCollision(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IDamageable damageable))
        {
            if (_isShieldActive)
            {
                Debug.Log("Shield Active: Damage Blocked");
                return;
            }

            CrashTheVehicle(damageable);
        }
    }

    private void CrashTheVehicle(IDamageable damageable)
    {
        var playerName = GetComponent<PlayerNetworkController>().PlayerName.Value;

        _cameraShake.ShakeCamera(3f, .8f);
        SetKillerUIClientRpc(damageable.GetKillerClientId(), playerName.ToString(), RpcTarget.Single(damageable.GetKillerClientId(), RpcTargetUse.Temp));
        damageable.Damage(_playerVehicleController, damageable.GetKillerName());
        _playerHealthController.TakeDamage(damageable.GetDamageAmount());
        int respawnTimer = damageable.GetRespawnTimer();
        SpawnerManager.Instance.RespawnPlayer(respawnTimer, OwnerClientId);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void SetKillerUIClientRpc(ulong killerClientId, FixedString32Bytes playerName, RpcParams rpcParams)
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(killerClientId, out var killerClient))
        {
            KillScreenUI.Instance.SetSmashUI(playerName.ToString());
            killerClient.PlayerObject.GetComponent<PlayerScoreController>().AddScore(1);
        }
    }

    public void SetShieldActive(bool isActive) => _isShieldActive = isActive;
    public void SetSpikeActive(bool isActive) => _isSpikeActive = isActive;

    public void OnPlayerRespawned()
    {
        enabled = true;
        _isCrashed = false;
        _playerHealthController.RestartHealth();
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) { return; }

        _playerVehicleController.OnVehicleCrashed -= PlayerVehicleController_OnVehicleCrashed;
        _playerDodgeController.OnDodgeStarted -= PlayerDodgeController_OnDodgeStarted;
        _playerDodgeController.OnDodgeFinished -= PlayerDodgeController_OnDodgeFinished;
    }
}
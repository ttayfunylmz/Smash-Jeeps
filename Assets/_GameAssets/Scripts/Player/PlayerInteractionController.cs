using Unity.Netcode;
using UnityEngine;

public class PlayerInteractionController : NetworkBehaviour
{
    private PlayerVehicleController _playerVehicleController;
    private PlayerSkillController _playerSkillController;

    private bool _isShieldActive;

    public override void OnNetworkSpawn()
    {
        if(!IsOwner) return;

        _playerVehicleController = GetComponent<PlayerVehicleController>();
        _playerSkillController = GetComponent<PlayerSkillController>();
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(!IsOwner) return;

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

            damageable.Damage(_playerVehicleController);
            
            int respawnTimer = damageable.GetRespawnTimer();
            SpawnManager.Instance.RespawnPlayer(respawnTimer, OwnerClientId);
        }
    }

    public void SetShieldActive(bool isActive) => _isShieldActive = isActive;
}
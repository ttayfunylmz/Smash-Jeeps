using System;
using DG.Tweening;
using TMPro;
using Unity.Cinemachine;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNetworkController : NetworkBehaviour
{
    public static event Action<PlayerNetworkController> OnPlayerSpawned;
    public static event Action<PlayerNetworkController> OnPlayerDespawned;

    [Header("References")]
    [SerializeField] private CinemachineCamera _playerCinemachineCamera;
    [SerializeField] private PlayerScoreController _playerScoreController;

    [Header("Canvas References")]
    [SerializeField] private Canvas _playerCanvas;
    [SerializeField] private TMP_Text _playerNameText;
    [SerializeField] private GameObject _healthBarBackgroundGameObject;
    [SerializeField] private Image _healthBarImage;

    [Header("Settings")]
    [SerializeField] private float _animationDuration = 0.5f;

    private PlayerVehicleController _playerVehicleController;
    private PlayerSkillController _playerSkillController;
    private PlayerInteractionController _playerInteractionController;
    private PlayerDodgeController _playerDodgeController;

    public NetworkVariable<FixedString32Bytes> PlayerName = new NetworkVariable<FixedString32Bytes>();

    public override void OnNetworkSpawn()
    {
        _playerCinemachineCamera.gameObject.SetActive(IsOwner);

        if(IsServer)
        {
            UserData userData 
                = HostSingleton.Instance.HostManager.NetworkServer.GetUserDataByClientId(OwnerClientId);
            
            PlayerName.Value = userData.UserName;

            OnPlayerSpawned?.Invoke(this);
        }

        if(!IsOwner) return;

        _playerVehicleController = GetComponent<PlayerVehicleController>();
        _playerSkillController = GetComponent<PlayerSkillController>();
        _playerInteractionController = GetComponent<PlayerInteractionController>();
        _playerDodgeController = GetComponent<PlayerDodgeController>();

        _playerVehicleController.OnVehicleCrashed += PlayerVehicleController_OnVehicleCrashed;
    }

    private void PlayerVehicleController_OnVehicleCrashed()
    {
        OnHealthBarChangedRpc(0f);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void OnHealthBarChangedRpc(float endValue)
    {
        _healthBarBackgroundGameObject.SetActive(true);
        _healthBarImage.DOFillAmount(endValue, _animationDuration).OnComplete(() =>
        {
            _healthBarBackgroundGameObject.SetActive(false);
        });
    }

    public PlayerScoreController GetPlayerScoreController() => _playerScoreController;

    public void OnPlayerRespawned()
    {
        _playerVehicleController.OnPlayerRespawned();
        _playerSkillController.OnPlayerRespawned();
        _playerInteractionController.OnPlayerRespawned();
        _playerDodgeController.OnPlayerRespawned();

        OnHealthBarChangedRpc(1f);
    }

    public override void OnNetworkDespawn()
    {
        if(IsServer)
        {
            OnPlayerDespawned?.Invoke(this);
        }

        if(IsOwner)
        {
            _playerVehicleController.OnVehicleCrashed -= PlayerVehicleController_OnVehicleCrashed;
        }
    }
}

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
    [Header("References")]
    [SerializeField] private CinemachineCamera _playerCinemachineCamera;

    [Header("Canvas References")]
    [SerializeField] private Canvas _playerCanvas;
    [SerializeField] private TMP_Text _playerNameText;
    [SerializeField] private GameObject _healthBarBackgroundGameObject;
    [SerializeField] private Image _healthBarImage;

    [Header("Settings")]
    [SerializeField] private float _animationDuration = 0.5f;

    private PlayerVehicleController _playerVehicleController;

    public override void OnNetworkSpawn()
    {
        _playerCinemachineCamera.gameObject.SetActive(IsOwner);

        if(!IsOwner) return;

        _playerVehicleController = GetComponent<PlayerVehicleController>();

        _playerVehicleController.OnVehicleCrashed += PlayerVehicleController_OnVehicleCrashed;
        // SpawnManager.Instance.OnPlayerRespawned += SpawnManager_OnPlayerRespawned;
    }

    private void SpawnManager_OnPlayerRespawned()
    {
        OnHealthBarChangedRpc(1f);
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
}

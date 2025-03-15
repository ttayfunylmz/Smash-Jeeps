using System;
using System.Collections;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

public class PlayerDodgeController : NetworkBehaviour
{
    public event Action OnDodgeStarted;
    public event Action OnDodgeFinished;

    [Header("References")]
    [SerializeField] private Rigidbody _vehicleRigidbody;
    [SerializeField] private BoxCollider _vehicleCollider;
    [SerializeField] private ParticleSystem _dodgeParticles;
    [SerializeField] private CameraShake _cameraShake;

    [Header("Settings")]
    [SerializeField] private float _dodgeTimer;
    [SerializeField] private float _dodgeTimerMax;
    [SerializeField] private float _upwardMovementDistance;
    [SerializeField] private float _animationDuration;
    [SerializeField] private Ease _animationEase;

    private PlayerVehicleController _playerVehicleController;

    private bool _canDodge = true;
    private bool _isDodgeCompleted;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        _playerVehicleController = GetComponent<PlayerVehicleController>();

        _playerVehicleController.OnVehicleCrashed += PlayerVehicleController_OnVehicleCrashed;
    }

    private void PlayerVehicleController_OnVehicleCrashed()
    {
        enabled = false;
    }

    private void Update()
    {
        if (!IsOwner) return;
        if (GameManager.Instance.GetGameState() != GameState.Playing) { return; }

        if (Input.GetKeyDown(KeyCode.R))
        {
            DodgeRpc();
        }

        SetDodgeTimer();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DodgeRpc()
    {
        if (!_canDodge) { return; }
        _canDodge = false;

        _dodgeParticles.Play();

        _vehicleCollider.enabled = false;

        if (NetworkManager.Singleton.LocalClientId == OwnerClientId)
        {
            OnDodgeStarted?.Invoke();
            _vehicleRigidbody.useGravity = false;
            _cameraShake.ShakeCamera(1f, .5f);
            _vehicleRigidbody.isKinematic = true;
        }

        transform.DOMove(new Vector3(transform.position.x, transform.position.y + _upwardMovementDistance, transform.position.z), _animationDuration)
                .SetEase(_animationEase)
                .OnComplete(() =>
                {
                    transform.DORotate(new Vector3(0f, transform.eulerAngles.y, 0f), _animationDuration)
                            .SetEase(_animationEase)
                            .OnComplete(() =>
                            {
                                _vehicleCollider.enabled = true;

                                if (NetworkManager.Singleton.LocalClientId == OwnerClientId)
                                {
                                    OnDodgeFinished?.Invoke();
                                    _vehicleRigidbody.useGravity = true;
                                    DOTween.KillAll();
                                    _vehicleRigidbody.isKinematic = false;
                                    SkillsUI.Instance.SetDodge(_dodgeTimerMax);
                                }

                                _isDodgeCompleted = true;
                            });
                });
    }

    private void SetDodgeTimer()
    {
        if (_isDodgeCompleted)
        {
            _dodgeTimer += Time.deltaTime;

            if (_dodgeTimer >= _dodgeTimerMax)
            {
                _dodgeTimer = 0f;
                SetDodgeBooleansRpc();
                SkillsUI.Instance.SetDodgeToReady();
            }
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetDodgeBooleansRpc()
    {
        _isDodgeCompleted = false;
        _canDodge = true;
    }

    public void OnPlayerRespawned()
    {
        enabled = true;
        _dodgeTimer = 0f;
        SetDodgeBooleansRpc();
        SkillsUI.Instance.SetDodgeToReady();
    }
}

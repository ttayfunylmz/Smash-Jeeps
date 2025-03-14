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

    [Header("Settings")]
    [SerializeField] private float _dodgeTimer;
    [SerializeField] private float _dodgeTimerMax;
    [SerializeField] private float _upwardMovementDistance;
    [SerializeField] private float _animationDuration;
    [SerializeField] private Ease _animationEase;

    private bool _canDodge = true;
    private bool _isDodgeCompleted;

    private void Update()
    {
        if(!IsOwner) return;
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

        OnDodgeStarted?.Invoke();

        _vehicleCollider.enabled = false;
        _vehicleRigidbody.useGravity = false;
        _vehicleRigidbody.isKinematic = true;
        _canDodge = false;

        transform.DOMove(new Vector3(transform.position.x, transform.position.y + _upwardMovementDistance, transform.position.z), _animationDuration)
                .SetEase(_animationEase)
                .OnComplete(() =>
                {
                    transform.DORotate(new Vector3(0f, transform.eulerAngles.y, 0f), _animationDuration)
                            .SetEase(_animationEase)
                            .OnComplete(() =>
                            {
                                DOTween.KillAll();
                                OnDodgeFinished?.Invoke();
                                _vehicleCollider.enabled = true;
                                _vehicleRigidbody.useGravity = true;
                                _vehicleRigidbody.isKinematic = false;
                                _isDodgeCompleted = true;
                            });
                });
    }

    private void SetDodgeTimer()
    {
        if(_isDodgeCompleted)
        {
            _dodgeTimer += Time.deltaTime;

            if(_dodgeTimer >= _dodgeTimerMax)
            {
                _dodgeTimer = 0f;
                _isDodgeCompleted = false;
                _canDodge = true;
            }
        }
    }
}

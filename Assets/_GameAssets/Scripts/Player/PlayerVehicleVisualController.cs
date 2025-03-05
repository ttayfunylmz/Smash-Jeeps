using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;
using System;
using Cysharp.Threading.Tasks;
using System.Collections;

public class PlayerVehicleVisualController : NetworkBehaviour
{
    [SerializeField] private Transform _jeepVisualTransform;
    [SerializeField] private Collider _playerCollider;
    [SerializeField] private Transform _wheelFrontLeft, _wheelFrontRight, _wheelBackLeft, _wheelBackRight;
    [SerializeField] private float _wheelsSpinSpeed, _wheelYWhenSpringMin, _wheelYWhenSpringMax;
    [SerializeField] private TrailRenderer[] _skidMarkTrails;

    private PlayerVehicleController _playerVehicleController;
    private Quaternion _wheelFrontLeftRoll;
    private Quaternion _wheelFrontRightRoll;

    private float _forwardSpeed;
    private float _steerInput;
    private float _steerAngle;
    private float _springsRestLength;

    private Dictionary<WheelType, float> _springsCurrentLength = new()
        {
            { WheelType.FrontLeft, 0.0f },
            { WheelType.FrontRight, 0.0f },
            { WheelType.BackLeft, 0.0f },
            { WheelType.BackRight, 0.0f }
        };

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }

        _playerVehicleController = GetComponent<PlayerVehicleController>();

        _wheelFrontLeftRoll = _wheelFrontLeft.localRotation;
        _wheelFrontRightRoll = _wheelFrontRight.localRotation;

        _springsRestLength = _playerVehicleController.Settings.SpringRestLength;
        _steerAngle = _playerVehicleController.Settings.SteerAngle;

        _playerVehicleController.OnVehicleCrashed += PlayerVehicleController_OnVehicleCrashed;
        SpawnerManager.Instance.OnPlayerRespawned += SpawnerManager_OnPlayerRespawned;
    }

    private void Update()
    {
        if (!IsOwner) return;
        if(GameManager.Instance.GetGameState() != GameState.Playing) { return; }

        UpdateVisualStates();
        RotateWheels();
        SetSuspension();
        SetSkidMarksAndParticles();
    }

    private void SpawnerManager_OnPlayerRespawned()
    {
        foreach(TrailRenderer trail in _skidMarkTrails)
        {
            trail.gameObject.SetActive(true);
        }
    }

    private void PlayerVehicleController_OnVehicleCrashed()
    {
        enabled = false;

        foreach(TrailRenderer trail in _skidMarkTrails)
        {
            trail.gameObject.SetActive(false);
        }
    }

    private void SetSkidMarksAndParticles()
    {
        if(Input.GetAxis("Vertical") != 0f && Input.GetAxis("Horizontal") != 0f)
        {
            foreach (TrailRenderer trail in _skidMarkTrails)
            {
                trail.emitting = true;
            }
        }
        else
        {
            foreach (TrailRenderer trail in _skidMarkTrails)
            {
                trail.emitting = false;
            }
        }
    }

    private void RotateWheels()
    {
        if (_springsCurrentLength[WheelType.FrontLeft] < _springsRestLength)
        {
            _wheelFrontLeftRoll *= Quaternion.AngleAxis(_forwardSpeed * _wheelsSpinSpeed * Time.deltaTime, Vector3.right);
        }

        if (_springsCurrentLength[WheelType.FrontRight] < _springsRestLength)
        {
            _wheelFrontRightRoll *= Quaternion.AngleAxis(_forwardSpeed * _wheelsSpinSpeed * Time.deltaTime, Vector3.right);
        }

        if (_springsCurrentLength[WheelType.BackLeft] < _springsRestLength)
        {
            _wheelBackLeft.localRotation *= Quaternion.AngleAxis(_forwardSpeed * _wheelsSpinSpeed * Time.deltaTime, Vector3.right);
        }

        if (_springsCurrentLength[WheelType.BackRight] < _springsRestLength)
        {
            _wheelBackRight.localRotation *= Quaternion.AngleAxis(_forwardSpeed * _wheelsSpinSpeed * Time.deltaTime, Vector3.right);
        }

        _wheelFrontLeft.localRotation = Quaternion.AngleAxis(_steerInput * _steerAngle, Vector3.up) * _wheelFrontLeftRoll;
        _wheelFrontRight.localRotation = Quaternion.AngleAxis(_steerInput * _steerAngle, Vector3.up) * _wheelFrontRightRoll;
    }

    private void SetSuspension()
    {
        float springFrontLeftRatio = _springsCurrentLength[WheelType.FrontLeft] / _springsRestLength;
        float springFrontRightRatio = _springsCurrentLength[WheelType.FrontRight] / _springsRestLength;
        float springBackLeftRatio = _springsCurrentLength[WheelType.BackLeft] / _springsRestLength;
        float springBackRightRatio = _springsCurrentLength[WheelType.BackRight] / _springsRestLength;

        _wheelFrontLeft.localPosition = new Vector3(_wheelFrontLeft.localPosition.x,
            _wheelYWhenSpringMin + (_wheelYWhenSpringMax - _wheelYWhenSpringMin) * springFrontLeftRatio,
            _wheelFrontLeft.localPosition.z);

        _wheelFrontRight.localPosition = new Vector3(_wheelFrontRight.localPosition.x,
            _wheelYWhenSpringMin + (_wheelYWhenSpringMax - _wheelYWhenSpringMin) * springFrontRightRatio,
            _wheelFrontRight.localPosition.z);

        _wheelBackRight.localPosition = new Vector3(_wheelBackRight.localPosition.x,
            _wheelYWhenSpringMin + (_wheelYWhenSpringMax - _wheelYWhenSpringMin) * springBackRightRatio,
            _wheelBackRight.localPosition.z);

        _wheelBackLeft.localPosition = new Vector3(_wheelBackLeft.localPosition.x,
            _wheelYWhenSpringMin + (_wheelYWhenSpringMax - _wheelYWhenSpringMin) * springBackLeftRatio,
            _wheelBackLeft.localPosition.z);
    }

    private void UpdateVisualStates()
    {
        _steerInput = Input.GetAxis("Horizontal");

        float forwardSpeed = Vector3.Dot(_playerVehicleController.Forward, _playerVehicleController.Velocity);
        _forwardSpeed = forwardSpeed;

        _springsCurrentLength[WheelType.FrontLeft] = _playerVehicleController.GetSpringCurrentLength(WheelType.FrontLeft);
        _springsCurrentLength[WheelType.FrontRight] = _playerVehicleController.GetSpringCurrentLength(WheelType.FrontRight);
        _springsCurrentLength[WheelType.BackLeft] = _playerVehicleController.GetSpringCurrentLength(WheelType.BackLeft);
        _springsCurrentLength[WheelType.BackRight] = _playerVehicleController.GetSpringCurrentLength(WheelType.BackRight);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void SetJeepVisualActiveRpc(bool isActive)
    {
        _jeepVisualTransform.gameObject.SetActive(isActive);
    }

    private IEnumerator SetVehicleVisualActiveCoroutine(float delay)
    {
        SetJeepVisualActiveRpc(false);
        _playerCollider.enabled = false;

        yield return new WaitForSeconds(delay);

        SetJeepVisualActiveRpc(true);
        _playerCollider.enabled = true;
        enabled = true;
    }

    public void SetVehicleVisualActive(float delay)
    {
        StartCoroutine(SetVehicleVisualActiveCoroutine(delay));
    }

    public override void OnNetworkDespawn()
    {
        if(!IsOwner) { return; }

        _playerVehicleController.OnVehicleCrashed -= PlayerVehicleController_OnVehicleCrashed;
        SpawnerManager.Instance.OnPlayerRespawned -= SpawnerManager_OnPlayerRespawned;
    }
}
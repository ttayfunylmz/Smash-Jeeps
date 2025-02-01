using UnityEngine;
using System.Collections.Generic;

public class JeepVisual : MonoBehaviour
{
    [SerializeField] private Transform _wheelFrontLeft, _wheelFrontRight, _wheelBackLeft, _wheelBackRight;
    [SerializeField] private float _wheelsSpinSpeed, _wheelYWhenSpringMin, _wheelYWhenSpringMax;

    private Vehicle _vehicle;
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

    private void Awake()
    {
        _vehicle = GetComponent<Vehicle>();
    }

    private void Start()
    {
        _wheelFrontLeftRoll = _wheelFrontLeft.localRotation;
        _wheelFrontRightRoll = _wheelFrontRight.localRotation;

        _springsRestLength = _vehicle.Settings.SpringRestLength;
        _steerAngle = _vehicle.Settings.SteerAngle;
    }

    private void Update()
    {
        UpdateVisualStates();
        RotateWheels();
        SetSuspension();
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

        float forwardSpeed = Vector3.Dot(_vehicle.Forward, _vehicle.Velocity);
        _forwardSpeed = forwardSpeed;

        _springsCurrentLength[WheelType.FrontLeft] = _vehicle.GetSpringCurrentLength(WheelType.FrontLeft);
        _springsCurrentLength[WheelType.FrontRight] = _vehicle.GetSpringCurrentLength(WheelType.FrontRight);
        _springsCurrentLength[WheelType.BackLeft] = _vehicle.GetSpringCurrentLength(WheelType.BackLeft);
        _springsCurrentLength[WheelType.BackRight] = _vehicle.GetSpringCurrentLength(WheelType.BackRight);
    }
}
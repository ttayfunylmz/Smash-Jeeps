using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using Unity.Netcode;
using UnityEngine;

public class PlayerVehicleController : NetworkBehaviour
{
    private class SpringData
    {
        public float currentLength;
        public float currentVelocity;
    }

    public event Action OnVehicleCrashed;

    private static readonly WheelType[] _wheels = new WheelType[]
    {
            WheelType.FrontLeft, WheelType.FrontRight, WheelType.BackLeft, WheelType.BackRight
    };

    private static readonly WheelType[] _frontWheels = new WheelType[] { WheelType.FrontLeft, WheelType.FrontRight };
    private static readonly WheelType[] _backWheels = new WheelType[] { WheelType.BackLeft, WheelType.BackRight };

    [Header("References")]
    [SerializeField] private VehicleSettingsSO _vehicleSettings;
    [SerializeField] private Rigidbody _vehicleRigidbody;
    [SerializeField] private BoxCollider _vehicleCollider;

    [Header("Settings")]
    [SerializeField] private float _crashForce;
    [SerializeField] private float _crashTorque; 

    private Transform _vehicleTransform;
    private Dictionary<WheelType, SpringData> _springDatas;

    private float _steerInput;
    private float _accelerateInput;

    public VehicleSettingsSO Settings => _vehicleSettings;
    public Vector3 Forward => _vehicleTransform.forward;
    public Vector3 Velocity => _vehicleRigidbody.linearVelocity;

    private void Awake()
    {
        _vehicleTransform = transform;

        _springDatas = new Dictionary<WheelType, SpringData>();
        foreach (WheelType wheel in _wheels)
        {
            _springDatas.Add(wheel, new());
        }
    }

    public override void OnNetworkSpawn()
    {
        _vehicleRigidbody.isKinematic = true;
        SetOwnerRigidbodyKinematicAsync();

        SpawnManager.Instance.OnPlayerRespawned += SpawnManager_OnPlayerRespawned;
    }

    private void Update()
    {
        if(!IsOwner) return;

        SetSteerInput(Input.GetAxis("Horizontal"));
        SetAccelerateInput(Input.GetAxis("Vertical"));

        
    }

    private void FixedUpdate()
    {
        if(!IsOwner) return;

        UpdateSuspension();
        UpdateSteering();
        UpdateAccelerate();
        UpdateBrakes();
        UpdateAirResistance();
    }

    private async void SetOwnerRigidbodyKinematicAsync()
    {
        if(IsOwner)
        {
            await UniTask.DelayFrame(1);
            _vehicleRigidbody.isKinematic = false;
        }
    }

    private void SpawnManager_OnPlayerRespawned()
    {
        enabled = true;
    }

    private void SetSteerInput(float steerInput)
    {
        _steerInput = Mathf.Clamp(steerInput, -1.0f, 1.0f);
    }

    private void SetAccelerateInput(float accelerateInput)
    {
        _accelerateInput = Mathf.Clamp(accelerateInput, -1.0f, 1.0f);
    }

    public float GetSpringCurrentLength(WheelType wheel)
    {
        return _springDatas[wheel].currentLength;
    }

    private void CastSpring(WheelType wheel)
    {
        Vector3 position = GetSpringPosition(wheel);

        float previousLength = _springDatas[wheel].currentLength;

        float currentLength;

        if (Physics.Raycast(position, -_vehicleTransform.up, out var hit, _vehicleSettings.SpringRestLength))
        {
            currentLength = hit.distance;
        }
        else
        {
            currentLength = _vehicleSettings.SpringRestLength;
        }

        _springDatas[wheel].currentVelocity = (currentLength - previousLength) / Time.fixedDeltaTime;
        _springDatas[wheel].currentLength = currentLength;
    }

    private Vector3 GetSpringRelativePosition(WheelType wheel)
    {
        Vector3 boxSize = _vehicleCollider.size;
        float boxBottom = boxSize.y * -0.5f;

        float paddingX = _vehicleSettings.WheelsPaddingX;
        float paddingZ = _vehicleSettings.WheelsPaddingZ;

        return wheel switch
        {
            WheelType.FrontLeft => new Vector3(boxSize.x * (paddingX - 0.5f), boxBottom, boxSize.z * (0.5f - paddingZ)),
            WheelType.FrontRight => new Vector3(boxSize.x * (0.5f - paddingX), boxBottom, boxSize.z * (0.5f - paddingZ)),
            WheelType.BackLeft => new Vector3(boxSize.x * (paddingX - 0.5f), boxBottom, boxSize.z * (paddingZ - 0.5f)),
            WheelType.BackRight => new Vector3(boxSize.x * (0.5f - paddingX), boxBottom, boxSize.z * (paddingZ - 0.5f)),
            _ => default,
        };
    }

    private Vector3 GetSpringPosition(WheelType wheel)
    {
        return _vehicleTransform.localToWorldMatrix.MultiplyPoint3x4(GetSpringRelativePosition(wheel));
    }

    private Vector3 GetSpringHitPosition(WheelType wheel)
    {
        Vector3 vehicleDown = -_vehicleTransform.up;
        return GetSpringPosition(wheel) + _springDatas[wheel].currentLength * vehicleDown;
    }

    private Vector3 GetWheelRollDirection(WheelType wheel)
    {
        bool frontWheel = wheel == WheelType.FrontLeft || wheel == WheelType.FrontRight;

        if (frontWheel)
        {
            var steerQuaternion = Quaternion.AngleAxis(_steerInput * _vehicleSettings.SteerAngle, Vector3.up);
            return steerQuaternion * _vehicleTransform.forward;
        }
        else
        {
            return _vehicleTransform.forward;
        }
    }

    private Vector3 GetWheelSlideDirection(WheelType wheel)
    {
        Vector3 forward = GetWheelRollDirection(wheel);
        return Vector3.Cross(_vehicleTransform.up, forward);
    }

    private Vector3 GetWheelTorqueRelativePosition(WheelType wheel)
    {
        Vector3 boxSize = _vehicleCollider.size;

        float paddingX = _vehicleSettings.WheelsPaddingX;
        float paddingZ = _vehicleSettings.WheelsPaddingZ;

        return wheel switch
        {
            WheelType.FrontLeft => new Vector3(boxSize.x * (paddingX - 0.5f), 0.0f, boxSize.z * (0.5f - paddingZ)),
            WheelType.FrontRight => new Vector3(boxSize.x * (0.5f - paddingX), 0.0f, boxSize.z * (0.5f - paddingZ)),
            WheelType.BackLeft => new Vector3(boxSize.x * (paddingX - 0.5f), 0.0f, boxSize.z * (paddingZ - 0.5f)),
            WheelType.BackRight => new Vector3(boxSize.x * (0.5f - paddingX), 0.0f, boxSize.z * (paddingZ - 0.5f)),
            _ => default,
        };

    }

    private Vector3 GetWheelTorquePosition(WheelType wheel)
    {
        return _vehicleTransform.localToWorldMatrix.MultiplyPoint3x4(GetWheelTorqueRelativePosition(wheel));
    }

    private float GetWheelGripFactor(WheelType wheel)
    {
        bool frontWheel = wheel == WheelType.FrontLeft || wheel == WheelType.FrontRight;
        return frontWheel ? _vehicleSettings.FrontWheelsGripFactor : _vehicleSettings.RearWheelsGripFactor;
    }

    private bool IsGrounded(WheelType wheel)
    {
        return _springDatas[wheel].currentLength < _vehicleSettings.SpringRestLength;
    }

    private void UpdateSuspension()
    {
        foreach (WheelType id in _springDatas.Keys)
        {
            CastSpring(id);
            float currentLength = _springDatas[id].currentLength;
            float currentVelocity = _springDatas[id].currentVelocity;

            float force = SpringMathExtensions.CalculateForceDamped(currentLength, currentVelocity,
                _vehicleSettings.SpringRestLength, _vehicleSettings.SpringStrength,
                _vehicleSettings.SpringDamper);

            _vehicleRigidbody.AddForceAtPosition(force * _vehicleTransform.up, GetSpringPosition(id));
        }
    }

    private void UpdateSteering()
    {
        foreach (WheelType wheel in _wheels)
        {
            if (!IsGrounded(wheel))
            {
                continue;
            }

            Vector3 springPosition = GetSpringPosition(wheel);

            Vector3 slideDirection = GetWheelSlideDirection(wheel);
            float slideVelocity = Vector3.Dot(slideDirection, _vehicleRigidbody.GetPointVelocity(springPosition));

            float desiredVelocityChange = -slideVelocity * GetWheelGripFactor(wheel);
            float desiredAcceleration = desiredVelocityChange / Time.fixedDeltaTime;

            Vector3 force = desiredAcceleration * _vehicleSettings.TireMass * slideDirection;
            _vehicleRigidbody.AddForceAtPosition(force, GetWheelTorquePosition(wheel));
        }
    }

    private void UpdateAccelerate()
    {
        if (Mathf.Approximately(_accelerateInput, 0.0f))
        {
            return;
        }

        float forwardSpeed = Vector3.Dot(_vehicleTransform.forward, _vehicleRigidbody.linearVelocity);
        bool movingForward = forwardSpeed > 0.0f;
        float speed = Mathf.Abs(forwardSpeed);

        if (movingForward && speed > _vehicleSettings.MaxSpeed)
        {
            return;
        }
        else if (!movingForward && speed > _vehicleSettings.MaxReverseSpeed)
        {
            return;
        }

        foreach (WheelType wheel in _wheels)
        {
            if (!IsGrounded(wheel))
            {
                continue;
            }

            Vector3 position = GetWheelTorquePosition(wheel);
            Vector3 wheelForward = GetWheelRollDirection(wheel);
            _vehicleRigidbody.AddForceAtPosition(_accelerateInput * _vehicleSettings.AcceleratePower * wheelForward, position);
        }
    }

    private void UpdateBrakes()
    {
        float forwardSpeed = Vector3.Dot(_vehicleTransform.forward, _vehicleRigidbody.linearVelocity);
        float speed = Mathf.Abs(forwardSpeed);

        float brakesRatio;

        const float ALMOST_STOPPING_SPEED = 2.0f;
        bool almostStopping = speed < ALMOST_STOPPING_SPEED;
        if (almostStopping)
        {
            brakesRatio = 1.0f;
        }
        else
        {
            bool accelerateContrary =
                !Mathf.Approximately(_accelerateInput, 0.0f) &&
                Vector3.Dot(_accelerateInput * _vehicleTransform.forward, _vehicleRigidbody.linearVelocity) < 0.0f;
            if (accelerateContrary)
            {
                brakesRatio = 1.0f;
            }
            // NO ACCELERATE INPUT
            else if (Mathf.Approximately(_accelerateInput, 0.0f))
            {
                brakesRatio = 0.1f;
            }
            else
            {
                return;
            }
        }

        foreach (WheelType wheel in _backWheels)
        {
            if (!IsGrounded(wheel))
            {
                continue;
            }

            Vector3 springPosition = GetSpringPosition(wheel);
            Vector3 rollDirection = GetWheelRollDirection(wheel);
            float rollVelocity = Vector3.Dot(rollDirection, _vehicleRigidbody.GetPointVelocity(springPosition));

            float desiredVelocityChange = -rollVelocity * _vehicleSettings.BrakesPower * brakesRatio;
            float desiredAcceleration = desiredVelocityChange / Time.fixedDeltaTime;

            Vector3 force = desiredAcceleration * _vehicleSettings.TireMass * rollDirection;
            _vehicleRigidbody.AddForceAtPosition(force, GetWheelTorquePosition(wheel));
        }
    }

    private void UpdateAirResistance()
    {
        _vehicleRigidbody.AddForce(_vehicleCollider.size.magnitude * _vehicleSettings.AirResistance * -_vehicleRigidbody.linearVelocity);
    }

    public void CrashVehicle()
    {
        OnVehicleCrashed?.Invoke();

        _vehicleRigidbody.AddForce(Vector3.up * _crashForce, ForceMode.Impulse);
        _vehicleRigidbody.AddTorque(Vector3.forward * _crashTorque, ForceMode.Impulse);
        enabled = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Vector3 vehicleDown = -transform.up;

            foreach (WheelType wheel in _springDatas.Keys)
            {
                // Spring
                Vector3 position = GetSpringPosition(wheel);
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(position, position + vehicleDown * _vehicleSettings.SpringRestLength);
                Gizmos.color = Color.red;
                Gizmos.DrawCube(GetSpringHitPosition(wheel), Vector3.one * 0.08f);

                // Wheel
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(position, GetWheelRollDirection(wheel));
                Gizmos.color = Color.red;
                Gizmos.DrawRay(position, GetWheelSlideDirection(wheel));
            }
        }
        else
        {
            if (_vehicleSettings != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(transform.position,
                    new Vector3(
                        _vehicleSettings.Width,
                        _vehicleSettings.Height,
                        _vehicleSettings.Length));
            }
        }
    }
#endif
}
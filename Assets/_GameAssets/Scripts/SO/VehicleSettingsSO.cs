using UnityEngine;

[CreateAssetMenu(fileName = "Vehicle Settings", menuName = "Scriptable Objects/Vehicle Settings")]
public class VehicleSettingsSO : ScriptableObject
{
    [Header("Shape")]
    [SerializeField] private float _width;
    [SerializeField] private float _height;
    [SerializeField] private float _length;

    [Header("Wheels")]
    [SerializeField][Range(0.0f, 0.5f)] private float _wheelsPaddingX;
    [SerializeField][Range(0.0f, 0.5f)] private float _wheelsPaddingZ;

    [Header("Body")]
    [SerializeField] private float _chassiMass;
    [SerializeField] private float _tireMass;

    [Header("Suspension")]
    [SerializeField] private float _springRestLength;
    [SerializeField] private float _springStrength;
    [SerializeField] private float _springDamper;

    [Header("Power")]
    [SerializeField] private float _acceleratePower;
    [SerializeField] private float _brakesPower;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _maxReverseSpeed;

    [Header("Handling")]
    [SerializeField][Range(0.0f, 60.0f)] private float _steerAngle = 45.0f;
    [SerializeField][Range(0.0f, 1.0f)] private float _frontWheelsGripFactor = 0.5f;
    [SerializeField][Range(0.0f, 1.0f)] private float _rearWheelsGripFactor = 0.5f;

    [Header("Other")]
    [SerializeField] private float _airResistance;

    public float Width => _width;
    public float Height => _height;
    public float Length => _length;

    public float WheelsPaddingX => _wheelsPaddingX;
    public float WheelsPaddingZ => _wheelsPaddingZ;

    public float ChassiMass => _chassiMass;
    public float TireMass => _tireMass;

    public float SpringRestLength => _springRestLength;
    public float SpringStrength => _springStrength;
    public float SpringDamper => _springDamper;

    public float AcceleratePower => _acceleratePower;
    public float BrakesPower => _brakesPower;
    public float MaxSpeed => _maxSpeed;
    public float MaxReverseSpeed => _maxReverseSpeed;

    public float SteerAngle => _steerAngle;
    public float FrontWheelsGripFactor => _frontWheelsGripFactor;
    public float RearWheelsGripFactor => _rearWheelsGripFactor;

    public float AirResistance => _airResistance;
}
using UnityEngine;

namespace ArcadeVehicleController
{
    [CreateAssetMenu]
    public class VehicleSettings : ScriptableObject
    {
        [Header("Shape")]
        [SerializeField] private float width;
        [SerializeField] private float height;
        [SerializeField] private float length;

        [Header("Wheels")]
        [SerializeField][Range(0.0f, 0.5f)] private float wheelsPaddingX;
        [SerializeField][Range(0.0f, 0.5f)] private float wheelsPaddingZ;

        [Header("Body")]
        [SerializeField] private float chassiMass;
        [SerializeField] private float tireMass;

        [Header("Susupension")]
        [SerializeField] private float springRestLength;
        [SerializeField] private float springStrength;
        [SerializeField] private float springDamper;

        [Header("Power")]
        [SerializeField] private float acceleratePower;
        [SerializeField] private float brakesPower;
        [SerializeField] private float maxSpeed;
        [SerializeField] private float maxReverseSpeed;

        [Header("Handling")]
        [SerializeField][Range(0.0f, 60.0f)] private float steerAngle = 45.0f;
        [SerializeField][Range(0.0f, 1.0f)] private float frontWheelsGripFactor = 0.5f;
        [SerializeField][Range(0.0f, 1.0f)] private float rearWheelsGripFactor = 0.5f;

        [Header("Other")]
        [SerializeField] private float airResistance;


        public float Width => width;
        public float Height => height;
        public float Length => length;

        public float WheelsPaddingX => wheelsPaddingX;
        public float WheelsPaddingZ => wheelsPaddingZ;

        public float ChassiMass => chassiMass;
        public float TireMass => tireMass;

        public float SpringRestLength => springRestLength;
        public float SpringStrength => springStrength;
        public float SpringDamper => springDamper;

        public float AcceleratePower => acceleratePower;
        public float BrakesPower => brakesPower;
        public float MaxSpeed => maxSpeed;
        public float MaxReverseSpeed => maxReverseSpeed;

        public float SteerAngle => steerAngle;
        public float FrontWheelsGripFactor => frontWheelsGripFactor;
        public float RearWheelsGripFactor => rearWheelsGripFactor;

        public float AirResistance => airResistance;
    }
}
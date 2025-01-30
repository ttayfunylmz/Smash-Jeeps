using UnityEngine;
using System.Collections.Generic;

namespace ArcadeVehicleController
{
    public class JeepVisual : MonoBehaviour
    {
        [SerializeField] private Transform wheelFrontLeft;
        [SerializeField] private Transform wheelFrontRight;
        [SerializeField] private Transform wheelBackLeft;
        [SerializeField] private Transform wheelBackRight;
        [SerializeField] private float wheelsSpinSpeed;
        [SerializeField] private float wheelYWhenSpringMin;
        [SerializeField] private float wheelYWhenSpringMax;

        private Quaternion wheelFrontLeftRoll;
        private Quaternion wheelFrontRightRoll;

        public bool IsMovingForward { get; set; }

        public float ForwardSpeed { get; set; }

        public float SteerInput { get; set; }

        public float SteerAngle { get; set; }

        public float SpringsRestLength { get; set; }

        public Dictionary<Wheel, float> SpringsCurrentLength { get; set; } = new()
        {
            { Wheel.FrontLeft, 0.0f },
            { Wheel.FrontRight, 0.0f },
            { Wheel.BackLeft, 0.0f },
            { Wheel.BackRight, 0.0f }
        };

        private void Start()
        {
            wheelFrontLeftRoll = wheelFrontLeft.localRotation;
            wheelFrontRightRoll = wheelFrontRight.localRotation;
        }

        private void Update()
        {
            if (SpringsCurrentLength[Wheel.FrontLeft] < SpringsRestLength)
            {
                wheelFrontLeftRoll *= Quaternion.AngleAxis(ForwardSpeed * wheelsSpinSpeed * Time.deltaTime, Vector3.right);
            }

            if (SpringsCurrentLength[Wheel.FrontRight] < SpringsRestLength)
            {
                wheelFrontRightRoll *= Quaternion.AngleAxis(ForwardSpeed * wheelsSpinSpeed * Time.deltaTime, Vector3.right);
            }

            if (SpringsCurrentLength[Wheel.BackLeft] < SpringsRestLength)
            {
                wheelBackLeft.localRotation *= Quaternion.AngleAxis(ForwardSpeed * wheelsSpinSpeed * Time.deltaTime, Vector3.right);
            }

            if (SpringsCurrentLength[Wheel.BackRight] < SpringsRestLength)
            {
                wheelBackRight.localRotation *= Quaternion.AngleAxis(ForwardSpeed * wheelsSpinSpeed * Time.deltaTime, Vector3.right);
            }

            wheelFrontLeft.localRotation = Quaternion.AngleAxis(SteerInput * SteerAngle, Vector3.up) * wheelFrontLeftRoll;
            wheelFrontRight.localRotation = Quaternion.AngleAxis(SteerInput * SteerAngle, Vector3.up) * wheelFrontRightRoll;

            float springFrontLeftRatio = SpringsCurrentLength[Wheel.FrontLeft] / SpringsRestLength;
            float springFrontRightRatio = SpringsCurrentLength[Wheel.FrontRight] / SpringsRestLength;
            float springBackLeftRatio = SpringsCurrentLength[Wheel.BackLeft] / SpringsRestLength;
            float springBackRightRatio = SpringsCurrentLength[Wheel.BackRight] / SpringsRestLength;

            wheelFrontLeft.localPosition = new Vector3(wheelFrontLeft.localPosition.x,
                wheelYWhenSpringMin + (wheelYWhenSpringMax - wheelYWhenSpringMin) * springFrontLeftRatio,
                wheelFrontLeft.localPosition.z);

            wheelFrontRight.localPosition = new Vector3(wheelFrontRight.localPosition.x,
                wheelYWhenSpringMin + (wheelYWhenSpringMax - wheelYWhenSpringMin) * springFrontRightRatio,
                wheelFrontRight.localPosition.z);

            wheelBackRight.localPosition = new Vector3(wheelBackRight.localPosition.x,
                wheelYWhenSpringMin + (wheelYWhenSpringMax - wheelYWhenSpringMin) * springBackRightRatio,
                wheelBackRight.localPosition.z);

            wheelBackLeft.localPosition = new Vector3(wheelBackLeft.localPosition.x,
                wheelYWhenSpringMin + (wheelYWhenSpringMax - wheelYWhenSpringMin) * springBackLeftRatio,
                wheelBackLeft.localPosition.z);
        }
    }
}
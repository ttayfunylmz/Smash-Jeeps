using UnityEngine;

namespace ArcadeVehicleController
{
    public class VisualsStateUpdater : MonoBehaviour
    {
        [SerializeField] private Vehicle vehicle;
        [SerializeField] private JeepVisual jeepVisual;
        [SerializeField] private ThirdPersonCameraController cameraController;

        private void Start()
        {
            jeepVisual.SpringsRestLength = vehicle.Settings.SpringRestLength;
            jeepVisual.SteerAngle = vehicle.Settings.SteerAngle;
            cameraController.FollowTarget = vehicle.transform;
        }

        private void Update()
        {
            jeepVisual.SteerInput = Input.GetAxis("Horizontal");

            float forwardSpeed = Vector3.Dot(vehicle.Forward, vehicle.Velocity);
            jeepVisual.ForwardSpeed = forwardSpeed;
            jeepVisual.IsMovingForward = forwardSpeed > 0.0f;

            jeepVisual.SpringsCurrentLength[Wheel.FrontLeft] = vehicle.GetSpringCurrentLength(Wheel.FrontLeft);
            jeepVisual.SpringsCurrentLength[Wheel.FrontRight] = vehicle.GetSpringCurrentLength(Wheel.FrontRight);
            jeepVisual.SpringsCurrentLength[Wheel.BackLeft] = vehicle.GetSpringCurrentLength(Wheel.BackLeft);
            jeepVisual.SpringsCurrentLength[Wheel.BackRight] = vehicle.GetSpringCurrentLength(Wheel.BackRight);

            cameraController.SpeedRatio = vehicle.Velocity.magnitude / vehicle.Settings.MaxSpeed;
        }
    }
}
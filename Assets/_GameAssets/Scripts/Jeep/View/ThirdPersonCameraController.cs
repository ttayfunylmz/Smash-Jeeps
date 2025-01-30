using UnityEngine;

namespace ArcadeVehicleController
{
    [RequireComponent(typeof(Camera))]
    public class ThirdPersonCameraController : MonoBehaviour
    {
        [SerializeField] private float distance = 10.0f;
        [SerializeField] private float height = 5.0f;
        [SerializeField] private float heightDamping = 2.0f;
        [SerializeField] private float rotationDamping = 3.0f;
        [SerializeField] private float moveSpeed = 1.0f;
        [SerializeField] private float normalFov = 60.0f;
        [SerializeField] private float fastFov = 90.0f;
        [SerializeField] private float fovDamping = 0.25f;

        private Transform cameraTransform;
        private Camera mainCamera;

        public Transform FollowTarget { get; set; }
        public float SpeedRatio { get; set; }

        private void Awake()
        {
            cameraTransform = transform;
            mainCamera = GetComponent<Camera>();
        }

        public void LateUpdate()
        {
            if (FollowTarget == null)
            {
                return;
            }

            float wantedRotationAngle = FollowTarget.eulerAngles.y;
            float wantedHeight = FollowTarget.position.y + height;
            float currentRotationAngle = cameraTransform.eulerAngles.y;
            float currentHeight = cameraTransform.position.y;

            currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

            currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

            Quaternion currentRotation = Quaternion.Euler(0.0f, currentRotationAngle, 0.0f);

            Vector3 desiredPosition = FollowTarget.position;
            desiredPosition -= currentRotation * Vector3.forward * distance;
            desiredPosition.y = currentHeight;

            cameraTransform.position = Vector3.MoveTowards(cameraTransform.position, desiredPosition, Time.deltaTime * moveSpeed);

            cameraTransform.LookAt(FollowTarget);

            const float FAST_SPEED_RATIO = 0.9f;
            float targetFov = SpeedRatio > FAST_SPEED_RATIO ? fastFov : normalFov;
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFov, Time.deltaTime * fovDamping);
        }
    }
}
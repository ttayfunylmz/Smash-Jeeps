using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private CameraMode _cameraMode;

    private void LateUpdate() 
    {
        switch(_cameraMode)
        {
            case CameraMode.LookAt:
                transform.LookAt(Camera.main.transform);
                break;
            case CameraMode.LookAtInverted:
                Vector3 dirFromCamera = transform.position - Camera.main.transform.position;
                transform.LookAt(transform.position + dirFromCamera);
                break;
            case CameraMode.CameraForward:
                transform.forward = Camera.main.transform.forward;
                break;
            case CameraMode.CameraForwardInverted:
                transform.forward = -Camera.main.transform.forward;
                break;
        }
    }
}

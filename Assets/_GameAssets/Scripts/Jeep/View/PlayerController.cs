using UnityEngine;
using UnityEngine.SceneManagement;

namespace ArcadeVehicleController
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Vehicle vehicle;

        private void Start()
        {
            Application.targetFrameRate = 60;
        }

        private void Update()
        {
            if (vehicle == null) return;

            vehicle.SetSteerInput(Input.GetAxis("Horizontal"));
            vehicle.SetAccelerateInput(Input.GetAxis("Vertical"));
        }
    }
}
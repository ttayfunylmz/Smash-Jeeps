using UnityEngine;

namespace ArcadeVehicleController
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(Rigidbody))] // Ensure Rigidbody is also attached
    public class VehicleAudioController : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] private AudioClip engineLoopSound;

        private AudioSource engineAudioSource;
        private Rigidbody vehicleRigidbody;

        [Header("Pitch Settings")]
        [SerializeField] private float minSpeed = 0.0f; // Minimum speed to hear the sound
        [SerializeField] private float maxSpeed = 30.0f; // Adjust as per your game's needs
        [SerializeField] private float minPitch = 0.8f; // Minimum pitch when at minSpeed
        [SerializeField] private float maxPitch = 1.2f; // Maximum pitch when at maxSpeed

        private void Awake()
        {
            engineAudioSource = GetComponent<AudioSource>();

            // Set up the engine audio source
            engineAudioSource.loop = true;
            engineAudioSource.playOnAwake = false;
            engineAudioSource.clip = engineLoopSound; // Assign the loop sound

            // Start playing the engine loop sound
            PlayEngineSound();
        }

        private void Start() 
        {
            vehicleRigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            // Calculate speed-related pitch
            float speed = vehicleRigidbody.linearVelocity.magnitude;
            float pitch = Mathf.Lerp(minPitch, maxPitch, Mathf.InverseLerp(minSpeed, maxSpeed, speed));

            // Update pitch of the engine sound
            engineAudioSource.pitch = pitch;
        }

        private void PlayEngineSound()
        {
            engineAudioSource.Play();
        }

        private void OnDisable()
        {
            // Stop the engine sound when this script is disabled
            StopEngineSound();
        }

        private void StopEngineSound()
        {
            engineAudioSource.Stop();
        }
    }
}

public interface ICollectible
{
    void Collect(PlayerSkillController playerSkillController, CameraShake cameraShake);
    void OnCollectRpc();
}

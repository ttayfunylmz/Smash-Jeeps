public interface IDamageable
{
    void Damage(PlayerVehicleController playerVehicleController);
    int GetRespawnTimer();
    ulong GetKillerClientId();
}

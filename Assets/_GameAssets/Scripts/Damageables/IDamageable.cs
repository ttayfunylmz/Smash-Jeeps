public interface IDamageable
{
    void Damage(PlayerVehicleController playerVehicleController);
    int GetDamageAmount();
    int GetRespawnTimer();
    ulong GetKillerClientId();
}

public interface IDamageable
{
    void Damage(PlayerVehicleController playerVehicleController, string playerName);
    int GetDamageAmount();
    int GetRespawnTimer();
    ulong GetKillerClientId();
    string GetKillerName();
}

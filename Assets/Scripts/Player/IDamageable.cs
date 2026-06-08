/// <summary>
/// Hasar alabilecek tüm objeler bu interface'i kullanır.
/// Düşmanlar, kırılabilir objeler vb. bu interface'i implement eder.
/// </summary>
public interface IDamageable
{
    void TakeDamage(float damage);
}

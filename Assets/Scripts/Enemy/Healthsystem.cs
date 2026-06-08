using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Genel saðlýk sistemi. IDamageable implement eder.
/// Düþman, oyuncu veya kýrýlabilir objelere eklenebilir.
/// </summary>
public class HealthSystem : MonoBehaviour, IDamageable
{
    [Header("Saðlýk")]
    [Tooltip("Maksimum can")]
    [SerializeField] private float maxHealth = 100f;

    [Tooltip("Baþlangýç caný (0 býrakýrsan maxHealth kullanýlýr)")]
    [SerializeField] private float startingHealth = 0f;

    [Header("Ölüm")]
    [Tooltip("Ölünce objeyi yok et (false = deaktif eder)")]
    [SerializeField] private bool destroyOnDeath = false;

    [Tooltip("Ölüm gecikmesi (saniye)")]
    [SerializeField] private float deathDelay = 1.5f;

    [Header("Olaylar")]
    public UnityEvent<float, float> OnHealthChanged; // (currentHP, maxHP)
    public UnityEvent OnDeath;
    public UnityEvent<float> OnDamaged; // (hasar miktarý)

    // Mevcut can
    private float currentHealth;
    private bool isDead = false;

    private void Start()
    {
        currentHealth = startingHealth > 0 ? startingHealth : maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// IDamageable arayüzü — WeaponController buraya çaðýrýyor
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        OnDamaged?.Invoke(damage);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        Debug.Log($"{gameObject.name} hasar aldý: {damage} | Kalan HP: {currentHealth}");

        if (currentHealth <= 0f)
            Die();
    }

    /// <summary>
    /// Can ekler (iksir, heal vb. için)
    /// </summary>
    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log($"{gameObject.name} iyileþti: {amount} | Kalan HP: {currentHealth}");
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        OnDeath?.Invoke();
        Debug.Log($"{gameObject.name} öldü!");

        if (destroyOnDeath)
            Destroy(gameObject, deathDelay);
        else
            Invoke(nameof(Deactivate), deathDelay);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    // Getter'lar
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;
    public float HealthPercent => currentHealth / maxHealth;
}
using UnityEngine;

/// <summary>
/// Can paketi — oyuncu dokunursa can verir ve yok olur.
/// 3D objeye ekle, Collider → Is Trigger yap.
/// </summary>
public class HealthPickup : MonoBehaviour
{
    [Header("İyileştirme")]
    [SerializeField] private float healAmount = 25f;

    [Header("Görsel Efekt")]
    [SerializeField] private float rotateSpeed = 90f;
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float bobHeight = 0.3f;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        // Döndür
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);

        // Yukarı aşağı sallan
        float y = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = startPos + new Vector3(0f, y, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        HealthSystem health = other.GetComponent<HealthSystem>();
        if (health == null) return;

        // Canı dolu ise alma
        if (health.CurrentHealth >= health.MaxHealth) return;

        health.Heal(healAmount);
        Debug.Log($"Can paketi alındı! +{healAmount} HP");

        Destroy(gameObject);
    }
}
using UnityEngine;

/// <summary>
/// Toplanabilir altın. Oyuncu dokunursa alır, GoldManager'a bildirir.
/// 3D objeye ekle, Collider → Is Trigger yap.
/// </summary>
public class GoldPickup : MonoBehaviour
{
    [Header("Altın")]
    [SerializeField] private int goldValue = 1;

    [Header("Ses")]
    [SerializeField] private AudioClip pickupSound;
    [SerializeField] private float soundVolume = 0.5f;

    [Header("Görsel Efekt")]
    [SerializeField] private float rotateSpeed = 120f;
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float bobHeight = 0.25f;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;

        // Altın parlaması
        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend != null && rend.material != null)
        {
            rend.material.EnableKeyword("_EMISSION");
            rend.material.SetColor("_EmissionColor", new Color(1f, 0.8f, 0f) * 1.5f);
        }
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

        // GoldManager'a bildir
        if (GoldManager.Instance != null)
            GoldManager.Instance.AddGold(goldValue);

        // Ses çal
        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, soundVolume);

        Destroy(gameObject);
    }
}
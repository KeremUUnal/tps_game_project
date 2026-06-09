using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Toplanabilir anahtar. Oyuncu dokunursa alır, UI mesajı gösterir.
/// 3D objeye ekle, Collider → Is Trigger yap.
/// </summary>
public class KeyPickup : MonoBehaviour
{
    [Header("Bilgi")]
    [SerializeField] private string keyName = "Gemi Anahtarı";
    [SerializeField] private string pickupMessage = "Gemi Anahtarı alındı!";

    [Header("UI")]
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private float messageDuration = 3f;

    [Header("Envanter")]
    [Tooltip("Envanter slotundaki Image (Slot2 altındaki ikon Image)")]
    [SerializeField] private Image inventorySlotIcon;
    [Tooltip("Anahtar sprite'ı")]
    [SerializeField] private Sprite keySprite;

    [Header("Görsel Efekt")]
    [SerializeField] private float rotateSpeed = 120f;
    [SerializeField] private float bobSpeed = 2f;
    [SerializeField] private float bobHeight = 0.3f;
    [SerializeField] private Color glowColor = new Color(1f, 0.85f, 0.2f, 1f);

    public static bool hasKey = false;

    private Vector3 startPos;
    private Renderer meshRenderer;

    private void Start()
    {
        startPos = transform.position;
        meshRenderer = GetComponentInChildren<Renderer>();
        hasKey = false;

        if (messageText != null)
            messageText.gameObject.SetActive(false);

        // Envanter slotunu başlangıçta boş göster
        if (inventorySlotIcon != null)
        {
            inventorySlotIcon.enabled = false;
        }

        if (meshRenderer != null && meshRenderer.material != null)
        {
            meshRenderer.material.EnableKeyword("_EMISSION");
            meshRenderer.material.SetColor("_EmissionColor", glowColor * 2f);
        }
    }

    private void Update()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);

        float y = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = startPos + new Vector3(0f, y, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        hasKey = true;
        Debug.Log(pickupMessage);

        // UI mesajı göster
        if (messageText != null)
        {
            messageText.text = pickupMessage;
            messageText.gameObject.SetActive(true);
        }

        // Envanter slotuna anahtar ikonu ekle
        if (inventorySlotIcon != null && keySprite != null)
        {
            inventorySlotIcon.sprite = keySprite;
            inventorySlotIcon.enabled = true;

            Image slotBg = inventorySlotIcon.transform.parent.GetComponent<Image>();
            if (slotBg != null)
                slotBg.color = new Color(0f, 0f, 0f, 0f);
        }

        Destroy(gameObject);
    }
}
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Crosshair'in görünürlük, boyut ve pulse animasyonunu yönetir.
/// Canvas altýndaki Crosshair objesine ekle.
/// WeaponController'dan SetEquipped, SetAiming, TriggerShootPulse methodlarý çaðrýlýr.
/// </summary>
public class CrosshairController : MonoBehaviour
{
    [Header("Boyut Ayarlarý")]
    [Tooltip("Silah çekili normal boyut")]
    [SerializeField] private float normalSize = 64f;

    [Tooltip("Aim sýrasýnda boyut (küçülür = odaklanýr)")]
    [SerializeField] private float aimSize = 32f;

    [Tooltip("Ateþ pulse boyutu (büyür sonra küçülür)")]
    [SerializeField] private float shootPulseSize = 90f;

    [Tooltip("Boyut geçiþ hýzý")]
    [SerializeField] private float sizeSmoothing = 15f;

    [Header("Renk / Alpha")]
    [Tooltip("Normal renk")]
    [SerializeField] private Color normalColor = new Color(1f, 1f, 1f, 0.85f);

    [Tooltip("Aim rengi")]
    [SerializeField] private Color aimColor = new Color(1f, 0.9f, 0.2f, 1f);

    [Tooltip("Pulse rengi (ateþ aný)")]
    [SerializeField] private Color pulseColor = new Color(1f, 0.4f, 0.1f, 1f);

    [Tooltip("Renk geçiþ hýzý")]
    [SerializeField] private float colorSmoothing = 15f;

    // Referanslar
    private RectTransform rectTransform;
    private Image image;

    // Durum
    private bool isEquipped = false;
    private bool isAiming = false;

    // Hedef deðerler
    private float targetSize;
    private Color targetColor;

    // Pulse
    private bool isPulsing = false;
    private float pulseTimer = 0f;
    private float pulseDuration = 0.08f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    private void Start()
    {
        targetSize = normalSize;
        targetColor = normalColor;

        // Baþlangýçta gizli
        gameObject.SetActive(false);
    }

    private void Update()
    {
        if (!isEquipped) return;

        HandlePulse();
        UpdateVisuals();
    }

    private void HandlePulse()
    {
        if (!isPulsing) return;

        pulseTimer -= Time.deltaTime;

        if (pulseTimer <= 0f)
        {
            isPulsing = false;
            // Pulse bitti, hedefi normale döndür
            targetSize = isAiming ? aimSize : normalSize;
            targetColor = isAiming ? aimColor : normalColor;
        }
    }

    private void UpdateVisuals()
    {
        // Boyut smooth geçiþ
        float currentSize = rectTransform.sizeDelta.x;
        float newSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * sizeSmoothing);
        rectTransform.sizeDelta = new Vector2(newSize, newSize);

        // Renk smooth geçiþ
        if (image != null)
            image.color = Color.Lerp(image.color, targetColor, Time.deltaTime * colorSmoothing);
    }

    // ==================== DIÞ ÇAÐRILAR (WeaponController'dan) ====================

    /// <summary>
    /// Silah çekme/kaldýrma durumu deðiþtiðinde çaðýr.
    /// </summary>
    public void SetEquipped(bool equipped)
    {
        isEquipped = equipped;
        gameObject.SetActive(equipped);

        if (equipped)
        {
            // Görünür olunca boyutu normal ayarla
            rectTransform.sizeDelta = new Vector2(normalSize, normalSize);
            if (image != null) image.color = normalColor;

            targetSize = normalSize;
            targetColor = normalColor;
        }
    }

    /// <summary>
    /// Aim durumu deðiþtiðinde çaðýr.
    /// </summary>
    public void SetAiming(bool aiming)
    {
        isAiming = aiming;

        if (!isPulsing)
        {
            targetSize = aiming ? aimSize : normalSize;
            targetColor = aiming ? aimColor : normalColor;
        }
    }

    /// <summary>
    /// Ateþ edildiðinde pulse efekti tetikle.
    /// </summary>
    public void TriggerShootPulse()
    {
        isPulsing = true;
        pulseTimer = pulseDuration;
        targetSize = shootPulseSize;
        targetColor = pulseColor;
    }
}
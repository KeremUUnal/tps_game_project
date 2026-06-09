using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

/// <summary>
/// Oyuncu HUD — can barý + hasar vignette efekti.
/// Canvas altýna ekle. Player'ý otomatik bulur.
/// </summary>
public class PlayerHUD : MonoBehaviour
{
    [Header("Can Barý")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Image healthFill;
    [SerializeField] private TMP_Text healthText;

    [Header("Can Barý Renkleri")]
    [SerializeField] private Color fullHealthColor = new Color(0.2f, 0.8f, 0.2f, 1f);
    [SerializeField] private Color midHealthColor = new Color(0.9f, 0.7f, 0.1f, 1f);
    [SerializeField] private Color lowHealthColor = new Color(0.8f, 0.1f, 0.1f, 1f);

    [Header("Hasar Vignette Efekti")]
    [SerializeField] private Volume damageVolume;
    [SerializeField] private float vignetteMaxIntensity = 0.55f;
    [SerializeField] private float vignetteDuration = 0.5f;

    [Header("Ekran Sarsýntýsý")]
    [SerializeField] private bool enableShake = true;
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeIntensity = 5f;

    private HealthSystem playerHealth;
    private Vignette vignette;
    private float vignetteTimer = 0f;
    private float shakeTimer = 0f;
    private RectTransform canvasRect;

    private void Start()
    {
        // Player'ý otomatik bul
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerHealth = playerObj.GetComponent<HealthSystem>();

        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged.AddListener(UpdateHealthBar);
            playerHealth.OnDamaged.AddListener(OnDamaged);
            UpdateHealthBar(playerHealth.CurrentHealth, playerHealth.MaxHealth);
        }

        // Vignette referansýný al
        if (damageVolume != null && damageVolume.profile.TryGet(out Vignette v))
        {
            vignette = v;
            vignette.intensity.value = 0f;
        }

        canvasRect = GetComponentInParent<Canvas>()?.GetComponent<RectTransform>();
    }

    private void Update()
    {
        // Vignette efekti — hýzlý parlayýp yavaþ söner
        if (vignetteTimer > 0f)
        {
            vignetteTimer -= Time.deltaTime;
            float t = vignetteTimer / vignetteDuration;
            float intensity = t * t * vignetteMaxIntensity;

            if (vignette != null)
                vignette.intensity.value = intensity;
        }
        else if (vignette != null && vignette.intensity.value > 0f)
        {
            vignette.intensity.value = 0f;
        }

        // Ekran sarsýntýsý
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;
            if (canvasRect != null)
            {
                float t = shakeTimer / shakeDuration;
                float x = Random.Range(-1f, 1f) * shakeIntensity * t;
                float y = Random.Range(-1f, 1f) * shakeIntensity * t;
                canvasRect.anchoredPosition = new Vector2(x, y);
            }
        }
        else if (canvasRect != null && canvasRect.anchoredPosition != Vector2.zero)
        {
            canvasRect.anchoredPosition = Vector2.zero;
        }
    }

    private void UpdateHealthBar(float current, float max)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = max;
            healthSlider.value = current;
        }

        if (healthFill != null)
        {
            float percent = current / max;
            if (percent > 0.5f)
                healthFill.color = Color.Lerp(midHealthColor, fullHealthColor, (percent - 0.5f) * 2f);
            else
                healthFill.color = Color.Lerp(lowHealthColor, midHealthColor, percent * 2f);
        }

        if (healthText != null)
            healthText.text = Mathf.CeilToInt(current) + " / " + Mathf.CeilToInt(max);
    }

    private void OnDamaged(float damage)
    {
        vignetteTimer = vignetteDuration;

        if (enableShake)
            shakeTimer = shakeDuration;
    }
}
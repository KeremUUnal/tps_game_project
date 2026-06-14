using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Grafik kalitesi seçimine göre post-processing efektlerini açýp kapatýr.
/// Bir tane oluþtur, sahnede her zaman bulunsun (DontDestroyOnLoad ile sahneler arasý taþýnýr).
/// </summary>
public class GraphicsManager : MonoBehaviour
{
    [Header("Volume Referansý")]
    [Tooltip("DamageVolume veya Global Volume objesi")]
    [SerializeField] private Volume globalVolume;

    public static GraphicsManager Instance { get; private set; }

    // Efekt referanslarý
    private Bloom bloom;
    private DepthOfField depthOfField;
    private LensDistortion lensDistortion;
    private ColorAdjustments colorAdjustments;
    private Tonemapping tonemapping;
    private Vignette vignette;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        FindVolumeIfNull();
        CacheEffects();
        ApplyQuality(PlayerPrefs.GetInt("Quality", 2));
    }

    private void FindVolumeIfNull()
    {
        if (globalVolume == null)
        {
            globalVolume = FindFirstObjectByType<Volume>();
        }
    }

    private void CacheEffects()
    {
        if (globalVolume == null || globalVolume.profile == null) return;

        globalVolume.profile.TryGet(out bloom);
        globalVolume.profile.TryGet(out depthOfField);
        globalVolume.profile.TryGet(out lensDistortion);
        globalVolume.profile.TryGet(out colorAdjustments);
        globalVolume.profile.TryGet(out tonemapping);
        globalVolume.profile.TryGet(out vignette);
    }

    /// <summary>
    /// Grafik kalitesini uygular.
    /// 0 = Düþük, 1 = Orta, 2 = Yüksek, 3 = Ultra
    /// </summary>
    public void ApplyQuality(int level)
    {
        // Unity Quality Settings
        QualitySettings.SetQualityLevel(level);

        // Sahne yeni yüklenmiþ olabilir, volume'u tekrar bul
        FindVolumeIfNull();
        CacheEffects();

        switch (level)
        {
            case 0: ApplyLow(); break;
            case 1: ApplyMedium(); break;
            case 2: ApplyHigh(); break;
            case 3: ApplyUltra(); break;
        }

        Debug.Log($"Grafik kalitesi uygulandý: Seviye {level}");
    }

    // ==================== DÜÞÜK ====================
    private void ApplyLow()
    {
        // Post-processing tamamen kapalý (performans)
        if (bloom != null) bloom.active = false;
        if (depthOfField != null) depthOfField.active = false;
        if (lensDistortion != null) lensDistortion.active = false;
        if (colorAdjustments != null) colorAdjustments.active = false;
        if (tonemapping != null) tonemapping.active = false;
        // Vignette hasar için açýk kalýr
        if (vignette != null) vignette.active = true;

        // Gölgeler kapalý
        QualitySettings.shadows = UnityEngine.ShadowQuality.Disable;
    }

    // ==================== ORTA ====================
    private void ApplyMedium()
    {
        // Sadece tonemapping ve color adjustments
        if (bloom != null) bloom.active = true;
        if (bloom != null) bloom.intensity.value = 0.15f;
        if (depthOfField != null) depthOfField.active = false;
        if (lensDistortion != null) lensDistortion.active = false;
        if (colorAdjustments != null) colorAdjustments.active = true;
        if (tonemapping != null) tonemapping.active = true;
        if (vignette != null) vignette.active = true;

        QualitySettings.shadows = UnityEngine.ShadowQuality.HardOnly;
    }

    // ==================== YÜKSEK ====================
    private void ApplyHigh()
    {
        // Çoðu efekt açýk
        if (bloom != null)
        {
            bloom.active = true;
            bloom.intensity.value = 0.3f;
        }
        if (depthOfField != null) depthOfField.active = false; // DOF performans yüksek
        if (lensDistortion != null) lensDistortion.active = true;
        if (colorAdjustments != null) colorAdjustments.active = true;
        if (tonemapping != null) tonemapping.active = true;
        if (vignette != null) vignette.active = true;

        QualitySettings.shadows = UnityEngine.ShadowQuality.All;
    }

    // ==================== ULTRA ====================
    private void ApplyUltra()
    {
        // Her þey açýk
        if (bloom != null)
        {
            bloom.active = true;
            bloom.intensity.value = 0.4f;
        }
        if (depthOfField != null) depthOfField.active = true;
        if (lensDistortion != null) lensDistortion.active = true;
        if (colorAdjustments != null) colorAdjustments.active = true;
        if (tonemapping != null) tonemapping.active = true;
        if (vignette != null) vignette.active = true;

        QualitySettings.shadows = UnityEngine.ShadowQuality.All;
    }
}
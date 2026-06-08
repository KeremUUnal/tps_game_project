using UnityEngine;

/// <summary>
/// Oyuncu portala girince belirtilen spawn noktasýna ýþýnlanýr.
/// Opsiyonel olarak ýþýk rengi ve skybox materyali deðiþtirilebilir.
/// Bu scripti portal objesine ekle. Collider'ý Is Trigger yap.
/// </summary>
public class Portal : MonoBehaviour
{
    [Header("Teleport")]
    [Tooltip("Oyuncunun ýþýnlanacaðý hedef nokta (boþ GameObject)")]
    [SerializeField] private Transform spawnPoint;

    [Header("Atmosfer Deðiþimi (Opsiyonel)")]
    [Tooltip("Iþýnlanýnca Directional Light'ýn rengini deðiþtir")]
    [SerializeField] private bool changeAtmosphere = false;

    [Tooltip("Yeni ýþýk rengi")]
    [SerializeField] private Color newLightColor = new Color(0.2f, 0.5f, 1f);

    [Tooltip("Yeni ýþýk yoðunluðu")]
    [SerializeField] private float newLightIntensity = 1.2f;

    [Tooltip("Yeni skybox materyali (boþ býrakýrsan deðiþmez)")]
    [SerializeField] private Material newSkybox;

    [Tooltip("Yeni fog rengi (opsiyonel)")]
    [SerializeField] private Color newFogColor = new Color(0.1f, 0.1f, 0.3f);

    [Tooltip("Fog aktif olsun mu?")]
    [SerializeField] private bool enableFog = false;

    // Referans
    private Light directionalLight;

    private void Start()
    {
        // Sahnedeki Directional Light'ý bul
        Light[] lights = FindObjectsByType<Light>(FindObjectsSortMode.None);
        foreach (Light l in lights)
        {
            if (l.type == LightType.Directional)
            {
                directionalLight = l;
                break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Sadece Player tag'li obje teleport olsun
        if (!other.CompareTag("Player")) return;

        if (spawnPoint == null)
        {
            Debug.LogWarning("Portal: Spawn noktasý atanmamýþ!");
            return;
        }

        // CharacterController varsa deaktif et (yoksa teleport çalýþmaz)
        CharacterController cc = other.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        // Teleport
        other.transform.position = spawnPoint.position;
        other.transform.rotation = spawnPoint.rotation;

        // CharacterController tekrar aktif
        if (cc != null) cc.enabled = true;

        Debug.Log("Portal: Oyuncu ýþýnlandý!");

        // Atmosfer deðiþimi
        if (changeAtmosphere)
            ChangeAtmosphere();
    }

    private void ChangeAtmosphere()
    {
        // Iþýk deðiþtir
        if (directionalLight != null)
        {
            directionalLight.color = newLightColor;
            directionalLight.intensity = newLightIntensity;
        }

        // Skybox deðiþtir
        if (newSkybox != null)
            RenderSettings.skybox = newSkybox;

        // Fog ayarlarý
        RenderSettings.fog = enableFog;
        if (enableFog)
            RenderSettings.fogColor = newFogColor;

        // Ortam ýþýðýný güncelle
        DynamicGI.UpdateEnvironment();

        Debug.Log("Portal: Atmosfer deðiþtirildi!");
    }
}
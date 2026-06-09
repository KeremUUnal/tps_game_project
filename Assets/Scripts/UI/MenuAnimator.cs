using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Menü arka planını hareketlendirir ve başlığa efekt ekler.
/// Canvas'a ekle.
/// </summary>
public class MenuAnimator : MonoBehaviour
{
    [Header("Arka Plan")]
    [Tooltip("Arka plan RawImage (deniz texture'ı atanacak)")]
    [SerializeField] private RawImage backgroundImage;

    [Tooltip("Arka plan kayma hızı")]
    [SerializeField] private Vector2 scrollSpeed = new Vector2(0.02f, 0.01f);

    [Header("Başlık Animasyonu")]
    [Tooltip("Oyun başlığı text")]
    [SerializeField] private TMP_Text titleText;

    [Tooltip("Başlık sallanma genliği")]
    [SerializeField] private float titleBobAmount = 8f;

    [Tooltip("Başlık sallanma hızı")]
    [SerializeField] private float titleBobSpeed = 1.5f;

    [Header("Altın Parçacık Efekti")]
    [Tooltip("Parçacık sistemi (opsiyonel)")]
    [SerializeField] private ParticleSystem goldParticles;

    [Header("Overlay")]
    [Tooltip("Karartma overlay paneli")]
    [SerializeField] private Image overlayPanel;

    [Tooltip("Overlay rengi (yarı saydam siyah)")]
    [SerializeField] private Color overlayColor = new Color(0f, 0f, 0f, 0.5f);

    private Vector3 titleStartPos;
    private float titleTimer;

    private void Start()
    {
        // Başlık başlangıç pozisyonu
        if (titleText != null)
            titleStartPos = titleText.rectTransform.anchoredPosition;

        // Overlay rengini ayarla
        if (overlayPanel != null)
            overlayPanel.color = overlayColor;

        // Başlık rengini ayarla — altın rengi
        if (titleText != null)
        {
            titleText.color = new Color(0.85f, 0.65f, 0.15f, 1f);
            titleText.fontStyle = FontStyles.Bold;
            titleText.enableVertexGradient = true;
            titleText.colorGradient = new VertexGradient(
                new Color(1f, 0.85f, 0.3f, 1f),     // sol üst — parlak altın
                new Color(1f, 0.85f, 0.3f, 1f),     // sağ üst
                new Color(0.7f, 0.45f, 0.1f, 1f),   // sol alt — koyu altın
                new Color(0.7f, 0.45f, 0.1f, 1f)    // sağ alt
            );
        }
    }

    private void Update()
    {
        AnimateBackground();
        AnimateTitle();
    }

    private void AnimateBackground()
    {
        if (backgroundImage == null) return;

        // Texture'ı yavaşça kaydır — deniz efekti
        Rect uvRect = backgroundImage.uvRect;
        uvRect.x += scrollSpeed.x * Time.deltaTime;
        uvRect.y += scrollSpeed.y * Time.deltaTime;
        backgroundImage.uvRect = uvRect;
    }

    private void AnimateTitle()
    {
        if (titleText == null) return;

        // Başlığı yukarı aşağı sallandır
        titleTimer += Time.deltaTime * titleBobSpeed;
        float yOffset = Mathf.Sin(titleTimer) * titleBobAmount;
        titleText.rectTransform.anchoredPosition = titleStartPos + new Vector3(0f, yOffset, 0f);
    }
}

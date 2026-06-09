using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Menü butonlarýna hover efekti, renk geçiþi ve modern görünüm ekler.
/// Her butona ekle.
/// </summary>
public class MenuButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Renkler")]
    [SerializeField] private Color normalColor = new Color(0.12f, 0.12f, 0.18f, 0.9f);
    [SerializeField] private Color hoverColor = new Color(0.85f, 0.65f, 0.2f, 1f);
    [SerializeField] private Color clickColor = new Color(1f, 0.8f, 0.3f, 1f);

    [Header("Yazý Renkleri")]
    [SerializeField] private Color normalTextColor = new Color(0.9f, 0.85f, 0.7f, 1f);
    [SerializeField] private Color hoverTextColor = new Color(0.1f, 0.08f, 0.05f, 1f);

    [Header("Animasyon")]
    [SerializeField] private float hoverScale = 1.05f;
    [SerializeField] private float animSpeed = 12f;

    private Image image;
    private TMP_Text text;
    private Vector3 originalScale;
    private Vector3 targetScale;
    private Color targetColor;
    private Color targetTextColor;
    private bool isHovering = false;

    private void Awake()
    {
        image = GetComponent<Image>();
        text = GetComponentInChildren<TMP_Text>();
        originalScale = transform.localScale;
        targetScale = originalScale;
        targetColor = normalColor;
        targetTextColor = normalTextColor;

        // Baþlangýç rengi
        if (image != null) image.color = normalColor;
        if (text != null) text.color = normalTextColor;
    }

    private void Update()
    {
        // Yumuþak geçiþ
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * animSpeed);

        if (image != null)
            image.color = Color.Lerp(image.color, targetColor, Time.unscaledDeltaTime * animSpeed);

        if (text != null)
            text.color = Color.Lerp(text.color, targetTextColor, Time.unscaledDeltaTime * animSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        targetScale = originalScale * hoverScale;
        targetColor = hoverColor;
        targetTextColor = hoverTextColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        targetScale = originalScale;
        targetColor = normalColor;
        targetTextColor = normalTextColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        targetColor = clickColor;
    }
}
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class TutorialPanel : MonoBehaviour
{
    [Header("Panel Ayarları")]
    [SerializeField] private float panelWidth = 220f;
    [SerializeField] private float panelPadding = 12f;
    [SerializeField] private Color panelColor = new Color(0f, 0f, 0f, 0.55f);
    [SerializeField] private Color borderColor = new Color(0.85f, 0.65f, 0.13f, 0.8f);

    [Header("Yazı Ayarları")]
    [SerializeField] private float titleFontSize = 16f;
    [SerializeField] private float keyFontSize = 13f;
    [SerializeField] private float descFontSize = 12f;
    [SerializeField] private Color titleColor = new Color(0.95f, 0.78f, 0.2f);
    [SerializeField] private Color keyColor = new Color(1f, 0.95f, 0.7f);
    [SerializeField] private Color descColor = new Color(0.85f, 0.85f, 0.85f);
    [SerializeField] private Color separatorColor = new Color(0.85f, 0.65f, 0.13f, 0.4f);

    [Header("Görünürlük")]
    [SerializeField] private bool startVisible = true;

    private GameObject panelObject;
    private bool isVisible;
    private CanvasGroup canvasGroup;

    private readonly (string key, string description)[] controls = new[]
    {
        ("WASD",      "Hareket"),
        ("Fare",      "Etrafına Bakınma"),
        ("Space",     "Zıplama"),
        ("Shift",     "Koşma"),
        ("Q",         "Silah Çek / Kılıfla"),
        ("Sağ Tık",   "Nişan Al"),
        ("Sol Tık",   "Ateş Et"),
        ("ESC",       "Duraklat"),
    };

    private void Start()
    {
        // Bu objenin RectTransform'unu Canvas'ı tamamen kaplayacak şekilde ayarla
        RectTransform rt = GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        isVisible = startVisible;
        BuildPanel();
    }

    private void Update()
    {
        // New Input System ile H tuşu kontrolü
        if (Keyboard.current != null && Keyboard.current[Key.H].wasPressedThisFrame)
        {
            isVisible = !isVisible;
            if (canvasGroup != null)
                canvasGroup.alpha = isVisible ? 1f : 0f;
        }
    }

    private void BuildPanel()
    {
        // --- Ana Panel ---
        panelObject = new GameObject("TutorialPanelContent");
        panelObject.transform.SetParent(transform, false);

        RectTransform panelRect = panelObject.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(1, 1);
        panelRect.anchorMax = new Vector2(1, 1);
        panelRect.pivot = new Vector2(1, 1);
        panelRect.anchoredPosition = new Vector2(-15f, -15f);
        panelRect.sizeDelta = new Vector2(panelWidth, 0);

        canvasGroup = panelObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = isVisible ? 1f : 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        Image panelBg = panelObject.AddComponent<Image>();
        panelBg.color = panelColor;

        Outline panelOutline = panelObject.AddComponent<Outline>();
        panelOutline.effectColor = borderColor;
        panelOutline.effectDistance = new Vector2(1.5f, 1.5f);

        VerticalLayoutGroup layout = panelObject.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(
            (int)panelPadding, (int)panelPadding,
            (int)panelPadding, (int)panelPadding
        );
        layout.spacing = 4f;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        ContentSizeFitter fitter = panelObject.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // --- Başlık ---
        CreateTitle(" KONTROLLER");
        CreateSeparator();

        // --- Kontrol satırları ---
        foreach (var (key, desc) in controls)
        {
            CreateControlRow(key, desc);
        }

        // --- Alt bilgi ---
        CreateSeparator();
        CreateHintText("[H] Gizle / Göster");
    }

    private void CreateTitle(string text)
    {
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(panelObject.transform, false);

        TextMeshProUGUI tmp = titleObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = titleFontSize;
        tmp.color = titleColor;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableAutoSizing = false;
        tmp.margin = new Vector4(0, 2, 0, 2);

        LayoutElement le = titleObj.AddComponent<LayoutElement>();
        le.preferredHeight = titleFontSize + 10f;
    }

    private void CreateSeparator()
    {
        GameObject sepObj = new GameObject("Separator");
        sepObj.transform.SetParent(panelObject.transform, false);

        Image sepImg = sepObj.AddComponent<Image>();
        sepImg.color = separatorColor;

        LayoutElement le = sepObj.AddComponent<LayoutElement>();
        le.preferredHeight = 1f;
        le.flexibleWidth = 1f;
    }

    private void CreateControlRow(string key, string description)
    {
        GameObject rowObj = new GameObject("Row_" + key);
        rowObj.transform.SetParent(panelObject.transform, false);

        HorizontalLayoutGroup hLayout = rowObj.AddComponent<HorizontalLayoutGroup>();
        hLayout.spacing = 8f;
        hLayout.childAlignment = TextAnchor.MiddleLeft;
        hLayout.childControlWidth = true;
        hLayout.childControlHeight = true;
        hLayout.childForceExpandWidth = false;
        hLayout.childForceExpandHeight = false;

        LayoutElement rowLe = rowObj.AddComponent<LayoutElement>();
        rowLe.preferredHeight = keyFontSize + 10f;

        // Tuş badge
        GameObject keyBg = new GameObject("KeyBadge");
        keyBg.transform.SetParent(rowObj.transform, false);

        Image keyBgImg = keyBg.AddComponent<Image>();
        keyBgImg.color = new Color(1f, 1f, 1f, 0.1f);

        LayoutElement keyLe = keyBg.AddComponent<LayoutElement>();
        keyLe.preferredWidth = 72f;
        keyLe.flexibleWidth = 0f;

        GameObject keyTextObj = new GameObject("KeyText");
        keyTextObj.transform.SetParent(keyBg.transform, false);

        RectTransform keyTextRect = keyTextObj.AddComponent<RectTransform>();
        keyTextRect.anchorMin = Vector2.zero;
        keyTextRect.anchorMax = Vector2.one;
        keyTextRect.offsetMin = new Vector2(4, 0);
        keyTextRect.offsetMax = new Vector2(-4, 0);

        TextMeshProUGUI keyTmp = keyTextObj.AddComponent<TextMeshProUGUI>();
        keyTmp.text = key;
        keyTmp.fontSize = keyFontSize;
        keyTmp.color = keyColor;
        keyTmp.fontStyle = FontStyles.Bold;
        keyTmp.alignment = TextAlignmentOptions.Center;

        // Açıklama
        GameObject descObj = new GameObject("Desc");
        descObj.transform.SetParent(rowObj.transform, false);

        TextMeshProUGUI descTmp = descObj.AddComponent<TextMeshProUGUI>();
        descTmp.text = description;
        descTmp.fontSize = descFontSize;
        descTmp.color = descColor;
        descTmp.alignment = TextAlignmentOptions.Left;

        LayoutElement descLe = descObj.AddComponent<LayoutElement>();
        descLe.flexibleWidth = 1f;
    }

    private void CreateHintText(string text)
    {
        GameObject hintObj = new GameObject("Hint");
        hintObj.transform.SetParent(panelObject.transform, false);

        TextMeshProUGUI tmp = hintObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 10f;
        tmp.color = new Color(0.6f, 0.6f, 0.6f, 0.7f);
        tmp.fontStyle = FontStyles.Italic;
        tmp.alignment = TextAlignmentOptions.Center;

        LayoutElement le = hintObj.AddComponent<LayoutElement>();
        le.preferredHeight = 16f;
    }
}
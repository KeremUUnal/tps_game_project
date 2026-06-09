using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Menü panellerine otomatik modern stil uygular.
/// Canvas'a ekle, Start'ta tüm UI elemanlarýný stillendirir.
/// </summary>
public class MenuStyleApplier : MonoBehaviour
{
    [Header("Tema Renkleri")]
    [SerializeField] private Color panelColor = new Color(0.05f, 0.05f, 0.1f, 0.85f);
    [SerializeField] private Color buttonColor = new Color(0.12f, 0.12f, 0.18f, 0.9f);
    [SerializeField] private Color buttonTextColor = new Color(0.9f, 0.85f, 0.7f, 1f);
    [SerializeField] private Color labelColor = new Color(0.75f, 0.7f, 0.6f, 1f);
    [SerializeField] private Color sliderFillColor = new Color(0.85f, 0.65f, 0.2f, 1f);
    [SerializeField] private Color sliderBgColor = new Color(0.15f, 0.15f, 0.2f, 1f);
    [SerializeField] private Color toggleColor = new Color(0.85f, 0.65f, 0.2f, 1f);
    [SerializeField] private Color dropdownColor = new Color(0.12f, 0.12f, 0.18f, 0.9f);

    [Header("Panel Ayarlarý")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;

    private void Start()
    {
        if (mainMenuPanel != null)
            StylePanel(mainMenuPanel);

        if (settingsPanel != null)
            StylePanel(settingsPanel);
    }

    private void StylePanel(GameObject panel)
    {
        // Panel arka planý
        Image panelImage = panel.GetComponent<Image>();
        if (panelImage != null)
            panelImage.color = panelColor;

        // Tüm butonlarý stillendir
        Button[] buttons = panel.GetComponentsInChildren<Button>(true);
        foreach (Button btn in buttons)
        {
            Image btnImage = btn.GetComponent<Image>();
            if (btnImage != null)
                btnImage.color = buttonColor;

            TMP_Text btnText = btn.GetComponentInChildren<TMP_Text>();
            if (btnText != null)
            {
                btnText.color = buttonTextColor;
                btnText.fontSize = 22;
            }

            // Hover efekti ekle
            if (btn.GetComponent<MenuButtonEffect>() == null)
                btn.gameObject.AddComponent<MenuButtonEffect>();
        }

        // Tüm slider'larý stillendir
        Slider[] sliders = panel.GetComponentsInChildren<Slider>(true);
        foreach (Slider slider in sliders)
        {
            // Fill rengi
            Image fill = slider.fillRect?.GetComponent<Image>();
            if (fill != null)
                fill.color = sliderFillColor;

            // Arka plan rengi
            Transform bg = slider.transform.Find("Background");
            if (bg != null)
            {
                Image bgImage = bg.GetComponent<Image>();
                if (bgImage != null)
                    bgImage.color = sliderBgColor;
            }

            // Handle rengi
            if (slider.handleRect != null)
            {
                Image handle = slider.handleRect.GetComponent<Image>();
                if (handle != null)
                    handle.color = sliderFillColor;
            }
        }

        // Tüm dropdown'larý stillendir
        TMP_Dropdown[] dropdowns = panel.GetComponentsInChildren<TMP_Dropdown>(true);
        foreach (TMP_Dropdown dd in dropdowns)
        {
            Image ddImage = dd.GetComponent<Image>();
            if (ddImage != null)
                ddImage.color = dropdownColor;

            TMP_Text ddText = dd.captionText;
            if (ddText != null)
                ddText.color = buttonTextColor;
        }

        // Tüm toggle'larý stillendir
        Toggle[] toggles = panel.GetComponentsInChildren<Toggle>(true);
        foreach (Toggle toggle in toggles)
        {
            Image checkmark = toggle.graphic as Image;
            if (checkmark != null)
                checkmark.color = toggleColor;
        }

        // Tüm label text'leri stillendir (buton dýþýndakiler)
        TMP_Text[] allTexts = panel.GetComponentsInChildren<TMP_Text>(true);
        foreach (TMP_Text txt in allTexts)
        {
            // Buton text'lerini atla
            if (txt.GetComponentInParent<Button>() != null) continue;
            if (txt.GetComponentInParent<TMP_Dropdown>() != null) continue;

            txt.color = labelColor;
        }
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Kazanma ekranýný animasyonlu ve estetik gösterir.
/// WinPanel objesine ekle.
/// </summary>
public class WinScreenAnimator : MonoBehaviour
{
    [Header("Baþlýk")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private string titleString = "TEBRÝKLER!";

    [Header("Alt Yazý")]
    [SerializeField] private TMP_Text subtitleText;
    [SerializeField] private string subtitleString = "Anahtarý buldun ve gemiye döndün!";

    [Header("Arka Plan")]
    [SerializeField] private Image backgroundPanel;

    [Header("Animasyon Ayarlarý")]
    [SerializeField] private float bgFadeDuration = 1f;
    [SerializeField] private float titleDelay = 0.5f;
    [SerializeField] private float titleScaleDuration = 0.6f;
    [SerializeField] private float subtitleDelay = 1.5f;
    [SerializeField] private float subtitleFadeDuration = 0.8f;
    [SerializeField] private float letterDelay = 0.05f;

    [Header("Renkler")]
    [SerializeField] private Color bgColor = new Color(0f, 0f, 0f, 0.85f);
    [SerializeField] private Color titleColorTop = new Color(1f, 0.85f, 0.3f, 1f);
    [SerializeField] private Color titleColorBottom = new Color(0.7f, 0.45f, 0.1f, 1f);
    [SerializeField] private Color subtitleColor = new Color(0.9f, 0.85f, 0.7f, 1f);

    private void OnEnable()
    {
        StartCoroutine(PlayWinAnimation());
    }

    private IEnumerator PlayWinAnimation()
    {
        // Baþlangýç — her þeyi gizle
        if (backgroundPanel != null)
            backgroundPanel.color = new Color(0f, 0f, 0f, 0f);

        if (titleText != null)
        {
            titleText.text = "";
            titleText.transform.localScale = Vector3.zero;
            titleText.enableVertexGradient = true;
            titleText.colorGradient = new VertexGradient(
                titleColorTop, titleColorTop,
                titleColorBottom, titleColorBottom
            );
        }

        if (subtitleText != null)
        {
            subtitleText.text = "";
            subtitleText.color = new Color(subtitleColor.r, subtitleColor.g, subtitleColor.b, 0f);
        }

        // 1. Arka plan fade in
        float elapsed = 0f;
        while (elapsed < bgFadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / bgFadeDuration;
            if (backgroundPanel != null)
                backgroundPanel.color = Color.Lerp(new Color(0f, 0f, 0f, 0f), bgColor, t);
            yield return null;
        }

        // 2. Baþlýk bekle
        yield return new WaitForSecondsRealtime(titleDelay);

        // 3. Baþlýk yazýsýný ayarla ve büyüterek göster
        if (titleText != null)
        {
            titleText.text = titleString;
            titleText.fontSize = 72;

            elapsed = 0f;
            while (elapsed < titleScaleDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / titleScaleDuration;
                // Bounce efekti
                float scale;
                if (t < 0.6f)
                    scale = Mathf.Lerp(0f, 1.2f, t / 0.6f);
                else
                    scale = Mathf.Lerp(1.2f, 1f, (t - 0.6f) / 0.4f);

                titleText.transform.localScale = Vector3.one * scale;
                yield return null;
            }
            titleText.transform.localScale = Vector3.one;
        }

        // 4. Alt yazý bekle
        yield return new WaitForSecondsRealtime(subtitleDelay - titleDelay - titleScaleDuration);

        // 5. Alt yazý harf harf yaz
        if (subtitleText != null)
        {
            subtitleText.color = subtitleColor;
            subtitleText.fontSize = 28;
            subtitleText.text = "";

            foreach (char c in subtitleString)
            {
                subtitleText.text += c;
                yield return new WaitForSecondsRealtime(letterDelay);
            }
        }

        // 6. Baþlýk sürekli parlama efekti
        if (titleText != null)
        {
            while (true)
            {
                float pulse = Mathf.Sin(Time.unscaledTime * 2f) * 0.1f + 1f;
                titleText.transform.localScale = Vector3.one * pulse;
                yield return null;
            }
        }
    }
}
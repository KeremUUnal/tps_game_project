using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// Ana menü yöneticisi. MainMenu sahnesindeki Canvas'a ekle.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [Header("Paneller")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Ayarlar UI")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Text volumeText;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    [Header("Sahne")]
    [SerializeField] private string gameSceneName = "Playground";

    // Filtrelenmiþ çözünürlük listesi — dropdown ile birebir eþleþir
    private List<Resolution> filteredResolutions = new List<Resolution>();

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        SetupVolumeSlider();
        SetupQualityDropdown();
        SetupResolutionDropdown();
        SetupFullscreenToggle();
    }

    private void SetupVolumeSlider()
    {
        if (volumeSlider == null) return;
        volumeSlider.minValue = 0f;
        volumeSlider.maxValue = 100f;
        volumeSlider.wholeNumbers = true;
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 80f);
        OnVolumeChanged(volumeSlider.value);
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
    }

    private void SetupQualityDropdown()
    {
        if (qualityDropdown == null) return;
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new List<string> { "Düþük", "Orta", "Yüksek", "Ultra" });
        int saved = PlayerPrefs.GetInt("Quality", 2);
        qualityDropdown.value = saved;
        qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
    }

    private void SetupResolutionDropdown()
    {
        if (resolutionDropdown == null) return;

        Resolution[] allResolutions = Screen.resolutions;
        filteredResolutions.Clear();
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        HashSet<string> seen = new HashSet<string>();
        int currentIndex = 0;

        // Kaydedilmiþ çözünürlüðü oku
        int savedWidth = PlayerPrefs.GetInt("ResWidth", Screen.currentResolution.width);
        int savedHeight = PlayerPrefs.GetInt("ResHeight", Screen.currentResolution.height);

        for (int i = 0; i < allResolutions.Length; i++)
        {
            string key = allResolutions[i].width + "x" + allResolutions[i].height;

            // Ayný geniþlik x yükseklik kombinasyonunu sadece bir kere ekle
            if (seen.Contains(key)) continue;
            seen.Add(key);

            string option = allResolutions[i].width + " x " + allResolutions[i].height;
            options.Add(option);
            filteredResolutions.Add(allResolutions[i]);

            // Kaydedilmiþ veya mevcut çözünürlüðü bul
            if (allResolutions[i].width == savedWidth &&
                allResolutions[i].height == savedHeight)
            {
                currentIndex = filteredResolutions.Count - 1;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.SetValueWithoutNotify(currentIndex);
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
    }

    private void SetupFullscreenToggle()
    {
        if (fullscreenToggle == null) return;
        fullscreenToggle.isOn = Screen.fullScreen;
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
    }

    // ==================== BUTONLAR ====================
    public void OnPlayButton() => SceneManager.LoadScene(gameSceneName);

    public void OnSettingsButton()
    {
        mainMenuPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void OnBackButton()
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void OnQuitButton()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // ==================== AYAR DEÐÝÞÝKLÝKLERÝ ====================
    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value / 100f;
        PlayerPrefs.SetFloat("Volume", value);
        PlayerPrefs.Save();
        if (volumeText != null)
            volumeText.text = Mathf.RoundToInt(value) + "%";
    }

    private void OnQualityChanged(int index)
    {
        PlayerPrefs.SetInt("Quality", index);
        PlayerPrefs.Save();

        if (GraphicsManager.Instance != null)
            GraphicsManager.Instance.ApplyQuality(index);
        else
            QualitySettings.SetQualityLevel(index);
    }

    private void OnResolutionChanged(int index)
    {
        if (index >= 0 && index < filteredResolutions.Count)
        {
            Resolution res = filteredResolutions[index];
            Screen.SetResolution(res.width, res.height, Screen.fullScreen);

            // Seçimi kaydet — oyun yeniden açýlýnca hatýrlasýn
            PlayerPrefs.SetInt("ResWidth", res.width);
            PlayerPrefs.SetInt("ResHeight", res.height);
            PlayerPrefs.Save();
        }
    }

    private void OnFullscreenChanged(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }
}
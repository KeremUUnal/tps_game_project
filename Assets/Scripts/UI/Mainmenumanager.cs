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

    private Resolution[] resolutions;

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
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            if (!options.Contains(option)) options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
                currentIndex = options.Count - 1;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentIndex;
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

        // GraphicsManager varsa onu kullan, yoksa basit QualitySettings
        if (GraphicsManager.Instance != null)
            GraphicsManager.Instance.ApplyQuality(index);
        else
            QualitySettings.SetQualityLevel(index);
    }

    private void OnResolutionChanged(int index)
    {
        if (index < resolutions.Length)
        {
            Resolution res = resolutions[index];
            Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        }
    }

    private void OnFullscreenChanged(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
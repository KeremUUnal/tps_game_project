using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// Oyun içi pause menüsü + ayarlar. ESC ile açýlýr.
/// Canvas'a ekle.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    [Header("Paneller")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Ayarlar UI")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TMP_Text volumeText;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullscreenToggle;

    [Header("Sahne")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;
    private Resolution[] resolutions;
    private PlayerInput playerInput;

    private void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        // Player'ýn InputSystem'ýný bul
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerInput = playerObj.GetComponent<PlayerInput>();

        SetupSettings();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (settingsPanel != null && settingsPanel.activeSelf)
                OnBackButton();
            else if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    // ==================== PAUSE ====================

    public void Pause()
    {
        isPaused = true;
        pausePanel.SetActive(true);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Oyuncu kontrollerini durdur
        if (playerInput != null)
            playerInput.enabled = false;
    }

    public void Resume()
    {
        isPaused = false;
        pausePanel.SetActive(false);

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Oyuncu kontrollerini geri aç
        if (playerInput != null)
            playerInput.enabled = true;
    }

    // ==================== BUTONLAR ====================

    public void OnResumeButton()
    {
        Resume();
    }

    public void OnSettingsButton()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void OnBackButton()
    {
        settingsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void OnMainMenuButton()
    {
        Time.timeScale = 1f;
        if (playerInput != null)
            playerInput.enabled = true;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void OnQuitButton()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // ==================== AYARLAR KURULUMU ====================

    private void SetupSettings()
    {
        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 100f;
            volumeSlider.wholeNumbers = true;
            volumeSlider.value = PlayerPrefs.GetFloat("Volume", 80f);
            OnVolumeChanged(volumeSlider.value);
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        if (qualityDropdown != null)
        {
            qualityDropdown.ClearOptions();
            qualityDropdown.AddOptions(new List<string> { "Düþük", "Orta", "Yüksek", "Ultra" });
            qualityDropdown.value = PlayerPrefs.GetInt("Quality", QualitySettings.GetQualityLevel());
            qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
        }

        if (resolutionDropdown != null)
        {
            resolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();
            List<string> options = new List<string>();
            int currentIndex = 0;

            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                if (!options.Contains(option))
                    options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width &&
                    resolutions[i].height == Screen.currentResolution.height)
                    currentIndex = options.Count - 1;
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentIndex;
            resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = Screen.fullScreen;
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
        }
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
        if (resolutions != null && index < resolutions.Length)
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
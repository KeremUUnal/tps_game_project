using UnityEngine;
using UnityEngine.SceneManagement;
using StarterAssets;

/// <summary>
/// Oyuncu ölüm yöneticisi. HealthSystem.OnDeath eventine baðlanýr.
/// PlayerArmature objesine ekle.
/// </summary>
public class PlayerDeath : MonoBehaviour
{
    [Header("Ölüm Ayarlarý")]
    [Tooltip("Ölümden sonra yeniden baþlama süresi")]
    [SerializeField] private float restartDelay = 3f;

    [Tooltip("Ölüm ekraný paneli (opsiyonel)")]
    [SerializeField] private GameObject deathScreenPanel;

    [Header("Referanslar")]
    [SerializeField] private HealthSystem healthSystem;

    private Animator anim;
    private ThirdPersonController tpsController;
    private WeaponController weaponController;
    private CharacterController characterController;
    private bool isDead = false;

    private static readonly int DeadHash = Animator.StringToHash("Dead");

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        tpsController = GetComponent<ThirdPersonController>();
        weaponController = GetComponent<WeaponController>();
        characterController = GetComponent<CharacterController>();

        if (healthSystem == null)
            healthSystem = GetComponent<HealthSystem>();

        if (healthSystem != null)
            healthSystem.OnDeath.AddListener(OnPlayerDeath);

        if (deathScreenPanel != null)
            deathScreenPanel.SetActive(false);
    }

    private void OnPlayerDeath()
    {
        if (isDead) return;
        isDead = true;

        // Ölüm animasyonu tetikle
        if (anim != null)
            anim.SetTrigger(DeadHash);

        // Kontrolleri devre dýþý býrak
        if (tpsController != null)
            tpsController.enabled = false;

        if (weaponController != null)
            weaponController.enabled = false;

        if (characterController != null)
            characterController.enabled = false;

        // Mouse'u göster
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Ölüm ekranýný göster
        if (deathScreenPanel != null)
            deathScreenPanel.SetActive(true);

        // Sahneyi yeniden baþlat
        Invoke(nameof(RestartScene), restartDelay);

        Debug.Log("Oyuncu öldü!");
    }

    private void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
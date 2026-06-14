using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;

/// <summary>
/// Oyun bitiş noktası. Anahtarı alan VE tüm düşmanları yenen oyuncu gemiye dönünce oyun biter.
/// Geminin yanına boş obje koy, Collider → Is Trigger yap.
/// </summary>
public class GameFinish : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject winPanel;
    [SerializeField] private TMP_Text messageText;

    [Header("Uyarı Mesajları")]
    [SerializeField] private string noKeyMessage = "Önce anahtarı bulmalısın!";
    [SerializeField] private string enemiesLeftMessage = "Önce tüm düşmanları yenmelisin! Kalan: ";
    [SerializeField] private TMP_Text hintText;

    [Header("Ayarlar")]
    [SerializeField] private float returnToMenuDelay = 5f;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    [Header("Düşman Takibi")]
    [Tooltip("Düşman tag'i (varsayılan: Enemy)")]
    [SerializeField] private string enemyTag = "Enemy";

    private bool gameFinished = false;

    private void Start()
    {
        if (winPanel != null)
            winPanel.SetActive(false);

        if (hintText != null)
            hintText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (gameFinished) return;

        // 1. Anahtar var mı?
        if (!KeyPickup.hasKey)
        {
            ShowHint(noKeyMessage);
            Debug.Log("Anahtar yok!");
            return;
        }

        // 2. Düşmanlar bitti mi?
        int remainingEnemies = CountAliveEnemies();
        if (remainingEnemies > 0)
        {
            ShowHint(enemiesLeftMessage + remainingEnemies);
            Debug.Log($"Kalan düşman: {remainingEnemies}");
            return;
        }

        // Oyunu bitir
        gameFinished = true;
        WinGame();
    }

    private int CountAliveEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        int alive = 0;
        foreach (GameObject enemy in enemies)
        {
            // Aktif olanlar = hayatta
            if (enemy.activeInHierarchy)
                alive++;
        }
        return alive;
    }

    private void ShowHint(string message)
    {
        if (hintText == null) return;
        hintText.text = message;
        hintText.gameObject.SetActive(true);
        CancelInvoke(nameof(HideHint));
        Invoke(nameof(HideHint), 3f);
    }

    private void HideHint()
    {
        if (hintText != null)
            hintText.gameObject.SetActive(false);
    }

    private void WinGame()
    {
        Debug.Log("Tebrikler! Oyunu kazandın!");

        var playerInput = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerInput>();
        if (playerInput != null)
            playerInput.enabled = false;

        if (winPanel != null)
            winPanel.SetActive(true);

        if (messageText != null)
            messageText.text = "TEBRİKLER!\nAnahtarı buldun, düşmanları yendin ve gemiye döndün!";

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        StartCoroutine(ReturnToMenuAfterDelay());
    }

    private IEnumerator ReturnToMenuAfterDelay()
    {
        yield return new WaitForSecondsRealtime(returnToMenuDelay);
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
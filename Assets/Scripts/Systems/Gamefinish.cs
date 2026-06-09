using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;

/// <summary>
/// Oyun bitiş noktası. Anahtarı alan oyuncu gemiye dönünce oyun biter.
/// Geminin yanına boş obje koy, Collider → Is Trigger yap.
/// </summary>
public class GameFinish : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Kazanma ekranı paneli")]
    [SerializeField] private GameObject winPanel;

    [Tooltip("Mesaj text'i (opsiyonel)")]
    [SerializeField] private TMP_Text messageText;

    [Header("Anahtarsız Mesaj")]
    [Tooltip("Anahtar yokken gösterilecek mesaj")]
    [SerializeField] private string noKeyMessage = "Önce anahtarı bulmalısın!";

    [Tooltip("Anahtar yokken mesaj text'i")]
    [SerializeField] private TMP_Text hintText;

    [Header("Ayarlar")]
    [SerializeField] private float returnToMenuDelay = 5f;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

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

        // Anahtar var mı kontrol et
        if (!KeyPickup.hasKey)
        {
            // İpucu göster
            if (hintText != null)
            {
                hintText.text = noKeyMessage;
                hintText.gameObject.SetActive(true);
                CancelInvoke(nameof(HideHint));
                Invoke(nameof(HideHint), 3f);
            }

            Debug.Log("Anahtar yok! Önce anahtarı bul.");
            return;
        }

        // Oyunu bitir
        gameFinished = true;
        WinGame();
    }

    private void WinGame()
    {
        Debug.Log("Tebrikler! Oyunu kazandın!");

        // Oyuncuyu durdur
        var playerInput = GameObject.FindGameObjectWithTag("Player")?.GetComponent<UnityEngine.InputSystem.PlayerInput>();
        if (playerInput != null)
            playerInput.enabled = false;

        // Kazanma ekranı göster
        if (winPanel != null)
            winPanel.SetActive(true);

        if (messageText != null)
            messageText.text = "TEBRİKLER!\nAnahtarı buldun ve gemiye döndün!";

        // Mouse göster
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Ana menüye dön (realtime bekle, timeScale 0 olsa bile çalışır)
        StartCoroutine(ReturnToMenuAfterDelay());
    }

    private IEnumerator ReturnToMenuAfterDelay()
    {
        yield return new WaitForSecondsRealtime(returnToMenuDelay);
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void HideHint()
    {
        if (hintText != null)
            hintText.gameObject.SetActive(false);
    }
}
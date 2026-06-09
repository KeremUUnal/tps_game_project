using UnityEngine;
using TMPro;

/// <summary>
/// Altýn sayacýný yönetir. Canvas'a ekle.
/// Player tag'li objeyi otomatik bulur.
/// </summary>
public class GoldManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text goldText;

    public static GoldManager Instance { get; private set; }
    public int GoldCount { get; private set; } = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    public void AddGold(int amount)
    {
        GoldCount += amount;
        UpdateUI();
        Debug.Log($"Altýn toplandý! Toplam: {GoldCount}");
    }

    private void UpdateUI()
    {
        if (goldText != null)
            goldText.text = GoldCount.ToString();
    }
}
using UnityEngine;

/// <summary>
/// Aktif olunca belirli süre sonra kendini gizler.
/// UI Text objesine ekle.
/// </summary>
public class AutoHideText : MonoBehaviour
{
    [SerializeField] private float hideDelay = 3f;

    private void OnEnable()
    {
        CancelInvoke();
        Invoke(nameof(Hide), hideDelay);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
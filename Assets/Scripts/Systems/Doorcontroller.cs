using UnityEngine;

/// <summary>
/// Oyuncu yaklaþýnca kapý otomatik açýlýr, uzaklaþýnca kapanýr.
/// Kapý objesine ekle. Bir trigger collider ekle (Is Trigger açýk).
/// </summary>
public class DoorController : MonoBehaviour
{
    public enum OpenDirection { Up, Left, Right }

    [Header("Açýlma Ayarlarý")]
    [Tooltip("Kapý açýlma yönü")]
    [SerializeField] private OpenDirection openDirection = OpenDirection.Left;

    [Tooltip("Kayma mesafesi")]
    [SerializeField] private float slideDistance = 3f;

    [Tooltip("Açýlma/kapanma hýzý")]
    [SerializeField] private float speed = 5f;

    private Vector3 closedPosition;
    private Vector3 openPosition;
    private bool playerInRange = false;

    private void Start()
    {
        closedPosition = transform.localPosition;

        switch (openDirection)
        {
            case OpenDirection.Up:
                openPosition = closedPosition + Vector3.up * slideDistance;
                break;
            case OpenDirection.Left:
                openPosition = closedPosition - transform.right * slideDistance;
                break;
            case OpenDirection.Right:
                openPosition = closedPosition + transform.right * slideDistance;
                break;
        }
    }

    private void Update()
    {
        Vector3 target = playerInRange ? openPosition : closedPosition;
        transform.localPosition = Vector3.Lerp(transform.localPosition, target, Time.deltaTime * speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}
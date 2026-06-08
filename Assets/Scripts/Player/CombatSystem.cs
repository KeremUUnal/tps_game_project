using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

/// <summary>
/// Silah çekme, ateş etme, nişan alma ve aim kamerası yöneten ana script.
/// Bu scripti PlayerArmature objesine ekle.
/// 
/// Inspector'da ayarlanması gerekenler:
/// - Gun Mesh In Belt: Beldeki/göğüsteki tabanca mesh objesi
/// - Gun Mesh In Hand: Elde tutulacak tabanca objesi (WeaponHolder altında)
/// - Fire Point: Silah namlu ucu (WeaponHolder altında boş obje)
/// - Normal Camera: Starter Assets'in PlayerFollowCamera'sındaki Cinemachine Camera
/// - Aim Camera: Yeni oluşturulacak aim Cinemachine Camera
/// - Player Animator: PlayerArmature'daki Animator
/// </summary>
public class CombatSystem : MonoBehaviour
{
    [Header("=== SİLAH OBJELERİ ===")]
    [Tooltip("Göğüste/belde duran tabanca mesh'i (silah çekilince gizlenecek)")]
    [SerializeField] private GameObject gunMeshInBelt;

    [Tooltip("Elde tutulacak tabanca objesi (silah çekilince görünecek)")]
    [SerializeField] private GameObject gunMeshInHand;

    [Tooltip("Ateş noktası - silahın namlu ucu")]
    [SerializeField] private Transform firePoint;

    [Header("=== KAMERA ===")]
    [Tooltip("Normal Cinemachine kamera (PlayerFollowCamera)")]
    [SerializeField] private CinemachineCamera normalCamera;

    [Tooltip("Aim Cinemachine kamera (yakın kamera)")]
    [SerializeField] private CinemachineCamera aimCamera;

    [Header("=== ATEŞİ AYARLARI ===")]
    [Tooltip("Her merminin verdiği hasar")]
    [SerializeField] private float damage = 25f;

    [Tooltip("Menzil (metre)")]
    [SerializeField] private float range = 50f;

    [Tooltip("Ateşler arası bekleme süresi (saniye)")]
    [SerializeField] private float fireRate = 0.4f;

    [Header("=== AIM AYARLARI ===")]
    [Tooltip("Aim sırasında hareket hızı çarpanı (0.5 = yarı hız)")]
    [SerializeField] private float aimSpeedMultiplier = 0.4f;

    [Tooltip("Normal kamera önceliği")]
    [SerializeField] private int normalCamPriority = 10;

    [Tooltip("Aim kamera önceliği (normalden yüksek olmalı)")]
    [SerializeField] private int aimCamPriority = 20;

    [Header("=== NİŞANGAH ===")]
    [Tooltip("Nişangah UI objesi (Canvas altında)")]
    [SerializeField] private GameObject crosshairUI;

    [Header("=== MERMİ İZİ ===")]
    [Tooltip("Mermi izi süresi")]
    [SerializeField] private float trailDuration = 0.06f;

    // Referanslar
    private Animator animator;
    private PlayerInputActions inputActions;
    private Camera mainCamera;

    // Animator parametre hash'leri (performans için)
    private static readonly int GunEquippedHash = Animator.StringToHash("GunEquipped");
    private static readonly int IsAimingHash = Animator.StringToHash("IsAiming");
    private static readonly int ShootHash = Animator.StringToHash("Shoot");

    // Durum değişkenleri
    private bool gunEquipped = false;
    private bool isAiming = false;
    private float nextFireTime = 0f;

    // Starter Assets referansı (hız kontrolü için)
    private StarterAssets.ThirdPersonController tpsController;
    private float originalMoveSpeed;
    private float originalSprintSpeed;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
        tpsController = GetComponent<StarterAssets.ThirdPersonController>();

        // Orijinal hızları kaydet
        if (tpsController != null)
        {
            originalMoveSpeed = tpsController.MoveSpeed;
            originalSprintSpeed = tpsController.SprintSpeed;
        }
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();

        // Silah çekme/kaldırma
        inputActions.Player.SwitchWeapon.performed += OnSwitchWeapon;

        // Aim (sağ tık)
        inputActions.Player.Aim.started += OnAimStarted;
        inputActions.Player.Aim.canceled += OnAimCanceled;

        // Ateş (sol tık)
        inputActions.Player.Fire.performed += OnFire;
    }

    private void OnDisable()
    {
        inputActions.Player.SwitchWeapon.performed -= OnSwitchWeapon;
        inputActions.Player.Aim.started -= OnAimStarted;
        inputActions.Player.Aim.canceled -= OnAimCanceled;
        inputActions.Player.Fire.performed -= OnFire;
        inputActions.Player.Disable();
    }

    private void Start()
    {
        // Başlangıçta silah kaldırılmış durumda
        SetGunEquipped(false);
        SetAiming(false);

        // Nişangahı başta gizle
        if (crosshairUI != null)
            crosshairUI.SetActive(false);
    }

    // ==================== SİLAH ÇEKME/KALDIRMA ====================

    /// <summary>
    /// Q tuşuna basıldığında silah çek/kaldır.
    /// </summary>
    private void OnSwitchWeapon(InputAction.CallbackContext context)
    {
        if (gunEquipped)
        {
            // Silahı kaldır
            SetGunEquipped(false);
            SetAiming(false);
        }
        else
        {
            // Silahı çek
            SetGunEquipped(true);
        }
    }

    /// <summary>
    /// Silah durumunu ayarlar, mesh'leri ve animasyonu günceller.
    /// </summary>
    private void SetGunEquipped(bool equipped)
    {
        gunEquipped = equipped;

        // Animator parametresini güncelle
        if (animator != null)
            animator.SetBool(GunEquippedHash, equipped);

        // Göğüsteki tabancayı gizle/göster
        if (gunMeshInBelt != null)
            gunMeshInBelt.SetActive(!equipped);

        // Eldeki tabancayı göster/gizle
        if (gunMeshInHand != null)
            gunMeshInHand.SetActive(equipped);

        // Nişangahı göster/gizle
        if (crosshairUI != null)
            crosshairUI.SetActive(equipped);

        // Silah kaldırılınca aim'i de kapat
        if (!equipped)
        {
            SetAiming(false);
        }
    }

    // ==================== NİŞAN ALMA (AIM) ====================

    /// <summary>
    /// Sağ tık basıldı - nişan almaya başla.
    /// </summary>
    private void OnAimStarted(InputAction.CallbackContext context)
    {
        // Sadece silah çekiliyken aim yapılabilir
        if (!gunEquipped) return;
        SetAiming(true);
    }

    /// <summary>
    /// Sağ tık bırakıldı - nişan bırak.
    /// </summary>
    private void OnAimCanceled(InputAction.CallbackContext context)
    {
        SetAiming(false);
    }

    /// <summary>
    /// Aim durumunu ayarlar, kamerayı ve hızı günceller.
    /// </summary>
    private void SetAiming(bool aiming)
    {
        isAiming = aiming;

        // Animator parametresini güncelle
        if (animator != null)
            animator.SetBool(IsAimingHash, aiming);

        // Kamera geçişi (aim kamerası daha yakın)
        if (normalCamera != null)
            normalCamera.Priority = aiming ? 0 : normalCamPriority;

        if (aimCamera != null)
            aimCamera.Priority = aiming ? aimCamPriority : 0;

        // Aim sırasında hareketi yavaşlat
        if (tpsController != null)
        {
            if (aiming)
            {
                tpsController.MoveSpeed = originalMoveSpeed * aimSpeedMultiplier;
                tpsController.SprintSpeed = originalSprintSpeed * aimSpeedMultiplier;
            }
            else
            {
                tpsController.MoveSpeed = originalMoveSpeed;
                tpsController.SprintSpeed = originalSprintSpeed;
            }
        }
    }

    // ==================== ATEŞ ETME ====================

    /// <summary>
    /// Sol tık - ateş et.
    /// </summary>
    private void OnFire(InputAction.CallbackContext context)
    {
        // Sadece silah çekiliyken ateş edilebilir
        if (!gunEquipped) return;

        // Cooldown kontrolü
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + fireRate;

        // Ateş animasyonunu tetikle
        if (animator != null)
            animator.SetTrigger(ShootHash);

        // Raycast ile ateş
        ShootRaycast();
    }

    /// <summary>
    /// Ekranın ortasından raycast gönderir ve isabet kontrolü yapar.
    /// </summary>
    private void ShootRaycast()
    {
        // Ekranın ortasından ray gönder
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        Vector3 targetPoint;

        if (Physics.Raycast(ray, out hit, range))
        {
            targetPoint = hit.point;

            // Hasar ver (IDamageable interface'i varsa)
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }

            Debug.Log("Vurulan: " + hit.collider.name + " | Mesafe: " + hit.distance.ToString("F1") + "m");
        }
        else
        {
            targetPoint = ray.GetPoint(range);
            Debug.Log("Iskaladı");
        }

        // Mermi izi çiz
        if (firePoint != null)
        {
            DrawBulletTrail(firePoint.position, targetPoint);
        }
    }

    /// <summary>
    /// Mermi izi efekti çizer.
    /// </summary>
    private void DrawBulletTrail(Vector3 start, Vector3 end)
    {
        GameObject trailObj = new GameObject("BulletTrail");
        LineRenderer lr = trailObj.AddComponent<LineRenderer>();

        lr.startWidth = 0.015f;
        lr.endWidth = 0.015f;
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.yellow;
        lr.endColor = new Color(1f, 0.8f, 0f, 0.5f);

        Destroy(trailObj, trailDuration);
    }

    // ==================== PUBLIC ERİŞİM ====================

    public bool IsGunEquipped() => gunEquipped;
    public bool IsAiming() => isAiming;
}

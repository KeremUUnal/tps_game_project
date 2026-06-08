using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;
using Unity.Cinemachine;
using System.Collections;

/// <summary>
/// Starter Assets + Yeni Input System ile uyumlu silah sistemi.
/// Bu scripti PlayerArmature objesine ekle.
/// </summary>
public class WeaponController : MonoBehaviour
{
    [Header("Kamera")]
    [SerializeField] private CinemachineCamera followCamera;

    [Header("Silah")]
    [SerializeField] private GameObject weaponObject;
    [SerializeField] private Transform firePoint;

    [Header("Mermi Trail")]
    [Tooltip("TrailRenderer'lý BulletTrail prefabý")]
    [SerializeField] private GameObject bulletTrailPrefab;

    [Tooltip("Mermi görsel hýzý (yüksek = daha hýzlý iz)")]
    [SerializeField] private float bulletSpeed = 80f;

    [Header("Aim FOV Ayarlarý")]
    [SerializeField] private float aimFov = 25f;
    [SerializeField] private float smoothSpeed = 10f;

    [Header("Aim Kamera Pozisyonu")]
    [SerializeField] private float aimCameraSide = 1f;
    [SerializeField] private float aimCameraDistance = 1.5f;
    [SerializeField] private Vector3 aimShoulderOffset = new Vector3(0.5f, 0f, 0f);

    [Header("Aim Hareket")]
    [SerializeField] private float aimSpeedMultiplier = 0.4f;
    [SerializeField] private float aimRotationSpeed = 20f;

    [Header("Ateþ Ayarlarý")]
    [SerializeField] private float damage = 25f;
    [SerializeField] private float range = 50f;
    [SerializeField] private float fireRate = 0.4f;

    [Header("UI")]
    [SerializeField] private CrosshairController crosshairController;

    // Durum
    private bool gunEquipped = false;
    private bool isAiming = false;
    private float nextFireTime = 0f;

    // Kamera deðerleri
    private float normalFov;
    private float normalCameraSide;
    private float normalCameraDistance;
    private Vector3 normalShoulderOffset;

    private float currentFov;
    private float currentCameraSide;
    private float currentCameraDistance;
    private Vector3 currentShoulderOffset;

    private float targetFov;
    private float targetCameraSide;
    private float targetCameraDistance;
    private Vector3 targetShoulderOffset;

    // Referanslar
    private Animator anim;
    private ThirdPersonController tpsController;
    private Camera mainCamera;
    private float originalMoveSpeed;
    private float originalSprintSpeed;
    private CinemachineThirdPersonFollow thirdPersonFollow;
    private PlayerInputActions inputActions;

    // Animator hash
    private static readonly int GunEquipHash = Animator.StringToHash("GunEquipped");
    private static readonly int AimingHash = Animator.StringToHash("Aiming");
    private static readonly int ShootHash = Animator.StringToHash("Shoot");

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.Player.SwitchWeapon.performed += OnSwitchWeapon;
        inputActions.Player.Aim.started += OnAimStarted;
        inputActions.Player.Aim.canceled += OnAimCanceled;
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
        anim = GetComponentInChildren<Animator>();
        tpsController = GetComponent<ThirdPersonController>();
        mainCamera = Camera.main;

        if (followCamera != null)
        {
            normalFov = followCamera.Lens.FieldOfView;
            thirdPersonFollow = followCamera.GetComponent<CinemachineThirdPersonFollow>();
            if (thirdPersonFollow != null)
            {
                normalCameraSide = thirdPersonFollow.CameraSide;
                normalCameraDistance = thirdPersonFollow.CameraDistance;
                normalShoulderOffset = thirdPersonFollow.ShoulderOffset;
            }
        }
        else
        {
            normalFov = 40f;
            Debug.LogWarning("Follow Camera atanmamýþ!");
        }

        currentFov = normalFov;
        currentCameraSide = normalCameraSide;
        currentCameraDistance = normalCameraDistance;
        currentShoulderOffset = normalShoulderOffset;

        targetFov = normalFov;
        targetCameraSide = normalCameraSide;
        targetCameraDistance = normalCameraDistance;
        targetShoulderOffset = normalShoulderOffset;

        if (tpsController != null)
        {
            originalMoveSpeed = tpsController.MoveSpeed;
            originalSprintSpeed = tpsController.SprintSpeed;
        }

        gunEquipped = false;
        isAiming = false;

        if (weaponObject != null)
            weaponObject.SetActive(false);
    }

    private void Update()
    {
        UpdateCameraSmooth();
    }

    private void LateUpdate()
    {
        if (isAiming)
        {
            float cameraYaw = mainCamera.transform.eulerAngles.y;
            Quaternion targetRotation = Quaternion.Euler(0f, cameraYaw, 0f);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                Time.deltaTime * aimRotationSpeed
            );
        }
    }

    private void UpdateCameraSmooth()
    {
        if (followCamera == null) return;

        float t = Time.deltaTime * smoothSpeed;

        currentFov = Mathf.Lerp(currentFov, targetFov, t);
        followCamera.Lens.FieldOfView = currentFov;

        if (thirdPersonFollow != null)
        {
            currentCameraSide = Mathf.Lerp(currentCameraSide, targetCameraSide, t);
            thirdPersonFollow.CameraSide = currentCameraSide;

            currentCameraDistance = Mathf.Lerp(currentCameraDistance, targetCameraDistance, t);
            thirdPersonFollow.CameraDistance = currentCameraDistance;

            currentShoulderOffset = Vector3.Lerp(currentShoulderOffset, targetShoulderOffset, t);
            thirdPersonFollow.ShoulderOffset = currentShoulderOffset;
        }
    }

    // ==================== SÝLAH ÇEKME ====================

    private void OnSwitchWeapon(InputAction.CallbackContext context)
    {
        gunEquipped = !gunEquipped;

        if (anim != null)
            anim.SetBool(GunEquipHash, gunEquipped);

        if (weaponObject != null)
            weaponObject.SetActive(gunEquipped);

        if (!gunEquipped && isAiming)
            SetAiming(false);

        if (crosshairController != null)
            crosshairController.SetEquipped(gunEquipped);

        Debug.Log(gunEquipped ? "Silah çekildi" : "Silah kaldýrýldý");
    }

    // ==================== AIM ====================

    private void OnAimStarted(InputAction.CallbackContext context)
    {
        if (!gunEquipped) return;
        SetAiming(true);
    }

    private void OnAimCanceled(InputAction.CallbackContext context)
    {
        SetAiming(false);
    }

    private void SetAiming(bool aiming)
    {
        isAiming = aiming;

        if (anim != null)
            anim.SetBool(AimingHash, aiming);

        if (crosshairController != null)
            crosshairController.SetAiming(aiming);

        if (aiming)
        {
            targetFov = aimFov;
            targetCameraSide = aimCameraSide;
            targetCameraDistance = aimCameraDistance;
            targetShoulderOffset = aimShoulderOffset;
        }
        else
        {
            targetFov = normalFov;
            targetCameraSide = normalCameraSide;
            targetCameraDistance = normalCameraDistance;
            targetShoulderOffset = normalShoulderOffset;
        }

        if (tpsController != null)
        {
            tpsController.MoveSpeed = aiming ? originalMoveSpeed * aimSpeedMultiplier : originalMoveSpeed;
            tpsController.SprintSpeed = aiming ? originalSprintSpeed * aimSpeedMultiplier : originalSprintSpeed;
        }
    }

    // ==================== ATEÞ ====================

    private void OnFire(InputAction.CallbackContext context)
    {
        if (!gunEquipped) return;
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + fireRate;

        if (anim != null)
            anim.SetTrigger(ShootHash);

        if (crosshairController != null)
            crosshairController.TriggerShootPulse();

        ShootRaycast();
    }

    private void ShootRaycast()
    {
        // Raycast kamera merkezinden — crosshair'in baktýðý yer
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            targetPoint = hit.point;

            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
                damageable.TakeDamage(damage);

            Debug.Log("Vurulan: " + hit.collider.name);
        }
        else
        {
            targetPoint = ray.GetPoint(range);
        }

        // Trail izi: FirePoint'ten hedef noktaya
        if (firePoint != null && bulletTrailPrefab != null)
            StartCoroutine(SpawnBulletTrail(firePoint.position, targetPoint));
    }

    private IEnumerator SpawnBulletTrail(Vector3 start, Vector3 end)
    {
        // Prefabý oluþtur
        GameObject trail = Instantiate(bulletTrailPrefab, start, Quaternion.identity);
        TrailRenderer tr = trail.GetComponent<TrailRenderer>();

        float distance = Vector3.Distance(start, end);
        float duration = distance / bulletSpeed;
        float elapsed = 0f;

        // FirePoint'ten hedefe doðru hareket ettir
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            trail.transform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        trail.transform.position = end;

        // Trail'in solmasý için biraz bekle sonra yok et
        if (tr != null)
        {
            float trailTime = tr.time;
            yield return new WaitForSeconds(trailTime);
        }

        Destroy(trail);
    }

    public bool IsGunEquipped() => gunEquipped;
    public bool IsAiming() => isAiming;
}
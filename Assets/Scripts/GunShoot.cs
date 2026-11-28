
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class GunShoot : MonoBehaviour
{
    public enum FireMode
    {
        TapToShoot,
        Automatic
    }

    [Header("Firing Mode")]
    public FireMode fireMode = FireMode.TapToShoot;

    [Header("Camera")]
    public Camera arCamera;

    [Header("Shooting")]
    public float range = 100f;
    public float fireRate = 1f;
    public int damage = 1;
    public LayerMask hitLayers = ~0;

    [Header("Gun References")]
    public Transform muzzlePoint;
    public AudioSource shootingSound;

    [Header("Effects")]
    public GameObject muzzleFlashPrefab;
    public GameObject hitEffect;
    public LineRenderer bulletLine;

    [Header("Bullet Line")]
    public float bulletLineDuration = 0.08f;
    public float bulletLineWidth = 0.025f;

    [Header("Gun Recoil")]
    public float recoilAmount = 0.1f;
    public float recoilSpeed = 10f;

    private float nextFireTime;
    private Vector3 originalPosition;
    private Vector3 targetPosition;

    private Coroutine tracerCoroutine;
    private Coroutine muzzleCoroutine;

    void Start()
    {
        originalPosition = transform.localPosition;
        targetPosition = originalPosition;

        if (bulletLine != null)
        {
            bulletLine.enabled = false;
            bulletLine.useWorldSpace = true;
            bulletLine.startWidth = bulletLineWidth;
            bulletLine.endWidth = bulletLineWidth;
        }
    }

    void Update()

    {
        
        HandleRecoil();

        if (arCamera == null)
            return;

        // Automatic mode: continuously fire
        if (fireMode == FireMode.Automatic)
        {
            Fire();
        }
        // Tap mode: fire once per tap
        else if (fireMode == FireMode.TapToShoot)
        {
            bool pressed = false;

            if (Touchscreen.current != null)
            {
                pressed = Touchscreen.current.primaryTouch
                    .press.wasPressedThisFrame;
            }
            else if (Mouse.current != null)
            {
                pressed = Mouse.current.leftButton
                    .wasPressedThisFrame;
            }

            if (pressed)
                Fire();
        }
    }

    public void FireButton()
    {
        Fire();
    }

    void Fire()
    {
        if (Time.time < nextFireTime || arCamera == null)
            return;

        nextFireTime = Time.time + fireRate;

        // Recoil
        targetPosition = originalPosition -
                         new Vector3(0, 0, recoilAmount);

        // Shooting sound
        if (shootingSound != null)
            shootingSound.Play();

        // Muzzle flash
       if (muzzleFlashPrefab != null && muzzlePoint != null)
        {
            GameObject flash = Instantiate(
                muzzleFlashPrefab,
                muzzlePoint.position,
                muzzlePoint.rotation,
                muzzlePoint
            );

            flash.transform.localPosition = Vector3.zero;
            flash.transform.localRotation = Quaternion.identity;

            // Explicitly play every particle system in the prefab
            ParticleSystem[] particles =
                flash.GetComponentsInChildren<ParticleSystem>(true);

            foreach (ParticleSystem ps in particles)
            {
                ps.gameObject.SetActive(true);
                ps.Play(true);
            }

            Destroy(flash, 3f);
        }
        else
        {
            Debug.LogError("Muzzle flash prefab or muzzle point is missing!");
        }

        // Shoot from the centre of the AR camera
        Ray ray = arCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        Vector3 endPoint = ray.origin + ray.direction * range;

        // Detect objects hit by the ray
        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            range,
            hitLayers,
            QueryTriggerInteraction.Ignore))
        {
            endPoint = hit.point;

            // Find enemy health component
            AlienHealth alien =
                hit.collider.GetComponentInParent<AlienHealth>();

            if (alien != null)
            {
                alien.TakeDamage(damage);
            }

            // Spawn impact effect
            if (hitEffect != null)
            {
                GameObject effect = Instantiate(
                    hitEffect,
                    hit.point,
                    Quaternion.LookRotation(hit.normal)
                );

                Destroy(effect, 3f);
            }
        }

        // Display bullet tracer
        if (bulletLine != null)
        {
            Vector3 startPoint = muzzlePoint != null
                ? muzzlePoint.position
                : transform.position;

            if (tracerCoroutine != null)
                StopCoroutine(tracerCoroutine);

            bulletLine.SetPosition(0, startPoint);
            bulletLine.SetPosition(1, endPoint);
            bulletLine.enabled = true;

            tracerCoroutine = StartCoroutine(HideBulletLine());
        }
    }

    void HandleRecoil()
    {
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPosition,
            Time.deltaTime * recoilSpeed
        );

        if (Vector3.Distance(
            transform.localPosition,
            targetPosition) < 0.01f)
        {
            targetPosition = originalPosition;
        }
    }


    IEnumerator HideBulletLine()
    {
        yield return new WaitForSeconds(bulletLineDuration);

        if (bulletLine != null)
            bulletLine.enabled = false;
    }
}

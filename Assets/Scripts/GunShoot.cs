using UnityEngine;

public class GunShoot : MonoBehaviour
{
    [Header("Camera")]
    public Camera arCamera;

    [Header("Shooting")]
    public float range = 100f;
    public float fireRate = 0.25f;
    public int damage = 1;

    [Header("Effects")]
    public GameObject muzzleFlash;
    public GameObject hitEffect;
    public LineRenderer bulletLine;
    public AudioSource shootingSound;

    [Header("Gun Recoil")]
    public float recoilAmount = 0.1f;
    public float recoilSpeed = 10f;

    private float nextFireTime;
    private Vector3 originalPosition;
    private Vector3 targetPosition;

    void Start()
    {
        originalPosition = transform.localPosition;
        targetPosition = originalPosition;

        if (muzzleFlash != null)
            muzzleFlash.SetActive(false);

        if (bulletLine != null)
            bulletLine.enabled = false;
    }

    void Update()
    {
        HandleRecoil();
    }

    public void FireButton()
    {
        if (Time.time < nextFireTime)
            return;

        nextFireTime = Time.time + fireRate;

        targetPosition = originalPosition - new Vector3(0, 0, recoilAmount);

        if (shootingSound != null)
            shootingSound.Play();

        if (muzzleFlash != null)
        {
            muzzleFlash.SetActive(true);
            Invoke(nameof(HideMuzzleFlash), 0.05f);
        }

        Ray ray = arCamera.ScreenPointToRay(
            new Vector3(Screen.width / 2f, Screen.height / 2f, 0)
        );

        Vector3 endPoint = ray.origin + ray.direction * range;

        if (Physics.Raycast(ray, out RaycastHit hit, range))
        {
            endPoint = hit.point;

            AlienHealth alien = hit.collider.GetComponentInParent<AlienHealth>();

            if (alien != null)
            {
                if (hitEffect != null)
                    Instantiate(hitEffect, hit.point, Quaternion.identity);

                alien.TakeDamage(damage);
            }
        }

        if (bulletLine != null)
        {
            bulletLine.enabled = true;
            bulletLine.SetPosition(0, transform.position);
            bulletLine.SetPosition(1, endPoint);

            Invoke(nameof(HideBulletLine), 0.05f);
        }
    }

    void HandleRecoil()
    {
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPosition,
            Time.deltaTime * recoilSpeed
        );

        if (Vector3.Distance(transform.localPosition, targetPosition) < 0.01f)
        {
            targetPosition = originalPosition;
        }
    }

    void HideMuzzleFlash()
    {
        if (muzzleFlash != null)
            muzzleFlash.SetActive(false);
    }

    void HideBulletLine()
    {
        if (bulletLine != null)
            bulletLine.enabled = false;
    }
}
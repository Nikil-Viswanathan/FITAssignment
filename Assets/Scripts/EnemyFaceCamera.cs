
using UnityEngine;

public class EnemyFaceCamera : MonoBehaviour
{
    [SerializeField] private Vector3 rotationOffset;

    private Transform playerCamera;

    void Start()
    {
        if (Camera.main != null)
            playerCamera = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (playerCamera == null)
        {
            if (Camera.main != null)
                playerCamera = Camera.main.transform;

            return;
        }

        Vector3 direction = playerCamera.position - transform.position;

        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = targetRotation * Quaternion.Euler(rotationOffset);
        }
    }
}

using UnityEngine;

public class CameraTrackingDebug : MonoBehaviour
{
    private float timer;

    void Start()
    {
        Debug.Log("CAMERA DEBUG SCRIPT STARTED!");
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 1f)
        {
            timer = 0f;

            if (Camera.main != null)
            {
                Vector3 pos = Camera.main.transform.position;

                Debug.Log(
                    "CAMERA WORLD POSITION: " + pos
                );
            }
            else
            {
                Debug.LogError("MAIN CAMERA NOT FOUND!");
            }
        }
    }
}

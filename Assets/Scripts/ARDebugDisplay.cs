using UnityEngine;
using TMPro;

public class ARDebugDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text debugText;
    [SerializeField] private Camera arCamera;

    private void Update()
    {
        if (debugText == null || arCamera == null)
            return;

        Vector3 cameraPos = arCamera.transform.position;

        string output = "CAMERA POSITION\n";
        output += cameraPos.ToString("F2");

        GameObject enemy = GameObject.FindWithTag("Enemy");

        if (enemy != null)
        {
            output += "\n\nENEMY POSITION\n";
            output += enemy.transform.position.ToString("F2");

            output += "\n\nDISTANCE: ";
            output += Vector3.Distance(
                cameraPos,
                enemy.transform.position
            ).ToString("F2");
        }
        else
        {
            output += "\n\nNO ENEMY FOUND";
        }

        debugText.text = output;
    }
}

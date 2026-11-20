using UnityEngine;

public class ARTrackingTest : MonoBehaviour
{
    [SerializeField] private Camera arCamera;

    private GameObject testCube;

    void Start()
    {
        Vector3 position =
            arCamera.transform.position +
            arCamera.transform.forward * 2f;

        testCube = GameObject.CreatePrimitive(
            PrimitiveType.Cube
        );

        testCube.transform.position = position;
        testCube.transform.localScale = Vector3.one * 0.3f;

        Debug.Log("TEST CUBE CREATED AT: " + position);
    }
}

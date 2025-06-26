using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    private Data_Player data;

    float xRotation = 0f;

    void Start()
    {
        data = GetComponentInParent<Data_Player>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Vector2 look = data.cameraSettings.lookInput * data.cameraSettings.mouseSensitivity * Time.deltaTime;

        // Rotação vertical (câmera)
        xRotation -= look.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        data.cameraSettings.cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Rotação horizontal (jogador)
        data.cameraSettings .playerBody.Rotate(Vector3.up * look.x);
    }
}

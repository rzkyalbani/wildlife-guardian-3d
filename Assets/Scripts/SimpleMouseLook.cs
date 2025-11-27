using UnityEngine;

public class SimpleMouseLook : MonoBehaviour
{
    public float mouseSensitivity = 200f; 
    public Transform playerBody; 

    private float xRotation = 0f;

    // HAPUS BAGIAN START YANG NGUNCI KURSOR
    // void Start() { ... } <-- Hapus atau kosongkan aja

    void Update()
    {
        // --- TAMBAHAN PENTING ---
        // Kalau game lagi Pause/Intro (TimeScale 0), berhenti di sini.
        // Jangan kunci kursor, jangan putar kamera.
        if (Time.timeScale == 0) return; 

        // Pastikan kursor terkunci kalau game lagi JALAN
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        // -------------------------

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); 

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        if(playerBody != null)
        {
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}
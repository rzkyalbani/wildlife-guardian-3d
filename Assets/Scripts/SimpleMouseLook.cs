using UnityEngine;

public class SimpleMouseLook : MonoBehaviour
{
    public float mouseSensitivity = 200f; 
    public Transform playerBody; 

    private float xRotation = 0f;

    void Update()
    {
        // --- CEK 1: APAKAH GAME LAGI PAUSE? ---
        if (Time.timeScale == 0) return;

        // --- CEK 2: APAKAH ADA UI (KEYPAD) YANG LAGI BUKA? ---
        // Kalau UIManager bilang "UI Aktif", maka:
        if (UIManager.instance != null && UIManager.instance.isUIActive)
        {
            // 1. Munculkan Kursor (Biar bisa klik)
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            // 2. Hentikan fungsi nengok (return)
            return; 
        }

        // --- KALAU TIDAK ADA UI & TIDAK PAUSE ---
        // Kunci kursor biar bisa nengok
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // --- LOGIKA MOUSE LOOK (FPP) ---
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
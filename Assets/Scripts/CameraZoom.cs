using UnityEngine;
using Unity.Cinemachine; // <-- Perhatikan Namespace baru ini

public class CameraZoom : MonoBehaviour
{
    // Di Unity 6, namanya jadi CinemachineCamera
    public CinemachineCamera cam; 

    [Header("Pengaturan Zoom")]
    public float zoomSpeed = 10f;
    public float minFOV = 20f; // Zoom in mentok
    public float maxFOV = 60f; // Zoom out mentok

    void Start()
    {
        // Cari komponen kamera di objek ini
        if (cam == null)
            cam = GetComponent<CinemachineCamera>();
    }

    void Update()
    {
        // 1. Deteksi Scroll Mouse
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");

        if (scrollInput != 0 && cam != null)
        {
            // 2. Ambil nilai FOV sekarang
            // Di Cinemachine 3, FOV ada di dalam properti 'Lens'
            float currentFOV = cam.Lens.FieldOfView;

            // 3. Hitung FOV baru (Kebalikan: Scroll Up = FOV Kecil = Zoom In)
            currentFOV -= scrollInput * zoomSpeed;

            // 4. Batasi (Clamp) biar gak kelewatan
            currentFOV = Mathf.Clamp(currentFOV, minFOV, maxFOV);

            // 5. Kembalikan nilai baru ke kamera
            cam.Lens.FieldOfView = currentFOV;
        }
    }
}
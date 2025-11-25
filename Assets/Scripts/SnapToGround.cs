using UnityEngine;

public class SnapToGround : MonoBehaviour
{
    [Header("Pengaturan")]
    public float raycastHeight = 100f; // Nembak dari ketinggian ini
    public float maxDistance = 500f;   // Jarak tembak maksimal
    
    [Header("Wajib Diisi")]
    public LayerMask whatIsGround; // Pilih layer 'Tanah' di sini
    
    [Header("Atur Manual")]
    public float yOffset = 0f; // Ganti angka ini kalau tenggelam/terbang

    void Start()
    {
        AlignToGround();
    }

    void AlignToGround()
    {
        // Posisi tembak dari atas objek
        Vector3 startPos = new Vector3(transform.position.x, transform.position.y + raycastHeight, transform.position.z);
        RaycastHit hit;

        // Gambar garis merah di Scene buat ngecek
        Debug.DrawRay(startPos, Vector3.down * maxDistance, Color.red, 20f);

        // Tembak laser!
        if (Physics.Raycast(startPos, Vector3.down, out hit, maxDistance, whatIsGround))
        {
            // KETEMU TANAH!
            // Langsung pindahin ke titik temu + offset manual
            transform.position = hit.point + (Vector3.up * yOffset);
            
            Debug.Log("✅ " + gameObject.name + " Nempel di tanah (Manual Mode)");
        }
        else
        {
            // Kalau gak nemu tanah, coba cek Layer-nya bener gak?
            Debug.LogWarning("❌ " + gameObject.name + " Gak nemu layer Tanah! Cek Inspector.");
        }
    }
}
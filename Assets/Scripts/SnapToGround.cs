using UnityEngine;

public class SnapToGround : MonoBehaviour
{
    [Header("Pengaturan")]
    public float raycastHeight = 100f;
    public float maxDistance = 500f;
    public float yOffset = 0f;
    
    // BARU: Kita tentukan layer apa yang boleh ditembak
    public LayerMask whatIsGround; 

    void Start()
    {
        AlignToGround();
    }

    void AlignToGround()
    {
        Vector3 startPos = new Vector3(transform.position.x, transform.position.y + raycastHeight, transform.position.z);
        RaycastHit hit;

        // Visual Debug (Biar kelihatan garis merahnya, pastikan tombol 'Gizmos' di Scene View NYALA)
        Debug.DrawRay(startPos, Vector3.down * maxDistance, Color.red, 20f);

        // PERUBAHAN UTAMA: Tambahkan 'whatIsGround' di parameter raycast
        if (Physics.Raycast(startPos, Vector3.down, out hit, maxDistance, whatIsGround))
        {
            // Kalau masuk sini, BERARTI PASTI KENA TANAH (karena pohon di-ignore)
            transform.position = hit.point + (Vector3.up * yOffset);
            Debug.Log("✅ " + gameObject.name + " Nempel di TANAH ASLI.");
        }
        else
        {
            Debug.LogWarning("❌ " + gameObject.name + " Gak nemu Layer 'Tanah'. Cek settingan Layer!");
        }
    }
}
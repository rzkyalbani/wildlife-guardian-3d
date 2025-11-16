using UnityEngine;

public class DangerZone : MonoBehaviour
{
    // Fungsi ini dipanggil saat ada yang masuk zona trigger ini
    private void OnTriggerEnter(Collider other)
    {
        // Cek dulu, yang masuk "Player" bukan?
        if (other.CompareTag("Player"))
        {
            // Jika iya, cari script "otak" di induk (BearTrapInteract)
            // dan panggil fungsi baru kita: "TrapPlayer()"
            GetComponentInParent<BearTrapInteract>().TrapPlayer(other.gameObject);
        }
    }
}
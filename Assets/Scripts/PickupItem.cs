using UnityEngine;
using TMPro; // Wajib untuk UI

// INI KODE FINAL - SUDAH 'REFRACTOR' DENGAN UIMANAGER
public class PickupItem : MonoBehaviour
{
    // HAPUS SEMUA SLOT UI!

    // Variabel UI kita ambil dari UIManager
    private GameObject interactTextObject; 
    private TMP_Text textComponent;
    private string pickupText = "Tekan [E] untuk Mengambil Batu";

    // Variabel internal
    private bool isPlayerNearby = false;
    private GameObject playerObject; // Untuk menyimpan siapa player-nya

    void Start()
    {
        // 'Nanya' ke Otak UI
        textComponent = UIManager.instance.interactTextComponent;
        interactTextObject = UIManager.instance.interactTextObject;

        // (Kita nggak SetActive(false) di sini, biarin OnTriggerExit yang ngurus)
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            playerObject = other.gameObject; // Simpan data si Player
            
            // Atur teks dan munculkan
            textComponent.text = pickupText;
            interactTextObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            playerObject = null; // Hapus data Player
            interactTextObject.SetActive(false); // Sembunyikan UI
        }
    }

    void Update()
    {
        // Jika player dekat DAN tekan 'E'
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            // Ambil script PlayerMovement dari player
            PlayerMovement playerScript = playerObject.GetComponent<PlayerMovement>();

            // Cek dulu, kalau player BELUM bawa item
            if (playerScript.isHoldingItem == false)
            {
                // 1. Set "ransel" player jadi true
                playerScript.isHoldingItem = true;
                
                // 2. Sembunyikan UI
                interactTextObject.SetActive(false);
                
                // 3. Hancurkan item ini (Batu)
                Destroy(gameObject);
                
                Debug.Log("PLAYER MENGAMBIL ITEM!");
            }
            else
            {
                // Player sudah bawa item, ransel penuh
                textComponent.text = "Ransel sudah penuh!";
            }
        }
    }
}
using UnityEngine;
using UnityEngine.UI; // Wajib untuk Slider
using TMPro;

// INI KODE FINAL - SUDAH 'REFRACTOR' DENGAN UIMANAGER
public class SnareInteract : MonoBehaviour
{
    // HAPUS SEMUA SLOT UI!

    [Header("Pengaturan Mini-Game")]
    public float barSpeed = 2.0f; // Seberapa cepat bar-nya gerak

    // Variabel UI kita ambil dari UIManager
    private TMP_Text textComponent;
    private GameObject interactTextObject;
    private Slider timingBar;
    private RectTransform timingBarRect;
    private RectTransform successZoneRect;
    
    // Variabel internal
    private float successMin;
    private float successMax;
    private bool isPlayerNearby = false;
    private bool isDisarming = false; // Lagi main mini-game?
    private int barDirection = 1; // 1 = ke kanan, -1 = ke kiri
    
    void Start()
    {
        // 'Nanya' ke Otak UI
        textComponent = UIManager.instance.interactTextComponent;
        interactTextObject = UIManager.instance.interactTextObject;
        timingBar = UIManager.instance.timingBar;
        timingBarRect = UIManager.instance.timingBarRect;
        successZoneRect = UIManager.instance.successZoneRect;

        // --- INI LOGIKA PINTARNYA ---
        // Kita hitung total lebar 'rel' bar-nya
        float totalWidth = timingBarRect.rect.width; 
        
        // Kita baca 'Left' dan 'Right' dari ZonaSukses
        float leftPixel = successZoneRect.offsetMin.x;
        float rightPixel = successZoneRect.offsetMax.x;

        // Kita konversi dari pixel ke nilai 0-1
        successMin = leftPixel / totalWidth;
        successMax = 1.0f - (Mathf.Abs(rightPixel) / totalWidth);
        // --- SELESAI ---

        interactTextObject.SetActive(false);
        timingBar.gameObject.SetActive(false); // Sembunyikan bar di awal
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            textComponent.text = "Tekan [E] untuk Menetralisir Jerat";
            interactTextObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            interactTextObject.SetActive(false);
            ResetMiniGame(); // Matikan mini-game kalau player pergi
        }
    }

    void Update()
    {
        if (!isPlayerNearby) return; // Keluar jika player jauh

        // 1. Jika player menekan 'E'
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isDisarming)
            {
                // ---- MULAI MINI-GAME ----
                isDisarming = true;
                timingBar.value = 0; // Mulai dari 0
                barDirection = 1; // Mulai gerak ke kanan
                timingBar.gameObject.SetActive(true); // Tampilkan bar
                textComponent.text = "Tekan [E] Tepat di Zona Hijau!";
            }
            else
            {
                // ---- PLAYER MENCOBA MENYELESAIKAN ----
                CheckSuccess();
            }
        }

        // 2. Jika mini-game sedang berjalan, gerakkan bar-nya
        if (isDisarming)
        {
            float currentValue = timingBar.value;
            currentValue += Time.deltaTime * barSpeed * barDirection;

            // Cek jika mentok, bolak-balik
            if (currentValue >= 1.0f)
            {
                currentValue = 1.0f;
                barDirection = -1; // Balik arah
            }
            else if (currentValue <= 0f)
            {
                currentValue = 0f;
                barDirection = 1; // Balik arah
            }
            
            timingBar.value = currentValue;
        }
    }

    void CheckSuccess()
    {
        float currentValue = timingBar.value;

        // Cek apakah ada di 'zona sukses'
        if (currentValue >= successMin && currentValue <= successMax)
        {
            // ---- SUKSES! ----
            Debug.Log("JERAT SUKSES DINONAKTIFKAN!");
            ResetMiniGame();
            GameManager.instance.OnTrapDisarmed(); // Lapor ke Otak Game
            Destroy(gameObject); // Hancurkan jebakan
        }
        else
        {
            // ---- GAGAL! ----
            Debug.Log("Gagal! Coba lagi.");
            ResetMiniGame();
            textComponent.text = "Gagal! Tekan [E] untuk Coba Lagi.";
        }
    }

    void ResetMiniGame()
    {
        isDisarming = false;
        timingBar.gameObject.SetActive(false); // Sembunyikan bar
        // Teks akan di-set ulang di OnTriggerEnter atau CheckSuccess
    }
}
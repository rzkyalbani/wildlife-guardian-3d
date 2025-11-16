using UnityEngine;
using TMPro;
using UnityEngine.UI;

// INI KODE FINAL - SUDAH 'REFRACTOR' DENGAN UIMANAGER
public class BearTrapInteract : MonoBehaviour
{
    // HAPUS SEMUA HEADER UI (Slot UI sudah dipindah ke UIManager)

    [Header("Pengaturan Jebakan")]
    public float disarmDuration = 2.0f;
    public float mashingPower = 10f; // Berapa 'poin' per klik 'E'
    public float mashingDecay = 5f; // Berapa 'poin' berkurang per detik

    // Variabel UI kita ambil dari UIManager
    private TMP_Text textComponent;
    private GameObject panelMashing;
    private Slider mashingBar;
    private GameObject interactTextObject; // Kita butuh GameObject-nya juga
    
    // Variabel teks (kita simpan di sini)
    private string defaultText = "Tekan & Tahan [E] untuk Menetralisir";
    private string failedText = "Bahaya! Butuh item berat untuk memicunya.";

    // Variabel internal
    private float disarmTimer = 0.0f;
    private float mashingProgress = 0f;
    private bool isPlayerNearby = false;
    private bool isPlayerTrapped = false;
    private PlayerMovement playerScript; // Simpan script player

    
    void Start()
    {
        // 'Nanya' ke Otak UI
        textComponent = UIManager.instance.interactTextComponent;
        panelMashing = UIManager.instance.panelMashing;
        mashingBar = UIManager.instance.mashingBar;
        interactTextObject = UIManager.instance.interactTextObject; // Ambil GameObject-nya

        // Pastikan semua UI mati di awal
        if(interactTextObject != null) interactTextObject.SetActive(false); 
        if(panelMashing != null) panelMashing.SetActive(false);
    }

    // --- FUNGSI TRIGGER ZONA AMAN ---
    private void OnTriggerEnter(Collider other)
    {
        if (isPlayerTrapped) return; 
        
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            playerScript = other.GetComponent<PlayerMovement>();
            textComponent.text = defaultText;
            interactTextObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isPlayerTrapped) return; 
        
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            playerScript = null;
            interactTextObject.SetActive(false);
            disarmTimer = 0.0f;
        }
    }

    // --- FUNGSI DIPANGGIL OLEH 'DangerZone.cs' ---
    public void TrapPlayer(GameObject playerObject)
    {
        if (isPlayerTrapped) return;

        Debug.Log("PLAYER KEJEBAK!");
        isPlayerTrapped = true;
        isPlayerNearby = false; 
        interactTextObject.SetActive(false); 
        
        playerScript = playerObject.GetComponent<PlayerMovement>();
        playerScript.enabled = false; // Matikan gerakan player

        panelMashing.SetActive(true);
        mashingProgress = 0f;
        mashingBar.value = mashingProgress;
    }

    // --- FUNGSI UNTUK LOLOS ---
    void FreePlayer()
    {
        Debug.Log("PLAYER LOLOS!");
        isPlayerTrapped = false;

        playerScript.enabled = true; // Nyalakan lagi gerakan player
        
        panelMashing.SetActive(false);
        mashingProgress = 0f;
    }

    // --- UPDATE (2 MODE) ---
    void Update()
    {
        if (isPlayerTrapped)
        {
            // --- MODE 2: JIKA LAGI KEJEBAK (Logika Mashing) ---
            if (Input.GetKeyDown(KeyCode.E))
            {
                mashingProgress += mashingPower;
            }
            
            mashingProgress -= mashingDecay * Time.deltaTime;
            mashingProgress = Mathf.Clamp(mashingProgress, 0, 100);
            
            mashingBar.value = mashingProgress;

            if (mashingProgress >= 100)
            {
                FreePlayer();
            }
        }
        else
        {
            // --- MODE 1: JIKA AMAN (Logika Disarm) ---
            if (!isPlayerNearby) return; 

            if (Input.GetKey(KeyCode.E))
            {
                if (playerScript.isHoldingItem == true)
                {
                    disarmTimer += Time.deltaTime;
                    textComponent.text = "Menggunakan item... " + (disarmDuration - disarmTimer).ToString("F1") + "s";

                    if (disarmTimer >= disarmDuration)
                    {
                        DisarmTrap();
                    }
                }
                else
                {
                    disarmTimer = 0.0f;
                    textComponent.text = failedText;
                }
            }
            else if (Input.GetKeyUp(KeyCode.E))
            {
                disarmTimer = 0.0f;
                textComponent.text = defaultText;
            }
        }
    }

    void DisarmTrap()
    {
        interactTextObject.SetActive(false);
        playerScript.isHoldingItem = false; 
        
        GameManager.instance.OnTrapDisarmed(); // Lapor ke Otak Game
        
        Destroy(gameObject); 
    }
}
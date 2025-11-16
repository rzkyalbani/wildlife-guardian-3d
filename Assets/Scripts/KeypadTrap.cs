using UnityEngine;
using TMPro;
using UnityEngine.UI;

// INI KODE FINAL - SUDAH 'REFRACTOR' DENGAN UIMANAGER
public class KeypadTrap : MonoBehaviour
{
    [Header("Kode Rahasia")]
    public string correctCode = "483";

    [Header("Pengaturan Hadiah")]
    public GameObject hewanPrefab; 

    // HAPUS SEMUA HEADER UI (Slot UI sudah dipindah ke UIManager)

    // Variabel UI kita ambil dari UIManager
    private TMP_Text interactTextComponent;
    private GameObject interactTextObject;
    private GameObject panelGembok;
    private TMP_InputField inputKode;
    private TMP_Text labelJudul;
    
    private bool isPlayerNearby = false;

    void Start()
    {
        // 'Nanya' ke Otak UI
        interactTextComponent = UIManager.instance.interactTextComponent;
        interactTextObject = UIManager.instance.interactTextObject;
        panelGembok = UIManager.instance.panelGembok;
        inputKode = UIManager.instance.inputKode;
        labelJudul = UIManager.instance.labelJudul;

        // Setup UI
        interactTextObject.SetActive(false);
        panelGembok.SetActive(false);

        // --- INI WAJIB ---
        // Kita harus 'AddListener' secara manual, karena slotnya udah nggak ada
        // Kita bersihkan dulu 'listener' lama (jaga-jaga)
        UIManager.instance.tombolBuka.onClick.RemoveAllListeners(); 
        UIManager.instance.tombolBuka.onClick.AddListener(CheckCode); 
        
        UIManager.instance.tombolTutup.onClick.RemoveAllListeners(); 
        UIManager.instance.tombolTutup.onClick.AddListener(CloseKeypad); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            interactTextComponent.text = "Terkunci gembok kode. Tekan [E]";
            interactTextObject.SetActive(true);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            interactTextObject.SetActive(false);
            CloseKeypad(); // Otomatis tutup panel kalau player pergi
        }
    }

    void Update()
    {
        // Cek jika player dekat DAN tekan 'E'
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            // Buka panel gemboknya
            OpenKeypad();
        }
    }

    void OpenKeypad()
    {
        panelGembok.SetActive(true); // Munculkan panel
        interactTextObject.SetActive(false); // Sembunyikan "Tekan [E]"
        
        // Bebaskan kursor mouse biar bisa ngetik
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseKeypad() // 'public' biar bisa diakses Tombol
    {
        if (!panelGembok.activeSelf) return; // Nggak usah ditutup kalau udah ketutup

        panelGembok.SetActive(false); // Sembunyikan panel
        
        // Kunci kursor mouse lagi untuk main
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void CheckCode() // 'public' biar bisa diakses Tombol
    {
        string codeAttempt = inputKode.text;

        if (codeAttempt == correctCode)
        {
            // --- BERHASIL ---
            Debug.Log("KODE BENAR! Kandang terbuka!");
            CloseKeypad();
            
            if (hewanPrefab != null)
            {
                Instantiate(hewanPrefab, transform.position, transform.rotation);
            }
            
            GameManager.instance.OnTrapDisarmed(); // Lapor ke Otak Game
            
            Destroy(gameObject); // Hancurkan kandang
        }
        else
        {
            // --- GAGAL ---
            Debug.Log("KODE SALAH!");
            labelJudul.text = "Kode Salah! Coba lagi.";
            inputKode.text = ""; // Kosongkan kolom input
        }
    }
}
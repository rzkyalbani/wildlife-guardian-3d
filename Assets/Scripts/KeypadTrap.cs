using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class KeypadTrap : MonoBehaviour
{
    [Header("Kode Rahasia Kandang Ini")]
    public string correctCode = "483"; 

    [Header("Petunjuk (SDGs/Edukasi)")]
    public bool isClueGiver = false; 
    [TextArea] public string clueMessage = "Hutan ini melindungi spesies langka."; // Pakai TextArea biar enak ngetik panjang
    public float messageDuration = 5f;

    [Header("Pengaturan Hadiah")]
    public GameObject hewanPrefab; 

    // Variabel UI
    private TMP_Text interactTextComponent;
    private GameObject interactTextObject;
    private GameObject panelGembok;
    private TMP_InputField inputKode;
    private TMP_Text labelJudul;
    
    private bool isPlayerNearby = false;

    void Start()
    {
        // --- AMBIL REFERENSI SAJA DI SINI ---
        if (UIManager.instance != null)
        {
            interactTextComponent = UIManager.instance.interactTextComponent;
            interactTextObject = UIManager.instance.interactTextObject;
            panelGembok = UIManager.instance.panelGembok;
            inputKode = UIManager.instance.inputKode;
            labelJudul = UIManager.instance.labelJudul;
        }
        
        // ❌ JANGAN AddListener DI SINI! 
        // Kalau di sini, nanti tombolnya rebutan sama kandang lain.
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if(interactTextComponent) interactTextComponent.text = "Terkunci gembok kode. Tekan [E]";
            if(interactTextObject) interactTextObject.SetActive(true);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if(interactTextObject) interactTextObject.SetActive(false);
            CloseKeypad(); 
        }
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E))
        {
            OpenKeypad();
        }
    }

    void OpenKeypad()
    {
        // --- PASANG LISTENER DI SINI (SAAT PANEL DIBUKA) ---
        // 1. Reset dulu biar gak numpuk
        UIManager.instance.tombolBuka.onClick.RemoveAllListeners(); 
        UIManager.instance.tombolTutup.onClick.RemoveAllListeners();

        // 2. Baru pasang fungsi punya kandang INI
        UIManager.instance.tombolBuka.onClick.AddListener(CheckCode); 
        UIManager.instance.tombolTutup.onClick.AddListener(CloseKeypad); 

        // Reset text input & judul biar bersih pas dibuka
        inputKode.text = "";
        labelJudul.text = "Masukkan Kode Pengaman";

        panelGembok.SetActive(true); 
        interactTextObject.SetActive(false); 
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseKeypad() 
    {
        if (!panelGembok.activeSelf) return; 
        
        // Opsional: Hapus listener lagi biar aman
        UIManager.instance.tombolBuka.onClick.RemoveAllListeners();
        UIManager.instance.tombolTutup.onClick.RemoveAllListeners();

        panelGembok.SetActive(false); 
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void CheckCode()
    {
        string codeAttempt = inputKode.text;

        // Trim() berguna untuk menghapus spasi yang tidak sengaja terketik
        if (codeAttempt.Trim() == correctCode)
        {
            Debug.Log("KODE BENAR!");
            CloseKeypad();
            
            // --- MENAMPILKAN PETUNJUK (Pastikan UIManager punya fungsi ini) ---
            if (isClueGiver)
            {
                // Aku sederhanakan jadi langsung kirim pesan string biar fleksibel
                UIManager.instance.ShowSuccessMessage(clueMessage, messageDuration);
            }
            
            if (hewanPrefab != null)
            {
                Instantiate(hewanPrefab, transform.position, transform.rotation);
            }
            
            if (GameManager.instance != null) 
                GameManager.instance.OnTrapDisarmed();
            
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("KODE SALAH! Input: " + codeAttempt + " vs Benar: " + correctCode);
            labelJudul.text = "Salah! Coba lagi.";
            inputKode.text = "";
            
            // Biar player tetap fokus di input field tanpa harus klik lagi (User Friendly)
            inputKode.ActivateInputField(); 
        }
    }
}
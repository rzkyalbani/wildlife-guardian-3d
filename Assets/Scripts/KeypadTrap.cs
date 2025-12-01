using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class KeypadTrap : MonoBehaviour
{
    [Header("Kode Rahasia Kandang Ini")]
    public string correctCode = "483"; 

    [Header("Petunjuk (SDGs/Edukasi)")]
    public bool isClueGiver = false; 
    [TextArea] public string clueMessage = "Hutan ini melindungi spesies langka."; 
    public float messageDuration = 5f;

    [Header("Pengaturan Hadiah")]
    public GameObject hewanPrefab; 

    // Referensi UI (Diambil dari UIManager)
    private TMP_Text interactTextComponent;
    private GameObject interactTextObject;
    private GameObject panelGembok;
    private TMP_InputField inputKode;
    private TMP_Text labelJudul;
    
    private bool isPlayerNearby = false;

    void Start()
    {
        // Ambil referensi dari UIManager
        if (UIManager.instance != null)
        {
            interactTextComponent = UIManager.instance.interactTextComponent;
            interactTextObject = UIManager.instance.interactTextObject;
            panelGembok = UIManager.instance.panelGembok;
            inputKode = UIManager.instance.inputKode;
            labelJudul = UIManager.instance.labelJudul;
        }
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
        // Reset Listener tombol biar gak rebutan sama kandang lain
        UIManager.instance.tombolBuka.onClick.RemoveAllListeners(); 
        UIManager.instance.tombolTutup.onClick.RemoveAllListeners();

        // Pasang fungsi kandang ini ke tombol UI
        UIManager.instance.tombolBuka.onClick.AddListener(CheckCode); 
        UIManager.instance.tombolTutup.onClick.AddListener(CloseKeypad); 

        // Reset tampilan UI
        inputKode.text = "";
        labelJudul.text = "Masukkan Kode Pengaman";

        UIManager.instance.isUIActive = true; 
        panelGembok.SetActive(true); 
        interactTextObject.SetActive(false); 
        
        // Munculkan kursor buat ngetik
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseKeypad() 
    {
        if (!panelGembok.activeSelf) return; 
        
        // Hapus listener biar bersih
        UIManager.instance.tombolBuka.onClick.RemoveAllListeners();
        UIManager.instance.tombolTutup.onClick.RemoveAllListeners();

        UIManager.instance.isUIActive = false;
        panelGembok.SetActive(false); 
        
        // Sembunyikan kursor lagi
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void CheckCode()
    {
        string codeAttempt = inputKode.text;

        if (codeAttempt.Trim() == correctCode)
        {
            Debug.Log("KODE BENAR!");
            CloseKeypad();
            
            // --- Tampilkan Petunjuk Individual (Jika ada) ---
            // Catatan: Jika ini jebakan terakhir, pesan ini mungkin akan tertimpa
            // oleh pesan "Mission Complete" dari GameManager. Itu normal.
            if (isClueGiver)
            {
                UIManager.instance.ShowSuccessMessage(clueMessage, messageDuration);
            }
            
            // Spawn Hewan
            if (hewanPrefab != null)
            {
                Instantiate(hewanPrefab, transform.position, transform.rotation);
            }
            
            // Lapor ke GameManager bahwa 1 jebakan beres
            if (GameManager.instance != null) 
                GameManager.instance.OnTrapDisarmed();
            
            Destroy(gameObject); // Hapus kandang
        }
        else
        {
            Debug.Log("KODE SALAH!");
            labelJudul.text = "Salah! Coba lagi.";
            inputKode.text = "";
            inputKode.ActivateInputField(); 
        }
    }
}
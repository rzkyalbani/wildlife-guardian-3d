using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections; 

public class UIManager : MonoBehaviour
{
    // --- Singleton Pattern ---
    public static UIManager instance;

    [Header("UI Interaksi Dasar")]
    public GameObject interactTextObject; 
    public TMP_Text interactTextComponent;

    [Header("UI Panel Mashing")]
    public GameObject panelMashing;
    public Slider mashingBar;

    [Header("UI Panel Timing")]
    public Slider timingBar;
    public RectTransform timingBarRect;
    public RectTransform successZoneRect;

    [Header("UI Panel Gembok")]
    public GameObject panelGembok;
    public TMP_InputField inputKode;
    public Button tombolBuka;
    public Button tombolTutup;
    public TMP_Text labelJudul;
    
    [Header("UI Narasi & Cerita")]
    public GameObject panelNarasi;      
    public TMP_Text textCerita;         
    public Button tombolLanjut;         

    [Header("Audio Effect")]
    public AudioSource audioSource;     
    public AudioClip sfxTombol;         
    public AudioClip sfxBenar;          
    public AudioClip sfxMenang;     

    [Header("Status Game")]
    public bool isUIActive = false;    

    // Variabel untuk mencegah teks numpuk
    private Coroutine typingCoroutine;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Pastikan UI yang tidak perlu mati dulu
        if (panelGembok) panelGembok.SetActive(false);
        if (interactTextObject) interactTextObject.SetActive(false);
        if (panelMashing) panelMashing.SetActive(false); 
        
        // Mulai Intro Game
        ShowIntro();
    }

    // --- FUNGSI SHOW INTRO ---
    public void ShowIntro()
    {
        if (panelNarasi == null) return; 

        // Stop ketikan lama jika ada
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        panelNarasi.SetActive(true);
        Time.timeScale = 0; // Pause Game
        
        // Munculkan Kursor Mouse
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true; 

        string pesanIntro = "INCOMING TRANSMISSION... [SECURE CHANNEL]\n\n" +
            "DARI: HQ (Markas Pusat)\n" +
            "KEPADA: Ranger 01\n\n" +
            "Ranger, satelit mendeteksi sindikat 'Iron Net' di Hutan Arutala. " +
            "Mereka memasang jebakan digital untuk menangkap satwa lindung.\n\n" +
            "PROTOKOL MISI:\n" +
            "1. Dekati RADIO di Base Camp untuk terima koordinat.\n" +
            "2. Retas jebakan & kumpulkan potongan kode.\n" +
            "3. Selamatkan Target Utama (Harimau) di kandang besar.\n\n" +
            "Hutan ini mengandalkanmu. Ganti.";

        typingCoroutine = StartCoroutine(TypeWriterEffect(pesanIntro));

        tombolLanjut.onClick.RemoveAllListeners();
        tombolLanjut.onClick.AddListener(CloseNarasi);
        
        if(audioSource && sfxTombol) audioSource.PlayOneShot(sfxTombol);
    }

    // --- FUNGSI SHOW PETUNJUK/PESAN ---
    public void ShowSuccessMessage(string message, float duration)
    {
        if (panelNarasi == null) return;

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        panelNarasi.SetActive(true);
        Time.timeScale = 0; // Pause Game
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        typingCoroutine = StartCoroutine(TypeWriterEffect(message));

        tombolLanjut.onClick.RemoveAllListeners();
        tombolLanjut.onClick.AddListener(CloseNarasi);
        
        if(audioSource && sfxBenar) audioSource.PlayOneShot(sfxBenar);
    }

    // --- FUNGSI WIN GAME (TAMAT) ---
    public void ShowWinGame()
    {
        if (panelNarasi == null) return;

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);

        panelNarasi.SetActive(true);
        Time.timeScale = 0; // Pause Game
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        string pesanMenang = "MISSION SUCCESS!\n\n" +
            "Target Utama berhasil diamankan. Sistem jebakan lumpuh.\n" +
            "Terima kasih, Ranger. Hutan ini berhutang nyawa padamu.\n\n" +
            "#LifeOnLand - SDG 15";

        typingCoroutine = StartCoroutine(TypeWriterEffect(pesanMenang));

        // Ubah tombol lanjut jadi tombol KELUAR
        TMP_Text teksTombol = tombolLanjut.GetComponentInChildren<TMP_Text>();
        if(teksTombol) teksTombol.text = "KELUAR GAME";
        
        tombolLanjut.onClick.RemoveAllListeners();
        tombolLanjut.onClick.AddListener(() => Application.Quit()); 
        
        if(audioSource && sfxMenang) audioSource.PlayOneShot(sfxMenang);
    }

    // --- FUNGSI TUTUP NARASI ---
    void CloseNarasi()
    {
        // PENTING: Matikan mesin ketik biar teks gak gentayangan
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        
        panelNarasi.SetActive(false);
        Time.timeScale = 1; // Lanjut Game
        
        // Kunci Kursor Mouse lagi (Mode FPS/TPP)
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Reset teks tombol jadi default
        TMP_Text teksTombol = tombolLanjut.GetComponentInChildren<TMP_Text>();
        if(teksTombol) teksTombol.text = "LANJUT >>";
    }

    // Efek mengetik huruf per huruf
    IEnumerator TypeWriterEffect(string fullText)
    {
        textCerita.text = ""; 
        foreach (char c in fullText)
        {
            textCerita.text += c;
            yield return new WaitForSecondsRealtime(0.02f); 
        }
    }
}
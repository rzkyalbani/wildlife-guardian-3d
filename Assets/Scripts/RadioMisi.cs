using UnityEngine;
using TMPro;

public class RadioMisi : MonoBehaviour
{
    [Header("Pengaturan Misi")]
    // Isi teks arah mata angin/briefing di sini lewat Inspector
    [TextArea(3, 10)] 
    public string[] missionBriefings; 

    private int currentEpisode = 0; 
    private bool isPlayerNearby = false;
    private bool isWaitingForPlayer = true; // Radio aktif di awal

    // Kita butuh referensi UIManager biar kode lebih pendek
    private UIManager uiManager;

    void Start()
    {
        uiManager = UIManager.instance;
        // Kita gak perlu matikan interactTextObject di sini, biar UIManager yg atur
    }

    // --- DIPANGGIL GAMEMANAGER SAAT MISI SELESAI ---
    public void ReadyForNextMission()
    {
        Debug.Log("Radio: Misi selesai, standby untuk misi berikutnya.");
        currentEpisode++; // Naik level
        
        // Cek apakah masih ada misi?
        if (currentEpisode < missionBriefings.Length)
        {
            isWaitingForPlayer = true; // Nyalakan radio lagi
            
            // Opsional: Kasih efek suara 'kresek-kresek' radio kalau mau
        }
        else
        {
            Debug.Log("Radio: Semua misi selesai!");
            isWaitingForPlayer = false; // Radio mati total
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isWaitingForPlayer)
        {
            isPlayerNearby = true;
            
            // --- PERBAIKAN DI SINI ---
            // Jangan tampilkan briefing panjang. Tampilkan instruksi singkat aja.
            uiManager.interactTextComponent.text = "Terima Laporan [E]";
            uiManager.interactTextObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            // Sembunyikan teks "Tekan E"
            uiManager.interactTextObject.SetActive(false);
        }
    }

    void Update()
    {
        if (isPlayerNearby && isWaitingForPlayer && Input.GetKeyDown(KeyCode.E))
        {
            StartBriefing();
        }
    }

    void StartBriefing()
    {
        // 1. Matikan interaksi radio (biar gak dispam)
        isWaitingForPlayer = false; 
        uiManager.interactTextObject.SetActive(false); // Sembunyikan "Tekan E"

        // 2. Ambil teks misi sesuai episode sekarang
        string pesanMisi = "";
        if (currentEpisode < missionBriefings.Length)
        {
            pesanMisi = missionBriefings[currentEpisode];
        }
        else
        {
            pesanMisi = "Sinyal hilang... Tidak ada laporan baru.";
        }

        // 3. TAMPILKAN LEWAT PANEL NARASI (Biar Keren!)
        // Kita pakai fungsi ShowSuccessMessage yang udah ada (karena sama-sama nampilin teks & pause game)
        // Atau kalau mau perfect, bikin fungsi baru ShowBriefing di UIManager, tapi ini juga bisa.
        uiManager.ShowSuccessMessage(pesanMisi, 0); 
        // Note: Durasi 0 gak ngaruh karena kita pakai tombol 'Lanjut'

        // 4. PANGGIL GAMEMANAGER UNTUK SPAWN LEVEL
        // Level akan muncul di background pas player lagi baca briefing
        GameManager.instance.StartMission(currentEpisode);
        
        Debug.Log("Radio: Misi Episode " + currentEpisode + " dimulai.");
    }
}
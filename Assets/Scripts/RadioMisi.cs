using UnityEngine;
using TMPro;

public class RadioMisi : MonoBehaviour
{
    [Header("Referensi UI")]
    public GameObject interactTextObject; 
    private TMP_Text textComponent;

    [Header("Pengaturan Misi")]
    // Kita buat 'daftar' teks misi. 
    // Kita isi di Inspector nanti.
    [TextArea(3, 5)]
    public string[] missionBriefings; // Daftar teks untuk tiap episode

    private int currentEpisode = 0; // Melacak kita di episode berapa
    private bool isPlayerNearby = false;
    private bool isWaitingForPlayer = true; // Radio 'aktif' menunggu pemain?

    void Start()
    {
        textComponent = UIManager.instance.interactTextComponent;
        interactTextObject.SetActive(false);
    }

    // --- FUNGSI BARU! ---
    // Ini akan dipanggil oleh 'GameManager' saat 1 episode selesai
    public void ReadyForNextMission()
    {
        Debug.Log("Radio siap untuk misi berikutnya!");
        isWaitingForPlayer = true; // Aktifkan radio lagi
        currentEpisode++; // Naik ke episode selanjutnya

        // Cek dulu, masih ada misi nggak di daftar kita?
        if (currentEpisode >= missionBriefings.Length)
        {
            // Kalau misi udah habis (tapi belum 'Tamat' dari GM)
            // Tampilkan teks 'standby'
            textComponent.text = "Radio hening... Sepertinya sudah aman.";
        }
        else
        {
            // Siapkan teks untuk misi berikutnya
            textComponent.text = missionBriefings[currentEpisode];
        }
    }

    // --- FUNGSI TRIGGER (Sedikit Diubah) ---
    private void OnTriggerEnter(Collider other)
    {
        // Cek: Apakah player? DAN Radio lagi 'nungguin' diklik?
        if (other.CompareTag("Player") && isWaitingForPlayer)
        {
            isPlayerNearby = true;
            // Tampilkan teks misi yang sesuai
            textComponent.text = missionBriefings[currentEpisode]; 
            interactTextObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            interactTextObject.SetActive(false);
        }
    }

    void Update()
    {
        // Cek: Player dekat? Radio nungguin? DAN tekan 'E'?
        if (isPlayerNearby && isWaitingForPlayer && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Player menerima Misi Episode: " + currentEpisode);
            
            // 1. Matikan radio (biar nggak bisa diklik berkali-kali)
            isWaitingForPlayer = false; 
            
            // 2. Panggil 'Otak Utama' dan suruh 'StartMission'
            //    sesuai 'currentEpisode'
            GameManager.instance.StartMission(currentEpisode);
            
            // 3. Sembunyikan teksnya
            interactTextObject.SetActive(false);
        }
    }
}
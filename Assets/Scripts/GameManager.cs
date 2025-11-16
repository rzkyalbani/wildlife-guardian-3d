using UnityEngine;
using TMPro; 

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Pengaturan UI")]
    public TMP_Text trapCounterText; 
    public GameObject winPanel; 

    [Header("Pengaturan Misi Episodik")]
    public GameObject[] daftarPrefabMisi; // Slot untuk 'Kardus' Misi
    public RadioMisi radioMisi; // BARU: Slot untuk si Radio
    public Transform missionSpawnPoint;
    
    private int trapsRemaining;
    private bool isMissionActive = false;
    private int currentEpisode = 0; // Melacak kita di episode berapa
    private GameObject currentMissionObject; // Simpan 'kardus' yang di-spawn

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        winPanel.SetActive(false);
        trapCounterText.gameObject.SetActive(false); 
    }

    // Ini dipanggil oleh RadioMisi
    public void StartMission(int episodeIndex)
    {
        if (isMissionActive) return;

        Debug.Log("MEMULAI EPISODE: " + episodeIndex);
        isMissionActive = true; 
        currentEpisode = episodeIndex; 

        // --- MENCIPTAKAN MISI ---
        // 1. "Ciptakan" (Instantiate) kardus misi ke dalam scene
        //    dan SIMPAN referensinya ke 'currentMissionObject'
        currentMissionObject = Instantiate(daftarPrefabMisi[episodeIndex], missionSpawnPoint.position, missionSpawnPoint.rotation, missionSpawnPoint);
        
        // 2. Tunda 1 frame (biar jebakannya 'terdaftar' dulu)
        Invoke("HitungJebakan", 0.1f); 
    }

    // Fungsi baru yang kita 'tunda'
    void HitungJebakan()
    {
        // 3. Hitung jebakan yang UDAH DICIPTAKAN
        GameObject[] traps = GameObject.FindGameObjectsWithTag("Jebakan");
        trapsRemaining = traps.Length;
        
        // 4. MUNCULKAN UI-nya
        trapCounterText.gameObject.SetActive(true);
        UpdateTrapUI();
    }

    // Fungsi ini dipanggil oleh script jebakan
    public void OnTrapDisarmed()
    {
        if (!isMissionActive) return;

        trapsRemaining--;
        UpdateTrapUI();

        // --- INI DIA LOGIKA BARUNYA ---
        if (trapsRemaining <= 0)
        {
            isMissionActive = false; // Misi (episode) ini selesai
            trapCounterText.gameObject.SetActive(false); // Sembunyikan UI skor
            
            // Hancurkan "kardus misi" yang lama (bersih-bersih)
            if(currentMissionObject != null)
            {
                Destroy(currentMissionObject);
            }

            // Cek dulu, ini episode terakhir BUKAN?
            if (currentEpisode + 1 < daftarPrefabMisi.Length)
            {
                // --- BELUM TERAKHIR ---
                // Belum tamat! Panggil radio dan suruh dia 'siap-siap'
                Debug.Log("EPISODE " + currentEpisode + " SELESAI! Siap untuk episode berikutnya.");
                radioMisi.ReadyForNextMission(); 
            }
            else
            {
                // --- SUDAH EPISODE TERAKHIR ---
                // Baru kita TAMAT
                Debug.Log("SEMUA EPISODE SELESAI! GAME TAMAT!");
                Invoke("ShowWinPanel", 1f); 
            }
        }
    }

    void UpdateTrapUI()
    {
        trapCounterText.text = "Jebakan Tersisa: " + trapsRemaining;
    }

    void ShowWinPanel()
    {
        winPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
using UnityEngine;
using TMPro; 
using UnityEngine.UI; 
using UnityEngine.SceneManagement; 

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Pengaturan UI")]
    public TMP_Text trapCounterText; 
    
    [Header("UI Timer & Kalah")]
    public TMP_Text timerText; 
    public GameObject losePanel; 
    public Button retryButton; 

    [Header("Pengaturan Misi Episodik")]
    public GameObject[] daftarPrefabMisi; // Daftar Level/Misi
    public RadioMisi radioMisi;           // Referensi ke Script Radio
    public Transform missionSpawnPoint;   // Lokasi muncul misi
    
    public float initialTimeLimit = 180f; // Waktu per misi

    private int trapsRemaining;
    private bool isMissionActive = false;
    private int currentEpisode = 0; 
    private GameObject currentMissionObject; 
    private float currentTime; 

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Reset UI saat game mulai
        losePanel.SetActive(false);
        trapCounterText.gameObject.SetActive(false); 
        timerText.gameObject.SetActive(false);

        if(retryButton != null)
        {
            retryButton.onClick.RemoveAllListeners();
            retryButton.onClick.AddListener(RestartEpisode);
        }
    }

    void Update()
    {
        if (isMissionActive)
        {
            currentTime -= Time.deltaTime; 

            // Cek Waktu Habis
            if (currentTime <= 0)
            {
                currentTime = 0;
                GameOver();
            }
            UpdateTimerUI();
        }
    }

    public void StartMission(int episodeIndex)
    {
        if (isMissionActive) return;

        Debug.Log("MEMULAI EPISODE: " + episodeIndex);
        isMissionActive = true; 
        currentEpisode = episodeIndex; 
        
        currentTime = initialTimeLimit;

        // Spawn Level Misi
        if (currentMissionObject != null) Destroy(currentMissionObject);
        currentMissionObject = Instantiate(daftarPrefabMisi[episodeIndex], missionSpawnPoint.position, missionSpawnPoint.rotation, missionSpawnPoint);
        
        // Hitung jebakan setelah jeda sebentar (biar spawn beres dulu)
        Invoke("HitungJebakan", 0.1f); 
        
        timerText.gameObject.SetActive(true);
    }

    void HitungJebakan()
    {
        // PENTING: Semua Prefab Kandang harus punya Tag "Jebakan"
        GameObject[] traps = GameObject.FindGameObjectsWithTag("Jebakan");
        trapsRemaining = traps.Length;
        
        trapCounterText.gameObject.SetActive(true);
        UpdateTrapUI();
    }

    // --- FUNGSI DIPANGGIL SAAT KEYPAD TERBUKA ---
    public void OnTrapDisarmed()
    {
        if (!isMissionActive) return;

        trapsRemaining--;
        UpdateTrapUI();

        if (trapsRemaining <= 0)
        {
            MissionSuccess();
        }
    }

    public void ApplyPenalty(float penaltySeconds)
    {
        if (!isMissionActive) return;
        currentTime -= penaltySeconds; 
        timerText.color = Color.red;
        Invoke("ResetTimerColor", 0.5f);
    }
    
    void ResetTimerColor()
    {
        timerText.color = Color.white;
    }

    // --- LOGIKA MENANG PER LEVEL ---
    void MissionSuccess()
    {
        isMissionActive = false; 
        trapCounterText.gameObject.SetActive(false); 
        timerText.gameObject.SetActive(false); 
        
        // Hancurkan level lama (Opsional)
        if(currentMissionObject != null) Destroy(currentMissionObject);

        // Cek apakah masih ada misi selanjutnya?
        if (currentEpisode + 1 < daftarPrefabMisi.Length)
        {
            // --- MASIH ADA LEVEL LAIN ---
            Debug.Log("EPISODE SELESAI! Balik ke Radio.");
            
            // 1. Suruh Radio siap-siap
            radioMisi.ReadyForNextMission(); 

            // 2. Tampilkan pesan suruh balik ke markas
            string pesanBalik = "TARGET DIAMANKAN!\n\n" +
                                "Area ini sudah bersih.\n" +
                                "Kembali ke RADIO BASE CAMP untuk koordinat selanjutnya.\n\n" +
                                "Over.";
            
            UIManager.instance.ShowSuccessMessage(pesanBalik, 0);
        }
        else
        {
            // --- SUDAH LEVEL TERAKHIR (TAMAT) ---
            Debug.Log("TAMAT!");
            
            // Tampilkan Layar Menang (Ending)
            UIManager.instance.ShowWinGame();
        }
    }

    void GameOver()
    {
        isMissionActive = false;
        if(currentMissionObject != null) Destroy(currentMissionObject);
        
        losePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        trapCounterText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
    }

    public void RestartEpisode()
    {
        losePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        StartMission(currentEpisode);
    }

    void UpdateTrapUI()
    {
        trapCounterText.text = "Jebakan Tersisa: " + trapsRemaining;
    }
    
    void UpdateTimerUI()
    {
        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
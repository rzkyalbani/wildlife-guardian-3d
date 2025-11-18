using UnityEngine;
using TMPro; 
using UnityEngine.UI; // Wajib untuk Button
using UnityEngine.SceneManagement; // Untuk reload scene kalau perlu

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Pengaturan UI")]
    public TMP_Text trapCounterText; 
    public GameObject winPanel; 
    
    [Header("UI Timer & Kalah")]
    public TMP_Text timerText; // Slot TeksTimer
    public GameObject losePanel; // Slot PanelKalah
    public Button retryButton; // Slot TombolRetry

    [Header("Pengaturan Misi Episodik")]
    public GameObject[] daftarPrefabMisi; 
    public RadioMisi radioMisi; 
    public Transform missionSpawnPoint; 
    
    // Setting Waktu (Detik) - Bisa diubah di Inspector per Episode nanti
    public float initialTimeLimit = 180f; // 3 Menit (default)

    private int trapsRemaining;
    private bool isMissionActive = false;
    private int currentEpisode = 0; 
    private GameObject currentMissionObject; 
    
    private float currentTime; // Waktu yang berjalan

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Reset semua UI
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        trapCounterText.gameObject.SetActive(false); 
        timerText.gameObject.SetActive(false);

        // Setup tombol retry
        if(retryButton != null)
        {
            retryButton.onClick.RemoveAllListeners();
            retryButton.onClick.AddListener(RestartEpisode);
        }
    }

    void Update()
    {
        // --- LOGIKA TIMER ---
        if (isMissionActive)
        {
            currentTime -= Time.deltaTime; // Kurangi waktu

            // Cek Kalah
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
        
        // Set Waktu Awal (3 Menit)
        currentTime = initialTimeLimit;

        // Spawn Misi
        if (currentMissionObject != null) Destroy(currentMissionObject);
        currentMissionObject = Instantiate(daftarPrefabMisi[episodeIndex], missionSpawnPoint.position, missionSpawnPoint.rotation, missionSpawnPoint);
        
        // Tunda hitung jebakan 1 frame
        Invoke("HitungJebakan", 0.1f); 
        
        // Munculkan Timer
        timerText.gameObject.SetActive(true);
    }

    void HitungJebakan()
    {
        GameObject[] traps = GameObject.FindGameObjectsWithTag("Jebakan");
        trapsRemaining = traps.Length;
        
        trapCounterText.gameObject.SetActive(true);
        UpdateTrapUI();
    }

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

    // --- FUNGSI HUKUMAN ---
    public void ApplyPenalty(float penaltySeconds)
    {
        if (!isMissionActive) return;
        
        currentTime -= penaltySeconds; // Kurangi waktu langsung!
        Debug.Log("HUKUMAN! Waktu berkurang " + penaltySeconds + " detik.");
        
        // Efek visual merah di teks timer (opsional, simpel aja)
        timerText.color = Color.red;
        Invoke("ResetTimerColor", 0.5f);
    }
    
    void ResetTimerColor()
    {
        timerText.color = Color.white;
    }

    void MissionSuccess()
    {
        isMissionActive = false; 
        trapCounterText.gameObject.SetActive(false); 
        timerText.gameObject.SetActive(false); // Sembunyikan timer
        
        if(currentMissionObject != null) Destroy(currentMissionObject);

        if (currentEpisode + 1 < daftarPrefabMisi.Length)
        {
            Debug.Log("EPISODE " + currentEpisode + " SELESAI!");
            radioMisi.ReadyForNextMission(); 
        }
        else
        {
            Debug.Log("TAMAT!");
            Invoke("ShowWinPanel", 1f); 
        }
    }

    void GameOver()
    {
        isMissionActive = false;
        Debug.Log("GAME OVER - WAKTU HABIS");
        
        // Hancurkan misi biar bersih
        if(currentMissionObject != null) Destroy(currentMissionObject);
        
        // Munculkan Panel Kalah
        losePanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Sembunyikan UI lain
        trapCounterText.gameObject.SetActive(false);
        timerText.gameObject.SetActive(false);
    }

    // Fungsi Tombol Retry
    public void RestartEpisode()
    {
        // Sembunyikan panel kalah
        losePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Mulai ulang episode yang sama
        StartMission(currentEpisode);
    }

    void UpdateTrapUI()
    {
        trapCounterText.text = "Jebakan Tersisa: " + trapsRemaining;
    }
    
    void UpdateTimerUI()
    {
        // Format Menit:Detik (Contoh 02:30)
        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void ShowWinPanel()
    {
        winPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
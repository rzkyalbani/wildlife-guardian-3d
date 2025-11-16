using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // --- Ini adalah Singleton Pattern (kayak GameManager) ---
    // Biar semua script bisa manggil 'UIManager.instance'
    public static UIManager instance;

    [Header("UI Interaksi Dasar")]
    public GameObject interactTextObject; // Slot TeksInteraksi
    public TMP_Text interactTextComponent;

    [Header("UI Panel Mashing")]
    public GameObject panelMashing;
    public Slider mashingBar;

    [Header("UI Panel Gembok")]
    public GameObject panelGembok;
    public TMP_InputField inputKode;
    public Button tombolBuka;
    public Button tombolTutup;
    public TMP_Text labelJudul;
    
    [Header("UI Panel Timing")]
    public Slider timingBar;
    public RectTransform timingBarRect;
    public RectTransform successZoneRect;
    
    // (Tambahin UI lain di sini kalau perlu, misal StaminaBar, TeksJebakan, dll)

    void Awake()
    {
        // Setup Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
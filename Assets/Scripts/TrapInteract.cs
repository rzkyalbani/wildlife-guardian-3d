using UnityEngine;
using TMPro; // Kita masih butuh ini

public class TrapInteract : MonoBehaviour
{
    // HAPUS SEMUA SLOT UI! Kita nggak butuh lagi.

    [Header("Pengaturan Jebakan")]
    public float disarmDuration = 3.0f; 
    
    private float disarmTimer = 0.0f;
    private bool isPlayerNearby = false;

    // Kita ambil 'textComponent' dari UIManager
    private TMP_Text textComponent; 

    void Start()
    {
        // 'Nanya' ke Otak UI
        textComponent = UIManager.instance.interactTextComponent;
        UIManager.instance.interactTextObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            textComponent.text = "Tekan [E] untuk Menetralisir"; // Teks awal
            UIManager.instance.interactTextObject.SetActive(true); // Munculkan!
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            UIManager.instance.interactTextObject.SetActive(false); // Sembunyikan!
            disarmTimer = 0.0f;
        }
    }

    void Update()
    {
        if (!isPlayerNearby) return;

        if (Input.GetKey(KeyCode.E))
        {
            disarmTimer += Time.deltaTime; 
            textComponent.text = "Menetralisir... " + (disarmDuration - disarmTimer).ToString("F1") + "s";

            if (disarmTimer >= disarmDuration)
            {
                DisarmTrap();
            }
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            disarmTimer = 0.0f;
            textComponent.text = "Tekan [E] untuk Menetralisir";
        }
    }

    void DisarmTrap()
    {
        UIManager.instance.interactTextObject.SetActive(false);
        GameManager.instance.OnTrapDisarmed(); // Lapor ke 'Otak'
        Destroy(gameObject); 
    }
}
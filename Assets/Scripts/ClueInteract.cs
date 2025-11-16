using UnityEngine;
using TMPro; // Wajib untuk Teks

// INI KODE FINAL - SUDAH 'REFRACTOR' DENGAN UIMANAGER
public class ClueInteract : MonoBehaviour
{
    // HAPUS SLOT UI!

    [Header("Isi Petunjuk")]
    [TextArea(3, 5)] 
    public string clueMessage; // Ini kita isi di Inspector!

    // Variabel UI kita ambil dari UIManager
    private TMP_Text textComponent;
    private GameObject interactTextObject;

    void Start()
    {
        // 'Nanya' ke Otak UI
        textComponent = UIManager.instance.interactTextComponent;
        interactTextObject = UIManager.instance.interactTextObject;

        // Pastikan teks mati di awal
        if(interactTextObject != null)
        {
            interactTextObject.SetActive(false);
        }
    }

    // Saat Player Masuk
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            textComponent.text = clueMessage; // Tampilkan pesan petunjuknya
            interactTextObject.SetActive(true);
        }
    }

    // Saat Player Keluar
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactTextObject.SetActive(false); // Sembunyikan lagi
        }
    }
}
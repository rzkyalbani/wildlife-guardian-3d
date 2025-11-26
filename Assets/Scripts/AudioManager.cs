using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Sumber Suara (Speaker)")]
    public AudioSource musicSource; // Drag AudioSource 1 ke sini
    public AudioSource sfxSource;   // Drag AudioSource 2 ke sini

    [Header("Koleksi Musik (BGM)")]
    public AudioClip bgmHutan;      // Suara ambience hutan/angin
    public AudioClip bgmMisi;       // Suara pas lagi tegang ngerjain misi

    [Header("Koleksi SFX")]
    public AudioClip sfxTombol;     // Klik UI
    public AudioClip sfxBenar;      // Kode benar
    public AudioClip sfxSalah;      // Kode salah
    public AudioClip sfxMenang;     // Mission Complete
    public AudioClip sfxKalah;      // Game Over
    public AudioClip sfxJalan;      // Langkah kaki (opsional)

    void Awake()
    {
        // Singleton biar bisa dipanggil dari mana aja
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Biar ganti scene musik ga putus
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Otomatis putar lagu hutan pas game mulai
        PlayMusic(bgmHutan);
    }

    // Fungsi putar lagu (Ganti kaset)
    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    // Fungsi putar SFX (Sekali bunyi)
    public void PlaySFX(AudioClip clip)
    {
        // PlayOneShot = Bunyiin tanpa motong suara lain
        sfxSource.PlayOneShot(clip);
    }
}
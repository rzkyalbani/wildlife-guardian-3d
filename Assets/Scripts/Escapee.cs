using UnityEngine;

public class Escapee : MonoBehaviour
{
    // Kecepatan kabur si hewan
    public float moveSpeed = 5f;

    // Berapa lama dia kabur sebelum hilang (misal: 5 detik)
    public float despawnTime = 5.0f;

    void Start()
    {
        // Perintahkan si hewan untuk "menghilang"
        // setelah [despawnTime] detik
        Destroy(gameObject, despawnTime);
    }

    void Update()
    {
        // Bergerak lurus ke depan (sesuai arah dia 'muncul')
        // transform.forward = arah "depan" dari objek ini
        transform.Translate(transform.forward * moveSpeed * Time.deltaTime);
    }
}
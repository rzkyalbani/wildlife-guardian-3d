using UnityEngine;
using UnityEngine.UI; // Wajib untuk RawImage

public class Compass : MonoBehaviour
{
    [Header("Referensi Kompas")]
    public RawImage compassImage; // Slot untuk 'CompassBar' kita
    
    [Header("Target yang Diikuti")]
    public Transform playerTransform; // Slot untuk 'Player'

    void Update()
    {
        // 1. Ambil "kotak" (Rect) dari gambar kompas
        Rect uvRect = compassImage.uvRect;

        // 2. Ubah posisi 'x' (horizontal) dari kotak ini
        //    berdasarkan rotasi Y (kiri-kanan) si Player.
        //    Kita bagi 720f biar nilainya jadi 0 sampai 1
        uvRect.x = playerTransform.eulerAngles.y / 720f;

        // 3. Terapkan "kotak" yang sudah digeser ini kembali ke gambar
        compassImage.uvRect = uvRect;
    }
}
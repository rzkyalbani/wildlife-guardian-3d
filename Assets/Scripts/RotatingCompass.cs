using UnityEngine;
using UnityEngine.UI;

public class RotatingCompass : MonoBehaviour
{
    [Header("Referensi")]
    public Transform playerTransform; // Siapa yang diikuti? (Player)
    public RectTransform compassRect; // Gambar kompasnya (UI)

    void Update()
    {
        if (playerTransform == null || compassRect == null) return;

        // Ambil rotasi Y player (dia hadap mana)
        float playerY = playerTransform.eulerAngles.y;

        // Terapkan ke rotasi Z gambar kompas (putar gambar)
        // Di Unity UI, rotasi Z positif itu berlawanan arah jarum jam.
        // Jadi kalau Player nengok Kanan (90 derajat), Kompas harus muter ke Kiri (biar Utara ada di kiri layar).
        // Maka rumusnya langsung pakai angka positif playerY.
        
        compassRect.rotation = Quaternion.Euler(0, 0, playerY);
    }
}
using UnityEngine;
using UnityEngine.SceneManagement; // WAJIB ADA: Buat pindah scene

public class MainMenuManager : MonoBehaviour
{
    public string sceneGameName = "Demo_scene01"; 

    public void PlayGame()
    {
        // Pindah ke scene game
        SceneManager.LoadScene(sceneGameName);
    }

    public void QuitGame()
    {
        Debug.Log("Keluar Game!");
        Application.Quit();
    }
}
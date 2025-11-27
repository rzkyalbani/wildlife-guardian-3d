using UnityEngine;
using Unity.Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    [Header("Setup Kamera")]
    public CinemachineCamera tppCamera; 
    public CinemachineCamera fppCamera; 

    [Header("Visual Player")]
    public Renderer[] playerBodyParts; 

    [Header("Referensi Player & Kontrol")]
    public PlayerMovement playerMovement; 
    public SimpleMouseLook mouseLookScript; // <-- DRAG SCRIPT SimpleMouseLook KE SINI!

    private bool isFPP = false;

    void Start()
    {
        SwitchToTPP(); // Mulai dari TPP
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (isFPP) SwitchToTPP();
            else SwitchToFPP();
        }
    }

    void SwitchToFPP()
    {
        isFPP = true;
        fppCamera.Priority = 20;
        tppCamera.Priority = 10;
        
        ToggleBody(false);
        
        // Aktifkan Mode FPS
        if(playerMovement) playerMovement.isFPP = true;
        if(mouseLookScript) mouseLookScript.enabled = true; // Nyalakan Mouse Look Manual
    }

    void SwitchToTPP()
    {
        isFPP = false;
        tppCamera.Priority = 20;
        fppCamera.Priority = 10;
        
        ToggleBody(true);

        // Aktifkan Mode TPP
        if(playerMovement) playerMovement.isFPP = false;
        if(mouseLookScript) mouseLookScript.enabled = false; // Matikan Mouse Look Manual
    }

    void ToggleBody(bool isVisible)
    {
        foreach (Renderer part in playerBodyParts)
        {
            if (part != null) part.enabled = isVisible;
        }
    }
}
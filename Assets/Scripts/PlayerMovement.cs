using UnityEngine;
using UnityEngine.UI; // Wajib untuk Slider

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;

    [Header("Referensi Penting")]
    public Transform cam; 
    public Animator animator; 
    
    [Header("Pengaturan Gerak")]
    public float normalSpeed = 5.0f; 
    public float sprintSpeed = 10.0f; 
    public float gravityValue = -9.81f;
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    private Vector3 playerVelocity;
    
    [Header("Inventori (DARI KODEMU)")]
    public bool isHoldingItem = false; 

    // ----- MEKANIKA STAMINA (DIPERBAIKI) -----
    [Header("Pengaturan Stamina")]
    public float maxStamina = 100f; 
    private float currentStamina;
    public float staminaDrainRate = 15f; 
    public float staminaRegenRate = 10f; 
    public Slider staminaBar; 
    // ------------------------------------

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>(); 
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        currentStamina = maxStamina; 
        staminaBar.maxValue = maxStamina; 
        staminaBar.value = currentStamina; 
    }

    void Update()
    {
        // 1. TANGANI GRAVITASI
        bool isGrounded = controller.isGrounded;
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }

        // 2. CEK INPUT
        float horizontalInput = Input.GetAxis("Horizontal"); 
        float verticalInput = Input.GetAxis("Vertical"); 
        Vector3 direction = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        // ----- LOGIKA SPRINT (DIPERBAIKI) -----
        
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && currentStamina > 0;
        
        float currentSpeed;
        if (isSprinting && direction.magnitude >= 0.1f)
        {
            // --- JIKA SEDANG SPRINT ---
            currentSpeed = sprintSpeed;
            currentStamina -= staminaDrainRate * Time.deltaTime; 
        }
        else
        {
            // --- JIKA JALAN BIASA (ATAU DIAM) ---
            currentSpeed = normalSpeed;
            
            // --- INI DIA PERBAIKANNYA ---
            // Cek apakah player SEDANG menahan Shift (tapi stamina 0)
            bool isTryingToSprint = Input.GetKey(KeyCode.LeftShift) && direction.magnitude >= 0.1f;
            
            // JANGAN isi stamina jika player lagi nahan Shift tapi stamina 0
            if (!isTryingToSprint && currentStamina < maxStamina) 
            {
                // Baru isi stamina kalau player MELEPAS tombol Shift
                currentStamina += staminaRegenRate * Time.deltaTime;
            }
            // -----------------------------
        }
        
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        staminaBar.value = currentStamina;
        // --------------------------------

        // 3. LOGIKA ROTASI
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            
            // 4. EKSEKUSI GERAKAN
            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime); 
        }

        // 5. Terapkan gravitasi
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        // 6. KIRIM SINYAL KE ANIMATOR
        animator.SetFloat("Speed", direction.magnitude);
    }
}
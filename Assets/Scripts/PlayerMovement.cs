using UnityEngine;
using UnityEngine.UI; 

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

    [Header("Pengaturan Lompat")]
    public float jumpHeight = 1.5f;

    [Header("Audio Langkah Kaki (BARU)")]
    public float stepIntervalWalk = 0.5f;   // Jarak bunyi pas jalan
    public float stepIntervalSprint = 0.3f; // Jarak bunyi pas lari (lebih cepat)
    private float stepTimer = 0f;

    private float turnSmoothVelocity;
    private Vector3 playerVelocity;
    
    [Header("Inventori")]
    public bool isHoldingItem = false; 

    [Header("Pengaturan Stamina")]
    public float maxStamina = 100f; 
    private float currentStamina;
    public float staminaDrainRate = 15f; 
    public float staminaRegenRate = 10f; 
    public Slider staminaBar; 

    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        if(animator == null) animator = GetComponentInChildren<Animator>(); 
        
        // Setup Kursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Setup Stamina
        currentStamina = maxStamina; 
        if(staminaBar != null)
        {
            staminaBar.maxValue = maxStamina; 
            staminaBar.value = currentStamina; 
        }
    }

    void Update()
    {
        // 1. GRAVITASI & GROUND CHECK
        bool isGrounded = controller.isGrounded;
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f; // Reset vertikal velocity pas napak tanah
        }

        // 2. LOMPAT
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
            
            // Kalau punya animasi lompat:
            // if(animator) animator.SetTrigger("Jump");
        }

        // 3. INPUT GERAK
        float horizontalInput = Input.GetAxis("Horizontal"); 
        float verticalInput = Input.GetAxis("Vertical"); 
        Vector3 direction = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        // 4. LOGIKA SPRINT & STAMINA
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && currentStamina > 0 && direction.magnitude >= 0.1f;
        
        float currentSpeed = normalSpeed;

        if (isSprinting)
        {
            // SPRINTING
            currentSpeed = sprintSpeed;
            currentStamina -= staminaDrainRate * Time.deltaTime; 
        }
        else
        {
            // WALKING / IDLE
            currentSpeed = normalSpeed;
            
            // Cek: Apakah player maksa lari tapi stamina habis?
            bool isTryingToSprint = Input.GetKey(KeyCode.LeftShift) && direction.magnitude >= 0.1f;
            
            // Regen cuma kalau TIDAK nekan Shift
            if (!isTryingToSprint && currentStamina < maxStamina) 
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
            }
        }
        
        // Update UI Stamina
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        if(staminaBar != null) staminaBar.value = currentStamina;

        // 5. EKSEKUSI GERAK & ROTASI
        if (direction.magnitude >= 0.1f)
        {
            // Hitung Rotasi (Ikuti Kamera)
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Hitung Arah Gerak
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            
            // Gerakkan Karakter
            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime); 

            // --- AUDIO LANGKAH KAKI (INTEGRASI) ---
            if (isGrounded)
            {
                // Kurangi timer
                stepTimer -= Time.deltaTime;

                if (stepTimer <= 0)
                {
                    // Bunyikan suara lewat AudioManager
                    if (AudioManager.instance != null && AudioManager.instance.sfxJalan != null)
                    {
                        AudioManager.instance.PlaySFX(AudioManager.instance.sfxJalan);
                    }

                    // Reset timer (pilih interval Lari atau Jalan)
                    stepTimer = isSprinting ? stepIntervalSprint : stepIntervalWalk;
                }
            }
        }
        else
        {
            // Kalau diam, reset timer langkah biar pas jalan langsung bunyi
            stepTimer = 0.1f;
        }

        // 6. TERAPKAN GRAVITASI
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        // 7. UPDATE ANIMATOR
        if(animator != null)
        {
            // Kirim 0 kalau diam, 0.5 kalau jalan, 1 kalau lari
            // (Sesuaikan Blend Tree kamu kalau beda angkanya)
            float animSpeed = 0f;
            if (direction.magnitude >= 0.1f)
            {
                animSpeed = isSprinting ? 1f : 0.5f; 
            }
            
            // Pakai DampTime biar transisi animasinya halus (gak kaku)
            animator.SetFloat("Speed", animSpeed, 0.1f, Time.deltaTime);
        }
    }
}
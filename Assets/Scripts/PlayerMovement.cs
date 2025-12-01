using UnityEngine;
using UnityEngine.UI; 

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;

    [Header("Referensi Penting")]
    public Transform cam; 
    public Animator animator; 
    
    [Header("Status Kamera")]
    public bool isFPP = false; // Dikontrol oleh CameraSwitcher

    [Header("Pengaturan Gerak")]
    public float normalSpeed = 5.0f; 
    public float sprintSpeed = 10.0f; 
    public float gravityValue = -9.81f;
    public float turnSmoothTime = 0.1f;

    [Header("Pengaturan Lompat")]
    public float jumpHeight = 1.5f;

    [Header("Audio Langkah Kaki")]
    public float stepIntervalWalk = 0.5f;   
    public float stepIntervalSprint = 0.3f; 
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
        // Cek Pause & Cek UI
        if (Time.timeScale == 0 || (UIManager.instance && UIManager.instance.isUIActive)) 
        {
            return; // Jangan lakukan apa-apa (Jangan kunci kursor, jangan gerak)
        }

        // 2. AUTO-LOCK KURSOR
        if (Cursor.lockState != CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // 3. GRAVITASI
        bool isGrounded = controller.isGrounded;
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f; 
        }

        // 4. LOMPAT
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
        }

        // 5. INPUT GERAK
        float horizontalInput = Input.GetAxis("Horizontal"); 
        float verticalInput = Input.GetAxis("Vertical"); 
        Vector3 direction = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        // 6. LOGIKA SPRINT & STAMINA
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && currentStamina > 0 && direction.magnitude >= 0.1f;
        float currentSpeed = isSprinting ? sprintSpeed : normalSpeed;

        if (isSprinting) currentStamina -= staminaDrainRate * Time.deltaTime; 
        else if (direction.magnitude < 0.1f || !Input.GetKey(KeyCode.LeftShift)) 
             if(currentStamina < maxStamina) currentStamina += staminaRegenRate * Time.deltaTime;

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        if(staminaBar != null) staminaBar.value = currentStamina;

        // --- HITUNG ARAH GERAK (FIXED) ---
        Vector3 moveDir = Vector3.zero;

        if (isFPP)
        {
            // === LOGIKA FPP (FIXED) ===
            // Kita HAPUS kode rotasi di sini biar gak tabrakan sama SimpleMouseLook.
            // Kita cuma hitung arah jalan berdasarkan arah hadap badan saat ini.
            
            // "transform.forward" otomatis sudah diputar oleh SimpleMouseLook
            Vector3 forwardMove = transform.forward * verticalInput;
            Vector3 rightMove = transform.right * horizontalInput;
            
            moveDir = (forwardMove + rightMove).normalized;
        }
        else
        {
            // === LOGIKA TPP (Tetap Sama) ===
            // Di TPP, script inilah yang memutar badan karakter
            if (direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);

                moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            }
        }

        // 7. EKSEKUSI GERAK
        // Di FPP, kita gerakkan walau direction 0 karena mungkin kita cuma muter badan (tapi velocity 0)
        // Di TPP, kita cuma gerak kalau ada input
        if (direction.magnitude >= 0.1f || (isFPP && moveDir.magnitude >= 0.1f)) 
        {
            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);

            // Audio Langkah
            if (isGrounded && moveDir.magnitude >= 0.1f)
            {
                stepTimer -= Time.deltaTime;
                if (stepTimer <= 0)
                {
                    if (AudioManager.instance) AudioManager.instance.PlaySFX(AudioManager.instance.sfxJalan);
                    stepTimer = isSprinting ? stepIntervalSprint : stepIntervalWalk;
                }
            }
        }
        else
        {
            stepTimer = 0.1f; 
        }

        // 8. TERAPKAN GRAVITASI
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        // 9. UPDATE ANIMATOR
        if(animator != null)
        {
            float animSpeed = 0f;
            // Gunakan input asli untuk animasi, biar pas strafe tetep jalan animasinya
            if (direction.magnitude >= 0.1f) animSpeed = isSprinting ? 1f : 0.5f; 
            animator.SetFloat("Speed", animSpeed, 0.1f, Time.deltaTime);
        }
    }
}
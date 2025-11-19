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

    // --- MEKANIKA LOMPAT ---
    public float jumpHeight = 1.5f; // Tinggi lompatan
    // -----------------------

    private float turnSmoothVelocity;
    private Vector3 playerVelocity;
    
    [Header("Inventori (DARI KODEMU)")]
    public bool isHoldingItem = false; 

    // ----- MEKANIKA STAMINA (FINAL) -----
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
        
        // Cari animator kalau belum di-assign
        if(animator == null) animator = GetComponentInChildren<Animator>(); 
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        currentStamina = maxStamina; 
        if(staminaBar != null)
        {
            staminaBar.maxValue = maxStamina; 
            staminaBar.value = currentStamina; 
        }
    }

    void Update()
    {
        // 1. TANGANI GRAVITASI & GROUND CHECK
        bool isGrounded = controller.isGrounded;
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }

        // --- LOGIKA LOMPAT ---
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // Rumus Fisika untuk Lompat: v = akar(h * -2 * g)
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
            
            // (Opsional) Kalau nanti ada animasi lompat:
            // animator.SetTrigger("Jump"); 
        }
        // ---------------------

        // 2. CEK INPUT GERAK
        float horizontalInput = Input.GetAxis("Horizontal"); 
        float verticalInput = Input.GetAxis("Vertical"); 
        Vector3 direction = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        // ----- LOGIKA SPRINT (FINAL FIX) -----
        
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
            
            // Cek apakah player SEDANG menahan Shift (tapi stamina 0/habis)
            bool isTryingToSprint = Input.GetKey(KeyCode.LeftShift) && direction.magnitude >= 0.1f;
            
            // HANYA isi stamina kalau pemain TIDAK menahan tombol lari
            if (!isTryingToSprint && currentStamina < maxStamina) 
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
            }
        }
        
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        if(staminaBar != null) staminaBar.value = currentStamina;
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

        // 5. TERAPKAN GRAVITASI & LOMPAT
        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        // 6. KIRIM SINYAL KE ANIMATOR
        if(animator != null) animator.SetFloat("Speed", direction.magnitude);
    }
}
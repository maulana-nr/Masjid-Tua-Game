using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Pastikan CharacterController ada di GameObject ini
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    // =======================
    // CAMERA & MOVEMENT
    // =======================

    [Header("Camera")]
    public Camera playerCam;

    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 5f;
    public float jumpPower = 0f;
    public float gravity = 10f;

    [Header("Camera Look Settings")]
    public float lookSpeed = 2f;
    public float lookXLimit = 75f;
    public float cameraRotationSmooth = 5f;

    // Footstep Sounds
    [Header("Footstep Sounds")]
    public AudioClip[] woodFootstepSounds;
    public AudioClip[] tileFootstepSounds;
    public AudioClip[] carpetFootstepSounds;
    public Transform footstepAudioPosition;
    public AudioSource audioSource;

    private bool isWalking = false;
    private bool isFootstepCoroutineRunning = false;
    private AudioClip[] currentFootstepSounds;

    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;
    float rotationY = 0;

    // Camera Zoom
    [Header("Camera Zoom")]
    public int ZoomFOV = 35;
    public int initialFOV = 50;
    public float cameraZoomSmooth = 1f;
    private bool isZoomed = false;
    

    // Movement / Dead state
    private bool canMove = true;
    [HideInInspector] public bool isDead = false;

    CharacterController characterController;

    // =======================
    // HEALTH & UI (dari playerlogic)
    // =======================

    [Header("Health Settings")]
    public float health = 100f;
    public float MaxHealth = 100f;
    public UIGameplayLogic1 UIGameplay;   // drag UI Gameplay Logic di sini kalau ada

    // =======================
    // SPOTLIGHT ATTACK (dari playerlogic)
    // =======================

    // [Header("Spotlight Settings")]
    // public Light spotlight;             // drag Spot Light / flashlight Light ke sini
    // public float spotlightDamage = 5f;
    // public float spotlightRange = 10f;
    // public float damageCooldown = 0.5f;
    // public LayerMask spotlightHitLayers; // layer yang boleh kena sinar (isi Enemy)

    // private float nextDamageTime;

    void Start()
    {
        // CharacterController
        characterController = GetComponent<CharacterController>();

        // Lock & hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Footstep awal = kayu
        currentFootstepSounds = woodFootstepSounds;

        // Simpan FOV awal kamera
        if (playerCam != null)
        {
            if (initialFOV == 0)
                initialFOV = Mathf.RoundToInt(playerCam.fieldOfView);
        }

        // Health & UI
        MaxHealth = health;
        if (UIGameplay != null)
        {
            UIGameplay.UpdateHealthBar(health, MaxHealth);
        }
    }

    void Update()
    {
        if (isDead) return;

        // ================= MOVEMENT =================
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;

        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        // Jump
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        characterController.Move(moveDirection * Time.deltaTime);

        // ================= CAMERA LOOK =================
        if (canMove)
        {
            rotationX -= Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            rotationY += Input.GetAxis("Mouse X") * lookSpeed;

            Quaternion targetRotationX = Quaternion.Euler(rotationX, 0, 0);
            Quaternion targetRotationY = Quaternion.Euler(0, rotationY, 0);

            if (playerCam != null)
            {
                playerCam.transform.localRotation = Quaternion.Slerp(
                    playerCam.transform.localRotation,
                    targetRotationX,
                    Time.deltaTime * cameraRotationSmooth
                );
            }

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotationY,
                Time.deltaTime * cameraRotationSmooth
            );
        }

        // ================= CAMERA ZOOM =================
        if (Input.GetButtonDown("Fire2"))
            isZoomed = true;
        if (Input.GetButtonUp("Fire2"))
            isZoomed = false;

        if (playerCam != null)
        {
            float targetFov = isZoomed ? ZoomFOV : initialFOV;
            playerCam.fieldOfView = Mathf.Lerp(
                playerCam.fieldOfView,
                targetFov,
                Time.deltaTime * cameraZoomSmooth
            );
        }

        // ================= FOOTSTEPS =================
        if ((curSpeedX != 0f || curSpeedY != 0f) && !isWalking && !isFootstepCoroutineRunning)
        {
            isWalking = true;
            StartCoroutine(PlayFootstepSounds(1.3f / (isRunning ? runSpeed : walkSpeed)));
        }
        else if (curSpeedX == 0f && curSpeedY == 0f)
        {
            isWalking = false;
        }
    }
        // ================= SPOTLIGHT DAMAGE =================
    //     CheckSpotlightEnemyHit();
    // }

    // Footstep coroutine
    IEnumerator PlayFootstepSounds(float footstepDelay)
    {
        isFootstepCoroutineRunning = true;

        while (isWalking)
        {
            if (currentFootstepSounds.Length > 0)
            {
                int randomIndex = Random.Range(0, currentFootstepSounds.Length);
                audioSource.transform.position = footstepAudioPosition.position;
                audioSource.clip = currentFootstepSounds[randomIndex];
                audioSource.Play();
                yield return new WaitForSeconds(footstepDelay);
            }
            else
            {
                yield break;
            }
        }

        isFootstepCoroutineRunning = false;
    }

    // Deteksi jenis lantai (kayu, dst)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wood"))
        {
            currentFootstepSounds = woodFootstepSounds;
        }
        // kalau nanti mau tile / carpet, tambahin lagi di sini
    }

    // ================= HEALTH & DAMAGE =================

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        health -= amount;
        if (health < 0) health = 0;

        if (UIGameplay != null)
        {
            UIGameplay.UpdateHealthBar(health, MaxHealth);
        }

        Debug.Log($"Player terkena damage: {amount}, sisa health: {health}");

        if (health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        canMove = false;
        Debug.Log("Player Mati");
        // TODO: panggil GameOver scene / UI di sini kalau sudah siap
        // SceneManager.LoadScene("GameOver");
    }

    // ================= SPOTLIGHT → DAMAGE ENEMY =================

    // void CheckSpotlightEnemyHit()
    // {
    //     if (spotlight == null || !spotlight.enabled) return;
    //     if (playerCam == null) return;

    //     // cek cooldown damage
    //     if (Time.time < nextDamageTime) return;

    //     // Debug ray
    //     Debug.DrawRay(playerCam.transform.position, playerCam.transform.forward * spotlightRange, Color.red);

    //     RaycastHit hit;
    //     if (Physics.Raycast(playerCam.transform.position,
    //                         playerCam.transform.forward,
    //                         out hit,
    //                         spotlightRange,
    //                         spotlightHitLayers))
    //     {
    //         Debug.Log($"Spotlight mengenai: {hit.collider.name}, Tag: {hit.collider.tag}, Layer: {hit.collider.gameObject.layer}");

    //         if (hit.collider.CompareTag("Enemy"))
    //         {
    //             logicenemy enemy = hit.collider.GetComponent<logicenemy>();
    //             if (enemy == null)
    //                 enemy = hit.collider.GetComponentInParent<logicenemy>();

    //             if (enemy != null)
    //             {
    //                 Debug.Log($"Memberikan {spotlightDamage} damage ke {enemy.gameObject.name}");
    //                 enemy.TakeDamage(spotlightDamage);
    //                 nextDamageTime = Time.time + damageCooldown;
    //             }
    //         }
    //     }
    // }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class playerlogic : MonoBehaviour
{
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;
    public float MaxHealth;
    public UIGameplayLogic UIGameplay;

    [HideInInspector]
    public bool isDead = false;
    public float health = 100f;

    [Header("Spotlight Settings")]
    public Light spotlight;
    public float spotlightDamage = 5f;
    public float spotlightRange = 10f;
    public float damageCooldown = 0.5f;
    private float nextDamageTime;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector]
    public bool canMove = true;
    public LayerMask spotlightHitLayers; // pastikan enemy termasuk layer ini

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        MaxHealth = health;
        UIGameplay.UpdateHealthBar(health, MaxHealth);
    }

    void Update()
    {
        if (!isDead)
        {
            HandleMovement();
            CheckSpotlightEnemyHit();
        }
    }

    private void HandleMovement()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runningSpeed : walkingSpeed) * Input.GetAxis("Horizontal") : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
            moveDirection.y = jumpSpeed;
        else
            moveDirection.y = movementDirectionY;

        if (!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;

        characterController.Move(moveDirection * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        health -= amount;

        if (health < 0)
            health = 0;

        UIGameplay.UpdateHealthBar(health, MaxHealth);

        Debug.Log($"Player terkena damage: {amount}, sisa health: {health}");

        if (health <= 0) Die();
    }


    //public void TakeDamage(float amount)
    //{
    //    if (isDead) return;
    //    health -= amount;
    //    Debug.Log($"Player terkena damage: {amount}, sisa health: {health}");
    //    if (health <= 0) Die();
    //}

    private void Die()
    {
        isDead = true;
        canMove = false;
        Debug.Log("Player Mati");
        //SceneManager.LoadScene("GameOver"); 
    }

    void CheckSpotlightEnemyHit()
    {
        if (!spotlight.enabled) return;

        // Debug garis raycast
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * spotlightRange, Color.red);

        // cek cooldown
        if (Time.time < nextDamageTime) return;

        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, spotlightRange, spotlightHitLayers))
        {
            // Debug detail hit
            Debug.Log($"Spotlight mengenai: {hit.collider.name}, Tag: {hit.collider.tag}, Layer: {hit.collider.gameObject.layer}");

            // cek tag enemy
            if (hit.collider.CompareTag("Enemy"))
            {
                // Cek komponen logicenemy di collider root atau child
                logicenemy enemy = hit.collider.GetComponent<logicenemy>();
                if (enemy == null)
                {
                    // cek parent jika collider ada di child
                    enemy = hit.collider.GetComponentInParent<logicenemy>();
                }

                if (enemy != null)
                {
                    Debug.Log($"Memberikan {spotlightDamage} damage ke {enemy.gameObject.name}");
                    enemy.TakeDamage(spotlightDamage);
                    nextDamageTime = Time.time + damageCooldown;
                }
            }
        }
    }
}

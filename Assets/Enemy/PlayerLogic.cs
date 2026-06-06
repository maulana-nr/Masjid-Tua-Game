using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerLogic : MonoBehaviour
{
    [Header("Player Settings")]
    public Transform PlayerOrientation;
    //public CameraLogic camlogic;
    public Animator anim;
    public float walkspeed, runspeed, jumppower, fallspeed, airMultiplier, HitPoints = 100f;

    private Rigidbody rb;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Vector3 jumpDirection;
    bool grounded = true, aerialboost = true;
    public bool isDead = false;
    bool AimMode = false, TPSMode = true;

    public float MaxHealth;

    [Header("SFX")]
    public AudioClip ShootAudio;
    public AudioClip StepAudio;
    public AudioClip DeathAudio;
    AudioSource PlayerAudio;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        PlayerOrientation = this.GetComponent<Transform>();
        rb.isKinematic = false;
        PlayerAudio = this.GetComponent<AudioSource>();
        MaxHealth = HitPoints;
    }

    void Update()
    {
        if (isDead)
        {
            anim.SetBool("IdleShoot", false);
            anim.SetBool("WalkShoot", false);
            anim.SetBool("Run", false);
            anim.SetBool("Walk", false);
            anim.SetBool("Jump", false);
            anim.SetBool("AimMode", false);
            return;
        }

        AimModeAdjuster();
        ShootLogic();
        Jump();

        if (grounded)
        {
            Movement();
        }

        if (Input.GetKey(KeyCode.F))
        {
            PlayerGetHit(100f);
        }
    }


    private void Movement()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        moveDirection = PlayerOrientation.forward * verticalInput + PlayerOrientation.right * horizontalInput;

        if (AimMode)
        {
            rb.AddForce(moveDirection.normalized * walkspeed * 10f, ForceMode.Force);
            return;
        }

        if (grounded && moveDirection != Vector3.zero)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                anim.SetBool("Run", true);
                anim.SetBool("Walk", false);
                rb.AddForce(moveDirection.normalized * runspeed * 10f, ForceMode.Force);
            }
            else
            {
                anim.SetBool("Walk", true);
                anim.SetBool("Run", false);
                rb.AddForce(moveDirection.normalized * walkspeed * 10f, ForceMode.Force);
            }
        }
        else
        {
            anim.SetBool("Run", false);
            anim.SetBool("Walk", false);
        }
    }


    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            jumpDirection = moveDirection.normalized;

            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(transform.up * jumppower, ForceMode.Impulse);

            if (jumpDirection != Vector3.zero)
            {
                rb.AddForce(jumpDirection * runspeed * 5f, ForceMode.Impulse);
            }

            grounded = false;
            anim.SetBool("Jump", true);
        }
        else if (!grounded)
        {
            rb.AddForce(Vector3.down * fallspeed * rb.mass, ForceMode.Force);

            if (aerialboost)
            {
                rb.AddForce(moveDirection.normalized * walkspeed * 10f * airMultiplier, ForceMode.Impulse);
                aerialboost = false;
            }
        }
    }


    public void AimModeAdjuster()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            AimMode = !AimMode;
            TPSMode = !AimMode;

            anim.SetBool("AimMode", AimMode);
            //camlogic.CameraModeChanger(TPSMode, AimMode);
        }
    }


    private void ShootLogic()
    {
        if (!AimMode)
        {
            anim.SetBool("IdleShoot", false);
            anim.SetBool("WalkShoot", false);
            return;
        }

        bool isMoving = moveDirection.sqrMagnitude > 0.0001f;

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (!PlayerAudio.isPlaying)
            {
                PlayerAudio.clip = ShootAudio;
                PlayerAudio.Play();
            }

            if (isMoving)
            {
                anim.SetBool("WalkShoot", true);
                anim.SetBool("IdleShoot", false);
            }
            else
            {
                anim.SetBool("IdleShoot", true);
                anim.SetBool("WalkShoot", false);
            }
        }
        else
        {
            anim.SetBool("WalkShoot", false);
            anim.SetBool("IdleShoot", false);

            anim.SetBool("Walk", isMoving);
            anim.SetBool("Run", false);
        }
    }


    public void StepSound()
    {
        PlayerAudio.clip = StepAudio;
        PlayerAudio.Play();
    }


    public void groundedchanger()
    {
        grounded = true;
        aerialboost = true;
        anim.SetBool("Jump", false);
    }



    // ==========================
    //      DAMAGE SYSTEM
    // ==========================
    public void PlayerGetHit(float damage)
    {
        if (isDead) return;

        anim.SetBool("IdleShoot", false);
        anim.SetBool("WalkShoot", false);

        AimMode = false;
        anim.SetBool("AimMode", false);

        HitPoints -= damage;

        if (HitPoints <= 0f)
        {
            HitPoints = 0f;
            isDead = true;

            anim.SetBool("Walk", false);
            anim.SetBool("Run", false);
            anim.SetBool("IdleShoot", false);
            anim.SetBool("WalkShoot", false);
            anim.SetBool("Jump", false);
            anim.SetBool("AimMode", false);

            anim.SetBool("Death", true);

            rb.velocity = Vector3.zero;
            rb.isKinematic = true;

            StartCoroutine(ResetDeathTrigger());
        }
    }

    // NEW: agar enemy bisa memanggil TakeDamage()
    public void TakeDamage(float damage)
    {
        PlayerGetHit(damage);
    }



    public void PlayDeathSound()
    {
        if (PlayerAudio && DeathAudio)
        {
            PlayerAudio.PlayOneShot(DeathAudio, 4f);
        }
    }


    private IEnumerator ResetDeathTrigger()
    {
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("Death", false);
    }


    public bool GetAimMode()
    {
        return AimMode;
    }

}

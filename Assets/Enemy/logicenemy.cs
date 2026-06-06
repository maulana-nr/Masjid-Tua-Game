using UnityEngine;
using UnityEngine.AI;

public class logicenemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float hitPoints = 50f;
    public float turnSpeed = 15f;
    public Transform target;
    public float chaseRange = 10f;
    public float attackRange = 2f;

    [Header("Attack Settings")]
    public float attackCooldown = 1.5f;
    public float attackDamage = 10f;
    private float nextAttackTime = 0f;

    [Header("Slash Effect")]
    public ParticleSystem SlashEffect;

    [Header("Sounds")]
    public AudioClip idleSound;
    public AudioClip walkingSound;
    public AudioClip chasingSound;
    private AudioSource audioSource;
    private AudioClip lastClip;


    [Header("Spotlight Detection")]
    public bool isInSpotlight = false;
    public float spotlightDamage = 5f; // Damage per frame jika terkena spotlight

    private NavMeshAgent agent;
    private Animator anim;
    private Vector3 startPosition;
    private bool isDead = false;
    public bool isStunned = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent != null)
            agent.stoppingDistance = attackRange;

        anim = GetComponentInChildren<Animator>();
        startPosition = transform.position;

        PlaceEnemyOnGround();
        target = FindObjectOfType<playerlogic>()?.transform;

    audioSource = GetComponent<AudioSource>();
    if (audioSource == null)
        audioSource = gameObject.AddComponent<AudioSource>(); // kalau belum ada

    }

    void Update()
    {
         if (isStunned)
        {
            SetAnimation("idle");
            return; // hentikan logika chase / walk sementara stun
        }

        if (isDead) return;

        // Damage dari spotlight
        if (isInSpotlight)
        {
            TakeDamage(spotlightDamage * Time.deltaTime); // gunakan Time.deltaTime agar tidak langsung habis
        }

        if (target == null)
        {
            target = FindObjectOfType<playerlogic>()?.transform;
            if (target == null)
            {
                SetAnimation("idle");
                return;
            }
        }

        playerlogic player = target?.GetComponent<playerlogic>();
        if (player == null || player.isDead)
        {
            SetAnimation("idle");
            PlaySound(idleSound);
            if (agent != null) agent.isStopped = true;
            SlashEffectToggleOff();
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= attackRange)
        {
            FaceTarget(player.transform.position);
            AttackPlayer(player);
        }
        else if (distanceToPlayer <= chaseRange || isInSpotlight)
        {
            FaceTarget(player.transform.position);
            ChaseTarget(player.transform.position);
            anim.SetBool("attack", false); // hentikan animasi attack
            SlashEffectToggleOff();
        }
        else
        {
            ReturnToStart();
            anim.SetBool("attack", false);
            SlashEffectToggleOff();
        }

        if (agent != null && !agent.pathPending && agent.velocity.magnitude < 0.1f)
            SetAnimation("idle");
            PlaySound(idleSound);
    }
    private void PlaySound(AudioClip clip)
    {
        if (clip == null) return;
        if (audioSource == null) return;

        // Jangan restart jika suara masih sama (biar tidak spam)
        if (audioSource.isPlaying && lastClip == clip) return;

        audioSource.clip = clip;
        audioSource.Play();
        lastClip = clip;
    }


    private void ChaseTarget(Vector3 pos)
    {
        if (agent == null || !agent.isOnNavMesh) return;
        agent.isStopped = false;
        agent.SetDestination(pos);
        SetAnimation("walk");
        PlaySound(chasingSound);
    }

    private void AttackPlayer(playerlogic player)
    {
        if (agent != null && agent.isOnNavMesh) agent.isStopped = true;

        anim.SetBool("attack", true); // pakai boolean

        if (Time.time >= nextAttackTime)
        {
            player.TakeDamage(attackDamage);
            SlashEffectToggleOn();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        hitPoints -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Remaining HP: {hitPoints}");

        if (hitPoints <= 0) Die();
        else anim.SetBool("hit", true);
    }

    public void BeginStun(float duration)
    {
        if (isDead) return;
        isStunned = true;
        if (agent != null && agent.enabled) agent.isStopped = true;
        SetAnimation("idle");
        CancelInvoke(nameof(EndStun));
        Invoke(nameof(EndStun), duration);
    }

    void EndStun()
    {
        isStunned = false;
        if (agent != null && agent.enabled) agent.isStopped = false;
    }


    private void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log($"{gameObject.name} has died.");

        SetAnimation("death");
        if (agent != null) agent.enabled = false;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        SlashEffectToggleOff();
        Destroy(gameObject, 2f);
    }

    private void ReturnToStart()
    {
        if (agent == null || !agent.isOnNavMesh) return;

        agent.isStopped = false;
        agent.SetDestination(startPosition);
        SetAnimation("walk");
        PlaySound(walkingSound);

    }

    private void FaceTarget(Vector3 pos)
    {
        Vector3 dir = (pos - transform.position).normalized;
        if (dir.magnitude > 0)
        {
            Quaternion lookRot = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * turnSpeed);
        }
    }

    private void SlashEffectToggleOn()
    {
        if (SlashEffect != null && !SlashEffect.isPlaying)
            SlashEffect.Play();
    }

    private void SlashEffectToggleOff()
    {
        if (SlashEffect != null && SlashEffect.isPlaying)
            SlashEffect.Stop();
    }

    private void SetAnimation(string state)
    {
        if (anim == null) return;

        anim.SetBool("idle", state == "idle");
        anim.SetBool("walk", state == "walk");
        anim.SetBool("attack", state == "attack");
        anim.SetBool("death", state == "death");
    }

    private void PlaceEnemyOnGround()
    {
        Vector3 spawnPos = transform.position + Vector3.up * 2f;
        RaycastHit hit;

        if (Physics.Raycast(spawnPos, Vector3.down, out hit, 10f))
        {
            if (hit.collider.CompareTag("Ground"))
            {
                transform.position = hit.point;

                if (agent != null)
                {
                    NavMeshHit navHit;
                    if (NavMesh.SamplePosition(transform.position, out navHit, 2f, NavMesh.AllAreas))
                    {
                        agent.Warp(navHit.position);
                        agent.enabled = true;
                    }
                    else
                    {
                        Debug.LogWarning("Enemy spawn terlalu jauh dari NavMesh!");
                        agent.enabled = false;
                    }
                }
            }
            else
            {
                Debug.LogWarning("Raycast mengenai objek bukan ground!");
            }
        }
        else
        {
            Debug.LogWarning("Enemy tidak menemukan ground di bawah spawn point!");
        }
    }

}

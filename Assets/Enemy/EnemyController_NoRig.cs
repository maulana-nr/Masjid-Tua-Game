using UnityEngine;
using UnityEngine.AI;

public class EnemyController_NoRig : MonoBehaviour
{
    public Transform[] waypoints;
    public float idleTime = 2f;
    public float walkSpeed = 2f;
    public float chaseSpeed = 4f;
    public float sightDistance = 10f;

    public AudioClip idleSound;
    public AudioClip walkingSound;
    public AudioClip chasingSound;

    private int currentWaypoint = 0;
    private float idleTimer = 0f;
    private Transform player;
    private AudioSource audioSource;
    private NavMeshAgent agent;

    // Floating Effect
    public float floatAmplitude = 0.2f; 
    public float floatSpeed = 2f;
    private float floatTimer;

    [Header("Attack Settings")]
    public float attackRange = 2f;
    public float attackCooldown = 2f;
    public float attackDamage = 50f;
    private float nextAttackTime = 0f;


    private enum State { Idle, Walk, Chase }
    private State currentState = State.Idle;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        audioSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();

        SetWaypoint();

        
    }

    private void Update()
    {
        FloatingEffect(); // Hantu selalu melayang

        switch (currentState)
        {
            case State.Idle:
                idleTimer += Time.deltaTime;
                PlaySound(idleSound);

                if (idleTimer >= idleTime)
                {
                    NextWaypoint();
                }

                CheckPlayer();
                break;

            case State.Walk:
                idleTimer = 0f;
                PlaySound(walkingSound);

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    currentState = State.Idle;
                }

                CheckPlayer();
                break;

            case State.Chase:
                PlaySound(chasingSound);
                agent.speed = chaseSpeed;
                agent.SetDestination(player.position);

                float distance = Vector3.Distance(transform.position, player.position);

                // Serang jika jarak cukup dekat
                if (distance <= attackRange)
                {
                    agent.isStopped = true;        // berhenti agar tidak muter muter
                    AttackPlayer();                // kasih damage
                }
                else
                {
                    agent.isStopped = false;       // kembali ngejar
                }

                // Jika player kabur / hilang
                if (distance > sightDistance)
                {
                    agent.speed = walkSpeed;
                    currentState = State.Walk;
                }
                break;

                    }
    }

    private void CheckPlayer()
    {
        RaycastHit hit;
        Vector3 dir = (player.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, dir, out hit, sightDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                currentState = State.Chase;
            }
        }
    }

    private void NextWaypoint()
    {
        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        SetWaypoint();
    }

    private void SetWaypoint()
    {
        agent.speed = walkSpeed;
        agent.SetDestination(waypoints[currentWaypoint].position);
        currentState = State.Walk;
    }

    // Gerakan melayang ke atas-bawah
    private void FloatingEffect()
    {
        floatTimer += Time.deltaTime * floatSpeed;
        float verticalOffset = Mathf.Sin(floatTimer) * floatAmplitude;

        Vector3 pos = transform.position;
        pos.y += verticalOffset * Time.deltaTime;
        transform.position = pos;

        // Goyang kiri-kanan halus
        transform.Rotate(0, Mathf.Sin(Time.time * 1.5f) * 0.2f, 0);
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null) return;

        if (!audioSource.isPlaying || audioSource.clip != clip)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    private void AttackPlayer()
    {
        if (Time.time < nextAttackTime) return; // cegah spam attack

        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.TakeDamage(attackDamage);
        }

        nextAttackTime = Time.time + attackCooldown;
    }



    private void OnDrawGizmos()
    {
        if (player == null) return;

        Gizmos.color = currentState == State.Chase ? Color.red : Color.green;
        Gizmos.DrawLine(transform.position, player.position);
    }
}

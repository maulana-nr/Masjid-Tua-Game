using UnityEngine;
using UnityEngine.AI;

public class JinLightTarget : MonoBehaviour
{
    [Header("Stun Settings")]
    public float stunDuration = 1.5f;

    [Header("Death Effect")]
    public GameObject deathEffectPrefab;    // prefab particle asap
    public float fadeDuration = 1.5f;       // durasi jin memudar

    [Header("Visual")]
    public Renderer bodyRenderer;           // drag mesh renderer jin
    public Color hitColor = Color.white;

    [Header("SFX (opsional)")]
    public AudioSource screamSfx;           // audio jeritan jin (boleh kosong)

    private NavMeshAgent agent;
    private Color originalColor;

    private bool isStunned;
    private float stunTimer;
    private bool isDissolving;

    private void Start()
    {
        if (bodyRenderer == null)
            bodyRenderer = GetComponentInChildren<Renderer>();

        if (bodyRenderer != null)
            originalColor = bodyRenderer.material.color;

        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
            agent = GetComponentInParent<NavMeshAgent>();
    }

    private void Update()
    {
        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0f)
            {
                EndStun();
            }
        }
    }

    /// <summary>
    /// Dipanggil dari FlashlightController saat ray senter mengenai jin.
    /// Damage PER DETIK dikirim dari flashlight (sudah dikali Time.deltaTime di sana).
    /// </summary>
    public void ApplyLightDamage(float damage)
    {
        // Kirim damage ke script AI utama (logicenemy)
        logicenemy enemy = GetComponent<logicenemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);   // logicenemy tetap yang pegang HP & anim kematian
            enemy.BeginStun(stunDuration);
        }

        // Efek visual flash
        if (bodyRenderer != null)
        {
            bodyRenderer.material.color = hitColor;
            CancelInvoke(nameof(ResetColor));
            Invoke(nameof(ResetColor), 0.1f);
        }

        // SFX jeritan
        if (screamSfx != null && !screamSfx.isPlaying)
        {
            screamSfx.Play();
        }

        // Stun navmesh
        StartStun();
    }

    void StartStun()
    {
        isStunned = true;
        stunTimer = stunDuration;

        if (agent != null && agent.enabled && agent.isOnNavMesh)
            agent.isStopped = true;   // hentikan gerak NavMeshAgent (musuh diem)
    }

    void EndStun()
    {
        isStunned = false;

        if (agent != null && agent.enabled && agent.isOnNavMesh)
            agent.isStopped = false;  // lanjut jalan/chase lagi
    }


    private void ResetColor()
    {
        if (bodyRenderer != null)
            bodyRenderer.material.color = originalColor;
    }

    /// <summary>
    /// Dipanggil dari logicenemy.Die() untuk memulai efek dissolve + asap.
    /// </summary>
    public void StartDissolve()
    {
        if (isDissolving) return;
        isDissolving = true;
        StartCoroutine(DieRoutine());
    }

    private System.Collections.IEnumerator DieRoutine()
    {
        // Matikan collider supaya tidak nabrak lagi
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // Spawn efek asap / particle
        if (deathEffectPrefab != null)
        {
            GameObject fx = Instantiate(
                deathEffectPrefab,
                transform.position,
                Quaternion.identity
            );
            Destroy(fx, 3f);
        }

        // Fade jin pelan-pelan (butuh material yang bisa transparan)
        if (bodyRenderer != null)
        {
            Material mat = bodyRenderer.material;
            Color startColor = mat.color;
            float t = 0f;

            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                float lerp = t / fadeDuration;

                Color c = startColor;
                c.a = Mathf.Lerp(1f, 0f, lerp);   // alpha dari 1 → 0
                mat.color = c;

                yield return null;
            }
        }

        Destroy(gameObject);
    }
}

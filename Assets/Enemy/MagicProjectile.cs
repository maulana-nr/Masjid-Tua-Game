using UnityEngine;

public class MagicProjectile : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 10f;
    public float lifetime = 3f;
    private Transform target;

    private void Awake()
    {
        Debug.Log("Projectile dibuat: " + gameObject.name);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void Start()
    {
        if (GetComponent<Collider>() == null)
            Debug.LogError("Projectile TIDAK punya collider!");

        if (!GetComponent<Collider>().isTrigger)
            Debug.LogError("Collider projectile HARUS isTrigger = true!");

        if (GetComponent<Rigidbody>() == null)
            Debug.LogError("Projectile TIDAK punya rigidbody!");

        Debug.Log("Target projectile: " + target);
    }

    private void Update()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward * 1f, Color.blue);

        if (target == null)
            Debug.LogWarning("Projectile kehilangan target!");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Projectile bertabrakan dengan: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("PLAYER terkena MAGIC projectile!");
        }

        if (!other.CompareTag("Enemy"))
        {
            Debug.Log("Projectile dihancurkan karena menabrak: " + other.name);
            Destroy(gameObject);
        }
    }

}

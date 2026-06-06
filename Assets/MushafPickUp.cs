using UnityEngine;

public class MushafPickUp : MonoBehaviour
{
    public GameObject handUI;

    private bool inReach = false;

    void Start()
    {
        handUI.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = true;
            handUI.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = false;
            handUI.SetActive(false);
        }
    }

    void Update()
    {
        if (inReach && Input.GetButtonDown("Interact"))
        {
            handUI.SetActive(false);

            // ➤ Tambah progress mushaf
            MushafManager.instance.AddMushaf();

            // ➤ hilangkan mushaf dari dunia
            Destroy(gameObject);
        }
    }
}

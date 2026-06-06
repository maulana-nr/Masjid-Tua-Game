using UnityEngine;

public class UstadInteract : MonoBehaviour
{
    public GameObject interactUI;  
    public GameObject dialoguePanel;
    public UstadDialog ustadDialog;

    private bool playerInRange;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interactUI.SetActive(false);
        }
    }

    private void Update()
    {
        // 🔥 Kalau panel dialog lagi aktif, JANGAN proses E di sini
        if (dialoguePanel != null && dialoguePanel.activeSelf)
            return;

        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            StartDialogue();
        }
    }

    void StartDialogue()
    {
        interactUI.SetActive(false);
        dialoguePanel.SetActive(true);

        FreezePlayer(true);               // 🔥 FREEZE PLAYER
        ustadDialog.StartDialogue();
    }

    public void FreezePlayer(bool freeze)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        var controller = player.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = !freeze;

        var movement = player.GetComponent<PlayerController>();
        if (movement != null) movement.enabled = !freeze;
    }
}

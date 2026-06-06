using UnityEngine;
using TMPro;
using System.Collections;

public class UstadDialog : MonoBehaviour
{
    public TextMeshProUGUI dialogText;
    public string[] sentences;
    public float typingSpeed = 0.03f;

    public AudioSource audioSource;
    public AudioClip typeSound;

    private int index = 0;
    private bool isTyping = false;
    private bool dialogActive = false;

    private UstadInteract interactSystem;

    private void Start()
    {
        interactSystem = FindObjectOfType<UstadInteract>();
    }

    public void StartDialogue()
    {
        index = 0;
        dialogText.text = "";
        dialogActive = true;

        StartCoroutine(TypeSentence(sentences[index]));
    }

    private void Update()
    {
        if (!dialogActive) return;

        // ⬇⬇⬇ GANTI INPUT DI SINI: hanya E atau Space
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogText.text = sentences[index];
                isTyping = false;
            }
            else
            {
                NextSentence();
            }
        }
    }

    IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            dialogText.text += letter;

            if (typeSound != null)
                audioSource.PlayOneShot(typeSound);

            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void NextSentence()
    {
        index++;

        if (index < sentences.Length)
        {
            StartCoroutine(TypeSentence(sentences[index]));
        }
        else
        {
            EndDialogue();
        }
    }

    void EndDialogue()
    {
        dialogActive = false;
        gameObject.SetActive(false);

        interactSystem.FreezePlayer(false);    // 🔥 PLAYER MOVE UNFREEZE
    }
}

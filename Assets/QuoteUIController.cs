using System.Collections;
using UnityEngine;
using TMPro;

public class QuoteUIController : MonoBehaviour
{
    [Header("Fade Settings")]
    public CanvasGroup canvasGroup;
    public float fadeInTime = 1.8f;
    public float holdTime = 3f;
    public float fadeOutTime = 1.5f;

    [Header("UI Text")]
    public TMP_Text quoteText;

    private void Reset()
    {
        // Bantu assign default jika dipasang baru
        canvasGroup = GetComponent<CanvasGroup>();
        quoteText = GetComponentInChildren<TMP_Text>();
    }

    public void ShowQuote(string additionalMessage = "")
    {
        StartCoroutine(ShowQuoteSequence(additionalMessage));
    }

    private IEnumerator ShowQuoteSequence(string additionalMessage)
    {
        // Ambil teks TMP default dari Inspector
        string existingText = quoteText.text;

        // Tampilkan "Cahaya telah padam..." + teks default TMP
        string firstMessage = "Cahaya telah padam...\n\n";
        yield return StartCoroutine(FadeText(firstMessage));

        // Tampilkan quote tambahan jika ada
        if (!string.IsNullOrEmpty(additionalMessage))
        {
            yield return StartCoroutine(FadeText(additionalMessage));
        }
    }

    private IEnumerator FadeText(string message)
    {
        quoteText.text = message;

        // Reset alpha supaya fade terlihat
        canvasGroup.alpha = 0f;

        // Pastikan CanvasGroup aktif
        canvasGroup.gameObject.SetActive(true);
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Fade In
        float t = 0f;
        while (t < fadeInTime)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(t / fadeInTime);
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // Hold
        yield return new WaitForSeconds(holdTime);

        // Fade Out
        t = 0f;
        while (t < fadeOutTime)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = 1f - Mathf.Clamp01(t / fadeOutTime);
            yield return null;
        }
        canvasGroup.alpha = 0f;

        // Optional: sembunyikan canvas setelah fade out
        // canvasGroup.gameObject.SetActive(false);
    }

    public IEnumerator ShowQuoteCoroutine(string additionalMessage = "")
    {
        yield return StartCoroutine(ShowQuoteSequence(additionalMessage));
    }
}

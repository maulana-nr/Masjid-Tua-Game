using UnityEngine;
using TMPro;
using System.Collections;

public class EndingSequence : MonoBehaviour
{
    [Header("Camera")]
    public Camera cinematicCam;
    public Camera playerCam;

    [Header("UI Panels")]
    public CanvasEndingUI canvasEnding;   // drag CanvasEnding (custom component)

    [Header("Settings")]
    public float delayBeforeFade = 4f;      // waktu tunggu setelah adzan
    public float fadeDuration = 2f;         // ke full hitam
    public float creditsFadeDuration = 2f;  // muncul credits
    public float creditsDuration = 8f;      // credits bertahan

    public void StartEnding()
    {
        StartCoroutine(PlayEnding());
    }

    IEnumerator PlayEnding()
    {
        yield return new WaitForSeconds(delayBeforeFade);

        // Switch camera
        if (playerCam != null) playerCam.gameObject.SetActive(false);
        if (cinematicCam != null) cinematicCam.gameObject.SetActive(true);

        // Enable Canvas Ending
        canvasEnding.gameObject.SetActive(true);

        // Fade screen to black
        yield return StartCoroutine(canvasEnding.FadePanel(0, 1, fadeDuration));

        // Show credits
        yield return StartCoroutine(canvasEnding.FadeCredits(0, 1, creditsFadeDuration));

        // Stay showing credits
        yield return new WaitForSeconds(creditsDuration);

        // Fade out credits
        yield return StartCoroutine(canvasEnding.FadeCredits(1, 0, creditsFadeDuration));

        // Optional exit / back to menu
        // UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        // Application.Quit();
    }
}

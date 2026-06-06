using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EndingCreditsUI : MonoBehaviour
{
    public CanvasGroup fadePanel;      // panel hitam
    public CanvasGroup creditsGroup;   // group teks credits
    public float fadeDuration = 2f;
    public float creditsDuration = 6f;

    void Start()
    {
        fadePanel.alpha = 0;
        creditsGroup.alpha = 0;
    }

    public IEnumerator PlayCredits()
    {
        // Fade to black
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(0, 1, t / fadeDuration);
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        // Fade in Credits
        t = 0;
        while (t < 2f)
        {
            t += Time.deltaTime;
            creditsGroup.alpha = Mathf.Lerp(0, 1, t / 2f);
            yield return null;
        }

        // Wait while credits visible
        yield return new WaitForSeconds(creditsDuration);

        // Fade out credits (opsional)
        t = 0;
        while (t < 2f)
        {
            t += Time.deltaTime;
            creditsGroup.alpha = Mathf.Lerp(1, 0, t / 2f);
            yield return null;
        }

        // ⬇ setelah credits selesai (pilih 1)
        // UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        // Application.Quit();
    }
}

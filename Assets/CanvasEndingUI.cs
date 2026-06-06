using UnityEngine;
using System.Collections;

public class CanvasEndingUI : MonoBehaviour
{
    public CanvasGroup fadePanel;
    public CanvasGroup creditsGroup;

    void Awake()
    {
        fadePanel.alpha = 0;
        creditsGroup.alpha = 0;
    }

    public IEnumerator FadePanel(float from, float to, float duration)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            fadePanel.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
    }

    public IEnumerator FadeCredits(float from, float to, float duration)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            creditsGroup.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndingCutscene : MonoBehaviour
{
    public GameObject mushafGlow;
    public GameObject lightBlast;
    public CanvasGroup endingText;
    public CanvasGroup fadeBlack;
    public AudioClip suaraAyatAtauAdzan;
    public string sceneSelanjutnya = "CreditsScene";

    public float lightSpeed = 12f;
    public float delayText = 6f;
    public float fadeOutTime = 3f;

    private AudioSource source;
    private bool started = false;

    void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
        if (mushafGlow) mushafGlow.SetActive(false);
        if (lightBlast) lightBlast.SetActive(false);
        if (endingText) endingText.alpha = 0;
        if (fadeBlack) fadeBlack.alpha = 0;
    }

    public void StartEnding()
    {
        if (started) return;
        started = true;

        // Stop player movement
        PlayerController pl = FindObjectOfType<PlayerController>();
        if (pl != null) pl.enabled = false;

        // Stop enemies
        foreach (var e in FindObjectsOfType<EnemyController_NoRig>())
            Destroy(e.gameObject);

        // Activate visual
        if (mushafGlow) mushafGlow.SetActive(true);
        if (lightBlast) lightBlast.SetActive(true);

        // Play sound
        if (suaraAyatAtauAdzan)
            source.PlayOneShot(suaraAyatAtauAdzan);

        StartCoroutine(ExpandLight());
        Invoke(nameof(ShowText), delayText);
    }

    IEnumerator ExpandLight()
    {
        float scale = 0.1f;
        while (scale < 200f)
        {
            scale += Time.deltaTime * lightSpeed;
            lightBlast.transform.localScale = Vector3.one * scale;
            yield return null;
        }
    }

    void ShowText()
    {
        StartCoroutine(FadeCanvas(endingText, 1, 3f));
        Invoke(nameof(ToBlack), 7f);
    }

    void ToBlack()
    {
        StartCoroutine(FadeCanvas(fadeBlack, 1, fadeOutTime));
        Invoke(nameof(NextScene), fadeOutTime + 1.5f);
    }

    void NextScene()
    {
        SceneManager.LoadScene(sceneSelanjutnya);
    }

    IEnumerator FadeCanvas(CanvasGroup cg, float target, float speed)
    {
        float start = cg.alpha;
        float t = 0;
        while (t < speed)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, target, t / speed);
            yield return null;
        }
    }
}

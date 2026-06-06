using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class MushafManager : MonoBehaviour
{
    public static MushafManager instance;

    [Header("Mushaf Progress")]
    public int collected = 0;
    public int total = 4;

    [Header("UI")]
    public Text mushafTextUI;     // UI Text biasa
    public CanvasGroup uiGroup;   // CanvasGroup untuk fade
    public float uiDisplayTime = 2f;

    [Header("Effects")]
    public AudioClip dingSfx;
    public float screenShakeAmount = 0.08f;
    public float screenShakeDuration = 0.15f;

    private AudioSource audioSource;
    private bool uiVisible = false;
    private float hideTimer = 0;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        uiGroup.alpha = 0; // UI tersembunyi di awal
        UpdateUI();
    }

    private void Update()
    {
        // Tekan TAB untuk memunculkan progress mushaf
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ShowUI();
        }

        // Auto hide UI
        if (uiVisible)
        {
            hideTimer -= Time.deltaTime;
            if (hideTimer <= 0)
                HideUI();
        }
    }

    public void AddMushaf()
    {
        collected++;
        UpdateUI();
        ShowUI(); // tampilkan saat mengambil mushaf

        // bunyi ding
        if (dingSfx != null)
            audioSource.PlayOneShot(dingSfx);

        // scale punch
        StartCoroutine(PunchEffect());

        // screen shake
        StartCoroutine(ScreenShake());

        if (collected >= total)
        {
            Debug.Log("Semua mushaf terkumpul!");

            // Pindah ke scene akhir
            SceneManager.LoadScene("scene_akhir");
        }
    }

    private void UpdateUI()
    {
        mushafTextUI.text = $"{collected}/{total} Mushaf";
    }

    // ===== UI SHOW + FADE IN =====
    public void ShowUI()
    {
        uiVisible = true;
        hideTimer = uiDisplayTime;
        StopAllCoroutines();
        StartCoroutine(FadeCanvas(1f, 0.25f)); // fade-in
    }

    // ===== UI HIDE + FADE OUT =====
    public void HideUI()
    {
        uiVisible = false;
        StopAllCoroutines();
        StartCoroutine(FadeCanvas(0f, 0.4f)); // fade-out
    }

    IEnumerator FadeCanvas(float targetAlpha, float duration)
    {
        float startAlpha = uiGroup.alpha;
        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            uiGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t / duration);
            yield return null;
        }
        uiGroup.alpha = targetAlpha;
    }

    // ===== Scale punch text =====
    IEnumerator PunchEffect()
    {
        RectTransform rect = mushafTextUI.rectTransform;

        rect.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        yield return new WaitForSeconds(0.06f);

        rect.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        yield return new WaitForSeconds(0.06f);

        rect.localScale = new Vector3(1f, 1f, 1f);
    }

    // ===== Screen shake =====
    IEnumerator ScreenShake()
    {
        if (Camera.main == null)
            yield break;

        Vector3 originalPos = Camera.main.transform.localPosition;
        float elapsed = 0;

        while (elapsed < screenShakeDuration)
        {
            elapsed += Time.deltaTime;
            float x = Random.Range(-screenShakeAmount, screenShakeAmount);
            float y = Random.Range(-screenShakeAmount, screenShakeAmount);
            Camera.main.transform.localPosition = originalPos + new Vector3(x, y, 0);
            yield return null;
        }

        Camera.main.transform.localPosition = originalPos;
    }
}

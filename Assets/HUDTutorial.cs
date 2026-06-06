using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDTutorial : MonoBehaviour
{
    [Header("HUD UI")]
    public CanvasGroup hudGroup;
    public Text hudText; // teks manual untuk daftar tombol

    [Header("Reminder UI (1 menit pertama)")]
    public Text reminderText; // Teks: "H = Control Panel"
    public float reminderDuration = 60f; // 1 menit

    [Header("Settings")]
    public float startDuration = 7f;        // lama tampil tutorial awal
    public float fadeInTime = 0.4f;
    public float fadeOutTime = 0.6f;
    public KeyCode showKey = KeyCode.H;

    private Coroutine fadeRoutine;
    private bool isVisible = false; // status HUD ON / OFF

    private void Start()
    {
        hudGroup.alpha = 0;
        isVisible = false;

        // tampilkan tutorial awal otomatis
        ShowHUD(startDuration);

        // reminder awal 1 menit
        if (reminderText != null)
        {
            reminderText.gameObject.SetActive(true);
            Invoke(nameof(HideReminder), reminderDuration);
        }
    }

    private void Update()
    {
        // Toggle HUD jika H ditekan
        if (Input.GetKeyDown(showKey))
        {
            if (isVisible)
                HideHUD();
            else
                ShowHUD(startDuration);
        }
    }

    public void ShowHUD(float visibleTime)
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeCanvas(1f, fadeInTime));
        isVisible = true;

        // auto hide setelah beberapa detik
        CancelInvoke(nameof(HideHUD));
        Invoke(nameof(HideHUD), visibleTime);
    }

    public void HideHUD()
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(FadeCanvas(0f, fadeOutTime));
        isVisible = false;
    }

    IEnumerator FadeCanvas(float targetAlpha, float duration)
    {
        float start = hudGroup.alpha;
        float t = 0;

        while (t < duration)
        {
            t += Time.deltaTime;
            hudGroup.alpha = Mathf.Lerp(start, targetAlpha, t / duration);
            yield return null;
        }

        hudGroup.alpha = targetAlpha;
    }

    // ========== Reminder Text ==========
    void HideReminder()
    {
        if (reminderText != null)
            reminderText.gameObject.SetActive(false);
    }
}

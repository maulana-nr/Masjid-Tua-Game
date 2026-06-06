using UnityEngine;
using TMPro;

public class TitleIntroAnimation : MonoBehaviour
{
    public float duration = 1.2f;
    public Vector3 startOffset = new Vector3(0, 80, 0);

    private TextMeshProUGUI text;
    private Vector3 startPos;
    private Vector3 endPos;
    private Vector3 originalScale;
    private float timer;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();

        endPos = transform.localPosition;
        startPos = endPos + startOffset;
        originalScale = transform.localScale;

        transform.localPosition = startPos;
        transform.localScale = originalScale * 0.8f;
        text.alpha = 0;
    }

    void Update()
    {
        if (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            float smoothT = Mathf.SmoothStep(0, 1, t);

            // Posisi turun
            transform.localPosition = Vector3.Lerp(startPos, endPos, smoothT);

            // Scale bounce tapi mengikuti ukuran asli
            float bounce = 1 + Mathf.Sin(t * Mathf.PI) * 0.08f;
            transform.localScale = Vector3.Lerp(originalScale * 0.8f, originalScale * bounce, smoothT);

            // Fade in
            text.alpha = Mathf.Lerp(0, 1, smoothT);
        }
    }
}

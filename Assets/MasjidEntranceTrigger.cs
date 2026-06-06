using UnityEngine;
using System.Collections;

public class MasjidEntranceTrigger : MonoBehaviour
{
    public GameObject jinChasing;     // Jin yang mengejar
    public Light lightAkhir;          // Lampu glow mushaf

    [Header("Sound Effects")]
    public AudioClip vanishSFX;       // suara jin hilang
    public AudioClip bellSFX;         // suara lonceng
    public float soundVolume = 1f;

    [Header("Camera Shake")]
    public Camera playerCam;
    public float shakeDuration = 0.5f;
    public float shakeStrength = 0.2f;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || triggered) return;
        triggered = true;

        // Hilangkan jin pengejar
        if (jinChasing != null)
            Destroy(jinChasing);

        // Glow mushaf
        if (lightAkhir != null)
            lightAkhir.enabled = true;

        // SFX hilang jin
        if (vanishSFX != null)
            AudioSource.PlayClipAtPoint(vanishSFX, transform.position, soundVolume);

        // SFX lonceng (delay agar masuk setelah vanish SFX)
        if (bellSFX != null)
            StartCoroutine(PlayBellDelayed(0.25f)); // bisa diubah delay-nya

        // Kamera shake
        if (playerCam != null)
            StartCoroutine(CameraShake());
    }

    IEnumerator PlayBellDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioSource.PlayClipAtPoint(bellSFX, transform.position, soundVolume);
    }

    IEnumerator CameraShake()
    {
        Vector3 originalPos = playerCam.transform.localPosition;
        float timer = 0;

        while (timer < shakeDuration)
        {
            timer += Time.deltaTime;
            float offsetX = Random.Range(-1f, 1f) * shakeStrength;
            float offsetY = Random.Range(-1f, 1f) * shakeStrength;

            playerCam.transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);
            yield return null;
        }

        playerCam.transform.localPosition = originalPos;
    }
}

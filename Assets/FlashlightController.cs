using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class FlashlightController : MonoBehaviour
{
    [Header("References")]
    public Light spotlight;          // drag Spot Light (anak dari Flashlight)
    public Animator flashlightAnimator;
    public Camera playerCamera;      // drag Camera (FPS) di sini

    [Header("Intensity Settings")]
    public float normalIntensity = 1.3f;
    public float intenseIntensity = 4f;
    public float normalSpotAngle = 100f;
    public float intenseSpotAngle = 70f;

    [Header("Battery Settings")]
    public float maxBattery = 100f;
    public float currentBattery = 100f;
    public float drainPerSecondNormal = 0.5f;
    public float drainPerSecondIntense = 5f;

    [Header("Sound Effects")]
    public AudioSource intenseSfx;   // AudioSource di Flashlight (Loop ON, PlayOnAwake OFF)

    [Header("Camera Zoom")]
    public float intenseFOV = 50f;
    public float zoomSpeed = 10f;
    private float normalFOV;

    [Header("Attack Settings")]
    public float spotlightDamage = 10f;
    public float spotlightRange = 10f;
    public float damageCooldown = 1f;
    public LayerMask spotlightHitLayers;   // pilih layer Enemy di sini

    [Header("UI Battery")]
    public Image batteryFill;

    [Header("UI Quote & Game Over")]
    public QuoteUIController quoteUI;    // drag QuotePanel yg punya CanvasGroup
    public GameObject gameOverPanel;     // drag panel Game Over
    public float delayBeforeQuote = 1f;  // delay sebelum quote muncul
    private bool quoteTriggered = false; // supaya hanya muncul sekali

    [Header("Audio Horror")]
    public AudioClip horrorClip;
    [Range(0f, 1f)] public float horrorVolume = 0.35f; // atur di Inspector
    private AudioSource horrorAudio;

    [Header("UI Logic")]
    public UIGameplayLogic1 uiLogic;

    private float nextDamageTime;

    private bool isOn = false;
    private bool isIntense = false;

    void Start()
    {
        if (spotlight == null)
            spotlight = GetComponentInChildren<Light>();

        if (playerCamera == null)
            playerCamera = Camera.main;

        if (playerCamera != null)
            normalFOV = playerCamera.fieldOfView;

        TurnOff();

        if (horrorClip != null)
        {
            horrorAudio = gameObject.AddComponent<AudioSource>();
            horrorAudio.clip = horrorClip;
            horrorAudio.spatialBlend = 0f;  // 2D audio
            horrorAudio.volume = 1f;
            horrorAudio.loop = false;
            horrorAudio.playOnAwake = false;
            horrorAudio.mute = false;
        }

        if (gameOverPanel != null)
        {
            CanvasGroup cg = gameOverPanel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.interactable = false;
                cg.blocksRaycasts = false;
            }
            gameOverPanel.SetActive(false);
        }
    }

    void Update()
    {
        HandleToggle();      // Q
        HandleIntensity();   // Klik kiri
        HandleBattery();     
        HandleZoom();        // zoom kamera
        HandleAttackRay();   // damage jin
    }

    // ============== INPUT ==============

    void HandleToggle()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (isOn) TurnOff();
            else if (currentBattery > 0f) TurnOnNormal();
        }
    }

    void HandleIntensity()
    {
        if (!isOn || currentBattery <= 0f) return;

        if (Input.GetMouseButtonDown(0))
        {
            StartIntense();
        }
        if (Input.GetMouseButtonUp(0))
        {
            StopIntense();
        }
    }

    void HandleBattery()
    {
        if (!isOn) return;

        // Kurangi baterai sesuai mode
        float drain = isIntense ? drainPerSecondIntense : drainPerSecondNormal;
        currentBattery -= drain * Time.deltaTime;
        currentBattery = Mathf.Clamp(currentBattery, 0f, maxBattery);

        // Matikan senter kalau baterai habis
        if (currentBattery <= 0f)
        {
            TurnOff();
        }

        // Jika baterai habis, mulai sequence quote + game over
        if (currentBattery <= 0f && !quoteTriggered)
        {
            quoteTriggered = true;
            StartCoroutine(TriggerQuoteSequence());
        }

        //Update UI Battery Bar
        if (batteryFill != null)
        {
            batteryFill.fillAmount = currentBattery / maxBattery;

            // Optional: warnai merah saat low battery
            if (currentBattery <= maxBattery * 0.2f)
                batteryFill.color = Color.red;
            else
                batteryFill.color = Color.yellow;
        }
    }

    void HandleZoom()
    {
        if (playerCamera == null) return;

        float targetFOV = isIntense ? intenseFOV : normalFOV;
        playerCamera.fieldOfView = Mathf.Lerp(
            playerCamera.fieldOfView,
            targetFOV,
            zoomSpeed * Time.deltaTime
        );
    }

    // ============== DAMAGE KE JIN ==============

    void HandleAttackRay()
    {
    // HANYA saat intense
    if (!isOn || !isIntense || currentBattery <= 0f) return;
    if (playerCamera == null) return;

    Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
    RaycastHit hit;

    if (Physics.Raycast(ray, out hit, spotlightRange))
    {
        // Cek apakah yang kena punya JinLightTarget
        JinLightTarget jin = hit.collider.GetComponentInParent<JinLightTarget>();
        if (jin != null)
        {
            // Kirim damage per detik ke jin (JinLightTarget yang akan teruskan ke logicenemy)
            jin.ApplyLightDamage(spotlightDamage * Time.deltaTime);
        }
    }
    }


    // ============== STATE CONTROL ==============

    void TurnOnNormal()
    {
        isOn = true;
        isIntense = false;

        if (spotlight != null)
        {
            spotlight.enabled = true;
            spotlight.intensity = normalIntensity;
            spotlight.spotAngle = normalSpotAngle;
        }

        if (flashlightAnimator != null)
        {
            flashlightAnimator.SetBool("isOn", true);
            flashlightAnimator.SetBool("isIntense", false);
        }
    }

    void TurnOff()
    {
        isOn = false;
        isIntense = false;

        if (spotlight != null)
            spotlight.enabled = false;

        if (flashlightAnimator != null)
        {
            flashlightAnimator.SetBool("isOn", false);
            flashlightAnimator.SetBool("isIntense", false);
        }

        if (intenseSfx != null)
        {
            intenseSfx.Stop();
            intenseSfx.time = 0f;
        }
    }

    void StartIntense()
    {
        if (!isOn) return;

        isIntense = true;

        if (spotlight != null)
        {
            spotlight.intensity = intenseIntensity;
            spotlight.spotAngle = intenseSpotAngle;
        }

        if (flashlightAnimator != null)
            flashlightAnimator.SetBool("isIntense", true);

        if (intenseSfx != null)
        {
            intenseSfx.Stop();
            intenseSfx.time = 0f;
            intenseSfx.Play();
        }
    }

    void StopIntense()
    {
        if (!isOn) return;

        isIntense = false;

        if (spotlight != null)
        {
            spotlight.intensity = normalIntensity;
            spotlight.spotAngle = normalSpotAngle;
        }

        if (flashlightAnimator != null)
            flashlightAnimator.SetBool("isIntense", false);

        if (intenseSfx != null)
        {
            intenseSfx.Stop();
            intenseSfx.time = 0f;
        }
    }

    // bisa dipakai nanti untuk pickup baterai
    public void AddBattery(float amount)
    {
        currentBattery = Mathf.Clamp(currentBattery + amount, 0f, maxBattery);
    }


    //tambahan
    IEnumerator TriggerQuoteSequence()
    {
        TurnOff();

        // Play horror audio
        if (horrorAudio != null && horrorClip != null)
        {
            horrorAudio.volume = horrorVolume;
            horrorAudio.PlayOneShot(horrorClip, horrorVolume);
        }

        yield return new WaitForSecondsRealtime(delayBeforeQuote);

        // Disable player movement
        var pl = FindObjectOfType<playerlogic>();
        if (pl != null) pl.canMove = false;

        // Show quote
        if (quoteUI != null)
        {
            yield return quoteUI.ShowQuoteCoroutine("Dalam gelap, imanmu lah satu-satunya penerang � jangan biarkan ia padam");

            // pastikan quote UI tidak menutup raycast
            CanvasGroup quoteCg = quoteUI.GetComponent<CanvasGroup>();
            if (quoteCg != null) quoteCg.blocksRaycasts = false;
        }

        // Tampilkan Game Over Panel
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            CanvasGroup cg = gameOverPanel.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.interactable = true;
                cg.blocksRaycasts = true;
            }

            // Aktifkan semua tombol
            Button[] buttons = gameOverPanel.GetComponentsInChildren<Button>();
            foreach (var b in buttons)
                b.interactable = true;
        }

        // Panggil UI logic untuk menampilkan teks Game Over
        if (uiLogic != null)
            uiLogic.GameResult(false);
    }

}

using UnityEngine;
using System.Collections;

public class AdzanTrigger : MonoBehaviour
{
    [Header("UI & Kamera")]
    public GameObject interactUI;
    public Camera playerCam;
    public Camera cinematicCam;

    [Header("Audio")]
    public AudioClip adzanClip;
    public AudioSource audioSource;

    [Header("JIN Ending")]
    public GameObject jinAkhir;
    public float burnDuration = 4f;

    [Header("Efek Bakar")]
    public GameObject burnEffectPrefab;
    public AudioClip jinScreamClip;
    public float screamVolume = 1f;

    [Header("Gameplay Canvas (HUD)")]
    public GameObject gameplayCanvas;

    [Header("Credit UI")]
    public GameObject canvasEnding; // Canvas background hitam + tulisan credit

    [Header("Interaksi")]
    public KeyCode interactionKey = KeyCode.E;

    private bool playerInRange = false;
    private bool alreadyPlayed = false;

    void Start()
    {
        if (interactUI != null) interactUI.SetActive(false);
        if (cinematicCam != null) cinematicCam.gameObject.SetActive(false);
        if (jinAkhir != null) jinAkhir.SetActive(false);
        if (canvasEnding != null) canvasEnding.SetActive(false); // pastikan tidak muncul diawal
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !alreadyPlayed)
        {
            playerInRange = true;
            interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            interactUI.SetActive(false);
        }
    }

    void Update()
    {
        if (playerInRange && !alreadyPlayed && Input.GetKeyDown(interactionKey))
        {
            StartCoroutine(PlayAdzanCinematic());
        }
    }

    IEnumerator PlayAdzanCinematic()
    {
        alreadyPlayed = true;
        interactUI.SetActive(false);

        // Matikan HUD
        if (gameplayCanvas != null)
            gameplayCanvas.SetActive(false);

        // Freeze player
        FreezePlayer(true);

        // Ganti kamera
        playerCam.gameObject.SetActive(false);
        cinematicCam.gameObject.SetActive(true);

        // Munculkan Jin
        jinAkhir.SetActive(true);

        // Efek api
        GameObject burnFX = null;
        if (burnEffectPrefab != null)
        {
            burnFX = Instantiate(burnEffectPrefab, jinAkhir.transform.position, Quaternion.identity);
            burnFX.transform.parent = jinAkhir.transform;
        }

        // Suara Jin
        if (jinScreamClip != null)
            audioSource.PlayOneShot(jinScreamClip, screamVolume);

        // Mainkan adzan
        if (adzanClip != null)
            audioSource.PlayOneShot(adzanClip);

        // Fade hilang jin (material transparansi)
        Renderer r = jinAkhir.GetComponentInChildren<Renderer>();
        Color original = r.material.color;
        float timer = 0;

        while (timer < burnDuration)
        {
            timer += Time.deltaTime;
            float fade = 1 - (timer / burnDuration);
            r.material.color = new Color(original.r, original.g, original.b, fade);
            yield return null;
        }

        Destroy(jinAkhir);
        if (burnFX != null) Destroy(burnFX);

        // Tunggu sedikit sebelum credit muncul
        yield return new WaitForSeconds(1f);

        // 🟢 TAMPILKAN CREDIT
        if (canvasEnding != null)
            canvasEnding.SetActive(true);

        // Aktifkan cursor untuk UI Main Menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;


        Debug.Log("ENDING → Credit ditampilkan");
    }

    void FreezePlayer(bool freeze)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        var controller = player.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = !freeze;

        var movement = player.GetComponent<PlayerController>();
        if (movement != null) movement.enabled = !freeze;
    }
}

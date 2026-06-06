using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanturnPickUpF : MonoBehaviour
{
    private GameObject OB;

    [Header("UI & Lanturn")]
    public GameObject handUI;      // ikon tangan / tulisan "Press E"
    public GameObject lanturn;     // tangan + senter yang nempel di player

    private bool inReach;

    // referensi ke script kontrol senter
    private FlashlightController flashlightController;

    void Start()
    {
        OB = this.gameObject;

        handUI.SetActive(false);

        // --- Player BELUM punya senter ---
        if (lanturn != null)
        {
            // cari FlashlightController di dalam LanturnArms / Flashlight
            flashlightController = lanturn.GetComponentInChildren<FlashlightController>(true);

            // sembunyikan dulu tangan + senter
            lanturn.SetActive(false);
        }

        // pastikan script kontrol senter belum aktif
        if (flashlightController != null)
        {
            flashlightController.enabled = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = true;
            handUI.SetActive(true);     // muncul tulisan/ikon E
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Reach"))
        {
            inReach = false;
            handUI.SetActive(false);    // hilang kalau menjauh
        }
    }

    void Update()
    {
        // ====== LOGIKA INTERACT SEBELUM AMBIL SENTER ======
        if (inReach && Input.GetButtonDown("Interact"))
        {
            handUI.SetActive(false);

            // aktifkan tangan + senter di player
            if (lanturn != null)
            {
                lanturn.SetActive(true);
            }

            // aktifkan kontrol senter (Q & klik kiri baru bisa dipakai)
            if (flashlightController != null)
            {
                flashlightController.enabled = true;
            }

            StartCoroutine(end());
        }
    }

    IEnumerator end()
    {
        yield return new WaitForSeconds(0.01f);
        // objek senter di lantai hilang → tidak bisa di-interact lagi
        Destroy(OB);
    }
}

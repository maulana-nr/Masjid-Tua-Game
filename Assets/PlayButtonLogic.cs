using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButtonLogic : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clickSound;

    void Awake()
    {
        // Supaya GameObject ini tetap hidup saat pindah scene
        DontDestroyOnLoad(gameObject);
    }

    public void PlayGame()
    {
        // Putar suara klik
        audioSource.PlayOneShot(clickSound);

        // Pindah ke scene Loading
        SceneManager.LoadScene("Loading");
    }

}

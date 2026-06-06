using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeSettings : MonoBehaviour
{
    public AudioMixer audioMixer;     // Drag AudioMixer di Inspector
    public Slider volumeSlider;       // Drag Slider UI

    void Start()
    {
        // Load volume kalau pernah disimpan
        if (PlayerPrefs.HasKey("MusicVolume"))
        {
            float savedVolume = PlayerPrefs.GetFloat("MusicVolume");
            volumeSlider.value = savedVolume;
            SetVolume(savedVolume);
        }
        else
        {
            volumeSlider.value = 1f;  // default full volume
        }

        // Pasang fungsi saat slider berubah
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float volumeValue)
    {
        // Convert slider (0–1) menjadi Decibel (-80 – 0)
        float dB = Mathf.Log10(volumeValue) * 20;
        audioMixer.SetFloat("MusicVolume", dB);

        // Save biar tidak hilang saat restart game
        PlayerPrefs.SetFloat("MusicVolume", volumeValue);
    }
}

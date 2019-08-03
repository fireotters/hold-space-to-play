using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
    {

    public AudioMixer mixer;
    public AudioSource sfxDemo;

    public void ChangeMusic(float sliderValue) {
        mixer.SetFloat("MusicVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("Music", sliderValue);
    }

    public void ChangeSFX(float sliderValue) {
        mixer.SetFloat("SFXVolume", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("SFX", sliderValue);
        if (!sfxDemo.isPlaying) {
            sfxDemo.Play();
        }
    }
}

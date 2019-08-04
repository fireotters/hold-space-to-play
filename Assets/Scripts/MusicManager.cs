using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
    {

    public AudioMixer mixer;
    public AudioSource sfxDemo, currentMusicPlayer;
    public AudioClip mainMenuMusic, track1, track2, track3;
    private int lastTrackRequested;

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

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeMusicTrack(int index)
    {
        if (index != lastTrackRequested)
        {
            switch (index)
            {
                case 0:
                    currentMusicPlayer.clip = mainMenuMusic;
                    break;

                case 1:
                    currentMusicPlayer.clip = track1;
                    break;

                case 2:
                    currentMusicPlayer.clip = track2;
                    break;

                case 3:
                    currentMusicPlayer.clip = track3;
                    break;
            }
            currentMusicPlayer.Play();
            lastTrackRequested = index;
        }
    }
}

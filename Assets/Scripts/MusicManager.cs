using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
    {

    public AudioMixer mixer;
    public AudioSource sfxDemo, currentMusicPlayer;
    public AudioClip mainMenuMusic, track1, track2, track3;
    private int lastTrackRequested = -1; // When first created, pick the scene's chosen song

    public static MusicManager instance;
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
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void FindAllSfxAndPlayPause(int intent)
    {
        List<GameObject> listOfSfxObjects = new List<GameObject>();
        listOfSfxObjects.AddRange(GameObject.FindGameObjectsWithTag("Key"));
        listOfSfxObjects.AddRange(GameObject.FindGameObjectsWithTag("Lock"));
        listOfSfxObjects.AddRange(GameObject.FindGameObjectsWithTag("Flag"));

        if (intent == 0) // Pause
        {
            foreach (GameObject sfxObject in listOfSfxObjects)
            {
                if (sfxObject.GetComponent<AudioSource>().isPlaying)
                {
                    sfxObject.GetComponent<AudioSource>().Pause();
                }
            }
        }
        if (intent == 1) // Resume
        {
            foreach (GameObject sfxObject in listOfSfxObjects)
            {
                if (!sfxObject.GetComponent<AudioSource>().isPlaying)
                {
                    sfxObject.GetComponent<AudioSource>().UnPause();
                }
            }
        }
    }

    public void ChangeMusicTrack(int index)
    {
        if (index != lastTrackRequested || index == 0)
        {
            currentMusicPlayer.enabled = true;
            if (currentMusicPlayer.isPlaying)
            {
                currentMusicPlayer.Stop();
            }
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

    public void PauseMusic()
    {
        if (currentMusicPlayer.isPlaying)
        {
            currentMusicPlayer.Pause();
        }
    }
    public void ResumeMusic()
    {
        if (!currentMusicPlayer.isPlaying)
        {
            currentMusicPlayer.UnPause();
        }
    }

}

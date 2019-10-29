using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSlider : MonoBehaviour
{
    private MusicManager musicManager;

    void Start()
    {
        musicManager = FindObjectOfType<MusicManager>();
    }

    public void ChangeMusic(float sliderValue)
    {
        musicManager.ChangeMusic(sliderValue);
    }
    public void ChangeSFX(float sliderValue)
    {
        musicManager.ChangeSFX(sliderValue);
    }
}

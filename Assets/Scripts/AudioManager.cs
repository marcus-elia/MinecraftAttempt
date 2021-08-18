using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public bool IS_MUTED; // The intro scene never plays music

    public AudioSource audioSource;
    public AudioClip bellsMajor;
    public AudioClip bellsMinor;

    public Slider audioSlider;

    private bool majorPlayed = false;
    private bool minorPlayed = false;
    private bool play = true;

    // Start is called before the first frame update
    void Start()
    {
        audioSource.loop = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayMajor()
    {
        if(play && !IS_MUTED)
        {
            audioSource.clip = bellsMajor;
            audioSource.Play();
            majorPlayed = true;
        }
        play = !(minorPlayed && majorPlayed);
    }
    public void PlayMinor()
    {
        if(play && !IS_MUTED)
        {
            audioSource.clip = bellsMinor;
            audioSource.Play();
            minorPlayed = true;
        }
        play = !(minorPlayed && majorPlayed);
    }

    public void ResetPlay()
    {
        play = true;
        minorPlayed = false;
        majorPlayed = false;
    }
}

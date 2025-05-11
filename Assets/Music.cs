using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    public static Music Instance;

    public AudioClip[] music;     

    private AudioSource audioSource;
    private int prevIndex = -1;
    private int currentTrackIndex = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.loop = false;

        PlayNextTrack();
    }

    void Update()
    {
        if (!audioSource.isPlaying)
        {
            PlayNextTrack();
        }
    }

    void PlayNextTrack()
    {
        if (music.Length == 0) return;


        currentTrackIndex = Random.Range(0, music.Length);

        if (prevIndex == currentTrackIndex)// dont play the same track and call function again
            return;

        prevIndex = currentTrackIndex;

        audioSource.clip = music[currentTrackIndex];
        audioSource.Play();
    }

    public void SetVolume(float vol)
    {
        if (audioSource != null)
            audioSource.volume = vol;
    }
}

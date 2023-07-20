using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrucibleAudio : MonoBehaviour
{
    /* This script is entirely Jonathan's work.
     * 
     * This script is used to control any audio
     * which is sourced from the crucible
     */
    AudioSource source;

    //Fetching audio source to play sounds
    private void Start()
    {
        source = GetComponent<AudioSource>();
        UpdateAudioLevels();
    }

    /*Updates volume whenever a camera is enabled,
     * for example when a scene is loaded
     */
    private void OnEnable()
    {
        UpdateAudioLevels();
    }

    /*Used to start playing the crucible audio
     * if it is not already playing
     */
    public void CrucibleSmelting()
    {
        if (!source.isPlaying)
            source.Play();
    }

    //Stops playing the crucible audio
    public void CrucibleEnd()
    {
        source.Stop();
    }

    //Updates volume to match the player's settings
    public void UpdateAudioLevels()
    {
        if (source != null)
            source.volume = PlayerPrefs.GetFloat("VolSFX");
        else
        {
            source = GetComponent<AudioSource>();
            source.volume = PlayerPrefs.GetFloat("VolSFX");
        }
    }
}

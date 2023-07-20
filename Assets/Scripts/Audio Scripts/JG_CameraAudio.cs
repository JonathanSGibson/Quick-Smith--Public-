using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAudio : MonoBehaviour
{
    /* This script is entirely Jonathan's work.
     * 
     * This script is used to control any audio
     * which is sourced from the camera,
     * including music and ambient sounds
     * (the sounds themselves were found
     * by others)
     */

    /*Audiosources for both music and ambient sounds
     * They are kept seperate so their volume can be adjusted seperately
    */
    public AudioSource musicSource;
    public AudioSource ambientSource;

    //Audio clips for fail and success music to be set in inspector
    [SerializeField]
    private AudioClip failMusic;
    [SerializeField]
    private AudioClip successMusic;

    /*Updates volume whenever a camera is enabled,
     * for example when a scene is loaded
     */
    private void OnEnable()
    {
        UpdateAudioLevels();
    }

    //Stops the daytime music and plays the game over music
    public void FailMusic()
    {
        musicSource.Stop();
        musicSource.PlayOneShot(failMusic);
    }

    //Stops the daytime music and plays the day complete music
    public void DayEndMusic()
    {
        musicSource.Stop();
        musicSource.PlayOneShot(successMusic);
    }

    //Starts playing the daytime music
    public void DayStartMusic()
    {
        musicSource.Play();
    }

    //Updates volume to match the player's settings
    public void UpdateAudioLevels()
    {
        musicSource.volume = PlayerPrefs.GetFloat("VolMusic");
        ambientSource.volume = PlayerPrefs.GetFloat("VolAmbient");
    }
}

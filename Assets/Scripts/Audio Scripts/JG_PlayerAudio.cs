using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    /* This script is entirely Jonathan's work.
     * 
     * This script is used to control any audio
     * which is sourced from the player,
     * including footsteps and interact sounds
     */

    AudioSource playerSounds;

    //Audio clips to be set in inspector
    [SerializeField]
    private AudioClip footstep;
    [SerializeField]
    private AudioClip footstep2;
    [SerializeField]
    private AudioClip hammer;

    //Bool used to toggle between the two footstep sounds
    bool step = false;

    //Fetching audio source to play sounds
    void Start()
    {
        playerSounds = GetComponent<AudioSource>();
    }

    /*Updates volume whenever a player is enabled,
     * for example when a scene is loaded
     * or a new player is added
     */
    private void OnEnable()
    {
        UpdateAudioLevels();
    }

    /* Plays footstep sound and controls
     * alternation of footsteps
     * 
     * Called by events fired in the player's animations
     */
    void FootDown()
    {
        if (step)
            playerSounds.PlayOneShot(footstep);
        else
            playerSounds.PlayOneShot(footstep2);
        step = !step;
    }

    /*Plays hammering sound
     * 
     * Called by events fired in the player's animations
     */
    void InteractSound()
    {
        playerSounds.PlayOneShot(hammer);
    }

    //Updates volume to match the player's settings
    public void UpdateAudioLevels()
    {
        if (playerSounds != null)
            playerSounds.volume = PlayerPrefs.GetFloat("VolSFX");
        else
        {
            playerSounds = GetComponent<AudioSource>();
            playerSounds.volume = PlayerPrefs.GetFloat("VolSFX");
        }
    }
}

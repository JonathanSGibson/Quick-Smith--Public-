using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerAudio : MonoBehaviour
{
    /* This script is entirely Jonathan's work.
     * 
     * This script is used to control any audio
     * which is sourced from the customer
     */
    AudioSource audio;

    //Audio clips to be set in inspector
    [SerializeField]
    public List<AudioClip> ordering;
    public List<AudioClip> orderTaken;
    public List<AudioClip> wrongOrder;

    //Fetching audio source to play sounds
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    /*Updates volume whenever a camera is enabled,
     * for example when the customer is spawned
     */
    private void OnEnable()
    {
        UpdateAudioLevels();
    }

    //Plays random clip from list of customer ordering sounds
    public void Order()
    {
        audio.PlayOneShot(ordering[Random.Range(0, ordering.Count)]);
    }

    //Plays random clip from list of customer taking their order sounds
    public void TakeOrder()
    {
        audio.PlayOneShot(orderTaken[Random.Range(0, orderTaken.Count)]);
    }

    //Plays random clip from list of customer "wrong order" sounds
    public void WrongOrder()
    {
        audio.PlayOneShot(orderTaken[Random.Range(0, orderTaken.Count)]);
    }

    //Updates volume to match the player's settings
    public void UpdateAudioLevels()
    {
        if (audio != null)
            audio.volume = PlayerPrefs.GetFloat("VolSFX");
        else
        {
            audio = GetComponent<AudioSource>();
            audio.volume = PlayerPrefs.GetFloat("VolSFX");
        }
    }
}

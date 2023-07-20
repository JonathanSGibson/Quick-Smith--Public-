using UnityEngine.Audio;
using System;
using UnityEngine;

public class Audio_Manager : MonoBehaviour
{
    // setting an array for all sound clips. I can then add audio clips in a cohesive list in the inspector 
    public Sound[] sounds;






    private void Awake()
    {
        // this for each chunk will reflect the controls i set in the Sounds script 
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }
    // this chuck handles the playing of a audio clip. This chunk means we can control when to 
    // play a sound via any other script in the game.
    public void Play (string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Play();
    }
}

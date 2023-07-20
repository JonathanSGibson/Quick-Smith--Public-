using UnityEngine.Audio;
using UnityEngine;

// code copied from Brackys
// https://www.youtube.com/watch?v=6OT43pvUyfY&t=425s&ab_channel=Brackeys

[System.Serializable]
public class Sound
{
    // declaring the audio clip input. This will give a list of possable audio clips
    public AudioClip clip;
    // setting the range for volume as a slider.
    [Range(0f,1f)]
    public float volume;
    // same but for pitch
    [Range(.1f, 3f)]
    public float pitch;
    // bool to control an audio clip to loop
    public bool loop;
    // setting the name of clip veriable
    public string name;

    // hiding for a cleaner look in the inspector
    [HideInInspector]
    public AudioSource source;




}
 
    
    







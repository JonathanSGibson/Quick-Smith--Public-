using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Jack's code
public class SoundsSettings : MonoBehaviour
{
    //used to get the sliders for the ui
    public Slider SFXSlider;
    public Slider MusicSlider;
    public Slider AmbientSlider;
    CameraAudio camera;
    CrucibleAudio crucible;
    GameObject[] player;

    private void Awake()
    {
        //get teh sliders in the scene and any audio compoenents that are needed
        camera = Camera.main.GetComponent<CameraAudio>();
        player = GameObject.FindGameObjectsWithTag("Player");
        crucible = GameObject.FindGameObjectWithTag("Crucible").GetComponent<CrucibleAudio>();
        //sets the sliders to display the value currently saved in player prefs
        SFXSlider.value =  PlayerPrefs.GetFloat("VolSFX");
        MusicSlider.value =  PlayerPrefs.GetFloat("VolMusic");
        AmbientSlider.value =  PlayerPrefs.GetFloat("VolAmbient");
    }

    public void UpdateSFXSlider()
    {
        //called by the slider when the value changes
        //updates player prefs to save the new selected volume 
        UpdatePlayerAudio();
        if (crucible != null)
            crucible.UpdateAudioLevels();
        else
        {
            crucible = GameObject.FindGameObjectWithTag("Crucible").GetComponent<CrucibleAudio>();
            if (crucible != null)
                crucible.UpdateAudioLevels();
        }
        PlayerPrefs.SetFloat("VolSFX", SFXSlider.value);
    }

    /// <summary>
    /// This function fetches all players and updates their audio (Made by Jonathan)
    /// </summary>
    public void UpdatePlayerAudio()
    {
        player = GameObject.FindGameObjectsWithTag("Player"); //This is done every time in case new players are added

        if (player != null)
        {
            foreach (GameObject character in player)
            {
                character.GetComponent<PlayerAudio>().UpdateAudioLevels();
            }
        }
    }

    public void UpdateMusicSlider()
    {
        //called by the slider when the value changes
        //updates player prefs to save the new selected volume 
        PlayerPrefs.SetFloat("VolMusic", MusicSlider.value);
        camera.UpdateAudioLevels();
    }
    
    public void UpdateAmbientSlider()
    {
        //called by the slider when the value changes
        //updates player prefs to save the new selected volume 
        PlayerPrefs.SetFloat("VolAmbient", AmbientSlider.value);
        camera.UpdateAudioLevels();
    }
}
//end of Jack's code
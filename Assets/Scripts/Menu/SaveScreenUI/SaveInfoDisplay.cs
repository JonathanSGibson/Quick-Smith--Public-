using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//Jack's code
public class SaveInfoDisplay : MonoBehaviour
{
    public TMP_Text name;
    public TMP_Text day;

    SaveProfile profile;
    SaveManager saveManager;

    private void Awake()
    {
        //get the save manager
        saveManager = GameObject.FindGameObjectWithTag("SaveManager").GetComponent<SaveManager>();
    }

    public void SetInfo(SaveProfile profile_)
    {
        //set the info of the save profile ui
        profile = profile_;
        name.text = profile_.name;
        if(profile_.inLevel) //if part way through a run add day to be displayed
        {
            day.gameObject.SetActive(true);
            day.text = "Day: " + profile_.levelSave.currentDay;
        }
        else //turn off if no currently in a run
        {
            day.gameObject.SetActive(false);
        }
    }

    public void ResetSave()
    {
        //set values to default if save is reset
        if(saveManager == null)
        {
            saveManager = GameObject.FindGameObjectWithTag("SaveManager").GetComponent<SaveManager>();
        }
        if (saveManager != null)
        {
            saveManager.saveData.profiles[profile.SaveID] = new SaveProfile(name.text, profile.SaveID, false);
            SetInfo(saveManager.saveData.profiles[profile.SaveID]);
        }
        else
        {
            Debug.LogError("Could Not find save manager in scene - ResetSave");
        }
    }

    public void SelectSave()
    {
        if (saveManager == null) //find save manager
        {
            saveManager = GameObject.FindGameObjectWithTag("SaveManager").GetComponent<SaveManager>();
        }
        if (saveManager != null)
        {
            //set profile to be selected profile
            saveManager.setProfileNumber(profile.SaveID);
            GameObject.FindGameObjectWithTag("SaveUIManager").GetComponent<SaveScreen>().UpdateCurrentProfileDisplay(); //update name on ui
        }
        else
        {
            Debug.LogError("Could Not find save manager in scene - SelectSave");
        }
    }
}
//end of Jack's code

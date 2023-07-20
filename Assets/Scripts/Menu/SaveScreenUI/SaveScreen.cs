using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
//Jack's code
public class SaveScreen : MonoBehaviour
{

    SaveManager saveManager;

    public GameObject SaveInfoPrefab;
    public GameObject PrefabSpawnParent;

    public GameObject NewSaveButton;

    public TMP_Text currentProfileText;
    // Start is called before the first frame update
    void Start()
    {
        saveManager = SaveManager.Instance;
        //for each profile
        for (int i = 0; i < saveManager.saveData.profiles.Count; i++)
        {
            GameObject temp = Instantiate(SaveInfoPrefab, PrefabSpawnParent.transform); //create a ui element
            temp.GetComponent<SaveInfoDisplay>().SetInfo(saveManager.saveData.profiles[i]); //set the data in it to be the saved data
        }
        //move new save button to end of list of profiles
        NewSaveButton.transform.parent = null;
        NewSaveButton.transform.parent = PrefabSpawnParent.transform;
        UpdateCurrentProfileDisplay(); //update the currently selected profile
    }

    public void newSave(SaveProfile profile_)
    {
        //spawn ui element
        GameObject temp = Instantiate(SaveInfoPrefab, PrefabSpawnParent.transform);
        temp.GetComponent<SaveInfoDisplay>().SetInfo(profile_);//set info to the new profile
        SaveManager.Instance.setProfileNumber(profile_.SaveID); //the profile to be the newly created one
        UpdateCurrentProfileDisplay(); //update the ui display for the current profile
        //set the new save button to be at the end of list of profiles
        NewSaveButton.transform.parent = null;
        NewSaveButton.transform.parent = PrefabSpawnParent.transform;
    }

    public void UpdateCurrentProfileDisplay()
    {
        //update display text
        currentProfileText.text = "Current Profile: " + SaveManager.Instance.getCurrentProfile().name;
    }
}
//end of Jack's code
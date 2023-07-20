using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewSave : MonoBehaviour
{

    public TMP_Text saveName;
    public SaveScreen savescreen;

    public TMP_Text ErrorBox;

    public GameObject profileScreen;

    private IEnumerator ErrorDisplay()
    {
        //turn off error box after 5 seconds
        yield return new WaitForSeconds(5);
        ErrorBox.text = "";
    }
    public void create()
    {
        if (saveName.text != "​") //check that the save name text box is not empty
        {
            SaveManager saveManager = SaveManager.Instance;
            //create new profile
            SaveProfile newProfile = new SaveProfile(saveName.text, saveManager.saveData.profiles.Count, false);
            saveManager.saveData.profiles.Add(newProfile); //add new save profile
            saveManager.Save();
            saveName.text = "​";
            savescreen.newSave(newProfile); //create new profile ui for save select screen
            profileScreen.SetActive(true);
            gameObject.SetActive(false); //turn off new save scene and turn back on save select screen
        }
        else
        {
            //if no name entered fill error box with text and start coroutine
            ErrorBox.text = "Please Enter A Name For The Save";
            StartCoroutine(ErrorDisplay());
        }
    }
}
//End of Jack's code

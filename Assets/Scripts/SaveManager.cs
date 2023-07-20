using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

//Jack's code
[System.Serializable]
public struct LevelSave
{
    public int currentLevel;//if set to -1 it doesnt have a current level to load
    public int currentDay;
    public List<Day> days; //days of the level that have passed
    public float customerIncreaseNumber;  //current increase percentange
    public float timeBetweenCustomersDecreaseNumber;  //current decrease percentage
    public ObjectMultipliers objectMultipliers;
    public int coins;

    public LevelSave(int currentLevel_, int currentDay_, List<Day> days_, float customerIncreaseNumber_, float timeBetweenCustomersDecreaseNumber_, ObjectMultipliers objectMultipliers_, int coins_)
    {
        currentLevel = currentLevel_;
        currentDay = currentDay_;
        days = days_;
        customerIncreaseNumber = customerIncreaseNumber_;
        timeBetweenCustomersDecreaseNumber = timeBetweenCustomersDecreaseNumber_;
        objectMultipliers = objectMultipliers_;
        coins = coins_;
    }
}

//needs other progress data added like progress etc outside of levels
[System.Serializable]
public struct SaveProfile
{
    public string name;
    public bool inLevel; //set to true if was last in a level and run hadnt ended
    public LevelSave levelSave; //any data for the current run if not in run will be empty

    public int SaveID;
    public SaveProfile(string name_, int saveID, bool inLevel_, LevelSave levelSave_ = default)
    {
        name = name_;
        inLevel = inLevel_;
        levelSave = levelSave_;
        SaveID = saveID;
    }
}
[System.Serializable]
//used to allow it to be saved in a json (would not save the list by itself)
public struct SaveData {
    public List<SaveProfile> profiles;
}

public class SaveManager : MonoBehaviour
{
    string Directory = "saveData.json"; //where it should be saved and its name
    public SaveData saveData;
    int currentProfile = 0;

    public static SaveManager Instance { get; private set; }

    private void Start()
    {
        LoadSave(); //load the save data from file
    }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this) //make sure that there is only 1 save mangaer in scene
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        LoadSave(); //reload data incase of any changes
    }

    public void LoadSave()
    {
        string temp;
        if (LoadFromFile(Directory, out temp))
            saveData = JsonUtility.FromJson<SaveData>(temp); //get the save data
        else //if no save data found then create default values
        {
            saveData.profiles.Add(new SaveProfile("Default Save",saveData.profiles.Count, false));
            for (int i = 0; i < saveData.profiles.Count; i++)
            {
                var profile = saveData.profiles[i];
                profile.levelSave.objectMultipliers = new ObjectMultipliers(true);
                saveData.profiles[i] = profile;
            }
            temp = JsonUtility.ToJson(saveData);
            WriteToFile(Directory, temp);
        }
    }

    //Code from: https://github.com/UnityTechnologies/UniteNow20-Persistent-Data/blob/main/FileManager.cs
    bool WriteToFile(string FileName, string FileContents)
    {
        var fullPath = Path.Combine(Application.persistentDataPath, FileName); //create directorry to be saved to

        try
        {
            File.WriteAllText(fullPath, FileContents); //write to file
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write to {fullPath} with exception {e}");
            return false;
        }
    }

    bool LoadFromFile(string a_FileName, out string result)
    {
        var fullPath = Path.Combine(Application.persistentDataPath, a_FileName);//create directorry to be loaded from

        try
        {
            result = File.ReadAllText(fullPath); //return all data found in file
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to read from {fullPath} with exception {e}");
            result = "";
            return false;
        }
    }
    //end of referenced code

    public void Save()
    {
        string temp = JsonUtility.ToJson(saveData); //convert data to json format
        WriteToFile(Directory, temp); //save to file
    }

    public SaveProfile getCurrentProfile()
    {
        return saveData.profiles[currentProfile];
    }

    public void UpdateCurrentProfile(SaveProfile profile)
    {
        saveData.profiles[currentProfile] = profile; //overwrite data in current profile
        Save();
    }

    public void setProfileNumber(int number) {
        if (number < saveData.profiles.Count)//make sure there is a profile avaliable
            currentProfile = number;
        else
            Debug.LogError("trying to update currentProfile to a profile that doesnt exist");
    }
}
//End of Jack's code
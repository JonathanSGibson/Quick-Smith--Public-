using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
//Jack's code

[System.Serializable]
public class StoredData
{
    [Header("Crafting Time is in seconds\nPlease set itemAvaliable to the amount of inputs but leave them as false")]
    public List<Recipe> recipes;  //list of recipes that can be used
    public List<WorktopItemsByType> worktopItems; //list of items that each worktop can accept as being placed on them
}
//each worktoptype in the game
[System.Serializable]
public enum WorktopType
{
    COUNTER,
    CRUCIBLE,
    COMBINER,
    ANVIL,
    WOODPILE,
    STONEPILE,
    COPPEROREPILE,
    TINOREPILE,
    IRONOREPILE,
    GOLDOREPILE,
    MINER,
    BIN,
    ORDERCOUNTER,
    SELLCOUNTER,
    BULKORDERCOUNTER
}
//how the crafting recipe should be made using
[System.Serializable]
public enum InteractionType
{
    PLACE,
    INTERACT
}
[System.Serializable]

public struct Recipe
{
    public InteractionType interactionType; //how hte player needs to interact to make the item
    public WorktopType worktopType; //what worktop the recipe works on
    public float craftingTime; // how long the recipe takes to craft
    public List<int> inputs; //what is needed to make the item
    public List<int> outputs; //what is made
    public bool[] itemAvaliable; //used when checking if the recipe is valid

    public Recipe(InteractionType interactionType_, WorktopType worktopType_, int[] inputs_, int[] outputs_)
    {
        interactionType = interactionType_;
        worktopType = worktopType_;
        inputs = new List<int>();
        outputs = new List<int>();
        

        foreach (int input in inputs_)
        {
            inputs.Add(input);
        }

        foreach (int output in outputs_)
        {
            outputs.Add(output);
        }

        itemAvaliable = new bool[inputs.Count];
        craftingTime = 1.0f;
    }
    public void ResetItems() //used in crafitng
    {
        for (int i = 0; i < itemAvaliable.Length; i++)
        {
            itemAvaliable[i] = false;
        }
    }
}
[System.Serializable]
public struct ItemID //used to store link a prefab to a number
{
    public GameObject item;
    public ItemID(GameObject item_)
    {
        item = item_;
    }
}
[System.Serializable]
public struct WorktopItemsByType
{
    //stores all of the items that a worktop can accept to be placed on it
    public WorktopType type;
    public List<int> itemIDs;

    public WorktopItemsByType(WorktopType worktopType, List<int> itemIDs_)
    {
        type = worktopType;
        itemIDs = itemIDs_;
    }
}
public class GameDataManager : MonoBehaviour
{
    public StoredData storedData;
    public List<ItemID> ids;
    public TextAsset gameDataJSON; //used to link the file to the game data
    string Directory = "Game Data/storedData.json"; //stored in local directory
    private void Start()
    {
        //loads any recipe data
        storedData = JsonUtility.FromJson<StoredData>(gameDataJSON.text);
    }
    //Code from: https://github.com/UnityTechnologies/UniteNow20-Persistent-Data/blob/main/FileManager.cs
    bool WriteToFile(string FileName, string FileContents)
    {
        var fullPath = Path.Combine(Application.dataPath, FileName);

        try
        {
            File.WriteAllText(fullPath, FileContents);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write to {fullPath} with exception {e}");
            return false;
        }
    }
    //end of referenced code

    //used in inspcetor to add new recipes to the json
    public void SaveRecipes()
    {
        string json = JsonUtility.ToJson(storedData);
        WriteToFile(Directory, json);
    }
}
//end of Jack's code
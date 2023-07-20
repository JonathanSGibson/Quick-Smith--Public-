using System.Collections.Generic;
using UnityEngine;

public class Crucible : InteractableObject
{
    //Jack's code
    Recipe workingRecipe;
    public float crucibleCraftingTimeLeft;
    //spots for fule to be put into so that it can smelt ore
    public int fuelStorageSize = 2;
    List<Object> fuel = new List<Object>();
    [Header ("in seconds")]
    public float timePerFuel = 1; //how long a fuel lasts
    float fuelTimeLeft = 0; //how long left on current fuel
    public List<GameObject> fuelPositions;
    int fuelToDeletePos = -1;

    CrucibleAudio crucibleAudio;

    new private void Start()
    {
        base.Start();
        itemProgressBar = gameObject.GetComponentInChildren<ObjectProgressBar>(); //get the progress bar

        crucibleCraftingTimeLeft = -1;
        for (int i = 0; i < fuelStorageSize; i++) //add the fuel spots
        {
            fuel.Add(null);
        }

        if(fuel.Count != fuelPositions.Count)
        {
            Debug.LogError("not enough positions given for fuel storage");
        }
        crucibleAudio = GetComponent<CrucibleAudio>();
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        if (playPhase)//only run if in play phase
        {
            if (FindWorkingRecipe(ref workingRecipe, InteractionType.PLACE))
            {
                if (fuelTimeLeft > 0) //if there is fuel
                {
                    crucibleAudio.CrucibleSmelting(); //Jonathan's code for audio
                    if (crucibleCraftingTimeLeft < 0) //check there is time left on recipe
                        crucibleCraftingTimeLeft = workingRecipe.craftingTime;
                    crucibleCraftingTimeLeft -= Time.deltaTime *objectMultiplier; //remove time from crafting
                    fuelTimeLeft -= Time.deltaTime; //remove time from fuel
                    itemProgressBar.UpdateBar(crucibleCraftingTimeLeft, workingRecipe.craftingTime); //set progress bar
                    itemProgressBar.SetColor(IncreaseColor);
                    lastTimeInteracted = Time.time;
                    if (crucibleCraftingTimeLeft <= 0)
                    {
                        crucibleCraftingTimeLeft = -1; //setting back to default value so that it knows when next recipe starts
                                                       //delete the items used in the recipes
                        itemProgressBar.UpdateBar(1, 1);
                        crucibleAudio.CrucibleEnd();//Jonathan's code for audio
                        for (int i = 0; i < items.Count; i++)
                        {
                            if (items[i] == null)
                                continue;
                            for (int j = 0; j < workingRecipe.inputs.Count; j++)
                            {
                                if (workingRecipe.itemAvaliable[j] && items[i].ID == workingRecipe.inputs[j]) //item used in recipe
                                {
                                    Destroy(items[i].gameObject); //destroy the item
                                    items[i] = null;
                                    workingRecipe.itemAvaliable[j] = false; //set it as used (destroyed)
                                                                            //set its inventory spot as empty
                                    break;
                                }
                            }
                        }
                        for (int i = 0; i < workingRecipe.itemAvaliable.Length; i++)//reset itemAvaliable
                        {
                            workingRecipe.itemAvaliable[i] = true;
                        }

                        foreach (int output in workingRecipe.outputs)
                        {
                            bool spotFound = false;
                            for (int i = 0; i < items.Count; i++)
                            {
                                if (items[i] == null) //spawn in item in a containter spot
                                {
                                    Object temp = Instantiate(GetItemFromID(output), itemPositions[i].transform).GetComponent<Object>();
                                    items[i] = temp;
                                    spotFound = true;
                                    break;
                                }
                            }
                            if (!spotFound) //create a temporary spot if no space
                            {
                                Object temp = Instantiate(GetItemFromID(output), itemPositions[itemPositions.Length - 1].transform).GetComponent<Object>();//if there is no spot put it in hte position of the last one
                                items.Add(temp);
                            }

                        }
                    }
                }
            }
            else { 
                if (crucibleCraftingTimeLeft != -1)
                {
                    crucibleCraftingTimeLeft = -1;//if no working recipe set startTime to null for next recipe (incase item is taken off mid smelt)
                }
                crucibleAudio.CrucibleEnd();//Jonathan's code for audio
            }

            if (fuelTimeLeft <= 0 && hasFuel()) //if fuel has been used 
            {
                if (fuelToDeletePos != -1) //remove a fuel
                {
                    Destroy(fuel[fuelToDeletePos].gameObject);
                    fuel[fuelToDeletePos] = null;
                    fuelToDeletePos = -1;
                }
                for (int i = fuel.Count - 1; i >= 0; i--) //set the position for the next avaliable fuel (if there isnt any set position to -1)
                {
                    if (fuel[i] != null)
                    {
                        fuelToDeletePos = i;
                        fuelTimeLeft = timePerFuel;
                        break;
                    }
                }
            }
        }
        else
            crucibleAudio.CrucibleEnd();//Jonathan's code for audio
    }

    public bool AddFuel(ref Object heldItem) //used ot add fuel to the ship
    {
        if (heldItem != null && heldItem.ID == 1)//wood id
        {
            for (int i = 0; i < fuel.Count; i++)
            {
                if(fuel[i] == null)
                {
                    fuel[i] = heldItem;
                    fuel[i].transform.parent = null;//remove previous parent
                    fuel[i].transform.parent = fuelPositions[i].transform;//set parent to the interactable object
                    fuel[i].transform.SetPositionAndRotation(fuelPositions[i].transform.position, fuelPositions[i].transform.rotation);//move it to the hands position
                    heldItem = null;
                    return true;
                }
            }
        }
        return false;
    }
    public bool hasFuel()
    {
        foreach (object item in fuel)
        {
            if(item != null)
            {
                return true;//if there is fuel it returns true
            }
        }
        return false; //if no fuel then it returns false
    }
    public void fuelClear() //used during reset
    {
        for (int i = 0; i < fuel.Count; i++)
        {
            if(fuel[i] != null) //delete all fuel
            {
                Destroy(fuel[i].gameObject);
                fuel[i] = null;
            }
        }
        //set to defualt values
        fuelTimeLeft = 0;
        fuelToDeletePos = -1;
    }
}
//End of Jack's code
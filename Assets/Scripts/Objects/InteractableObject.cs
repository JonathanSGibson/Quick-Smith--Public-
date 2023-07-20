using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//jack's code
public class InteractableObject : Object
{
    [SerializeField]
    protected bool playPhase = false;//used to tell if in play phase and allow the player to interact and place on the objects
    public GameObject[] itemPositions;
    [Header("used to generate the amount of item spots on an interactable object")]
    public int heldItemAmount = 1;
    public List<Object> items; //the items being held

    public ObjectProgressBar itemProgressBar; //the bar above the worktop to show the progress of a craft on them
    public WorktopType type; //the type of worktop
    public List<Recipe> recipes; //recipes that worktop can use

    protected bool loadedRecipes = false; //used to load the recipes for that worktop only once

    protected ObjectMultiplierStore objectMultipliers; //the multipliers used for the shop
    protected GameDataManager dataManager; //the reference to the game data manager used for finding recipes
    public float objectMultiplier; //the multiplier for this worktop

    protected float recipeCraftingTimeLeft = -1; //how long is left for the current craft. if set to -1 then nothing is being crafted

    public float secondsBeforeProgressDecrease = 5.0f; //how long after the player interacted it has to be before the craft time increases back to full
    public float decreaseAmountPerSecond = 0.25f; //how quickly it increases to full
    public Color IncreaseColor; //the colours to tell the player if the progressbar is increasing or decreasing
    public Color DecreaseColor;

    [SerializeField]
    protected bool Miner; //used to tell if the item is part of the ore miner (minecarts or ore miner)

    protected float lastTimeInteracted; //teh time it was last interacted with

    // Start is called before the first frame update
    protected void Start()
    {
        items = new List<Object>(); //set the list to the default value of empty
        for (int i = 0; i < heldItemAmount; i++)
        {
            items.Add(null); //add the number of spots the item can hold
        }

        //find all required scripts in scene
        dataManager = GameObject.FindGameObjectWithTag("dataManager").GetComponent<GameDataManager>();
        if (!dataManager)
            Debug.LogError("could not find game data mangaer in scene");
        GameObject gameManager = GameObject.FindGameObjectWithTag("GameManager");
        if(gameManager)
            objectMultipliers = gameManager.GetComponent<ObjectMultiplierStore>();
        if (!objectMultipliers)
            Debug.LogError("could not find object multipler store in scene");
        itemProgressBar = gameObject.GetComponentInChildren<ObjectProgressBar>(); //get the progress bar from children

        //highlight code
        materials = new List<Material>();
        foreach (var renderer in renderers)
        {
            //A single child might have mutliple materials on it, so has to find all materials on it
            materials.AddRange(new List<Material>(renderer.materials));
        }

        Miner = type == WorktopType.STONEPILE || type == WorktopType.COPPEROREPILE || type == WorktopType.TINOREPILE || type == WorktopType.IRONOREPILE || type == WorktopType.GOLDOREPILE || type == WorktopType.MINER;
    }

    private void OnEnable()
    {
        Start(); //when enabled run through start code again (mostly used in the tutorial)
    }
    // Update is called once per frame
    protected void Update()
    {
        if (!loadedRecipes)
        {
            //load the recipes for the workstation and update any shop multipliers
            UpdateMultipliers();
            loadedRecipes = true;
            if (dataManager)
            {
                foreach (Recipe recipe in dataManager.storedData.recipes)
                {
                    if (recipe.worktopType == type)
                    {
                        recipes.Add(recipe);
                    }
                }
            }
            else
                Debug.LogError("Could not find Recipe Manager in scene");
            if (!itemProgressBar)
                itemProgressBar = gameObject.GetComponentInChildren<ObjectProgressBar>();
        }

        Recipe workingRecipe = default;
        if (!FindWorkingRecipe(ref workingRecipe, InteractionType.INTERACT)) //if there isnt a working recipe on the worktop 
        {
            recipeCraftingTimeLeft = -1;//reset if the item is picked up
        }

        if(!Miner && recipeCraftingTimeLeft != -1 && lastTimeInteracted + secondsBeforeProgressDecrease <= Time.time) //used to decrease the progress bar if not ineracted
        {
            //does not happen on ore miner as it has to be done in the ore miner script
            if(recipeCraftingTimeLeft < workingRecipe.craftingTime) // if not at the crafting time of the recipe
            {
                recipeCraftingTimeLeft += decreaseAmountPerSecond * Time.deltaTime; //decrease the progress bar and set colour
                itemProgressBar.SetColor(DecreaseColor);
                itemProgressBar.UpdateBar(recipeCraftingTimeLeft, workingRecipe.craftingTime);
            }
            else
            {
                //reset progress bar when on or above crafting time
                recipeCraftingTimeLeft = -1;
                itemProgressBar.UpdateBar(1, 1);
                itemProgressBar.SetColor(IncreaseColor);
            }
        }
    }

    public bool Interact()
    {
        if (playPhase) //only interact if in play phase
        {
            //if of specific type call the override from the child script (player interact called this parent script and wont call the override)
            if (type == WorktopType.MINER)
                return gameObject.GetComponent<OreMiner>().interact();
            else if (type == WorktopType.COMBINER)
                return gameObject.GetComponent<Combiner>().Interact();
            else if (type == WorktopType.ORDERCOUNTER)
                return gameObject.GetComponent<OrderCounter>().Interact();
            else //if not of type above
            {
                Recipe workingRecipe = default;
                if (items.Count <= itemPositions.Length && FindWorkingRecipe(ref workingRecipe, InteractionType.INTERACT))//will not let you craft while there are more then the amount of assigned slots used
                {
                    if (recipeCraftingTimeLeft < 0) //if at 0 set back to full
                    {
                        recipeCraftingTimeLeft = workingRecipe.craftingTime;
                    }
                    recipeCraftingTimeLeft -= Time.deltaTime * objectMultiplier; //remove time from crafting time
                    itemProgressBar.UpdateBar(recipeCraftingTimeLeft, workingRecipe.craftingTime); //update the progress bar
                    itemProgressBar.SetColor(IncreaseColor);
                    lastTimeInteracted = Time.time;//set for checking if the bar should decrease
                    if (recipeCraftingTimeLeft <= 0)
                    {
                        recipeCraftingTimeLeft = -1; //setting back to default value so that it knows when next recipe starts                       
                        itemProgressBar.UpdateBar(1, 1);
                        //delete the items used in the recipes
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

                        foreach (int output in workingRecipe.outputs) //spawn the output items
                        {
                            bool spotFound = false;
                            for (int i = 0; i < items.Count; i++) //for each spot on the counter
                            {
                                if (items[i] == null) //if spot is empty spawn the item
                                {
                                    Object temp = Instantiate(GetItemFromID(output), itemPositions[i].transform).GetComponent<Object>();
                                    items[i] = temp;
                                    spotFound = true;
                                    break;
                                }
                            }
                            if (!spotFound) //if no spot was found create a temporart spot and rotate the item to stand out from other items on the worktop
                            {
                                Object temp = Instantiate(GetItemFromID(output), itemPositions[itemPositions.Length - 1].transform).GetComponent<Object>();//if there is no spot put it in hte position of the last one
                                temp.transform.SetPositionAndRotation(temp.transform.position, Quaternion.Euler(temp.transform.rotation.eulerAngles + new Vector3(0, Random.Range(10, 350), 0)));
                                items.Add(temp);
                            }

                        }
                    }
                    return true;
                }
                else
                {
                    if (recipeCraftingTimeLeft != -1)
                    {
                        recipeCraftingTimeLeft = -1;//if no working recipe set startTime to null for next recipe (incase item is taken off mid smelt)
                        itemProgressBar.UpdateBar(1, 1);
                    }
                    return false;
                }
            }

        }
        return false;
    }

    public Object PickUpPlace(Object heldItem)
    {
        if (playPhase)
        {
            if (type == WorktopType.CRUCIBLE)
            {
                bool fuelAdded = false;
                fuelAdded = gameObject.GetComponent<Crucible>().AddFuel(ref heldItem); //checks if the item is a valid fuel for the crucible
                if (fuelAdded) //if fuel is added pickUpPlace is done so rest doesnt need to happen
                    return heldItem;

            }
            if (heldItem == null && !CheckIfAnEmptySlot()) //checks if there is an item on the object
            {
                return PickUp(heldItem);
            }
            if (heldItem != null && CheckIfItemIsPlaceable(heldItem))
            {
                if (CheckIfSpace()) //checks if there is a space for it to be placed into
                {
                    return Place(heldItem);
                }
                else if (!CheckIfAnEmptySlot()) //if no empty spot swap with one in hand
                {
                    return SwapItem(heldItem);
                }
            }
        }
        return heldItem;
    }


    private Object PickUp(Object heldItem)
    {
        for (int i = items.Count - 1; i >= 0; i--)//work backwards through the items on the object
        {
            if (items[i] != null) //if there is an item in that spot
            {//move it from the object to the players hand
                heldItem = items[i];
                items[i] = null;
                break;
            }
        }
        CheckStorageSize();
        if (itemProgressBar != null)
            itemProgressBar.UpdateBar(1, 1);
        //FindRecipies();
        return heldItem;
    }
    private Object Place(Object heldItem)
    {
        for (int i = 0; i < items.Count; i++)// for each item
        {
            if (items[i] == null)//if item slot if empty
            {
                items[i] = heldItem; //put held item in slot
                heldItem.transform.parent = null;//remove previous parent
                heldItem.transform.parent = itemPositions[i].transform;//set parent to the interactable object
                heldItem.transform.SetPositionAndRotation(itemPositions[i].transform.position, itemPositions[i].transform.rotation);//move it to the hands position
                //Jonathan's code
                if (type == WorktopType.SELLCOUNTER)
                {
                    gameObject.GetComponent<SellCounter>().CheckSellingItems(heldItem);
                }
                else if (type == WorktopType.BULKORDERCOUNTER)
                {
                    gameObject.GetComponent<BulkOrderCounter>().CheckItem(heldItem);
                }
                //end of jonathan's code
                if (itemProgressBar != null) //reset progress bar as new item was added
                    itemProgressBar.UpdateBar(1, 1);
                return null;
            }
        }
        return heldItem;
    }
    private Object SwapItem(Object heldItem)
    {
        int i = itemPositions.Length - 1;
        Object temp = items[i];
        //move new item into position before storing it
        heldItem.transform.parent = null;//remove previous parent
        heldItem.transform.parent = itemPositions[i].transform;//set parent to the players hand
        heldItem.transform.SetPositionAndRotation(itemPositions[i].transform.position, itemPositions[i].transform.rotation);//move it to the hands position
        items[i] = heldItem;
        if (itemProgressBar != null)
            itemProgressBar.UpdateBar(1, 1); //reset progress bar as new item was added
        return temp;
    }

    private bool CheckIfItemIsPlaceable(Object heldItem) //checks to see if the item id matches one that is valid for the worktop
    {
        Object heldObject = heldItem.GetComponent<Object>();
        if (heldObject)
        {
            int ItemID = heldObject.GetID();
            List<int> validItems = new List<int>();
            foreach (WorktopItemsByType worktopType in dataManager.storedData.worktopItems)
            {
                if (worktopType.type == type)
                {
                    validItems = worktopType.itemIDs;
                    break;
                }
            }
            foreach (int ID in validItems)
            {
                if (ID == ItemID)
                    return true;
            }
            return false;
        }
        Debug.Log("could not get Object component on item");
        return false;
    }

    //needs reworking
    private bool CheckIfAnEmptySlot()
    {
        for (int i = items.Count - 1; i >= 0; i--)
        {
            if (items[i] != null)//if slot has something in it if so it is not empty
                return false;
        }
        return true;
    }

    private bool CheckIfSpace()
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == null)
                return true; //check if there is a spot on the counter
        }
        return false;
    }

    /// <summary>
    /// checks if there are more then the desired amount of items on an object. if there are empty spots in the list, it is shortened to the max storage amount from placing items (allows for overflow from crafting to fit)
    /// </summary>
    private void CheckStorageSize()
    {
        if (items.Count > itemPositions.Length)
        {
            List<Object> tempItemStorage = new List<Object>();
            foreach (Object item in items)
            {
                if (item != null)
                    tempItemStorage.Add(item);
            }
            if (tempItemStorage.Count < itemPositions.Length)
            {
                int missingAmount = itemPositions.Length - tempItemStorage.Count;
                for (int i = 0; i < missingAmount; i++)
                {
                    tempItemStorage.Add(null);
                }
            }
            items = new List<Object>(itemPositions.Length);
            foreach (Object item in tempItemStorage)
            {
                items.Add(item);
            }
        }
    }

    public GameObject GetItemFromID(int itemID) //get the prefab from an id (used in spawning output items)
    {
        for (int i = 0; i < dataManager.ids.Count; i++)
        {
            if (i == itemID)
            {
                return dataManager.ids[i].item;
            }
        }
        Debug.LogError("Trying to find an object for an id that doesnt exist\nthe id that was searched for was " + itemID);
        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="workingRecipe">used to store the found working recipe</param>
    /// <returns></returns>
    public bool FindWorkingRecipe(ref Recipe workingRecipe, InteractionType type)
    {
        ResetRecipes();
        foreach (Recipe recipe in recipes) // for each recipe 
        {
            if (recipe.interactionType != type) //if recipe is not of correct type skip to next recipe 
                continue;
            for (int i = 0; i < items.Count; i++)    //for each item on the object
            {
                if (items[i] == null) //if no item is on the object skip
                    continue;
                for (int j = 0; j < recipe.inputs.Count; j++) //for each item used in the recipe
                {
                    if (recipe.itemAvaliable[j] && items[i].ID == recipe.inputs[j]) //if the item is used in the recipe and has not aleady been used
                    {
                        recipe.itemAvaliable[j] = false;//set the item as used so it cannot be used again
                    }
                }
            }
            bool allItemsFound = true;
            foreach (bool avaliableItem in recipe.itemAvaliable) //go through all of the item avaliable bools and if one has not been used discard the recipe
            {
                if (avaliableItem)
                {
                    allItemsFound = false;
                    break;
                }
            }
            ResetRecipes();
            if (allItemsFound)//if the recipe works then set as the working recipe (to be returned to calling function)
            {
                workingRecipe = recipe;
                return true;
            }
        }

        return false;
    }
    /// <summary>
    /// resets all  of the item avaliable of the recipes to true to be used in the next search
    /// </summary>
    protected void ResetRecipes()
    {
        foreach (Recipe recipe in recipes)
        {
            for (int i = 0; i < recipe.itemAvaliable.Length; i++)
            {
                recipe.itemAvaliable[i] = true;
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dayEnd">used to say if it is the end of the day (if true will set object to prepPhase)</param>
    public void Clear(bool dayEnd)
    {
        //destroy all items in item slots
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] != null)
            {
                Destroy(items[i].gameObject);
                items[i] = null;
            }
        }
        ResetRecipes();
        recipeCraftingTimeLeft = -1;
        //set back to default
        if (itemProgressBar != null)
        {
            itemProgressBar.UpdateBar(1, 1); //turn off any partial progress bar
        }
        if (type == WorktopType.CRUCIBLE)
        {
            gameObject.GetComponent<Crucible>().fuelClear(); //clear the fuel in the crucible
        }
        if(type == WorktopType.COMBINER)
        {
            gameObject.GetComponent<Combiner>().ResetCombinerController(); //reset values in hte combiner controller
        }
        if (dayEnd)
        {
            playPhase = false;
        }
    }

    public void StartDay()
    {
        playPhase = true;
    }

    public void UpdateMultipliers()
    {
        //go through multiplers and update the multiplier for hte worktop
        if (objectMultipliers != null)
        {
            switch (type)
            {
                case WorktopType.COUNTER:
                    objectMultiplier = objectMultipliers.multipliers.counterMultipler;
                    break;
                case WorktopType.CRUCIBLE:
                    objectMultiplier = objectMultipliers.multipliers.crucibleMultipler;
                    break;
                case WorktopType.COMBINER:
                    objectMultiplier = objectMultipliers.multipliers.combinerMultipler;
                    break;
                case WorktopType.ANVIL:
                    objectMultiplier = objectMultipliers.multipliers.anvilMultipler;
                    break;
                case WorktopType.MINER:
                    objectMultiplier = objectMultipliers.multipliers.oreMinerMultipler;
                    break;

                case WorktopType.STONEPILE:
                    heldItemAmount = objectMultipliers.multipliers.stoneBarrelStorage;
                    if (heldItemAmount > items.Count)
                    {
                        for (int i = 0; i < heldItemAmount - items.Count; i++)
                        {
                            items.Add(null);
                        }
                    }
                    break;
                case WorktopType.COPPEROREPILE:
                    heldItemAmount = objectMultipliers.multipliers.copperBarrelStorage;
                    if (heldItemAmount > items.Count)
                    {
                        for (int i = 0; i < heldItemAmount - items.Count; i++)
                        {
                            items.Add(null);
                        }
                    }
                    break;
                case WorktopType.TINOREPILE:
                    heldItemAmount = objectMultipliers.multipliers.tinBarrelStorage;
                    if (heldItemAmount > items.Count)
                    {
                        for (int i = 0; i < heldItemAmount - items.Count; i++)
                        {
                            items.Add(null);
                        }
                    }
                    break;
                case WorktopType.IRONOREPILE:
                    heldItemAmount = objectMultipliers.multipliers.ironBarrelStorage;
                    if (heldItemAmount > items.Count)
                    {
                        for (int i = 0; i < heldItemAmount - items.Count; i++)
                        {
                            items.Add(null);
                        }
                    }
                    break;
                case WorktopType.GOLDOREPILE:
                    heldItemAmount = objectMultipliers.multipliers.goldBarrelStorage;
                    if (heldItemAmount > items.Count)
                    {
                        for (int i = 0; i < heldItemAmount - items.Count; i++)
                        {
                            items.Add(null);
                        }
                    }
                    break;
            }
        }
        else
            objectMultiplier = 1;
    }

    //object highlighting code
    //code from https://www.sunnyvalleystudio.com/blog/unity-3d-selection-highlight-using-emission
    [SerializeField]
    private List<Renderer> renderers;
    [SerializeField]
    private Color colour = Color.white;

    //store all the materials on the object
    //is filled in start
    private List<Material> materials;

    public void SetHighlight(bool hightlight)
    {
        if (hightlight)
        {
            foreach (Material material in materials)
            {
                //turn on emmision and set colour
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", colour);
                material.SetTexture("_EmissionMap", null);
            }
        }
        else if (!hightlight)
        {
            foreach (Material material in materials)
            {
                material.DisableKeyword("_EMISSION");
            }
        }
    }
    //end of cited code

}
//end of Jack's code


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulkOrderCounter : InteractableObject
{
    /* This script is primarily Jonathan's work,
     * with additions to handle visuals from Jason
     * 
     * This script controls the bulk order system and counter
     */


    public BonusOrderVisuals cartEmpty; //Jason's script to control visuals

    //managers
    GameManager gameManager;
    GameDataManager gameData;

    //today's order
    List<int> components; //The IDs of the COMPONENTS of the order
    int order; //The ID of the COMPLETE ITEM of the order
    int numberGiven; //The number of the order the player has given

    Day currentDay; //Information for the current day

    /* Dictionary for storing the UI sprites for every possible component for the bulk order to generate
     * The key should be set as the ID of the component
     * The sprite should be an image of the component
     */
    [System.Serializable]
    public class SpriteDictionaryPair
    {
        public int key;
        public Sprite value;
    }

    public GameObject orderCanvas; //The canvas which displays the order
    public Image componentOne; //The UI component of the first half of their order
    public Image componentTwo;//The UI component for the second half of their order [NOTE: THIS WILL NEED TO BE EXPANDED IF 3 COMPONENT ITEMS ARE ADDED]
    Camera mainCam;

    [Header("Make the int value (Key) the ID of the item, and the sprite the image of that item")]
    public List<SpriteDictionaryPair> allComponentImages; //List of every possible component's ID and their image (This is used to display them on the inspector and load them into a dictionary)
    Dictionary<int, Sprite> allComponents = new Dictionary<int, Sprite>(); //Actual dictionary of each image and their ID

    /// <summary>
    /// Fetches game manager and game data for relevent information
    /// </summary>
    new void Start()
    {
        base.Start();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        gameData = GameObject.FindGameObjectWithTag("dataManager").GetComponent<GameDataManager>();
    }

    /// <summary>
    /// Loads every sprite image from the list into the actual dictionary and gets the main camera
    /// </summary>
    private void Awake()
    {
        foreach (SpriteDictionaryPair pair in allComponentImages)
        {
            allComponents[pair.key] = pair.value;
        }
        mainCam = Camera.main;
        componentOne.gameObject.SetActive(false);
        componentTwo.gameObject.SetActive(false);
    }
    /// <summary>
    /// Makes the UI face the camera [NOTE: THIS SHOULD BE MOVED OUT OF UPDATE FOR PERFORMANCE REASONS]
    /// </summary>
    new void Update()
    {
        base.Update();
        orderCanvas.transform.LookAt(mainCam.transform);
        orderCanvas.transform.up = mainCam.transform.up;
    }
    /// <summary>
    /// Generates and starts displaying the order for the bulk order
    /// </summary>
    public void GenerateOrder()
    {
        currentDay = gameManager.GetCurrentDay();

        //Creates a 2D array containing every valid recipe (first index is based on result type (eg sword), second is the specific recipe (eg wooden handle + stone blade))
        List<List<Recipe>> recipeList = new List<List<Recipe>>();
        for (int i = 0; i < currentDay.completeItemIDs.Count; i++)
        {
            for (int n = 0; n < gameData.storedData.recipes.Count; n++)
            {
                //for each recipe check if the output matches
                if (gameData.storedData.recipes[n].outputs[0] == currentDay.completeItemIDs[i])
                {
                    //then check if the inputs are valid
                    bool allValid = true;
                    foreach (int ID in gameData.storedData.recipes[n].inputs)
                    {
                        bool componentValid = false;
                        foreach (int compID in currentDay.componentIDs)
                        {
                            if (compID == ID)
                            {
                                componentValid = true;
                                break;
                            }
                        }
                        if (!componentValid)
                        {
                            allValid = false;
                            break;
                        }
                    }
                    if (allValid)
                    {
                        while (recipeList.Count <= i)
                            recipeList.Add(new List<Recipe>());
                        recipeList[i].Add(gameData.storedData.recipes[n]);
                    }
                }
            }
        }
        //Generates a random order from the created list of valid orders
        order = Random.Range(0, recipeList.Count);
        components = recipeList[order][Random.Range(0, recipeList[order].Count)].inputs;
        order = recipeList[order][0].outputs[0];
        numberGiven = 0;

        //Set and activate visuals
        componentOne.gameObject.SetActive(true);
        componentTwo.gameObject.SetActive(true);

        if (allComponents[components[0]] != null)
            componentOne.sprite = allComponents[components[0]];
        else
            Debug.LogError("Image for component not found. Please check the customer prefab");

        if (allComponents[components[1]] != null)
            componentTwo.sprite = allComponents[components[1]];
        else
            Debug.LogError("Image for component not found. Please check the customer prefab");
    }

    /// <summary>
    /// Checks the placed item matches the order, and adds to the amount to be given at the end of the day if correct
    /// </summary>
    /// <param name="item">The item to check</param>
    new public void CheckItem(Object item)
    {
        bool valid = false;
        //Checks that the item matches the order
        if (item.GetID() == order)
        {
            //Loops through each components of each item and checks they match
            valid = true;
            foreach (int component in components)
            {
                bool thisOneValid = false;

                //THIS WHOLE SECTION IS SWORD SPECIFIC AND NEEDS TO BE REPLACED TO BE MORE GENERIC
                List<int> swordComponents = item.GetComponent<Sword>().components;

                foreach(int swordID in swordComponents)
                {
                    if (swordID == component)
                    {
                        thisOneValid = true;
                        break;
                    }
                }
                //END OF SWORD SPECIFIC SECTION
                if (!thisOneValid)
                {
                    valid = false;
                    break;
                }
            }
        }
        //If the item matches then delete it, update the visuals (using Jason's script) and increase numbergiven
        if (valid)
        {
            numberGiven++;
            cartEmpty.CrateStatus();
            if (items[0] != null)
            {
                Destroy(items[0].gameObject);
                items[0] = null;
            }
        }
    }

    //Adds to the player's coins based on the cost of each item and the number of bulk thresholds surpassed
    public void BulkOrderDayEnd()
    {
        //Adds the bonus of the highest threshold surpassed
        int addition = 0;
        foreach (Vector2Int pair in currentDay.bulkThresholdsAndValues)
        {
            if (numberGiven >= pair.x)
                addition = pair.y;
            else
                break;
        }

        //Adds the cumulative value of each item given as calculated by adding together the component costs
        int orderCost = 0;
        foreach (int componentID in components)
        {
            foreach (ItemID item in gameData.ids)
            {
                bool _break = false;
                if (item.item.GetComponent<Object>().GetID() == componentID)
                {
                    orderCost += item.item.GetComponent<Object>().SellPrice;
                    break;
                }
            }
        }

        //Adds value calculated to coins and resets visuals
        addition += orderCost * numberGiven;
        gameManager.AddCoins(addition);
        cartEmpty.EmptyCart();
        componentOne.gameObject.SetActive(false);
        componentTwo.gameObject.SetActive(false);
    }

    /// <summary>
    /// Gets the number of item given during current day
    /// </summary>
    /// <returns>Number of items given during current day</returns>
    public int GetCurrentItems()
    {
        return numberGiven;
    }
}
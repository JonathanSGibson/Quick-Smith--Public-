using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//store current day*
//store items available to be made (Both complete items and components)*
//store range of number of customers customers*
//store information for customers (arrival time, time between customers, patience)


[System.Serializable]
public struct Day
{
    public int dayNumber; //With day 0 being the tutorial (This should maybe be replaced with just checking their order in the list)
    [Header("IDs for complete items (eg sword) available during this day")]
    public List<int> completeItemIDs; //IDs for complete items (eg sword) available during this day
    [Header("IDs for component items (eg gold sword) available during this day")]
    public List<int> componentIDs; //IDs for component items (eg gold sword) available during this day
    [Header("If you want a set number make the maximum and minimum equal")]
    public int minCustomers; //Minimum number of customers to arrive (If you want a set number make the maximum and minimum equal) (inclusive)
    public int maxCustomers; //Maximum number of customers to arrive (inclusive)
    public float customerPatience; //In seconds, for before their order is taken
    public float customerOrderPatience; //In seconds, for after you've taken their order
    public float minTimeBetweenCustomers; //Minimum gap between customers arriving in seconds (If you want a set number make the maximum and minimum equal) (inclusive)
    public float maxTimeBetweenCustomers; //Maximum gap between customers arriving in seconds (inclusive)
    [Header("The X value is the number of the item to reach the threshold, the Y value is. Ensure that they are in ascending order of the x value")]
    public List<Vector2Int> bulkThresholdsAndValues;

    public Day(int dayNumber_, List<int> completeItemIDs_, List<int> componenetIDs_, int minCustomers_, int maxCustomers_, float customerPatience_, float customerOrderPatience_, float minTimeBetweenCustomers_, float maxTimeBetweenCustomers_, List<Vector2Int> bulkThresholdsAndValues_)
    {
        dayNumber = dayNumber_;
        completeItemIDs = completeItemIDs_;
        componentIDs = componenetIDs_;
        minCustomers = minCustomers_;
        maxCustomers = maxCustomers_;
        customerPatience = customerPatience_;
        customerOrderPatience = customerOrderPatience_;
        minTimeBetweenCustomers = minTimeBetweenCustomers_;
        maxTimeBetweenCustomers = maxTimeBetweenCustomers_;
        bulkThresholdsAndValues = bulkThresholdsAndValues_;
    }
}

[System.Serializable]
public struct DayAdditions
{
    public int day;
    public List<int> componentIds;
    public List<int> completeItemIDs;
    public List<Vector2Int> newBulkThresholdsAndValues;

}

[System.Serializable]
public struct PlayerStartDayValues<T, Y, Z> {
    public T player;
    public Y startDay;
    public Z UiCheckMark;

    public PlayerStartDayValues(T first_, Y second_, Z third_)
    {
        player = first_;
        startDay = second_;
        UiCheckMark = third_;
    }
}


public class GameManager : MonoBehaviour
{
    public int currentDay; 
    public List<Day> days;

    public int coins;

    public CustomerController customerController;
    public SaveManager saveManager;
    public ObjectMultiplierStore objectMultiplierStore;

    public float customerIncreaseNumber;
    public float timeBetweenCustomersDecreaseNumber;
    [Header("put only the amount to change by (25% = 0.25)")]
    public float percentageChange;
    public List<DayAdditions> dayAdditions;
    public float customerPatience;
    public float customerOrderPatience;
    public float minDay1Customers;
    public float maxDay1Customers;
    public float minDay1TimeBetweenCustomers;
    public float maxDay1TimeBetweenCustomer;
    public List<Vector2Int> day1BulkThresholdsAndValues;
    [Header("number of days before level is complete")]
    public int numberOfDays;

    HUDDisplayNumbers hudDisplayNumbers;
    public bool gameFailed = false;

    [SerializeField]
    public bool playPahse {get; private set; }

    [Header("Day Cycle Settings")]
    public Light Sun;
    public Gradient SunColor;
    public AnimationCurve BrightnessCurve;

    public AnimationCurve NightLightCurve;
    public List<Light> NightLights;
    public float nightBrightnessMultiplier = 4;

    [Header("Prep Phase sun values")]
    [Range(0.0f, 1.0f)]
    public float sunPosition;
    [Range(0.0f, 1.0f)]
    public float sunColor;
    [Range(0.0f, 1.0f)]
    public float sunBrightness;

    float dayStartTime;

    float dayLength;

    public Slider DayProgressBar;
    public Image DayProgressBarColor;

    Vector3 SunStartValues;

    public List<PlayerStartDayValues<GameObject, bool, Image>> playerDayStart;

    public GameObject UiCheckPrefab;
    public GameObject UiCheckPosition;
    private void Start()
    {
        //get otehr scripts
        GameObject customerManagerObj = GameObject.FindGameObjectWithTag("CustomerManager");

        if (customerManagerObj != null)
            customerController = customerManagerObj.GetComponent<CustomerController>();
        else
            Debug.LogError("Could not find customer controller in scene");

        objectMultiplierStore = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ObjectMultiplierStore>();

        hudDisplayNumbers = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUDDisplayNumbers>();

        //Jack's code
        SunStartValues = Sun.transform.eulerAngles; //set up default values for sun position

        //get save profile
        saveManager = GameObject.FindGameObjectWithTag("SaveManager").GetComponent<SaveManager>();
        if (saveManager.getCurrentProfile().inLevel) //if profile already has a run
        {
            //get day data
            SaveProfile currentProfile = saveManager.getCurrentProfile();
            days = currentProfile.levelSave.days;
            currentDay = currentProfile.levelSave.currentDay;
            customerIncreaseNumber = currentProfile.levelSave.customerIncreaseNumber;
            timeBetweenCustomersDecreaseNumber = currentProfile.levelSave.timeBetweenCustomersDecreaseNumber;
            objectMultiplierStore.multipliers = currentProfile.levelSave.objectMultipliers; //load any upgrades that they may have saved
            coins = currentProfile.levelSave.coins;
        } 
        else
        {
            //if new run then create first day
            CreateNextDay();
        }

        //set up hud ui
        hudDisplayNumbers.SetCoins(coins);
        hudDisplayNumbers.SetExpectedCustomers(days[currentDay-1].minCustomers, days[currentDay-1].maxCustomers);
        hudDisplayNumbers.SetDay(currentDay);
        hudDisplayNumbers.SetSliderVisability(false);

        //turn off niight lights
        if(NightLights.Count > 0)
        {
            for (int i = 0; i < NightLights.Count; i++)
            {
                NightLights[i].intensity = 0;
            }
        }
        //End of Jack's Code
    }
    
    private void Update()
    {
        //Jack's Code
        //day cycle code
        if (playPahse) //if day cycle should be running
        {
            //get current progress of the day
            float dayPercent = (Time.time - dayStartTime) / dayLength;
            if (dayPercent > 1)
                dayPercent = 1;//cap to a whole day
            DayProgressBar.value = dayPercent;//set ui progress bar
            DayProgressBarColor.color = new Color (SunColor.Evaluate(dayPercent).r, SunColor.Evaluate(dayPercent).g, SunColor.Evaluate(dayPercent).b, 0.25f); //set progress bar colour
            Sun.color = SunColor.Evaluate(dayPercent); //set sun colour
            Sun.intensity = BrightnessCurve.Evaluate(dayPercent); //set sun brightness
            Sun.gameObject.transform.rotation = Quaternion.Euler(180 * dayPercent, SunStartValues.y, SunStartValues.z); //set sun rotation in sky

            for (int i = 0; i < NightLights.Count; i++) //for each night light
            {
                NightLights[i].intensity = nightBrightnessMultiplier * NightLightCurve.Evaluate(dayPercent); //set brightness 
            }
        }
        //if in prep phase 
        else if (Sun.color != SunColor.Evaluate(sunColor) || Sun.gameObject.transform.eulerAngles.x != sunPosition || Sun.intensity != BrightnessCurve.Evaluate(sunBrightness)) 
        {
            Sun.color = SunColor.Evaluate(sunColor); //set to prep phase colour
            Sun.transform.localEulerAngles = new Vector3(180 * sunPosition, SunStartValues.y, SunStartValues.z); //set to prep phase angle
            Sun.intensity = BrightnessCurve.Evaluate(sunBrightness); //set to prep phase brightness
            for (int i = 0; i < NightLights.Count; i++)
            {
                NightLights[i].intensity = 0; //turn off night lights
            }
        }

        //player starting day
        if (!playPahse && playerDayStart.Count > 0)
        {
            bool startDay = true;
            foreach (var player in playerDayStart)
            {
                //check if the player has not pressed start day
                if(player.startDay == false)
                {
                    startDay = false;
                    break;
                }
            }
            //plays start day if all players have said to start
            if (startDay)
            {
                for (int i = 0; i < playerDayStart.Count; i++)
                {
                    //set back to false ready for next day
                    playerDayStart[i] = new PlayerStartDayValues<GameObject, bool, Image>(playerDayStart[i].player, false, playerDayStart[i].UiCheckMark);
                    playerDayStart[i].UiCheckMark.gameObject.SetActive(false);
                }
                DayStart();
            }
        }
        //if the number of players has changed, update the number of people needed to start the day
        if(GameObject.FindGameObjectWithTag("PlayerHandler").GetComponent<PlayerHandler>().GetMaxPlayers() != playerDayStart.Count)
        {
            UpdatePlayerStartDayCount(); 
        }

        //End of Jack's code
    }
    public void GameOver()
    {
        ClearInventories(); //clear the inventory of all work stations and players (Jack's code)
        //Jonathan's code
        Debug.Log("Lose");
        gameFailed = true;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAnimations>().DefeatAnimation();
        saveManager.UpdateCurrentProfile(new SaveProfile(saveManager.getCurrentProfile().name, saveManager.getCurrentProfile().SaveID, false));//set the save to not be in a level (Jack's code)
        GameObject.FindGameObjectWithTag("CustomerManager").GetComponent<CustomerController>().CustomerControllerGameOver();
        Camera.main.GetComponent<CameraAudio>().FailMusic();
        //End of Jonathan's code
    }

    public void DayEnd()
    {
        //Jack's Code
        if (playPahse)
        {
            playPahse = false;
            ClearInventories(); //clear work station and players inventory
            //Jonathan's code
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAnimations>().VictoryAnimation();
            GameObject.FindGameObjectWithTag("BulkOrder").GetComponent<BulkOrderCounter>().BulkOrderDayEnd();
            //end of Jonathan's code
            CreateNextDay();//create the next day
            //save progress
            saveManager.UpdateCurrentProfile(new SaveProfile(saveManager.getCurrentProfile().name, saveManager.getCurrentProfile().SaveID, true, new LevelSave(SceneManager.GetActiveScene().buildIndex, currentDay, days, customerIncreaseNumber, timeBetweenCustomersDecreaseNumber, objectMultiplierStore.multipliers, coins)));
            Camera.main.GetComponent<CameraAudio>().DayEndMusic();

            //turn on prep phase hud and turn off play phase hud
            hudDisplayNumbers.SetExpectedCustomerVisability(true);
            hudDisplayNumbers.SetSliderVisability(false);
            hudDisplayNumbers.SetExpectedCustomers(days[currentDay - 1].minCustomers, days[currentDay - 1].maxCustomers);
            hudDisplayNumbers.SetDay(currentDay);
            //reset values for day cycle
            dayStartTime = 0;
            dayLength = 0;
            if (NightLights.Count > 0)
            {
                for (int i = 0; i < NightLights.Count; i++)
                {
                    NightLights[i].intensity = 0;
                }
            }
        }
        //End of Jack's code
    }

    public void DayStart()
    {
        //Jack's code
        if (!playPahse)//only run in prep phase 
        {
            foreach (GameObject obj in FindObjectsOfType<GameObject>())
            {
                if (obj.layer == 6)//if on interactable layer
                {
                    InteractableObject objScript = obj.GetComponent<InteractableObject>();
                    if (objScript != null)
                    {
                        objScript.StartDay(); //set objects to be interactable
                    }
                }
            }
            //save any changes (shop updagrades) made in prep phase
            saveManager.UpdateCurrentProfile(new SaveProfile(saveManager.getCurrentProfile().name, saveManager.getCurrentProfile().SaveID, true, new LevelSave(SceneManager.GetActiveScene().buildIndex, currentDay, days, customerIncreaseNumber, timeBetweenCustomersDecreaseNumber, objectMultiplierStore.multipliers, coins)));
            playPahse = true;
            dayStartTime = Time.time; //set the start of day for day cycle
            dayLength = 0;
            //Jonathan's code
            GameObject.FindGameObjectWithTag("BulkOrder").GetComponent<BulkOrderCounter>().GenerateOrder();
            customerController.GenerateQueue();
            //end of Jonathan's code

            hudDisplayNumbers.SetSliderVisability(true); //turn on play phase ui
            hudDisplayNumbers.SetExpectedCustomerVisability(false); //turn off prep phase ui
            Camera.main.GetComponent<CameraAudio>().DayStartMusic(); // Jonathan's code

        }
        //End of Jack's code
    }

    
    /// <summary>
    /// Returns the information for the current day(Jonathan's function)
    /// </summary>
    /// <returns>The current day</returns>
    public Day GetCurrentDay()
    {
        //return current day
        if (currentDay > 0)
        {
            return days[currentDay - 1];
        }
        return days[0];
    }

    private void ClearInventories()
    {
        //Jack's code
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.layer == 6)//if on interactable layer
            {
                InteractableObject objScript = obj.GetComponent<InteractableObject>();
                if (objScript != null)
                {
                    objScript.Clear(true); //empty items from object and move it to prepPhase
                }
            }
        }
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            PlayerInventory inventory = player.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                inventory.Clear(); //clears the item in the player inventory
            }
        }
        //End of Jack's code
    }

    public void CreateNextDay()
    {
        //Jack's code
        //find if any items are added on the day if so add them
        DayAdditions tempAdditions = default;
        bool additionsFound = false;
        foreach (var addition in dayAdditions)
        {
            if (addition.day == currentDay + 1)
            {
                tempAdditions = addition;
                additionsFound = true;
                break;
            }
        }
        List<int> completeItemIDs = new List<int>();
        List<int> componentIds = new List<int>();
        
        Day previousDay = default;
        if (days.Count > 0) //do not do for startday
        {
            previousDay = days[currentDay - 1];//index starts from 0
                                                   //get all craftable items from previous days
            completeItemIDs = previousDay.completeItemIDs;
            componentIds = previousDay.componentIDs;
        }
        List<Vector2Int> newBulkThresholds = previousDay.bulkThresholdsAndValues; //Jonathan's code
        if (additionsFound)
        {
            //add aditional items if found earlier
            foreach (var componenet in tempAdditions.componentIds)
            {
                componentIds.Add(componenet);
            }
            foreach (var item in tempAdditions.completeItemIDs)
            {
                completeItemIDs.Add(item);
            }
            newBulkThresholds = tempAdditions.newBulkThresholdsAndValues;//Jonathan's code
        }
        //Jonathan's code
        if (currentDay == 0)
        {
            newBulkThresholds = day1BulkThresholdsAndValues; 
        }
        //End of Jonathan's code
        if (days.Count > 0)
        {
            //create the new day
            days.Add(new Day(currentDay + 1, 
                                 completeItemIDs,
                                 componentIds,
                                 Mathf.RoundToInt(previousDay.minCustomers + customerIncreaseNumber), //increase the number of customers expected to come
                                 Mathf.RoundToInt(previousDay.maxCustomers + customerIncreaseNumber),
                                 customerPatience,
                                 customerOrderPatience,
                                 previousDay.minTimeBetweenCustomers - timeBetweenCustomersDecreaseNumber, //decrease the time between customers
                                 previousDay.maxTimeBetweenCustomers - timeBetweenCustomersDecreaseNumber,
                                 newBulkThresholds));
        }
        else //create day 1 with default values
        {
            days.Add(new Day(currentDay + 1,
                                 completeItemIDs,
                                 componentIds,
                                 Mathf.RoundToInt(minDay1Customers),
                                 Mathf.RoundToInt(maxDay1Customers),
                                 customerPatience,
                                 customerOrderPatience,
                                 minDay1TimeBetweenCustomers,
                                 maxDay1Customers,
                                 day1BulkThresholdsAndValues));
        }
        //set the numbers to be ready for next day
        customerIncreaseNumber *= 1 + percentageChange;
        timeBetweenCustomersDecreaseNumber *= 1 - percentageChange;
        currentDay++;

        //end of Jack's code
    }

    // Jack's code
    public void QuitSave()
    {
        
        if (!playPahse)//save the game if they hit the quit button in the prep phase
        {
            saveManager.UpdateCurrentProfile(new SaveProfile(saveManager.getCurrentProfile().name, saveManager.getCurrentProfile().SaveID, true, new LevelSave(SceneManager.GetActiveScene().buildIndex, currentDay, days, customerIncreaseNumber, timeBetweenCustomersDecreaseNumber, objectMultiplierStore.multipliers, coins)));
        }
    }

    public void AddCoins(int addition)
    {
        //add coins and update ui
        coins += addition;
        hudDisplayNumbers.SetCoins(coins);
    }

    public void AddTimeToDay(float time)
    {
        //add time to the day length for the day cycle to last until the last customer arrives
        dayLength += time;
    }

    public void UpdatePlayerStartDayCount()
    {
        //clear list of previous players 
        for (int i = 0; i < playerDayStart.Count; i++)
        {
            Destroy(playerDayStart[i].UiCheckMark.gameObject);
        }

        playerDayStart = new List<PlayerStartDayValues<GameObject, bool, Image>>();

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        //find each player and add to the list
        foreach (var player in players)
        {
            Image image = Instantiate(UiCheckPrefab, UiCheckPosition.transform).GetComponent<Image>();
            image.gameObject.SetActive(false);
            playerDayStart.Add(new PlayerStartDayValues<GameObject, bool, Image>(player, false, image)); //create a slot for each player
        }
    }
    //called by player to update their start button
    public void PlayerStartButton(GameObject player)
    {
        if (!playPahse)
        {
            for (int i = 0; i < playerDayStart.Count; i++)
            {//if item in list is the current player
                if (playerDayStart[i].player == player)
                {
                    playerDayStart[i] = new PlayerStartDayValues<GameObject, bool, Image>(player, !playerDayStart[i].startDay, playerDayStart[i].UiCheckMark);//set start day to be opposite of when it was last pressed (flips on and off each tiem pressed)
                }
                if (playerDayStart[i].startDay) //if they have start day set to true set hte ui element active
                {
                    playerDayStart[i].UiCheckMark.gameObject.SetActive(true);
                    playerDayStart[i].UiCheckMark.color = playerDayStart[i].player.GetComponentInChildren<Outline>().OutlineColor;
                }
                else //else turn off
                {
                    playerDayStart[i].UiCheckMark.gameObject.SetActive(false);
                }
            }
        }
    }
    //End of Jack's code
}

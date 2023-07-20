using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerController : MonoBehaviour
{
    /* This script is entirely made by Jonathan
     * 
     * This script manages the customer and the queue
     */

    //Queue information
    public List<GameObject> allCustomers; //List that the customers are placed into when they are created, before they have joined the queue
    public List<GameObject> queue; //List of all customers currently in the queue with NULL for empty spaces, 0 is the front of the queue
    public int orderingStation; //The index of the queue

    //Misc information
    public int totalDailyCustomers;
    public GameManager gameManager; 
    public GameDataManager gameData; 
    public GameObject customerPrefab; //The prefab to be spawned when customers are generated [NOTE: this needs to be made a list when multiple different customer types exist]
    public float dailyProgress; //Between 0 and 1 where 0 is no customers having arrived and 1 is all customers having arrived
    public SellCounter sellCounter; //Script for the sell counter
    public GameObject orderCounter;


    //Information from game manager;
    List<int> availableItemIDs; //List of every COMPLETE ITEM ID the customers can order
    List<int> availableComponentIDs; //List of every COMPONENT ID the customer can order
    float lastCustomerArrival; //The time at which the last customer arrives

    //Customer pathing information
    public List<GameObject> pathfindingNodes; //Every node the customer's should pathfind through when queuing
    public GameObject orderGivenNode; //The node the customer's pathfind to to despawn after they receive their order
    public GameObject spawnPoint; //The location at which customers spawn in (as customers do not collide with eachother it does not matter that they all spawn at one point)

    float lastOrderTaken = 0; // Stores the time the previous order was taken (to avoid simultaneous taking of orders)
    public float minTimeBetweenTakingOrders = 0.5f; // The gap between players being able to take orders (to avoid simultaneous taking of orders)

    //tutorial stuff
    public bool isTutorial = false;

    float soundTimerEnd = -1; //The time at which a customer last played their "wrong order" sound
    public float soundTimerDuration = 5f; //The duration of time between customers playing their "wrong order" sound


    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        gameData = GameObject.FindGameObjectWithTag("dataManager").GetComponent<GameDataManager>();
    }

    /// <summary>
    /// Sets each customer to be the furthest along the queue they can be
    /// </summary>
    public void QueueManagement()
    {
        //Moves each item as far along the queue as it can
        bool looping = true;
        while (looping)
        {
            looping = false;
            for (int i = queue.Count - 1; i > 0; i--)
            {
                //check if next space in the queue is accessible and empty, if so move it to that position
                if (queue[i] != null)
                    if (queue[i - 1] == null)
                    {
                        if (i - 1 >= orderingStation || (i - 1 < orderingStation && queue[i].GetComponent<Customer>().GetOrderTaken()))
                        {
                            //Moves them to next spot in queue
                            queue[i - 1] = queue[i];
                            queue[i - 1].GetComponent<Customer>().GiveNewNode(i - 1); //Gives it the node to path to
                            //If at either of the stopping points then stops at that point
                            if (i - 1 == orderingStation)
                                queue[i - 1].GetComponent<Customer>().AtOrdering(sellCounter.gameObject);
                            else if (i - 1 == 0)
                                queue[i - 1].GetComponent<Customer>().AtSelling(orderCounter);
                            //empties their previous spot
                            queue[i] = null;
                            looping = true;
                        }
                    }
            }
        }
        
    }

    /// <summary>
    /// Checks if there is a customer at the ordering station, and if the time between orders has elapsed, then takes the order of the customer
    /// </summary>
    public void OrderingStationInteract()
    {
        if (queue.Count > orderingStation)
        {
            if (queue[orderingStation] != null && Time.time > (lastOrderTaken + minTimeBetweenTakingOrders))
            {
                if (queue[orderingStation].GetComponent<Customer>().TakeOrder())
                {
                    //Calls "queue management" as a place in the queue has changed
                    QueueManagement();
                    lastOrderTaken = Time.time;
                }
            }
        }
    }


    /// <summary>
    /// Adds a customer to the queue
    /// </summary>
    /// <param name="customer">The customer to be added</param>
    /// <returns>Whether they have been successfully added to the queue</returns>
    public bool AddToQueue(GameObject customer)
    {
        //Re-organises everything in the queue in case any customers are out of place before beginning
        QueueManagement();

        //Checks the back of the queue is empty
        if (queue[queue.Count - 1] == null)
        {
            //Adds customer to queue, removes it from pre-queue list and sets its pathfinding to begin
            queue[queue.Count - 1] = customer;
            queue[queue.Count - 1].GetComponent<Customer>().GiveNewNode(queue.Count - 1);
            allCustomers.Remove(customer);

            //Adjust daily progress for use in the UI
            if (!isTutorial)
                dailyProgress = 1 - ((float)allCustomers.Count / (float)totalDailyCustomers);
            //Moves the customer (and all other customers) as far along the queue as possible
            QueueManagement();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Generates all customers and all required variables and adds them to the pre-queue list
    /// </summary>
    public void GenerateQueue()
    {
        //Reset existing lists of customers
        allCustomers = new List<GameObject>();
        queue = new List<GameObject>();

        //Gets the information from the current day and calculates number of customers to create
        Day currentDay = gameManager.GetCurrentDay();
        totalDailyCustomers = Random.Range(currentDay.minCustomers, currentDay.maxCustomers);

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


        float previousArrival = 0;
        for (int i = 0; i < totalDailyCustomers; i++)
        {
            //Generates a customer and all required information
            Customer newCustomer = Instantiate(customerPrefab, spawnPoint.transform.position, Quaternion.identity).GetComponent<Customer>();
            newCustomer.gameObject.transform.SetParent(gameObject.transform);

            int order = Random.Range(0, recipeList.Count);
            List<int> components = recipeList[order][Random.Range(0, recipeList[order].Count)].inputs;
            order = recipeList[order][0].outputs[0];
            float waitTime = Random.Range(currentDay.minTimeBetweenCustomers, currentDay.maxTimeBetweenCustomers);
            float arrival = previousArrival + waitTime;
            previousArrival = arrival;
            float patience = currentDay.customerPatience;
            float orderPatience = currentDay.customerOrderPatience;

            newCustomer.Spawn(order, components, arrival, patience, orderPatience, sellCounter, pathfindingNodes);

            allCustomers.Add(newCustomer.gameObject);
        }
        allCustomers[allCustomers.Count - 1].GetComponent<Customer>().MakeLastCustomer();

        //Generates an empty spot in the queue for every customer
        foreach(GameObject node in pathfindingNodes)
        {
            queue.Add(null);
        }
    }

    /// <summary>
    /// Checks whether an item matches the order of the customer at the "sell counter"
    /// </summary>
    /// <param name="item">The item to be checked</param>
    /// <returns>Whether the item matches or not</returns>
    public bool CheckSoldItem(Object item)
    {
        if (queue.Count > 0)
        {
            if (queue[0] != null)
            {
                bool valid = false;
                if (item.GetID() == queue[0].GetComponent<Customer>().GetOrder())
                {
                    valid = true;
                    //Loops through each component of the customer at the sell counter's order and checks it against the components in the given item
                    foreach (int component in queue[0].GetComponent<Customer>().GetOrderComponents())
                    {
                        bool thisOneValid = false;

                        //THIS WHOLE SECTION IS SWORD SPECIFIC AND NEEDS TO BE REPLACED TO BE MORE GENERIC
                        List<int> swordComponents = item.GetComponent<Sword>().components;

                        foreach (int swordID in swordComponents)
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

                //If the item matches the sell price is calculated by adding the cost of aech components
                if (valid)
                {
                    int price = 0;
                    foreach (int component in queue[0].GetComponent<Customer>().GetOrderComponents())
                    {
                        foreach(ItemID _item in gameData.ids)
                        {
                            if (_item.item.GetComponent<Object>().ID == component)
                            {
                                price += _item.item.GetComponent<Object>().SellPrice;
                                break;
                            }
                        }
                    }
                    //Call relevant functions in both customer and customer audio, as well as emptying the slot in the
                    if (!isTutorial)
                        gameManager.AddCoins(price);
                    queue[0].GetComponent<Customer>().OrderRecieved(orderGivenNode);
                    queue[0].GetComponent<CustomerAudio>().TakeOrder();
                    queue[0] = null;
                    
                    if (isTutorial)
                        GameObject.FindGameObjectWithTag("Tutorial").GetComponent<TutorialTransisions>().OrderGiven();

                    //Moves every customer along in the queue
                    QueueManagement();
                    return true;
                }
                else
                {
                    //If the order doesn't match the customer will play an audio clip
                    //this is done with a timer so there is not constant noise if you place the wrong order
                    if (soundTimerEnd < Time.time)
                    {
                        queue[0].GetComponent<CustomerAudio>().WrongOrder();
                        soundTimerEnd = Time.time + soundTimerDuration;
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Spawns a single customer (For use in the tutorial)
    /// </summary>
    public void SpawnIndividualCustomer()
    {
        allCustomers = new List<GameObject>();
        queue = new List<GameObject>();

        while(queue.Count != pathfindingNodes.Count)
        {
            queue.Add(null);
        }

        Customer newCustomer = Instantiate(customerPrefab, spawnPoint.transform.position, Quaternion.identity).GetComponent<Customer>();
        newCustomer.gameObject.transform.SetParent(gameObject.transform);

        //Values are hardcoded as they are only for use in the tutorial - this should be moved into a seperate script for the tutorial
        int order = 7;
        List<int> components = new List<int>();
        components.Add(2);
        components.Add(11);
        float arrival = 0;
        float patience = 200;
        float orderPatience = 200;

        newCustomer.Spawn(order, components, arrival, patience, orderPatience, sellCounter, pathfindingNodes, isTutorial);

        allCustomers.Add(newCustomer.gameObject);
    }

    /// <summary>
    /// All required functionality for the customers on game over
    /// </summary>
    public void CustomerControllerGameOver()
    {
        foreach(GameObject customer in queue)
        {
            if (customer != null)
                customer.GetComponent<Customer>().CustomerGameOver();
        }

        foreach (GameObject customer in allCustomers)
        {
            if (customer != null)
                customer.GetComponent<Customer>().CustomerGameOver();
        }
    }
}

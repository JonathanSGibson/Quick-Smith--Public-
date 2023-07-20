using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    /* This script is mostly Jonathan's work,
     * Jack added in the progress bar and its functionality.
     * 
     * This script is used to control any audio
     * which is sourced from the customer
     */

    //misc
    CustomerController customerManager;
    SellCounter sellCounter;

    //Pathing and movement variables
    public float moveSpeed;
    public float pathfindingStopDistance; //How far away from their node they will stop (without this they overshoot and double back repeatedly)

    List<GameObject> pathfindingNodes; //Each node (in reverse order) of the queue for the customer to path through
    int currentNode = 0; //Index of the above list that is the node the customer is moving towards
    int targetNode = -1; //Index of the above list that is the node the customer is aiming to get to (via every other between them, hence why it is seperate from the current node)
    Rigidbody rb;

    //Order values
    int orderID; //ID of the COMPLETE ITEM they order (eg sword)
    List<int> componentIDs; //IDs of the COMPONENTS of their order (eg wooden handle, stone blade)

    //Queuing and patience values
    float arrivalTime; //When the customer will join the queue
    float joinedQueue; //The time at which the customer joined the queue (seperate to arrivalTime in case they fail to join queue)
    float orderTakenTime; //The time at which the customer's order is taken by the player
    float startTime; //The time at which the day is started and all customers are generated
    float patience; //The duration of the customer's patience before their order is taken by the player
    float orderPatience; //The duration of the customer's patience between their order being taken and them receiving it

    bool atOrderNode; //Whether the customer is at the node adjoined to the order taking counter
    bool queueing; //Whether the customer is in the queue or not
    bool orderTaken; //Whether the customer's order has been taken
    bool orderGiven; //Whether the customer has recieved their order
    bool atOrdering; //Whether in the position in the queue to be at the ordering counter (but not necessarily reached it yet)
    bool atSellCounter; //Whether the customer is at the selling counter
    bool queueingBeyondQueue = false; //Whether the customer is "queuing" but there is no room for them in the actual queue
    bool gameOverCalled = false; //Whether the game has already ended (this is to avoid it being called repeatedly)
    bool checkingSellCounter = false; //Whether the customer is checking the selling counter for their order
    bool isLastCustomer = false; //Whether the customer is the last customer in the day

    GameObject currentCounter; //The counter the customer is at, if any

    //Tutorial stuff
    bool isTutorial;

    List<GameObject> pathFindingNode; //List of every node in the path the customers follow
    GameObject deathNode; //Node that customers pathfind to after leaving the queue, on arrival customers delete themselves

    public ObjectProgressBar progressBar; //Progress bar to display customer patience

    GameManager gameManager;

    CustomerAudio audio;

    /// <summary>
    /// Fetches all required components from the customer object and the game manager
    /// </summary>
    private void Start()
    {
        audio = GetComponent<CustomerAudio>();
        rb = GetComponent<Rigidbody>();
        atOrderNode = false;
        progressBar = gameObject.GetComponentInChildren<ObjectProgressBar>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    /// <summary>
    /// Controls joining of the queue and triggering game over
    /// </summary>
    public void Update()
    {
        if (orderTaken)
        {
            //if the customer is waiting for their order to be delivered
            if (Time.time - orderTakenTime >= orderPatience && !gameOverCalled)
                GameOver();
        }
        else if (queueing)
        {
            //If the customer is queueing but is neither waiting to have their order to be taken nor have already had their order taken
            if (Time.time - joinedQueue >= patience && !gameOverCalled)
                GameOver();
        }
        else
        {
            //If the customer is yet to join the queue;
            if (!orderGiven && Time.time - startTime >= arrivalTime)
                JoinQueue();
        }
    }

    /// <summary>
    /// Controls movement of the customer towards the relevant node
    /// </summary>
    public void FixedUpdate()
    {
        //Target node is set to -1 as a default for this check,
        //this means customers will not attempt to move before they are given a node
        if (targetNode >= 0)
        {
            //If the customer is not close enough to their current node to stop they keep moving
            if (queueing && Vector3.Distance(transform.position, pathfindingNodes[currentNode].transform.position) > pathfindingStopDistance)
            {
                transform.LookAt(pathfindingNodes[currentNode].transform);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                rb.velocity = new Vector3(transform.forward.x * moveSpeed, rb.velocity.y, transform.forward.z * moveSpeed);
            }
            //If the customer is close enough to their current node they stop
            else
            {
                rb.velocity = new Vector3(0, rb.velocity.y, 0);

                //If the customer is at the ordering counter sets a bool which allows the player to take their order
                if (atOrdering && currentNode == targetNode)
                    atOrderNode = true;

                //If the node the customer is at is adjoined to a counter they look towards the counter
                if ((atSellCounter || atOrdering) && Vector3.Distance(transform.position, pathfindingNodes[targetNode].transform.position) < pathfindingStopDistance)
                {
                    transform.LookAt(currentCounter.transform);
                    transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                    if (checkingSellCounter)
                        sellCounter.CheckSellingItems(sellCounter.items[0]);
                }

                //If the target is further along the queue, sets the customers current node to be the next one in the queue
                //As a note the queue is in reverse order (index 0 is the end of the queue)
                if (currentNode > targetNode)
                    currentNode--;

                //If they have arrived at the selling counter, checks if they order is already there
                if (orderTaken && atSellCounter)
                    ArriveAtSellCounter();
            }
        }
        //If their current node is lower than 0 (as is set when they take their order),
        //then the customer moves towards the "death node" (the point at which they despawn)
        else if (orderGiven && Vector3.Distance(transform.position, deathNode.transform.position) > pathfindingStopDistance)
        {
            transform.LookAt(deathNode.transform);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            rb.velocity = new Vector3(transform.forward.x * moveSpeed, rb.velocity.y, transform.forward.z * moveSpeed);
        }
        //If the customer is within stop distance of the "death node" they destroy themselves
        else if (orderGiven && Vector3.Distance(transform.position, deathNode.transform.position) <= pathfindingStopDistance)
        {
            if (isLastCustomer)
                gameManager.DayEnd();
            Destroy(gameObject);
        }
        //Stops the customer if they have no node AND haven't taken their order,
        //this should only happen when they have not yet joined the queue, but acts to catch unexpected circumstances as well
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }

        //Jack's contributions start
        progressBar.UpdateBar(1-GetPatience());//set their patience bar, 1-num so that the bar decreases instead of increasing
        //Jack's contributions end
    }

    /// <summary>
    /// Sets all required values for each customer
    /// </summary>
    /// <param name="_order">//ID of the COMPLETE ITEM they order (eg sword) </param>
    /// <param name="_components">IDs of the COMPONENTS of their order (eg wooden handle, stone blade)</param>
    /// <param name="_arrival">When the customer will join the queue</param>
    /// <param name="_patience">The duration of the customer's patience before their order is taken by the player</param>
    /// <param name="_orderPatience">The duration of the customer's patience between their order being taken and them receiving it</param>
    /// <param name="_sellCounter">Whether the customer is at the selling counter</param>
    /// <param name="_pathfindingNodes">Each node (in reverse order) of the queue for the customer to path through</param>
    /// <param name="tutorial">Whether the customer has been spawned in the tutorial</param>
    public void Spawn(int _order, List<int> _components, float _arrival, float _patience, float _orderPatience, SellCounter _sellCounter, List<GameObject> _pathfindingNodes, bool tutorial = false)
    {
        customerManager = transform.GetComponentInParent<CustomerController>();
        orderID = _order;
        componentIDs = _components;
        arrivalTime = _arrival;
        patience = _patience;
        orderPatience = _orderPatience;
        startTime = Time.time;
        sellCounter = _sellCounter;
        isTutorial = tutorial;
        pathfindingNodes = _pathfindingNodes;
        currentNode = pathfindingNodes.Count-1;
        GetComponent<OrderDisplayManager>().HideOrder();
    }

    /// <summary>
    /// Customer attemps to join the queue, if failed will start queuing beyond queue
    /// </summary>
    public void JoinQueue()
    {
        if (customerManager.AddToQueue(gameObject))
            queueing = true;
        if (!queueingBeyondQueue)
        {
            joinedQueue = Time.time;
            queueingBeyondQueue = true;
        }
        
    }

    /// <summary>
    /// Function to take the customer's order
    /// </summary>
    /// <returns>Returns false if the customer has not yet arrived at the counter</returns>
    public bool TakeOrder()
    {
        if (atOrderNode)
        {
            orderTaken = true;
            orderTakenTime = Time.time;
            GetComponent<OrderDisplayManager>().DisplayOrder();
            audio.Order();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Does relevant game over sutff
    /// </summary>
    public void GameOver()
    {   //If in tutorial simply resets patience
        if (isTutorial)
        {
            
            if (orderTaken)
                orderTakenTime = Time.time;
            else if (queueing)
                joinedQueue = Time.time;
        }
        //If not in tutorial calls the game over function in game manager
        else
            gameManager.GameOver();
    }
 
    /// <returns>Patience as a value from 1 to 0 where 0 is no remaining patience</returns>
    public float GetPatience()
    {
        if (orderTaken)
            return (orderPatience - (Time.time - orderTakenTime)) / orderPatience;
        else if (queueing)
            return (patience - (Time.time - joinedQueue)) / patience;
        else
            return 0;
    }

    /// <returns>Returns whether their order has been taken or not</returns>
    public bool GetOrderTaken()
    {
        return orderTaken;
    }

    /// <summary>
    /// Sets a new node for the customer to target
    /// </summary>
    /// <param name="newNode">The index of the target node within the customer's pathfinding nodes</param>
  
    //This is called by the CustomerController as it controls the queue management
    public void GiveNewNode(int newNode)
    {
        if (newNode >=0 && newNode < pathfindingNodes.Count)
            targetNode = newNode;
    }

    /// <returns>The IDs of each component of the customer's order</returns>
    public List<int> GetOrderComponents()
    {
        return componentIDs;
    }


    /// <returns>The ID of the complete item of the customer's order</returns>
    public int GetOrder()
    {
        return orderID;
    }

    /// <summary>
    /// Sets all variables required for when the customer receives the order
    /// </summary>
    /// <param name="_targetNode">The node the customer should path to before despawning</param>
    public void OrderRecieved(GameObject _targetNode)
    {
        queueing = false;
        orderTaken = false;
        orderGiven = true;
        targetNode = -1; //Set to -1 so the customer does not path to any nodes (except the "death node")
        deathNode = _targetNode;
        GetComponent<OrderDisplayManager>().HideOrder();
    }

    /// <summary>
    /// Sets the customer to check if the sell counter already has their order
    /// </summary>
    public void ArriveAtSellCounter()
    {
        checkingSellCounter = true;
    }

    /// <summary>
    /// Sets the customer to be able to be ordered
    /// </summary>>
    /// <param name="orderCounter">The order counter, so the customer can look at it</param>
    public void AtOrdering(GameObject orderCounter)
    {
        currentCounter = orderCounter;
        atOrdering = true;
    }

    /// <summary>
    /// Sets the customer to be able to be ordered
    /// </summary>>
    /// <param name="sellCounter">The selling counter, so the customer can look at it</param>
    public void AtSelling(GameObject sellCounter)
    {
        atSellCounter = true;
        currentCounter = sellCounter;
    }

    /// <summary>
    /// Sets that game over has been called so it is not called, repeteadly, by every customer
    /// </summary>
    public void CustomerGameOver()
    {
        gameOverCalled = true;
    }

    /// <summary>
    /// Sets the customer to be the last customer, meaning they will end the day when they recieve their order
    /// </summary>
    public void MakeLastCustomer()
    {
        isLastCustomer = true;
        if(gameManager == null)
        {
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        }
        gameManager.AddTimeToDay(arrivalTime+(patience+orderPatience)/2);
    }
}

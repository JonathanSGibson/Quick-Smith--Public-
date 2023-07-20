using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{

    /* This script was mostly made by Jonathan,
     * with some additions by Jack
     * 
     * This script controls what happens when a player presses interact,
     * or pickup/place from the player's side 
     * (The work station side of this is handled by the work station's script)
     */
    public float rayDistance; //Raycast distance for old version of this code which remains in comments
    bool interactPressed = false; //Whether the player is pressing interact or not
    PlayerInventory inventory;
    public GameObject HUDManager;

    GridSystem gridSystem; //New way of working out what is in front of the player uses the grid system
    float straightAngles; //The size of the up/down/left/right angles
    [Range(0,90)]
    public float diagonalAngles = 45; //The size of the diagonal angles (Can be made smaller to make it easy to interact with straight angles or vica verca)

    List<GameObject> highlightedObjects; // Objects in front of the player to be highlighted - added by Jack

    /// <summary>
    /// Fetches all required values and checks angles are valid
    /// </summary>
    private void OnEnable()
    {
        HUDManager = GameObject.FindGameObjectWithTag("HUD");
        gridSystem = GameObject.FindGameObjectWithTag("Grid").GetComponent<GridSystem>();
        straightAngles = 90f - diagonalAngles;
        if (diagonalAngles < 0 || straightAngles < 0)
            Debug.LogError("Please ensure diagonalAngles is between 0 and 90");
        inventory = gameObject.GetComponent<PlayerInventory>();
        highlightedObjects = null;
    }

    /// <summary>
    /// If the player is interacting fetches the objects in front
    /// Highlights objects in front of player
    /// </summary>
    void Update()
    {
        if (interactPressed)
        {
            //Commented out code is the old way of doing it via raycasts

            ////Fires a raycast forward from the player, calls the interact function on anything interactable
            //LayerMask interactableMask = LayerMask.GetMask("Interactable");
            //RaycastHit rayHit;


            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * rayDistance, Color.yellow);

            //if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out rayHit, rayDistance, interactableMask))
            //{
            //    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * rayHit.distance, Color.red);
            //    var objectScript = rayHit.collider.gameObject.GetComponent<InteractableObject>(); // THIS NEEDS TO BE CHANGED TO JACK'S INTERACT SCRIPT
            //    objectScript.Interact();
            //}

            //Fetches items in front and calls each of their interact functions
            List<GameObject> raycastObjects = GetObjectsInFront();
            if (raycastObjects != null)
            {
                foreach (GameObject interactable in raycastObjects)
                {
                    InteractableObject objectScript = interactable.GetComponent<InteractableObject>();
                    bool temp = objectScript.Interact();
                    GetComponent<PlayerAnimations>().InteractAnimation(temp);
                }
            }
            else
                GetComponent<PlayerAnimations>().InteractAnimation(false);

        }
        else
            GetComponent<PlayerAnimations>().InteractAnimation(false);

        //The following is Jack's code
        //turns on the emmision of the counter infront of the player
        if ((GameObject.FindGameObjectWithTag("GameManager") != null && GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().playPahse) || (GameObject.FindGameObjectWithTag("TutorialGameManager") != null && GameObject.FindGameObjectWithTag("TutorialGameManager").GetComponent<TutorialGameManager>().playPhase))
        {
            //turn off previous frames highlights
            if (highlightedObjects != null)
            {
                foreach (GameObject interactable in highlightedObjects) //for each object from the last frame turn off hightlight
                {
                    InteractableObject objectScript = interactable.GetComponent<InteractableObject>();
                    if(objectScript != null)
                        objectScript.SetHighlight(false);
                }
            }
            highlightedObjects = GetObjectsInFront(); //find hte objects infront of the player and set them to be highlighted
            if (highlightedObjects != null)
            {
                
                foreach (GameObject interactable in highlightedObjects)
                {
                    InteractableObject objectScript = interactable.GetComponent<InteractableObject>(); 
                    if (objectScript != null)
                        objectScript.SetHighlight(true);
                }
            }
        }
        //Jack's code end
    }

    /// <summary>
    /// Sets interact whenever the button is pressed or released
    /// </summary>
    /// <param name="input">The input value</param>
    public void OnInteract(InputValue input)
    {
        interactPressed = !interactPressed;//allows for the key to be held to interact
    }

    public void OnPickupPlace(InputValue input)
    {
        //Once again the commented out code is the old way of getting objects via raycasts

        //Fires a raycast forward from the player, calls the interact function on anything interactable
        //LayerMask interactableMask = LayerMask.GetMask("Interactable");
        //RaycastHit rayHit;


        //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * rayDistance, Color.yellow);

        //if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out rayHit, rayDistance, interactableMask))
        //{
        //    Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * rayHit.distance, Color.red);
        //    var objectScript = rayHit.collider.gameObject.GetComponent<InteractableObject>(); // THIS NEEDS TO BE CHANGED TO JACK'S INTERACT SCRIPT
        //    Object returnedHelditem = objectScript.PickUpPlace(inventory.heldObject);


        //    if (returnedHelditem != null)//if an object is picked up off the object
        //    {
        //        returnedHelditem.transform.parent = null;//remove previous parent
        //        returnedHelditem.transform.parent = inventory.handPosition.transform;//set parent to the players hand
        //        returnedHelditem.transform.SetPositionAndRotation(inventory.handPosition.transform.position, inventory.handPosition.transform.rotation);//move it to the hands position
        //    }
        //    inventory.heldObject = returnedHelditem;//whatever the retunred object is (even null), set it to the held object (null will clear the item that was placed down)
        //}


        if (Time.timeScale != 0) //does not activate if game is paused
        {

            //Gets all objects in grid square in front of the player and  loops through them, calling the relevant function on each object
            List<GameObject> raycastObjects = GetObjectsInFront();
            if (raycastObjects != null)
            {
                foreach (GameObject interactable in raycastObjects)
                {
                    InteractableObject objectScript = interactable.GetComponent<InteractableObject>();
                    Object returnedHelditem = objectScript.PickUpPlace(inventory.heldObject);
                    if (returnedHelditem != null)//if an object is picked up off the object
                    {
                        returnedHelditem.transform.parent = null;//remove previous parent
                        returnedHelditem.transform.parent = inventory.handPosition.transform;//set parent to the players hand
                        returnedHelditem.transform.SetPositionAndRotation(inventory.handPosition.transform.position, inventory.handPosition.transform.rotation);//move it to the hands position
                    }
                    inventory.heldObject = returnedHelditem;//whatever the retunred object is (even null), set it to the held object (null will clear the item that was placed down)
                }
            }
        }
    }

    /// <summary>
    /// Pauses the game when button is pressed
    /// </summary>
    /// <param name="input">Input value</param>
    public void OnPause(InputValue input)
    {
        if(Time.timeScale != 0)
            HUDManager.GetComponent<PlayerHUDButtons>().Pause();
    }

    /// <summary>
    /// Uses grid system to get all objects in front of the player as determined by their input angle and the angles set in the inspector
    /// </summary>
    /// <returns></returns>
    public List<GameObject> GetObjectsInFront()
    {
        if (gridSystem != null)
        {
            Vector2Int relativeGridIndex; //Which direction from the player to check (eg [1,1] for top right)
            Vector2Int playerGridIndex; //The grid index of the player
            Vector2Int targetGridIndex; // The grid index to get objects inside (as determined using the above values)
            LayerMask interactableMask = LayerMask.GetMask("Interactable");

            //From the player's current rotation calculate which grid index (relative to the player) they are facing
            float playerAngle = gameObject.GetComponent<Rigidbody>().rotation.eulerAngles.y;
            if (0.5 * straightAngles < playerAngle && playerAngle < 0.5 * straightAngles + diagonalAngles)
                relativeGridIndex = new Vector2Int(1, 1); //top right
            else if (0.5 * straightAngles + diagonalAngles < playerAngle && playerAngle < 1.5 * straightAngles + diagonalAngles)
                relativeGridIndex = new Vector2Int(1, 0); // right
            else if (1.5 * straightAngles + diagonalAngles < playerAngle && playerAngle < 1.5 * straightAngles + 2 * diagonalAngles)
                relativeGridIndex = new Vector2Int(1, -1); //bottom right
            else if (1.5 * straightAngles + 2 * diagonalAngles < playerAngle && playerAngle < 2.5 * straightAngles + 2 * diagonalAngles)
                relativeGridIndex = new Vector2Int(0, -1); //bottom
            else if (2.5 * straightAngles + 2 * diagonalAngles < playerAngle && playerAngle < 2.5 * straightAngles + 3 * diagonalAngles)
                relativeGridIndex = new Vector2Int(-1, -1); //bottom left
            else if (2.5 * straightAngles + 3 * diagonalAngles < playerAngle && playerAngle < 3.5 * straightAngles + 3 * diagonalAngles)
                relativeGridIndex = new Vector2Int(-1, 0); //left
            else if (3.5 * straightAngles + 3 * diagonalAngles < playerAngle && playerAngle < 3.5 * straightAngles + 4 * diagonalAngles)
                relativeGridIndex = new Vector2Int(-1, 1); //top left
            else if (!(0.5 * straightAngles < playerAngle && playerAngle < 3.5 * straightAngles + 4 * diagonalAngles))
                relativeGridIndex = new Vector2Int(0, 1); //top
            else
            {
                Debug.LogError("You've unlocked the forbidden angle");
                return null;
            }
            //Top works differently as it deals with the point the number goes over from 359 to 0

            //Get the player's grid index and calculate the index of where they're facing, and check that's in range
            playerGridIndex = gridSystem.GetIndexFromPosition(transform.position);
            targetGridIndex = playerGridIndex + relativeGridIndex;

            if (gridSystem.CheckIndexInRange(targetGridIndex.x, targetGridIndex.y))
                //return every interactable game object in that grid index
                return (gridSystem.GetObjectsInIndex(targetGridIndex.x, targetGridIndex.y, interactableMask));
            else
                Debug.LogWarning("Player targetting out of grid");
        }
        else
            Debug.LogError("Please include a grid");
        return null;
    }

    /// <summary>
    /// Marks the player as ready to start the day when they input
    /// This function was made by Jack
    /// </summary>
    public void OnStartDay()
    {
        if(GameObject.FindGameObjectWithTag("GameManager") != null && Time.timeScale != 0)
        {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().PlayerStartButton(gameObject);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderDisplayManager : MonoBehaviour
{
    /* This script is entirely made by Jonathan
     * 
     * This script controls the visuals associated with the customers orders
     */

    /* Dictionary for storing the UI sprites for every possible component for the customer to order.
     * The key should be set as the ID of the component
     * The sprite should be an image of the component
     */
    [System.Serializable]
    public class SpriteDictionaryPair
    {
        public int key;
        public Sprite value;
    }

    public GameObject orderCanvas; // The canvas to display their order on
    public Image componentOne; //The UI component of the first half of their order
    public Image componentTwo; //The UI component for the second half of their order [NOTE: THIS WILL NEED TO BE EXPANDED IF 3 COMPONENT ITEMS ARE ADDED]
    public Image background; //The background of the order [NOTE: CURRENTLY NOT USED]
    Camera mainCam;

    [Header("Make the int value (Key) the ID of the item, and the sprite the image of that item")]
    public List<SpriteDictionaryPair> allComponentImages; //List of every possible component's ID and their image (This is used to display them on the inspector and load them into a dictionary)
    Dictionary<int, Sprite> allComponents = new Dictionary<int, Sprite>(); //Actual dictionary of each image and their ID

    /// <summary>
    /// Loads every sprite image from the list into the actual dictionary and gets the main camera
    /// </summary>
    private void Awake()
    {
        foreach(SpriteDictionaryPair pair in allComponentImages)
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
    private void Update()
    {
        orderCanvas.transform.LookAt(mainCam.transform);
        orderCanvas.transform.up = mainCam.transform.up;
    }

    /// <summary>
    /// Displays the order on the UI
    /// </summary>
    public void DisplayOrder()
    {
        //Activates UI components so they are visible
        componentOne.gameObject.SetActive(true);
        componentTwo.gameObject.SetActive(true);

        //Gets each ID from their order and finds the relevant image to display
        List<int> IDs = GetComponent<Customer>().GetOrderComponents();
        if (allComponents[IDs[0]] != null)
            componentOne.sprite = allComponents[IDs[0]];
        else
            Debug.LogError("Image for component not found. Please check the customer prefab");

        if (allComponents[IDs[1]] != null)
            componentTwo.sprite = allComponents[IDs[1]];
        else
            Debug.LogError("Image for component not found. Please check the customer prefab");
    }

    /// <summary>
    /// Turns off the UI display
    /// </summary>
    public void HideOrder()
    {
        componentOne.gameObject.SetActive(false);
        componentTwo.gameObject.SetActive(false);
    }
}

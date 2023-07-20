using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderCounter  : InteractableObject
{
    /* This script is entirely made by Jonathan,
     * however it inherits from InteractableObject which Jack created
     * 
     * This script controls the counter where the player can take orders from customers
     */

    public CustomerController customerController;

    new void Start()
    {
        base.Start();
    }

    new void Update()
    {
        base.Update();
    }

    /// <summary>
    /// Calls the relevant function in the customer controller to handle 
    /// </summary>
    /// <returns>Always returns false as it is not an ongoing effect</returns>
    new public bool Interact()
    {
        customerController.OrderingStationInteract();
        return false;
    }
}

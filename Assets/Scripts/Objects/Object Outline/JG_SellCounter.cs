using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellCounter : InteractableObject
{
    /*This script is made entirely by Jonathan,
     * however it inherets from the InteractableObject script made by Jack
     * 
     * This script controls the counter the player places orders for them to be sold to customers
     */

    public CustomerController customerController; // The relevant customer controller for the scene

    new void Start()
    {
        base.Start();
    }

    new void Update()
    {
        base.Update();
    }

    /// <summary>
    /// Checks whether an item matches the order of the relevant customer
    /// </summary>
    /// <param name="item">The item to check</param>
    public void CheckSellingItems(Object item)
    {
        if (item != null)
            if (customerController.CheckSoldItem(item))
            {
                ItemSold();
            }
    }

    /// <summary>
    /// Deletes the item and removes it from the relevent list
    /// </summary>
    public void ItemSold()
    {
        if (items[0] != null)
        {
            Destroy(items[0].gameObject);
            items[0] = null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompleteItemBase : Object
{
    /* This Script was written by both Jack and Jonathan in its entirity
     * 
     * It is a more generic form of the sword script (which now inherets from it)
     */
    public List<int> components; //The IDs of the components of the item


    /// <summary>
    /// Sets the values of the component from the list of components
    /// </summary>
    /// <param name="data">The component IDs of the items</param>
    public void giveData(List<int> data)
    {
        components = data;
        GameDataManager dataManager = GameObject.FindGameObjectWithTag("dataManager").GetComponent<GameDataManager>();
        foreach (int component in components)
        {
            SellPrice += dataManager.ids[component].item.GetComponent<Object>().SellPrice;
        }
    }
}

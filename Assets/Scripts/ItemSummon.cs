using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Jack's code
//only for testing purposes
//used to spawn in items to a player for testing 
public class ItemSummon : MonoBehaviour
{
    
    public PlayerInventory inventory;
    public GameDataManager manager;
    public TextMeshProUGUI idList;
    bool doneOnce = false;
    public void Update()
    {
        if (!doneOnce)
        {
            //displays the name of the item next to their id
            string text = "IDs:\n";
            for (int i = 0; i < manager.ids.Count; i++)
            {
                text += i + ": " + manager.ids[i].item.name +"\n";
            }
            idList.text = text;
            doneOnce = true;
        }
    }
    public void Summon(int id) //on a button will find an item based on id and put it in the players hand
    {
        if (inventory.heldObject == null) {
            GameObject item = null;
            for (int i = 0; i < manager.ids.Count; i++)
            {
                if (i == id)
                {
                    item = manager.ids[i].item;
                    break;
                }
            }
            item = Instantiate(item, inventory.handPosition.transform);
            item.transform.SetPositionAndRotation(inventory.handPosition.transform.position, inventory.handPosition.transform.rotation);
            inventory.heldObject = item.GetComponent<Object>();
        }
    }
}
//End of Jack's code
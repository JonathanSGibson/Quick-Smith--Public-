using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePiles : InteractableObject
{
    //Jack's code
    public int ResourceID;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start(); //calls scripts from interactable object (no resource piles specific code currently)
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        if (playPhase)
        {
            while (items.Count != 2)//makes sure there is a spawn spot and a dump spot
            {
                items.Add(null); 
            }
            if (items[0] == null)
            {
                Object temp = Instantiate(GetItemFromID(ResourceID), itemPositions[0].transform).GetComponent<Object>();
                items[0] = temp; //add in the item in slot 1 if there is no item in it
            }
            if (items[1] != null)
            {
                Destroy(items[1].gameObject); //destroy the item in slot 2 so that they can place down excess of the item
                items[1] = null;
            }
        }
    }
}
//End of Jack's code
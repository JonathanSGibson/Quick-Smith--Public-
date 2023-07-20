using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bin : InteractableObject
{
     //Jack's code
    new void Update()
    {
        base.Update();//calls scripts from interactable object 
        foreach (var position in itemPositions) //for each inventory spot, if there is an item delete it
        {
            for (int i = 0; i < position.transform.childCount; i++)
            {
                if (position.transform.GetChild(i) != null)
                {
                    Destroy(position.transform.GetChild(i).gameObject);
                }
            }
        }
        
    }
}
//end of Jack's code
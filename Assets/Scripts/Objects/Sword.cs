using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : CompleteItemBase
{
    private void Start()
    {
        //Jack's code
        //turns off parts of sword that not valid in the recipe
        //this is due to all models being part of the prefab so that any combination can be made without having loads of prefabs
        for (int i = 0; i < gameObject.transform.GetChild(0).transform.childCount; i++)
        {
            for (int j = 0; j <  components.Count; j++)
            {
                if(components[j] == gameObject.transform.GetChild(0).transform.GetChild(i).GetComponent<Object>().ID)
                {
                    gameObject.transform.GetChild(0).transform.GetChild(i).gameObject.SetActive(true); //if the item is part of the recipe keep on
                    break;
                }
                else
                {
                    gameObject.transform.GetChild(0).transform.GetChild(i).gameObject.SetActive(false); //if not turn off
                }
            }
        }
        //end of Jack's code
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreBarrel : InteractableObject
{
    //Jack's code
    public int storedResource;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();//calls scripts from interactable object (no OreBarrel specific code currently)
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update(); //calls scripts from interactable object (no OreBarrel specific code currently)
    }
}
//End of Jack's code
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combiner : InteractableObject
{
    //jack's code
    public CombinerController combinerController;

    new public bool Interact()
    {
        return combinerController.interact(this); //when the combiner is interacted with call the function in the combiner controller
    }

    public void ResetCombinerController()
    {
        combinerController.ResetToDefault(); //used to reset the combiner controller code at end of day
    }
}
//end of Jack's code

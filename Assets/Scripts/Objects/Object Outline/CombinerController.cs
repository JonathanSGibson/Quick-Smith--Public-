using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Jack's code
public class CombinerController : InteractableObject
{
    public List<Combiner> combiners; //the combiners taht are assigned to this controller

    new void Start()
    {
        base.Start();
        foreach (Combiner combiner in combiners) //set the the combiner controller to be this
        {
            combiner.combinerController = this;
        }
        float temp = itemProgressBar.transform.parent.transform.position.y; //move the progress bar to be centred above the combiners
        itemProgressBar.transform.parent.transform.position = Vector3.Lerp(combiners[0].transform.position, combiners[1].transform.position, 0.5f);
        itemProgressBar.transform.parent.transform.position = new Vector3(itemProgressBar.transform.parent.transform.position.x, temp, itemProgressBar.transform.parent.transform.position.z);
    }

    public bool interact(Combiner interactedCombiner)
    {
        items = new List<Object>();
        foreach (Combiner combiner in combiners) //add all of the items on its combiners to see if a valid recipe is created
        {
            foreach(Object item in combiner.items)
            {
                items.Add(item);
            }
        }
        Recipe workingRecipe = default;
        if ( FindWorkingRecipe(ref workingRecipe, InteractionType.INTERACT))//will not let you craft while there are more then the amount of assigned slots used
        {
            if (recipeCraftingTimeLeft < 0)
            {
                
                recipeCraftingTimeLeft = workingRecipe.craftingTime;

            }
            recipeCraftingTimeLeft -= Time.deltaTime; //remove time each frame to remove time
            itemProgressBar.UpdateBar(recipeCraftingTimeLeft, workingRecipe.craftingTime);
            itemProgressBar.SetColor(IncreaseColor);
            lastTimeInteracted = Time.time; //used to know wehn the player last interacted for decreasing the progress bar
            if (recipeCraftingTimeLeft <= 0)
            {
                recipeCraftingTimeLeft = -1; //setting back to default value so that it knows when next recipe starts
                itemProgressBar.UpdateBar(1, 1);
                //delete the items used in the recipes needs to be done from inside each combiner
                foreach (Combiner combiner in combiners)
                {
                    for (int i = 0; i < combiner.items.Count; i++)
                    {
                        if (items[i] == null)
                            continue;
                        for (int j = 0; j < workingRecipe.inputs.Count; j++)
                        {
                            if (workingRecipe.itemAvaliable[j] && combiner.items[i].ID == workingRecipe.inputs[j]) //item used in recipe
                            {
                                Destroy(combiner.items[i].gameObject); //destroy the item
                                combiner.items[i] = null;
                                workingRecipe.itemAvaliable[j] = false; //set it as used (destroyed)
                                                                        //set its inventory spot as empty
                                break;
                            }
                        }
                    }
                }
                for (int i = 0; i < workingRecipe.itemAvaliable.Length; i++)//reset itemAvaliable
                {
                    workingRecipe.itemAvaliable[i] = true;
                }

                foreach (int output in workingRecipe.outputs) //spawn each output in the combiner that was last interacted on
                {
                    bool spotFound = false;
                    for (int i = 0; i < interactedCombiner.items.Count; i++)
                    {
                        if (interactedCombiner.items[i] == null) //if an empty spot then place the output in that spot
                        {
                            Object temp = Instantiate(GetItemFromID(output), interactedCombiner.itemPositions[i].transform).GetComponent<Object>();
                            if(output == 7)
                            {
                                    temp.GetComponent<Sword>().giveData(workingRecipe.inputs);
                            }
                            interactedCombiner.items[i] = temp;
                            spotFound = true;
                            break;
                        }
                    }
                    if (!spotFound) //if no spot found create a temporary spot
                    {
                        Object temp = Instantiate(GetItemFromID(output), interactedCombiner.itemPositions[interactedCombiner.itemPositions.Length - 1].transform).GetComponent<Object>();//if there is no spot put it in hte position of the last one
                        temp.transform.SetPositionAndRotation(temp.transform.position, Quaternion.Euler(temp.transform.rotation.eulerAngles + new Vector3(0, Random.Range(10, 350), 0)));
                        items.Add(temp);
                    }

                }
            }
            return true;
        }
        else
            if (recipeCraftingTimeLeft != -1)
            recipeCraftingTimeLeft = -1;//if no working recipe set startTime to null for next recipe (incase item is taken off mid smelt)

        return false;
    }

    public void ResetToDefault()
    {
        itemProgressBar.UpdateBar(1); //set back to default values
        recipeCraftingTimeLeft = -1;

    }
}
//end of Jack's code
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OreMiner : InteractableObject
{
    //Jack's code
    //values for each ore barrel
    public List<OreBarrel> barrels;
    [Header("if set to -1 this ore will not spawn")]
    public float stoneMineTime = 1.0f;
    public float copperMineTime = 1.0f;
    public float tinMineTime = 1.0f;
    public float ironMineTime = 1.0f;
    public float goldMineTime = 1.0f;

    [Header("")]
    public int stoneID;
    public int copperID;
    public int tinID;
    public int ironID;
    public int goldID;

     float stoneTimer;
     float copperTimer;
     float tinTimer;
     float ironTimer;
     float goldTimer;

    OreBarrel stoneBarrel;
    OreBarrel copperBarrel;
    OreBarrel tinBarrel;
    OreBarrel ironBarrel;
    OreBarrel goldBarrel;

    ObjectProgressBar stoneBar;
    ObjectProgressBar copperBar;
    ObjectProgressBar tinBar;
    ObjectProgressBar ironBar;
    ObjectProgressBar goldBar;
    new private void Start()
    {
        base.Start();
        //find each barrel
        copperBarrel = FindBarrel(copperID);
        tinBarrel = FindBarrel(tinID);
        ironBarrel = FindBarrel(ironID);
        goldBarrel = FindBarrel(goldID);
        stoneBarrel = FindBarrel(stoneID);

        //set up timers for each barrel
        stoneTimer = stoneMineTime;
        copperTimer = copperMineTime;
        tinTimer = tinMineTime;
        ironTimer = ironMineTime;
        goldTimer = goldMineTime;

        //find the progress bar for each barrel
        if(stoneBarrel != null)
        stoneBar = stoneBarrel.GetComponentInChildren<ObjectProgressBar>();
        if (copperBarrel != null)
            copperBar = copperBarrel.GetComponentInChildren<ObjectProgressBar>();
        if (tinBarrel != null)
            tinBar = tinBarrel.GetComponentInChildren<ObjectProgressBar>();
        if (ironBarrel != null)
            ironBar = ironBarrel.GetComponentInChildren<ObjectProgressBar>();
        if (goldBarrel != null)
            goldBar = goldBarrel.GetComponentInChildren<ObjectProgressBar>();
    }

    new private void Update()
    {
        base.Update();
        //if the player hasnt interacted in a specific amount of time
        if (Time.time >= lastTimeInteracted + secondsBeforeProgressDecrease)
        {
            //decrease timers
            stoneTimer += decreaseAmountPerSecond * Time.deltaTime;
            copperTimer += decreaseAmountPerSecond * Time.deltaTime;
            tinTimer += decreaseAmountPerSecond * Time.deltaTime;
            ironTimer += decreaseAmountPerSecond * Time.deltaTime;
            goldTimer += decreaseAmountPerSecond * Time.deltaTime;
            //if bar is set to emprt set to default value
            if (stoneTimer > stoneMineTime)
            {
                stoneTimer = stoneMineTime;
            }
            if (copperTimer > copperMineTime)
            {
                copperTimer = copperMineTime;
            }
            if (tinTimer > tinMineTime)
            {
                tinTimer = tinMineTime;
            }
            if (ironTimer > ironMineTime)
            {
                ironTimer = ironMineTime;
            }
            if (goldTimer > goldMineTime)
            {
                goldTimer = goldMineTime;
            }
            //update the progress bars
            stoneBar.UpdateBar(stoneTimer, stoneMineTime);
            stoneBar.SetColor(DecreaseColor);

            copperBar.UpdateBar(copperTimer, copperMineTime);
            copperBar.SetColor(DecreaseColor);

            tinBar.UpdateBar(copperTimer, copperMineTime);
            tinBar.SetColor(DecreaseColor);

            ironBar.UpdateBar(copperTimer, copperMineTime);
            ironBar.SetColor(DecreaseColor);

            goldBar.UpdateBar(copperTimer, copperMineTime);
            goldBar.SetColor(DecreaseColor);
        }
    }
    public bool interact()
    {
        /*This will decrease the timers for each barrel if they are not full
         * Once the timer is full it will spawn in the new item inside of the barrel
         */
        lastTimeInteracted = Time.time;
        if (stoneBarrel != null && stoneBarrel.gameObject.activeSelf)
        {
            bool stoneFull = true;
            for (int i = 0; i < stoneBarrel.items.Count; i++)
            {
                if (stoneBarrel.items[i] == null)
                {
                    stoneFull = false;
                    break;
                }
            }
            if (!stoneFull)
            {
                stoneTimer -= Time.deltaTime * objectMultiplier;
                stoneBar.UpdateBar(stoneTimer, stoneMineTime);
                stoneBar.SetColor(IncreaseColor);

                if (stoneTimer <= 0 && stoneMineTime != -1)
                {
                    stoneBar.UpdateBar(1, 1);
                    Object temp = Instantiate(GetItemFromID(stoneID), stoneBarrel.transform).GetComponent<Object>();
                    temp = stoneBarrel.PickUpPlace(temp);
                    if (temp != null)
                        Destroy(temp);

                    stoneTimer = stoneMineTime;
                }
            }
            else
            {
                stoneBar.UpdateBar(1, 1);
            }
        }
        if (copperBarrel != null && copperBarrel.gameObject.activeSelf)
        {
            bool copperFull = true;
            for (int i = 0; i < copperBarrel.items.Count; i++)
            {
                if (copperBarrel.items[i] == null)
                {
                    copperFull = false;
                    break;
                }
            }
            if (!copperFull)
            {
                copperTimer -= Time.deltaTime * objectMultiplier;
                copperBar.UpdateBar(copperTimer, copperMineTime);
                copperBar.SetColor(IncreaseColor);
                if (copperTimer <= 0 && copperMineTime != -1)
                {
                    copperBar.UpdateBar(1, 1);
                    Object temp = Instantiate(GetItemFromID(copperID), copperBarrel.transform).GetComponent<Object>();
                    temp = copperBarrel.PickUpPlace(temp);
                    if (temp != null)
                        Destroy(temp);
                    copperTimer = copperMineTime;
                }
            }
            else
            {
                copperBar.UpdateBar(1, 1);
            }
        }
        if (tinBarrel != null && tinBarrel.gameObject.activeSelf)
        {
            bool tinFull = true;
            for (int i = 0; i < tinBarrel.items.Count; i++)
            {
                if (tinBarrel.items[i] == null)
                {
                    tinFull = false;
                    break;
                }
            }
            if (!tinFull)
            {
                tinTimer -= Time.deltaTime * objectMultiplier;
                tinBar.UpdateBar(tinTimer, tinMineTime);
                tinBar.SetColor(IncreaseColor);
                if (tinTimer <= 0 && tinMineTime != -1)
                {


                    Object temp = Instantiate(GetItemFromID(tinID), tinBarrel.transform).GetComponent<Object>();
                    temp = tinBarrel.PickUpPlace(temp);
                    if (temp != null)
                        Destroy(temp);
                    copperBar.UpdateBar(1, 1);
                    tinTimer = tinMineTime;
                }
            }
            else
            {
                tinBar.UpdateBar(1, 1);
            }
        }
        if (ironBarrel != null && ironBarrel.gameObject.activeSelf)
        {
            bool ironFull = true;
            for (int i = 0; i < ironBarrel.items.Count; i++)
            {
                if (ironBarrel.items[i] == null)
                {
                    ironFull = false;
                    break;
                }
            }
            if (!ironFull)
            {
                ironTimer -= Time.deltaTime * objectMultiplier;
                ironBar.UpdateBar(ironTimer, ironMineTime);
                ironBar.SetColor(IncreaseColor);
                if (ironTimer <= 0 && ironMineTime != -1)
                {


                    Object temp = Instantiate(GetItemFromID(ironID), ironBarrel.transform).GetComponent<Object>();
                    temp = ironBarrel.PickUpPlace(temp);
                    if (temp != null)
                        Destroy(temp);
                    copperBar.UpdateBar(1, 1);
                    ironTimer = ironMineTime;
                }
            }
            else
            {
                ironBar.UpdateBar(1, 1);
            }
        }
        if (goldBarrel != null && goldBarrel.gameObject.activeSelf)
        {
            bool goldFull = true;
            for (int i = 0; i < goldBarrel.items.Count; i++)
            {
                if (goldBarrel.items[i] == null)
                {
                    goldFull = false;
                    break;
                }
            }
            if (!goldFull)
            {
                goldTimer -= Time.deltaTime * objectMultiplier;
                goldBar.UpdateBar(goldTimer, goldMineTime);
                goldBar.SetColor(IncreaseColor);
                if (goldTimer <= 0 && goldMineTime != -1)
                {

                    Object temp = Instantiate(GetItemFromID(goldID), goldBarrel.transform).GetComponent<Object>();
                    temp = goldBarrel.PickUpPlace(temp);
                    if (temp != null)
                        Destroy(temp);
                    copperBar.UpdateBar(1, 1);

                    goldTimer = goldMineTime;
                }
            }
            else
            {
                goldBar.UpdateBar(1, 1);
            }
        }
        return true;
    }

    private OreBarrel FindBarrel(int id)
    {
        foreach(OreBarrel barrel in barrels)
        {
            if(barrel.gameObject.activeSelf && barrel.storedResource == id)
            {
                return barrel;
            }
        }
        return null;
    }
}
//End of Jack's code
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Jack's code
/// <summary>
/// make sure salePrice[1] is equal to soldValue[1] for this to show the right price for each item
/// </summary>
[System.Serializable]
public struct saleValues
{
    public WorktopType worktopType;
    /// <summary>
    /// the price that the item is sold for
    /// </summary>
    public List<int> salePrice;
    /// <summary>
    /// the new speed / storage size that they are purchasing
    /// </summary>
    public List<float> soldValue;
}
public class ShopUI : MonoBehaviour
{
    public ObjectMultiplierStore objectMultiplierStore;
    [Header("Make sure salePrice[1] is equal to soldValue[1] for this to show the right price for each item")]
    public List<saleValues> sellableItems;

    public List<shopItem> generatedSellPrefabs;

    public GameManager gameManager;

    public CanvasGroup shopUI;

    private void Start()
    {
        //get required scripts
        shopUI = this.GetComponent<CanvasGroup>();
        objectMultiplierStore = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ObjectMultiplierStore>();
        if (!objectMultiplierStore)
            Debug.LogError("could not find object multipler store in scene");
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>(); //NEEDS CHANGING WHEN SOTRED IN RIGHT POSITION
        if (!gameManager)
            Debug.LogError("could not find Game Manager in scene");
        //set prices to default values
        UpdatePrices();
    }

    public void UpdatePrices()
    {
        //for each item on screen
        foreach (saleValues values in sellableItems)
        {
            //for each prefab
            foreach(shopItem item in generatedSellPrefabs)
            {
                //if the prefab is for the values type
                if (item.type == values.worktopType)
                {
                    for (int i = 0; i < values.soldValue.Count; i++)
                    {
                        //depending on the item type find the current value and set the ui to display the price of the upgrade and the upgrade amount
                        switch (item.type)
                        {
                            case WorktopType.COUNTER:
                                if (values.soldValue[i] == objectMultiplierStore.multipliers.counterMultipler)
                                {
                                    if (i < values.soldValue.Count - 1)
                                    {
                                        item.price.text = values.salePrice[i + 1].ToString();
                                        item.info.text = "Increase crafting speed by " + ((values.soldValue[i + 1] - 1) * 100) + "%";//-1 to get increase amount and multiply by 100 to get percentage (1.1-1)*100=10
                                    }
                                    else
                                    {
                                        item.sold.alpha = 1;
                                        item.saleText.interactable = false;
                                        item.sold.blocksRaycasts = true;
                                    }
                                }
                                break;
                            case WorktopType.CRUCIBLE:
                                if (values.soldValue[i] == objectMultiplierStore.multipliers.crucibleMultipler)
                                {
                                    if (i < values.soldValue.Count - 1)
                                    {
                                        item.price.text = values.salePrice[i + 1].ToString();
                                        item.info.text = "Increase smelting speed by " + ((values.soldValue[i + 1] - 1) * 100) + "%";//-1 to get increase amount and multiply by 100 to get percentage (1.1-1)*100=10
                                    }
                                    else
                                    {
                                        item.sold.alpha = 1;
                                        item.saleText.interactable = false;
                                        item.sold.blocksRaycasts = true;
                                    }
                                }
                                break;
                            case WorktopType.COMBINER:
                                if (values.soldValue[i] == objectMultiplierStore.multipliers.combinerMultipler)
                                {
                                    if (i < values.soldValue.Count - 1)
                                    {
                                        item.price.text = values.salePrice[i + 1].ToString();
                                        item.info.text = "Increase crafting speed by " + ((values.soldValue[i + 1] - 1) * 100) + "%";//-1 to get increase amount and multiply by 100 to get percentage (1.1-1)*100=10
                                    }
                                    else
                                    {
                                        item.sold.alpha = 1;
                                        item.saleText.interactable = false;
                                        item.sold.blocksRaycasts = true;
                                    }
                                }
                                break;
                            case WorktopType.ANVIL:
                                if (values.soldValue[i] == objectMultiplierStore.multipliers.anvilMultipler)
                                {
                                    if (i < values.soldValue.Count - 1)
                                    {
                                        item.price.text = values.salePrice[i + 1].ToString();
                                        item.info.text = "Increase crafting speed by " + ((values.soldValue[i + 1] - 1) * 100) + "%";//-1 to get increase amount and multiply by 100 to get percentage (1.1-1)*100=10
                                    }
                                    else
                                    {
                                        item.sold.alpha = 1;
                                        item.saleText.interactable = false;
                                        item.sold.blocksRaycasts = true;
                                    }
                                }
                                break;
                            case WorktopType.MINER:
                                if (values.soldValue[i] == objectMultiplierStore.multipliers.oreMinerMultipler)
                                {
                                    if (i < values.soldValue.Count - 1)
                                    {
                                        item.price.text = values.salePrice[i + 1].ToString();
                                        item.info.text = "Increase mining speed by " + ((values.soldValue[i+1] - 1) * 100) + "%";//-1 to get increase amount and multiply by 100 to get percentage (1.1-1)*100=10
                                    }
                                    else
                                    {
                                        item.sold.alpha = 1;
                                        item.saleText.interactable = false;
                                        item.sold.blocksRaycasts = true;
                                    }
                                }
                                break;

                            case WorktopType.STONEPILE:
                                if (values.soldValue[i] == objectMultiplierStore.multipliers.stoneBarrelStorage)
                                {
                                    if (i < values.soldValue.Count - 1)
                                    {
                                        item.price.text = values.salePrice[i + 1].ToString();
                                        item.info.text = "Increase storage size to " + values.soldValue[i + 1]+" items";//-1 to get increase amount and multiply by 100 to get percentage (1.1-1)*100=10
                                    }
                                    else
                                    {
                                        item.sold.alpha = 1;
                                        item.saleText.interactable = false;
                                        item.sold.blocksRaycasts = true;
                                    }
                                }
                                break;
                            case WorktopType.COPPEROREPILE:
                                if (values.soldValue[i] == objectMultiplierStore.multipliers.copperBarrelStorage)
                                {
                                    if (i < values.soldValue.Count - 1)
                                    {
                                        item.price.text = values.salePrice[i + 1].ToString();
                                        item.info.text = "Increase storage size to " + values.soldValue[i + 1] + " items";//-1 to get increase amount and multiply by 100 to get percentage (1.1-1)*100=10
                                    }
                                    else
                                    {
                                        item.sold.alpha = 1;
                                        item.saleText.interactable = false;
                                        item.sold.blocksRaycasts = true;
                                    }
                                }
                                break;
                            case WorktopType.TINOREPILE:
                                if (values.soldValue[i] == objectMultiplierStore.multipliers.tinBarrelStorage)
                                {
                                    if (i < values.soldValue.Count - 1)
                                    {
                                        item.price.text = values.salePrice[i + 1].ToString();
                                        item.info.text = "Increase storage size to " + values.soldValue[i + 1] + " items";//-1 to get increase amount and multiply by 100 to get percentage (1.1-1)*100=10
                                    }
                                    else
                                    {
                                        item.sold.alpha = 1;
                                        item.saleText.interactable = false;
                                        item.sold.blocksRaycasts = true;
                                    }
                                }
                                break;
                            case WorktopType.IRONOREPILE:
                                if (values.soldValue[i] == objectMultiplierStore.multipliers.ironBarrelStorage)
                                {
                                    if (i < values.soldValue.Count - 1)
                                    {
                                        item.price.text = values.salePrice[i + 1].ToString();
                                        item.info.text = "Increase storage size to " + values.soldValue[i + 1] + " items";//-1 to get increase amount and multiply by 100 to get percentage (1.1-1)*100=10
                                    }
                                    else
                                    {
                                        item.sold.alpha = 1;
                                        item.saleText.interactable = false;
                                        item.sold.blocksRaycasts = true;
                                    }
                                }
                                break;
                            case WorktopType.GOLDOREPILE:
                                if (values.soldValue[i] == objectMultiplierStore.multipliers.goldBarrelStorage)
                                {
                                    if (i < values.soldValue.Count - 1)
                                    {
                                        item.price.text = values.salePrice[i + 1].ToString();
                                        item.info.text = "Increase storage size to " + values.soldValue[i + 1] + " items";//-1 to get increase amount and multiply by 100 to get percentage (1.1-1)*100=10
                                    }
                                    else
                                    {
                                        item.sold.alpha = 1;
                                        item.saleText.interactable = false;
                                        item.sold.blocksRaycasts = true;
                                    }
                                }
                                break;
                            

                        }
                    }
                }
            }
        }
    }

    public void Buy(int typeAffected)
    {
        //when a button is pressed
        bool found = false;
        foreach (saleValues values in sellableItems)
        {
            //find worktop type equal to the type it affects
            if((int)values.worktopType == typeAffected)
            {
                found = true;
                bool complete = false;
                //ignore the last sold value as it cannot be bought
                for (int i = 0; i < values.soldValue.Count - 1; i++)
                {
                    //if they have enough money update the multipler to the new value
                    if (values.salePrice[i + 1] <= gameManager.coins)
                    {

                        switch (typeAffected)
                        {
                            case (int)WorktopType.COUNTER:
                                if (values.soldValue[i] == objectMultiplierStore.multipliers.counterMultipler)
                                {
                                    objectMultiplierStore.multipliers.counterMultipler = values.soldValue[i + 1];
                                    gameManager.AddCoins(-values.salePrice[i + 1]);
                                    complete = true;
                                }
                                break;
                            case (int)WorktopType.CRUCIBLE:
                                if (values.soldValue[i] == objectMultiplierStore.multipliers.crucibleMultipler)
                                {
                                    objectMultiplierStore.multipliers.crucibleMultipler = values.soldValue[i + 1];
                                    gameManager.AddCoins(-values.salePrice[i + 1]);
                                    complete = true;
                                }
                                break;
                            case (int)WorktopType.COMBINER:
                                if (values.soldValue[i] == objectMultiplierStore.multipliers.combinerMultipler)
                                {
                                    objectMultiplierStore.multipliers.combinerMultipler = values.soldValue[i + 1];
                                    gameManager.AddCoins(-values.salePrice[i + 1]);
                                    complete = true;
                                }
                                break;
                            case (int)WorktopType.ANVIL:
                                if (values.soldValue[i] == objectMultiplierStore.multipliers.anvilMultipler)
                                {
                                    objectMultiplierStore.multipliers.anvilMultipler = values.soldValue[i + 1];
                                    gameManager.AddCoins(-values.salePrice[i + 1]);
                                    complete = true;
                                }
                                break;
                            case (int)WorktopType.MINER:
                                if (values.soldValue[i] == objectMultiplierStore.multipliers.oreMinerMultipler)
                                {
                                    objectMultiplierStore.multipliers.oreMinerMultipler = values.soldValue[i + 1];
                                    gameManager.AddCoins(-values.salePrice[i + 1]);
                                    complete = true;
                                }
                                break;

                            case (int)WorktopType.STONEPILE:
                                if (values.soldValue[i] == objectMultiplierStore.multipliers.stoneBarrelStorage)
                                {
                                    objectMultiplierStore.multipliers.stoneBarrelStorage = (int)values.soldValue[i + 1];
                                    gameManager.AddCoins(-values.salePrice[i + 1]);
                                    complete = true;
                                }
                                break;
                            case (int)WorktopType.COPPEROREPILE:
                                if (values.soldValue[i] == objectMultiplierStore.multipliers.copperBarrelStorage)
                                {
                                    objectMultiplierStore.multipliers.copperBarrelStorage = (int)values.soldValue[i + 1];
                                    gameManager.AddCoins(-values.salePrice[i + 1]);
                                    complete = true;
                                }
                                break;
                            case (int)WorktopType.TINOREPILE:
                                if (values.soldValue[i] == objectMultiplierStore.multipliers.tinBarrelStorage)
                                {
                                    objectMultiplierStore.multipliers.tinBarrelStorage = (int)values.soldValue[i + 1];
                                    gameManager.AddCoins(-values.salePrice[i + 1]);
                                    complete = true;
                                }
                                break;
                            case (int)WorktopType.IRONOREPILE:
                                if (values.soldValue[i] == objectMultiplierStore.multipliers.ironBarrelStorage)
                                {
                                    objectMultiplierStore.multipliers.ironBarrelStorage = (int)values.soldValue[i + 1];
                                    gameManager.AddCoins(-values.salePrice[i + 1]);
                                    complete = true;
                                }
                                break;
                            case (int)WorktopType.GOLDOREPILE:
                                if (values.soldValue[i] == objectMultiplierStore.multipliers.goldBarrelStorage)
                                {
                                    objectMultiplierStore.multipliers.goldBarrelStorage = (int)values.soldValue[i + 1];
                                    gameManager.AddCoins(-values.salePrice[i + 1]);
                                    complete = true;
                                }
                                break;
                        }
                        if (complete)
                            break;

                    }
                }

                //update the multipliers for each item in the scene
                foreach (GameObject obj in FindObjectsOfType<GameObject>())
                {
                    InteractableObject objScript = obj.GetComponent<InteractableObject>();
                    if(objScript != null)
                    {
                        objScript.UpdateMultipliers();
                    }

                }
                //update hte prices to the new price
                UpdatePrices();
                break;
            }
        }
        if(!found)
        {
            Debug.Log("worktop type " + typeAffected + " should not be set as a button as its speed cannot be changed"); 
        }
    }

    public void CloseButton()
    {
        //close the shop ui
        if (shopUI != null)
        {
            shopUI.alpha = 0;
            shopUI.interactable = false;
            shopUI.blocksRaycasts = false;
            Time.timeScale = 1;
        }
    }
}
//end of Jack's code

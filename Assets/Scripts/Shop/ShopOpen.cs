using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ShopOpen : MonoBehaviour
{
    //Jack's code
    public CanvasGroup shopUI;
    private void Awake()
    {
        //find shop ui and turn off
        shopUI = GameObject.FindGameObjectWithTag("ShopUI").GetComponent<CanvasGroup>();
        if (shopUI != null)
        {
            shopUI.alpha = 0;
            shopUI.interactable = false;
            shopUI.blocksRaycasts = false;
        }
    }
    public void OnShop()
    {
        //when called check the shop ui has been found
        if(shopUI == null)
        {
            shopUI = GameObject.FindGameObjectWithTag("ShopUI").GetComponent<CanvasGroup>();
        }
        if (!GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().playPahse && shopUI != null)
        {
            //if turned on then turn off
            if (shopUI.alpha == 1)
            {
                shopUI.alpha = 0;
                shopUI.interactable = false;
                shopUI.blocksRaycasts = false;
                Time.timeScale = 1;
            }
            //if turned off and the game isnt paused then turn on
            else if (Time.timeScale != 0)
            {
                shopUI.alpha = 1;
                shopUI.interactable = true;
                shopUI.blocksRaycasts = true;
                Time.timeScale = 0;
                shopUI.gameObject.GetComponent<ShopUI>().UpdatePrices();
            }
        }
    }

    //turn off ui if button is pressed
    public void CloseButton()
    {
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

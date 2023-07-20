using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDDisplayNumbers : MonoBehaviour
{
    //Jasons Code
    //references to text boxs on player user interface
    public TMP_Text coinText;
    public TMP_Text expectedCustomerText;
    public TMP_Text dayText;
    public Slider sliderDisplay;

    public GameObject StartDayUI;
    public GameObject ShopButtonUI;

    float testInt;

    //displaying coins on player UI
    public void SetCoins(int v)
    {
        coinText.text = v.ToString();
    }

    //displaying the exspected customers for the day on the player UI
    public void SetExpectedCustomers(int lower, int higher)
    {
        expectedCustomerText.text = lower.ToString() + " - " + higher.ToString();
    }

    //displaying the current day on the player UI
    public void SetDay(int m)
    {
        dayText.text = m.ToString();
    }

    //setting the slider to zero
    public void SetSlider(float o)
    {
        sliderDisplay.value = o;
    }

    //setting the slider to display
    public void SetSliderVisability(bool on)
    {
        sliderDisplay.transform.parent.gameObject.SetActive(on);
        StartDayUI.SetActive(!on);
        ShopButtonUI.SetActive(!on);
    }

    //setting the expected customer visability
    public void SetExpectedCustomerVisability(bool on)
    {
        expectedCustomerText.gameObject.SetActive(on);
    }
}

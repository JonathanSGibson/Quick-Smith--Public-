using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//Jack's code
public class shopItem : MonoBehaviour
{
    //stores the data for what to display on each shop item
    public WorktopType type;
    public TextMeshProUGUI price;
    public TextMeshProUGUI info;
    public CanvasGroup saleText;
    public CanvasGroup sold;

    void Start()
    {
        sold.alpha = 0;
        sold.interactable = false;
        sold.blocksRaycasts = false;
        saleText.alpha = 1;
        saleText.interactable = true;
    }
}
//end of Jack's code

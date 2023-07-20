using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusOrderVisuals : MonoBehaviour
{
    //Jasons Code
    //references to game objects 
    public List<GameObject> crates;
    public BulkOrderCounter numberOfSwords;
    public GameManager setThreshold;

    private void Start()
    {
        setThreshold = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    //setting all crates to inactive when called(end of day)
    public void EmptyCart()
    {
        foreach (GameObject box in crates)
        {
            box.SetActive(false); 
        }
    }

    public void CrateStatus()
    {
        //sets the first crate active when the first sword is placed 
        if (numberOfSwords.GetCurrentItems() > 0 && crates[0].activeSelf == false)
        {
            crates[0].SetActive(true);
        }

        int cratesIndex = 0;

        //checks the number of swords against the threshold
        foreach(Vector2Int pair in setThreshold.GetCurrentDay().bulkThresholdsAndValues)
        {
            if (numberOfSwords.GetCurrentItems() >= pair.x)
                cratesIndex++;
            else
                break;
        }

        //sets each crate to active based on the above threshold 
        for (int i = 0; i <= cratesIndex; i++)
        {
            crates[i].SetActive(true);
        }
    }
}

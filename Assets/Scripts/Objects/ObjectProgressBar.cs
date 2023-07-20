using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RengeGames.HealthBars;
using UnityEngine.UI;
//Jack's code
public class ObjectProgressBar : MonoBehaviour
{
    Camera mainCamera;

    public RadialSegmentedHealthBar itemProgressBar;
    public Image progressBar;

    bool onCustomer = false; //used to change it to empty instead of fill
    
    void Start()
    {
        mainCamera = Camera.main; //find the camera

        itemProgressBar.SetPercent(0); //set the progressbar to be empty
    }

    void Update()
    {
        transform.LookAt(mainCamera.transform);//set to look at the camera so that it is at the right angle
        transform.transform.up = mainCamera.transform.up; // set to face up based on the camera

        if(Time.timeScale == 0 && progressBar.enabled) //if the game is paused turn off so that it doesn show in background
        { 
            progressBar.enabled = false;
        }
        if(Time.timeScale != 0 && !progressBar.enabled) //if the game is player set active
        {
            progressBar.enabled = true;
        }
    }

    public void UpdateBar(float timeLeft, float timeToComplete)
    {
        float value = 1 - (timeLeft / timeToComplete); //update the percent of the bar

        itemProgressBar.SetPercent(value);
    }

    public void UpdateBar(float value) //override to be able to give in as a value between 0 and 1

    {
        value = 1 - value;

        itemProgressBar.SetPercent(value);
    }
    public void SetColor(Color color) //allows the color of the progress bar to be set
    {
        itemProgressBar.InnerColor.Value = color;
        
    }
}
//End of Jack's code

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    //Jasons Code
    //setting the canvas groups
    public CanvasGroup mainMenu;
    public CanvasGroup tutorialPlayQuestion;

    public TMP_Text currentProfile;

    //setting the canvas to hidden at the start of the game
    private void Start()
    {
        tutorialPlayQuestion.alpha = 0;
        tutorialPlayQuestion.interactable = false;
        tutorialPlayQuestion.blocksRaycasts = false;
        UpdateCurrentProfile();
    }

    //changing the canvas between the two when the button is pressed
    public void play()
    {
        tutorialPlayQuestion.alpha = 1;
        tutorialPlayQuestion.interactable = true;
        tutorialPlayQuestion.blocksRaycasts = true;

        mainMenu.alpha = 0;
        mainMenu.interactable = false;
        mainMenu.blocksRaycasts = false;
    }

    //quitting the game set up
    public void quit()
    {
        Application.Quit();
    }

    //playing the tutorial set up
    public void playTutorial()
    {
        SceneManager.LoadScene(1);
    }

    //loading the playable level setup
    public void playLevel1()
    {
        SceneManager.LoadScene(2);
    }

    //switching between the two canvas groups on button press
    public void ExitTutorial()
    {
        tutorialPlayQuestion.alpha = 0;
        tutorialPlayQuestion.interactable = false;
        tutorialPlayQuestion.blocksRaycasts = false;

        mainMenu.alpha = 1;
        mainMenu.interactable = true;
        mainMenu.blocksRaycasts = true;
    }

    //update player profile on button press
    public void UpdateCurrentProfile()
    {
        currentProfile.text = "Current Profile:\n" + SaveManager.Instance.getCurrentProfile().name;
    }
}


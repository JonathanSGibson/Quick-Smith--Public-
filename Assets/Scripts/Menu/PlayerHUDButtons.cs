using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerHUDButtons : MonoBehaviour
{
    //Jason Code
    //setting up canvas groups
    public CanvasGroup pauseMenu;
    public CanvasGroup playerHUD;
    public CanvasGroup failScreen;
    public CanvasGroup victoryScreen;

    //bool setup for displaying the correct canvas group
    public bool gamePaused = false;
    public GameManager fail;
    public bool gameSuccess = false;
    public bool isTutorial = false;

    //setting the nescesary canvas groups to be inactive at the start of the game
    void Start()
    {
        Time.timeScale = 1;
        pauseMenu.alpha = 0;
        pauseMenu.interactable = false;
        pauseMenu.blocksRaycasts = false;

        failScreen.alpha = 0;
        failScreen.interactable = false;
        failScreen.blocksRaycasts = false;

        victoryScreen.alpha = 0;
        victoryScreen.interactable = false;
        victoryScreen.blocksRaycasts = false;
    }

    //setup for resuming the game during the pause menu
    public void resume()
    {
        Time.timeScale = 1;
        pauseMenu.alpha = 0;
        pauseMenu.interactable = false;
        pauseMenu.interactable = false;

        playerHUD.alpha = 1;
        playerHUD.interactable = true;
        playerHUD.blocksRaycasts = true;
        gamePaused = false;
    }

    //set up for returning to the main menu
    public void mainMenu()
    {
        if (PlayerHandler.Instance != null)
            PlayerHandler.Instance.DespawnAllPlayers();
        SceneManager.LoadScene(0);
    }

    //setup for quiting the game
    public void quit()
    {
        if (!isTutorial)
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().QuitSave();//save if in prepphase
        Application.Quit();
    }

    private void Update()
    {
        if (!isTutorial)
        {
            //displaying pause menu when the game is paused
            if (gamePaused == true && fail.GetComponent<GameManager>().gameFailed == false && !gameSuccess)
            {
                Time.timeScale = 0;
                pauseMenu.alpha = 1;
                pauseMenu.interactable = true;
                pauseMenu.blocksRaycasts = true;

                playerHUD.alpha = 0;
                playerHUD.interactable = false;
                playerHUD.blocksRaycasts = false;
            }

            //displaying the failed menu when the player fails
            if (fail.GetComponent<GameManager>().gameFailed == true && !gameSuccess)
            {
                Time.timeScale = 0;
                failScreen.alpha = 1;
                failScreen.interactable = true;
                failScreen.blocksRaycasts = true;

                playerHUD.alpha = 0;
                playerHUD.interactable = false;
                playerHUD.blocksRaycasts = false;
            }

            //(not used) setting success screen when the player wins
            if (gameSuccess == true && fail.GetComponent<GameManager>().gameFailed == false)
            {
                Time.timeScale = 0;
                victoryScreen.alpha = 1;
                victoryScreen.interactable = true;
                victoryScreen.blocksRaycasts = true;

                playerHUD.alpha = 0;
                playerHUD.interactable = false;
                playerHUD.blocksRaycasts = false;
            }
        }
        else if (gamePaused)
        {
            Time.timeScale = 0;
            pauseMenu.alpha = 1;
            pauseMenu.interactable = true;
            pauseMenu.blocksRaycasts = true;

            playerHUD.alpha = 0;
            playerHUD.interactable = false;
            playerHUD.blocksRaycasts = false;
        }
    }

    public void Pause()
    {
        gamePaused = true;
    }

    /// <summary>
    /// Pauses the game if the game is minimised (This function is an addition by Jonathan)
    /// </summary>
    /// <param name="focus">Whether the game is the focused window or not</param>
    private void OnApplicationFocus(bool focus)
    {
        GameObject shop = GameObject.FindGameObjectWithTag("ShopUI");
        if (!focus && !(shop != null && shop.GetComponent<ShopUI>().shopUI.alpha == 1))
            Pause();
    }
}

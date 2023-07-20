using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManagerDoneButton : MonoBehaviour
{
    /* This script is entirely made by Jonathan
     * 
     * This script controls what happens when the done button is pressed on the player manager ui,
     * only inside of the pause menu! Not on main menu! Main menu's done button is handled entirely in the inspector
     */
    bool firstTime = true; // if this is the first time the player has gone to this UI (If so it goes straight to gameplay after done is pressed rather than the pause menu)
    public GameObject pauseMenu;
    public GameObject playerManagerUI;

    public void SetFirstTime(bool _firstTime)
    {
        firstTime = _firstTime;
    }

    /// <summary>
    /// Disables the player manager UI and either goes to the gameplay or goes to the pause menu based on the firsTime bool
    /// </summary>
    public void DonePressed()
    {   
        if (firstTime)
        {
            GameObject.FindGameObjectWithTag("HUD").GetComponent<PlayerHUDButtons>().resume();
            pauseMenu.SetActive(true);
            playerManagerUI.SetActive(false);
            firstTime = false;
        }
        else
        {
            pauseMenu.SetActive(true);
            playerManagerUI.SetActive(false);
        }
        PlayerHandler.Instance.UpdateOutlineColor();
        GameObject.FindGameObjectWithTag("PlayerSpawner").GetComponent<PlayerSpawner>().SpawnPlayers(); //Spawns all unspawned players
    }
}

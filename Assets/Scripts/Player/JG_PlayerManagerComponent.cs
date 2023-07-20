using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerManagerComponent : MonoBehaviour
{
    /* This script is entirely made by Jonathan
     * 
     * This controls the UI for adding/removing players
     * 
     * This script controls each individual component of the UI to display active players, add players and remove players
     */

    public Button playerDeleteButton; // button to delete player
    public TextMeshProUGUI playerIndex; //which player slot it is (eg "Player 1")
    public TextMeshProUGUI inputDeviceName; //control scheme currently in use by that player
    public TextMeshProUGUI joinMessage; //Message saying slot can be joined ie "Press any button to join"

    /// <summary>
    /// If the player slot is active, set the components displaying information about the player active and disables the text saying to join
    /// </summary>
    public void PlayerActive()
    {
        playerIndex.gameObject.SetActive(true);
        inputDeviceName.gameObject.SetActive(true);
        joinMessage.gameObject.SetActive(false);
    }

    /// <summary>
    /// If the player slot is inactive, disable the components displaying information about the player and activate the text saying to join
    /// </summary>
    public void PlayerInactive()
    {
        playerIndex.gameObject.SetActive(false);
        inputDeviceName.gameObject.SetActive(false);
        joinMessage.gameObject.SetActive(true);
    }

    /// <summary>
    /// Updates the informational UI with all required information from the player corresponding to this component
    /// </summary>
    /// <param name="playerInfo">The information being provided to update the UI elements</param>
    public void UpdateUIElements(PlayerInformationForUI playerInfo)
    {
        if (!playerInfo.slotActive)
            PlayerInactive();
        else
        {
            inputDeviceName.text = playerInfo.inputDeviceName;
            playerIndex.text = "Player " + (playerInfo.index + 1).ToString();
            PlayerActive();
        }
    }

    /// <summary>
    /// The function to be called by the delete player button, calls a function to delete the player
    /// </summary>
    public void RemovePlayerButton()
    {
        GetComponentInParent<PlayerManagerUI>().RemovePlayer(gameObject);
    }
}

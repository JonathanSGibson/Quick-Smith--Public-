using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWrapper : MonoBehaviour
{
    /* This script is mostly made by Jonathan,
     * with some additions/modifications made by Jack
     * 
     * This script acts as a wrapper around the player and takes their inputs,
     * this is so playerwrappers can be spawned with unity's inbuilt input manager
     * without having to spawn the player character
     */
    GameObject player; //The player character

    /// <summary>
    /// Sets the player character as the first child of the wrapper
    /// </summary>
    private void Awake()
    {
        player = transform.GetChild(0).gameObject;
    }

    /// <summary>
    /// Spawns (enables) the player character
    /// </summary>
    /// <param name="spawnPos">The position to be spawned at</param>
    /// <param name="color">The colour of the player's outline</param>
    public void SpawnPlayer(Vector3 spawnPos, Color color)
    {
        player.SetActive(true);
        player.transform.position = spawnPos;
        setOutlineColor(color); //This is jack's addition (as is the colour parameter)
    }

    /// <summary>
    /// Despawns (disables) the player character
    /// </summary>
    public void DespawnPlayer()
    {
        player.SetActive(false);
    }

    //Start of functions which just take inputs and pass them to the player

    public void OnMovement(InputValue input)
    {
        if (player.active)
            transform.GetComponentInChildren<Movement>().OnMovement(input);
    }

    public void OnStationary(InputValue input)
    {
        if (player.gameObject.active)
            transform.GetComponentInChildren<Movement>().OnStationary(input);
    }

    public void OnInteract(InputValue input)
    {
        if (player.gameObject.active)
            transform.GetComponentInChildren<PlayerInteract>().OnInteract(input);
    }

    public void OnPickupPlace(InputValue input)
    {
        if (player.gameObject.active)
            transform.GetComponentInChildren<PlayerInteract>().OnPickupPlace(input);
    }

    public void OnPause(InputValue input)
    {
        if (player.gameObject.active)
            transform.GetComponentInChildren<PlayerInteract>().OnPause(input);
    }

    public void OnShop(InputValue input)
    {
        if (player.gameObject.active)
            transform.GetComponentInChildren<ShopOpen>().OnShop();
    }

    public void OnStartDay()
    {
        if (player.gameObject.active)
            transform.GetComponentInChildren<PlayerInteract>().OnStartDay();
    }

    public bool CheckActive()
    {
        return transform.GetChild(0).gameObject.activeSelf;
    }

    //End of input passing

    /// <summary>
    /// If the player inputs the delete button calls the function to delete the related player
    /// This is so a player can be deleted from the controller which controls it not just from the mouse
    /// </summary>
    public void OnDeletePlayer()
    {
        GameObject temp = GameObject.FindGameObjectWithTag("PlayerManagerUI");
        if (temp != null && temp.activeInHierarchy)
            GetComponentInParent<PlayerHandler>().RemovePlayerFromWrapper(gameObject);
    }

    

    /// <summary>
    /// Sets the outline colour of the child (This is Jack's function)
    /// </summary>
    /// <param name="color">The colour to be set to</param>
    public void setOutlineColor(Color color)
    {
        transform.GetChild(0).GetComponentInChildren<Outline>().OutlineColor = color;
        transform.GetChild(0).GetComponentInChildren<Outline>().OutlineMode = Outline.Mode.OutlineVisible;
    }

}
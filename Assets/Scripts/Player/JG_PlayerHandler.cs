using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHandler : MonoBehaviour
{
    /* This script is mostly made by Jonathan,
     * with additions from Jack
     * 
     * This script handles all the players, their creation, destruction and spawning
     */


    // used this for making a singleton in unity https://gamedevbeginner.com/singletons-in-unity-the-right-way/


    public int maxPlayers; //Maximum number of players the game allows
    public static PlayerHandler Instance { get; private set; }

    public List<Color> playerColors; //Colours for multiplayer outline, made by Jack

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

    }

    /// <summary>
    /// Fetches the maximum number of players
    /// </summary>
    /// <returns>The maximum number of players</returns>
    public int GetMaxPlayers()
    {
        return maxPlayers;
    }

    /// <summary>
    /// Removes a player by deleting the playerwrapper at that index (all playerwrappers are children of the player handler)
    /// </summary>
    /// <param name="index">The index of the player to be deleted</param>
    public void RemovePlayer(int index)
    {
        if (index < transform.childCount)
            Destroy(transform.GetChild(index).gameObject);
    }

    /// <summary>
    /// Removes a player by deleting the wrapper, does so by being given by the wrapper rather than via index
    /// </summary>
    /// <param name="wrapper">The wrapper to delete</param>
    public void RemovePlayerFromWrapper(GameObject wrapper)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject == wrapper)
                RemovePlayer(i);
        }
    }

    /// <summary>
    /// Fetches information from each player that is required for the multiplayer menu
    /// </summary>
    /// <param name="index">The index of the player to get the information from</param>
    /// <returns>The information from the player</returns>
    public PlayerInformationForUI GetPlayerInformationForUI(int index)
    {
        if (index < transform.childCount)
        {
            string inputDeviceName = transform.GetChild(index).GetComponent<PlayerInput>().devices[0].displayName;
            return new PlayerInformationForUI(true, index, inputDeviceName);
        }
        else
        {
            return new PlayerInformationForUI(false);
        }
    }

    /// <summary>
    /// Enables each player within their playerwrapper if not already enabled, effectively spawning the player in a playable state
    /// </summary>
    /// <param name="startSpawn">Position of the first player spawned</param>
    /// <param name="xOffset">Distance between each player in X</param>
    /// <param name="zOffset">Distance between each player in Z</param>
    public void SpawnAllPlayers(Vector3 startSpawn, float xOffset, float zOffset)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (!transform.GetChild(i).GetComponent<PlayerWrapper>().CheckActive())
            {
                transform.GetChild(i).GetComponent<PlayerWrapper>().SpawnPlayer(startSpawn, playerColors[i]);
                startSpawn.x += xOffset;
                startSpawn.z += zOffset;
            }
        }
    }

    /// <summary>
    /// Disables all players via their playerwrapper, effectively despawning them
    /// </summary>
    public void DespawnAllPlayers()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<PlayerWrapper>().CheckActive())
            {
                transform.GetChild(i).GetComponent<PlayerWrapper>().DespawnPlayer();
            }
        }
    }

    /// <summary>
    /// When a player joins via unity's inputmanager component (Which is enabled/disabled via the ui) checks if the input is from a mouse (as we do not want a player which is only assigned to a mouse), if so it is deleting if not resets their transform
    /// </summary>
    /// <param name="player"></param>
    public void OnPlayerJoined(PlayerInput player)
    {
        //Immediately deletes
        if (player.devices[0].displayName == "Mouse" || player.devices[0].displayName == "mouse")
            Destroy(player.gameObject);
        else
            player.gameObject.transform.parent = gameObject.transform;
    }

    /// <summary>
    /// Sets the outline colour of each player (Jack's code)
    /// </summary>
    public void UpdateOutlineColor()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
           transform.GetChild(i).GetComponent<PlayerWrapper>().setOutlineColor(playerColors[i]);
        }
    }
}


    



//All children are player wrappers
//When a player joins they create a playerwrapper which is a child

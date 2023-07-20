using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    /*The script is entirely made by Jonathan
     * 
     * This script controls the spawning of players from their playerwrappers in the level
     */
    public Vector3 spawnPos = new Vector3(0, 0, 0); //The position the first player is spawned at
    public float spawnOffsetX = 2; //The X offset of each player from the previous one spawned
    public float spawnOffsetZ = 2; //The Z offset of each player from the previous one spawned
    public GameObject pauseMenu;
    public GameObject playerManagerUI;

    /// <summary>
    /// If there is an active player the players are spawned, if not load the player manager UI
    /// </summary>
    private void Awake()
    {
        //If player count > 0
        if (PlayerHandler.Instance.transform.childCount > 0)
            SpawnPlayers();
        else
        {
            GameObject.FindGameObjectWithTag("HUD").GetComponent<PlayerHUDButtons>().Pause();
            pauseMenu.SetActive(false);
            playerManagerUI.SetActive(true);
        }
    }

    /// <summary>
    /// Call spawn all players with the position and offset set in this script
    /// </summary>
    public void SpawnPlayers()
    {
        PlayerHandler.Instance.SpawnAllPlayers(spawnPos, spawnOffsetX, spawnOffsetZ);
    }

}

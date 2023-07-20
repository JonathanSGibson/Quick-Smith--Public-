using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/* This script is entirely made by Jonathan
 * 
 * This script controls the overall player manager UI (which is made up of player manager components)
 */

/// <summary>
/// This class acts to store in one place every piece of information required for display on the UI
/// </summary>
public class PlayerInformationForUI
{
    public PlayerInformationForUI(bool _slotActive, int _index = 0, string _inputDeviceName = null)
    {
        slotActive = _slotActive;
        index = _index;
        inputDeviceName = _inputDeviceName;
    }

    public bool slotActive;
    public int index;
    public string inputDeviceName;
}

public class PlayerManagerUI : MonoBehaviour
{
    public GameObject playerManagerUIPrefab; //A prefab of the components of the UI display [NOTE: this is not used, originally the UI was going to be automatically spawned but it is now done manually for more precise/simple control]
    public GameObject playerManagerDoneButtonPrefab; //A prefab of the done button for the UI display [NOTE: see above]
    public float spawnOffset; //The offset each component has from eachother [NOTE: see above]
    public bool temp = false; //Set to true to spawn the UI, exists for testing purposes [NOTE: see above]

    /// <summary>
    /// Updates each UI component [NOTE: this should be moved out of update for performance purposes]
    /// </summary>
    private void Update()
    {
        //Commented out code from old system

        //if (temp)
        //{
        //    SpawnUI();
        //    temp = false;
        //}
        UpdateAllUI();
    }

    /// <summary>
    /// Updates the UI and fetches and enables joining through the playinputmanager
    /// </summary>
    private void OnEnable()
    {
        UpdateAllUI();
        PlayerHandler.Instance.gameObject.GetComponent<PlayerInputManager>().EnableJoining();
    }

    /// <summary>
    /// Disables joining through the inputmanager
    /// </summary>
    private void OnDisable()
    {
        PlayerHandler.Instance.gameObject.GetComponent<PlayerInputManager>().DisableJoining();
    }

    /// <summary>
    /// Old code to automatically spawn UI [NOTE: was never succesfully tested as the method of displaying UI was changed before it was finished]
    /// </summary>
    public void SpawnUI()
    {
        Debug.Log("ui spawning");
        float spawnPos = 0;
        for (int i = 0; i < PlayerHandler.Instance.GetMaxPlayers(); i++)
        {
            GameObject newObject = Instantiate(playerManagerUIPrefab, transform);
            newObject.transform.position += new Vector3(0, spawnPos, 0);
            spawnPos += spawnOffset;
        }

        GameObject _newObject = Instantiate(playerManagerDoneButtonPrefab);
        _newObject.transform.parent = transform;
        _newObject.transform.position += new Vector3(0, spawnPos, 0);

        UpdateAllUI();
        PlayerHandler.Instance.gameObject.GetComponent<PlayerInputManager>().EnableJoining();
    }

    /// <summary>
    /// Old code to delete the UI and disable joining
    /// </summary>
    public void DeleteUI()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i));
        }

        PlayerHandler.Instance.gameObject.GetComponent<PlayerInputManager>().DisableJoining();
    }

    /// <summary>
    /// The function called when the "remove player" is pressed to remove the player
    /// </summary>
    /// <param name="playerUIPrefabWhoCalled"></param>
    public void RemovePlayer(GameObject playerUIPrefabWhoCalled)
    {        
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject == playerUIPrefabWhoCalled)
            {
                PlayerHandler.Instance.RemovePlayer(i-1);
                PlayerHandler.Instance.GetComponent<PlayerInputManager>().DisableJoining(); //Joining is enabled and disabled again to allow the removed controller to be added again
                PlayerHandler.Instance.GetComponent<PlayerInputManager>().EnableJoining();
                UpdateAllUI();
                break;
            }
        }
    }

    /// <summary>
    /// Goes through each element of the UI and updates them to display the corerct information
    /// </summary>
    public void UpdateAllUI()
    {
        bool anyPlayers = false;
        List<Transform> uiComponents = new List<Transform>();
        //Go through each child, if they are of the correct type add them to the list - this is done so the index matches the player index AND it ignores other components which are a child of the player manager (such as backgrounds or blurs)
        for (int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).tag == "PlayerManagerUI")
            {
                uiComponents.Add(transform.GetChild(i));
            }
        }
        //Goes through each element, gets the information for it, and updates it
        for (int i = 0; i < uiComponents.Count; i++)
        {
            PlayerInformationForUI information = PlayerHandler.Instance.GetPlayerInformationForUI(i);
            uiComponents[i].GetComponent<PlayerManagerComponent>().UpdateUIElements(information);
            if (information.slotActive)
                anyPlayers = true;
        }
        transform.GetChild(transform.childCount - 1).gameObject.SetActive(anyPlayers); // Enables the done button only if there is at least one player
    }

    /// <summary>
    /// Spawns players if a playerspawner is present
    /// </summary>
    public void SpawnPlayers()
    {
        PlayerSpawner spawner = GameObject.FindGameObjectWithTag("PlayerSpawner").GetComponent<PlayerSpawner>();
        if (spawner!= null)
            spawner.SpawnPlayers();
    }
}

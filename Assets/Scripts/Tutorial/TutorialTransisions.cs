using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class TutorialTransisions : MonoBehaviour
{
    //Jasons Code
    //references to the game objects in the scene
    public GameObject oreMiner;
    public GameObject crucible;
    public Anvil anvil;
    public CombinerController combiner;
    public GameObject bin;
    public SellCounter servingCounter;
    public OrderCounter orderCounter;
    public Counter counter1;
    public Counter counter2;
    public OreBarrel tinOre;
    public OreBarrel copperOre;
    public CustomerController callCustomer;

    //bools for each section of the tutorial
    public bool woodHandleMade = false;
    public bool oreMined = false;
    public bool bladeMade = false;
    public bool swordMade = false;
    public bool customerServed = false;
    public bool binUsed = false;
    public bool tutorialFinished = false;
    bool walkingControls = false;
    bool getLog = false;
    bool enumOn = false;

    public TMP_Text tutorialText;
    public GameObject textBackground;

    public List<string> dialogList;
    int dialogPosition;
    float textTimerDuration;

    //timer used for seperating each text box used
    private IEnumerator TutorialWait()
    {
        enumOn = true;
        yield return new WaitForSeconds(textTimerDuration);
        //remove the textbox from the screen after the duration
        tutorialText.gameObject.SetActive(false);
        textBackground.gameObject.SetActive(false);
        enumOn = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        //setting the required objects invisible at the start the level
        oreMiner.SetActive(false);
        crucible.SetActive(false);
        anvil.gameObject.SetActive(false);
        combiner.gameObject.SetActive(false);
        bin.SetActive(false);
        servingCounter.gameObject.SetActive(false);
        orderCounter.gameObject.SetActive(false);

        //loading the first text box and setting its display duration
        textTimerDuration = 15;
        tutorialText.text = dialogList[0];
        StartCoroutine(TutorialWait());
    }

    // Update is called once per frame
    void Update()
    {
        //changing to the next textbox in the introduction
        if (!walkingControls && !enumOn)
        {
            textTimerDuration = 15;
            DisplayNextLine();
            walkingControls = true;
        }

        if (!getLog && walkingControls && !enumOn)
        {
            textTimerDuration = 30;
            DisplayNextLine();
            getLog = true;
        }

        if (!woodHandleMade && getLog && !enumOn) 
        {
            //checking if either of the counters has a woodenhandle made on it
            if ((counter1.items[0] != null && counter1.items[0].GetComponent<Object>().ID == 2) || (counter2.items[0] != null && counter2.items[0].GetComponent<Object>().ID == 2))
            {
                woodHandleMade = true;
            }
        } 

        //checking if the handle has been made and the ore miner has not spawned to set it active in the scene
        if (woodHandleMade == true && !oreMiner.activeSelf && !enumOn)
        {
            textTimerDuration = 15;
            DisplayNextLine();
            oreMiner.SetActive(true);
        }
        
        //checking if the oreminer has been activated in the scene
        if(oreMiner.activeSelf && oreMined == false)
        {
            //checking if the tin ore and the copper ore has been mined
            if ((tinOre.items[0] != null &&  tinOre.items[0].GetComponent<Object>().ID == 9) && (copperOre.items[0] != null && copperOre.items[0].GetComponent<Object>().ID == 8))
            {
                oreMined = true;
            }
        }

        //checking whether the ore has been mined and the next stations have not spawned in to set them active in the scene
        if (oreMined == true && !crucible.activeSelf && !anvil.gameObject.activeSelf && !enumOn)
        {
            textTimerDuration = 30;
            DisplayNextLine();
            crucible.SetActive(true);
            anvil.gameObject.SetActive(true);
        }

        //checking if the crucible and anvil have been activated in the scene
        if (crucible.activeSelf && anvil.gameObject.activeSelf && bladeMade == false)
        {
            //checking if the anvil has the blade made on it
            if (anvil.GetComponent<Anvil>().items[0] != null && anvil.GetComponent<Anvil>().items[0].GetComponent<Object>().ID == 11)
            {
                bladeMade = true;
                
            }
        }

        //checking if the blade has been made and the bin has not spawned to set it active in the scene
        if (bladeMade == true && !bin.gameObject.activeSelf && !enumOn)
        {
            textTimerDuration = 15;
            DisplayNextLine();
            bin.gameObject.SetActive(true);
        }

        //checking if the player has removed the bronze ingot from the scene
        if (bin.gameObject.activeSelf && binUsed == false)
        {
            GameObject bronzeIngots = GameObject.FindGameObjectWithTag("bronzeIngot");
            if (bronzeIngots == null)
            {
                binUsed = true;
            }
        }

        //checking the bin has been used and the combine has not been spawned to set it active in the scene
        if (binUsed == true && !combiner.gameObject.activeSelf && !enumOn)
        {
            textTimerDuration = 15;
            DisplayNextLine();
            combiner.gameObject.SetActive(true);
        }
            
        //checking if the combiners are active in the scene
        if (combiner.gameObject.activeSelf && swordMade == false)
        {
            bool itemFound = false;
            //checking if the combiner has a sword made on it
            for (int i = 0; i < combiner.items.Count; i++)
            {
                if (combiner.items[i] != null && combiner.items[i].GetComponent<Object>().ID == 7)
                {
                    //checking if the sword base object has the correct component pieces to be the correct sword required for the overall sword
                    Sword sword = combiner.items[i].GetComponent<Sword>();
                    bool bladeFound = false;
                    bool handleFound = false;
                    for (int j = 0; j < sword.components.Count; j++)
                    {
                        if (sword.components[j] == 2)
                        {
                            handleFound = true;
                        }
                        else if (sword.components[j] == 11)
                        {
                            bladeFound = true;
                        }
                    }
                    if (bladeFound == true && handleFound == true)
                    {
                        itemFound = true;
                        break;
                    }
                }
            }
            if (itemFound == true)
            {
                swordMade = true;
            }
        }

        //checking if the sword has been made and the serving/order counters have not been spawned to set them active in the scene
        if (swordMade == true && !servingCounter.gameObject.activeSelf && !orderCounter.gameObject.activeSelf && !enumOn)
        {
            textTimerDuration = 30;
            DisplayNextLine();
            servingCounter.gameObject.SetActive(true);
            orderCounter.gameObject.SetActive(true);
            callCustomer.SpawnIndividualCustomer();
        }

        //checking if the order and serving counters exist in the scene
        if (servingCounter.gameObject.activeSelf && orderCounter.gameObject.activeSelf && customerServed == false)
        {
            GameObject swordBase = GameObject.FindGameObjectWithTag("SwordBase");
            GameObject customer = GameObject.FindGameObjectWithTag("Customer");
            if (customer == null && swordBase == null)
            {
                customerServed = true;
            }
        }

        if (customerServed && !enumOn && !tutorialFinished)
        {
            textTimerDuration = 15;
            DisplayNextLine();
            tutorialFinished = true;
        }

        //takes the player back to the main menu if the level has been completed
        if (customerServed == true && tutorialFinished && !enumOn)
        {
            PlayerHandler.Instance.DespawnAllPlayers();
            SceneManager.LoadScene(0);
        }

    }

    //setting the text to display the next line in the list when called
    public void DisplayNextLine()
    {
        tutorialText.gameObject.SetActive(true);
        textBackground.gameObject.SetActive(true);
        dialogPosition++;
        if (dialogPosition < dialogList.Count && dialogList[dialogPosition] != null)
        {
            tutorialText.text = dialogList[dialogPosition];
        }
        StartCoroutine(TutorialWait());
    }

    public void OrderGiven()
    {
        textTimerDuration = 15;
        DisplayNextLine();
        customerServed = true;
    }
}

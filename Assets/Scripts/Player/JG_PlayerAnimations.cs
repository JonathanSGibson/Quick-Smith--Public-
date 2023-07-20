using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    /* This script is entirely made by Jonathan
     * 
     * This script controls what animation the player character plays
     */

    Animator playerAnimator;
    Rigidbody rb;
    PlayerInventory inventory;
    bool interacting = false; // Whether the player is interacting or not
    public float stopThreshold; //The velocity magnitude at which the player is considered stopped (without this the player will start walking if slightly pushed by the other player)
    public float stopTimeToIdle; //The duration at which the player needs to be below the above velocity to start idle animation (without this the player flicks between animations visually unpleasantly)
    float stopTimer = -1; //The time at which idle animation will start if not interrupted
    bool timing = false; //Whether the timer is counting down

    /// <summary>
    /// Fetches all required scripts/components
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
        inventory = GetComponent<PlayerInventory>();
    }

    /// <summary>
    /// Sets the animation based on player movement and whether they're carring anything
    /// </summary>
    void Update()
    {
        //Does not interrupt if the current clip is winning, defeat or interacting
        string clipName = playerAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
        if (clipName == "Win Jumping Emote")
        {

        }
        else if (clipName == "Defeat")
        {

        }
        else if (interacting)
        {

        }
        //If the player is going fast enough not to idle resets the timer and starts the correct animation
        else if (rb.velocity.magnitude > stopThreshold)
        {
            if (inventory.heldObject != null)
                playerAnimator.Play("Carrying Running");
            else
            {
                playerAnimator.Play("Running");
            }
            timing = false;
            stopTimer = -1;
        }
        //If the timer has run out starts idling and resets the timer
        else if (stopTimer <= Time.time && stopTimer > -1)
        {
            if (inventory.heldObject != null)
                playerAnimator.Play("Carrying Idle");
            else
                playerAnimator.Play("Idle");
            timing = false;
            stopTimer = -1;
        }
        //If velocity is low but the timer is not going, start the timer
        else if (!timing)
        {
            stopTimer = Time.time + stopTimeToIdle;
            timing = true;
        }
    }

    /// <summary>
    /// Sets the bool for interacting and starts the interact animation if required
    /// </summary>
    /// <param name="_interacting">Whether the player is interacting or not</param>
    public void InteractAnimation(bool _interacting)
    {
        interacting = _interacting;
        if (interacting)
            playerAnimator.Play("Hammering In Place");
    }

    public void VictoryAnimation()
    {
        playerAnimator.Play("Win Jumping Emote");
    }

    public void DefeatAnimation()
    {
        playerAnimator.Play("Defeat");
    }

}

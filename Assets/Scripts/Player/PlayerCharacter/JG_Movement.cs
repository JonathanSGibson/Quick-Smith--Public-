using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Animations;

public class Movement : MonoBehaviour
{
    /* This script is entirely Jonathan's work
     * 
     * This script is used to control the player's movement
     */

    public bool stationary; //For if the player is only rotating
    public float moveSpeed;
    public float turnSpeed; //[NOTE: Turn limiting not currently implemented]
    public Rigidbody rb;
    float inputRotation; //Storing the rotation current being input, this is stored to calculate what objects are in front of the player
    public bool inputting; //Whether the player is currently inputting or not
    public Animator animator;


    /// <summary>
    /// Sets starting values
    /// </summary>
    void Start()
    {
        inputRotation = 0;
        inputting = false;
        stationary = false;
    }

    /// <summary>
    /// Resets stationary variable when enabled (This is to stop it inverting if game is paused while a player is inputting the stationary button)
    /// </summary>
    private void OnEnable()
    {
        stationary = false;
    }

    /// <summary>
    /// Rotates and moves the player
    /// </summary>
    private void FixedUpdate()
    {
        if (inputting)
        {
            //takes player's input (as gained in OnMovement) and alters the player character's rotation to face that direction
            // this doesn't strictly need to be in fixedupdate but once rotation limiting is added it will
            float newRotation = inputRotation;

            transform.eulerAngles = new Vector3(transform.eulerAngles.x, newRotation, transform.eulerAngles.z);

            //if the player is not holding down the "stationary" button, move them forwards,  else stop
            if (!stationary)
            {
                rb.velocity = new Vector3(transform.forward.x * moveSpeed, rb.velocity.y, transform.forward.z * moveSpeed);
                //animator.ResetTrigger("Idle");
                //animator.SetTrigger("IsRunning");
                
            }
            else
            {
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
                //animator.SetTrigger("Idle");
            }
        }
        else //stop if player is not inputting
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            //animator.ResetTrigger("IsRunning");
            //animator.SetTrigger("Idle");
        }
        
        
    }

    /// <summary>
    /// Takes player inputs and assigns variables for use
    /// </summary>
    /// <param name="input">The input values</param>
    public void OnMovement(InputValue input)
    {
        // takes player input as a vector and converts it to degrees
        // sets "inputting" bool true or false to be used in fixedupdate to rotate the player
        Vector2 inputDirection = input.Get<Vector2>();
        if (inputDirection == new Vector2(0, 0))
            inputting = false;
        else
        {
            inputRotation = Mathf.Atan2(inputDirection.x, inputDirection.y);
            inputRotation *= Mathf.Rad2Deg;
            inputting = true;
        }
    }

    /// <summary>
    /// Sets the stationary value when its input
    /// </summary>
    /// <param name="input">Input value</param>
    public void OnStationary(InputValue input)
    {
        // sets bool used in fixedupdate to determine whether to move the player
        stationary = !stationary;
    }

    /// <summary>
    /// Resets stationary if the application is defocused in order to prevent inverting of its value
    /// </summary>
    /// <param name="focus"></param>
    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
            stationary = false;
    }
}

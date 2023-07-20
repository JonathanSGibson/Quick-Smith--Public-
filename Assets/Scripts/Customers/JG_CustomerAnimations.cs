using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerAnimations : MonoBehaviour
{
    Animator customerAnimator;
    Rigidbody rb;
    public float stopThreshold; //Velocity magnitude at or below which the customer is considered stopped
    public float stopTimeToIdle; //How long the customer needs to be stopped before they start their idle animation
    float stopTimer = -1; //When they stopped
    bool timing = false; //Whether the timer for being stopped has started or not

    /// <summary>
    /// Fetches rigidbody and animator components
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        customerAnimator = GetComponent<Animator>();
    }

    /// <summary>
    /// Plays animation based on current movement
    /// </summary>
    void Update()
    {
        if (rb.velocity.magnitude > stopThreshold)
        {
            customerAnimator.Play("Walking");
            timing = false;
            stopTimer = -1;
        }
        else if (stopTimer <= Time.time && stopTimer > -1)
        {
            customerAnimator.Play("Idle");
            timing = false;
            stopTimer = -1;
        }
        //Idles based on a timer so if they stop momentarily due to pathing it does not flash between animations
        else if (!timing)
        {
            stopTimer = Time.time + stopTimeToIdle;
            timing = true;
        }
    }
}

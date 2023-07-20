using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// A simple free camera to be added to a Unity game object.
/// 
/// Keys:
///	wasd / arrows	- movement
///	q/e 			- up/down (local space)
///	r/f 			- up/down (world space)
///	pageup/pagedown	- up/down (world space)
///	hold shift		- enable fast movement mode
///	right mouse  	- enable free look
///	mouse			- free look / rotation
///     
/// </summary>
public class FreeCam : MonoBehaviour
{
    bool forward = false;
    bool back = false;
    bool left = false;
    bool right = false;
    bool up = false;
    bool down = false;
    Vector2 mousePos;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    bool fastMode;
    /// <summary>
    /// Normal speed of camera movement.
    /// </summary>
    public float movementSpeed = 10f;

    /// <summary>
    /// Speed of camera movement when shift is held down,
    /// </summary>
    public float fastMovementSpeed = 100f;

    /// <summary>
    /// Sensitivity for free look.
    /// </summary>
    public float freeLookSensitivity = 1f;


    void Update()
    {
        var movementSpeed = fastMode? this.fastMovementSpeed: this.movementSpeed;

        if (left)
        {
            transform.position = transform.position + (-transform.right * movementSpeed * Time.deltaTime);
        }

        if (right)
        {
            transform.position = transform.position + (transform.right * movementSpeed * Time.deltaTime);
        }

        if (forward)
        {
            transform.position = transform.position + (transform.forward * movementSpeed * Time.deltaTime);
        }

        if (back)
        {
            transform.position = transform.position + (-transform.forward * movementSpeed * Time.deltaTime);
        }

        if (up)
        {
            transform.position = transform.position + (transform.up * movementSpeed * Time.deltaTime);
        }

        if (down)
        {
            transform.position = transform.position + (-transform.up * movementSpeed * Time.deltaTime);
        }

        float newRotationX = transform.localEulerAngles.y + mousePos.x * freeLookSensitivity;
        float newRotationY = transform.localEulerAngles.x - mousePos.y * freeLookSensitivity;
        transform.localEulerAngles = new Vector3(newRotationY, newRotationX, 0f);
    }

    void OnMousePos(InputValue input)
    {
        mousePos = input.Get<Vector2>();
    }

    void OnMovement(InputValue input)
    {
        forward = false;
        back = false;
        left = false;
        right = false;
        Vector2 movement = input.Get<Vector2>();
        if (movement.y > 0)
            forward = true;
        if (movement.y < 0)
            back = true;
        if (movement.x > 0)
            right = true;
        if (movement.x < 0)
            left = true;
    }

    void OnVertical(InputValue input)
    {
        up = false;
        down = false;
        float vertical = input.Get<float>();

        if (vertical > 0)
            up = true;
        if (vertical < 0)
            down = true;
    }

    void OnFastMove()
    {
        fastMode = !fastMode;
    }
}

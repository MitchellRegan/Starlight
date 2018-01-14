using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerShipController))]
public class ShipTiltAndRoll : MonoBehaviour
{
    //Reference to our player ship that this is attached to
    private PlayerShipController ourShip;

    //The amount of time that the player has to double tap
    public float doubleTapBufferTime = 0.1f;
    //The current amount of time left for the player to spin
    private float currentBufferTime = 0f;

    //The amount of time that a spin takes
    public float timeDuringSpin = 1f;
    //The current amount of time left for the current spin
    private float currentSpinTime = 0f;




	// Use this for initialization
	private void Start ()
    {
        //Getting our player ship reference
        this.ourShip = this.GetComponent<PlayerShipController>();
	}
	

	// Update is called once per frame
	private void Update ()
    {
        //If our current double tap buffer time is above 0, we count down
        if(this.currentBufferTime > 0)
        {
            this.currentBufferTime -= Time.deltaTime;
        }

        //If our current spin time is above 0, we count down and prevent actions from being taken
        if(this.currentSpinTime > 0)
        {
            this.currentSpinTime -= Time.deltaTime;
            return;
        }

        //Bools to track if we're pressing the left or right tilt buttons
        bool rightTilt_Press = false;
        bool leftTilt_Press = false;
        //Bools to track if we're holding the left or right tilt buttons
        bool rightTilt_Hold = false;
        bool leftTilt_Hold = false;

        //If the player pressed the button to roll right on this frame
        if (this.ourShip.ourController.CheckButtonPressed(this.ourShip.ourCustomInputs.rollRight_Controller) ||
            Input.GetKeyDown(this.ourShip.ourCustomInputs.rollRight_Keyboard))
        {
            rightTilt_Press = true;
            rightTilt_Hold = true;
        }
        //Otherwise if the player is still holding the button to roll right on this frame
        else if(this.ourShip.ourController.CheckButtonDown(this.ourShip.ourCustomInputs.rollRight_Controller) ||
            Input.GetKey(this.ourShip.ourCustomInputs.rollRight_Keyboard))
        {
            rightTilt_Hold = true;
        }

        //If the player pressed the button to roll left on this frame
        if ((this.ourShip.ourController.CheckButtonPressed(this.ourShip.ourCustomInputs.rollLeft_Controller) ||
            Input.GetKeyDown(this.ourShip.ourCustomInputs.rollLeft_Keyboard)))
        {
            leftTilt_Press = true;
            leftTilt_Hold = true;
        }
        //Otherwise if the player is still holding the button to roll left on this frame
        else if(this.ourShip.ourController.CheckButtonDown(this.ourShip.ourCustomInputs.rollLeft_Controller) ||
            Input.GetKey(this.ourShip.ourCustomInputs.rollLeft_Keyboard))
        {
            leftTilt_Hold = true;
        }

        //If at least one of the buttons are held (this is to make sure one doesn't have priority)
        if(rightTilt_Hold != leftTilt_Hold)
        {
            //If the player is wanting to move right
            if (rightTilt_Hold)
            {
                //If the player just clicked the right tilt button
                if (rightTilt_Press)
                {
                    //If we're within the buffer time for the double tap spin, we can spin right
                    if (this.currentBufferTime > 0)
                    {
                        this.StartSpin(true);
                    }
                    //Otherwise, we start the buffer time
                    else
                    {
                        this.currentBufferTime = this.doubleTapBufferTime;
                    }
                }
            }
            //If the player is wanting to move left
            else if (leftTilt_Hold)
            {
                //If the player just clicked the left tilt button
                if (leftTilt_Press)
                {
                    //If we're within the buffer time for the double tap spin, we can spin left
                    if (this.currentBufferTime > 0)
                    {
                        this.StartSpin(false);
                    }
                    //Otherwise, we start the buffer time
                    else
                    {
                        this.currentBufferTime = this.doubleTapBufferTime;
                    }
                }
            }
        }
    }


    //Function called from Update to start spinning. True => Right, False => Left
    private void StartSpin(bool spinRight_)
    {

    }
}

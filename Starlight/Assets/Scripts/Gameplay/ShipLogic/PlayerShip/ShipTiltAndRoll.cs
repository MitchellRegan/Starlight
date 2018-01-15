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

    //The amount of time that a roll takes
    public float timeDuringRoll = 1f;
    //The current amount of time left for the current roll
    private float currentRollTime = 0f;

    //The number of rolls our ship does
    public int numberOfRolls = 3;

    //The max angle that we can tilt at
    public float maxTiltAngle = 80;

    //The number of degrees that we rotate while tilting
    public float tiltDegreesPerFrame = 1f;

    //The multiplier for which direction we rotate while rolling. +1 for Left, -1 for Right
    private int rollDirection = -1;

    //The total degrees that we're rotating during the current roll
    private float totalRollDegrees = 0;

    

	// Use this for initialization
	private void Start ()
    {
        //Getting our player ship reference
        this.ourShip = this.GetComponent<PlayerShipController>();
	}
	

	// Update is called once per frame
	private void Update ()
    {
        //If the game is paused, nothing happens
        if (PauseGame.isGamePaused)
        {
            return;
        }

        //If our current double tap buffer time is above 0, we count down
        if (this.currentBufferTime > 0)
        {
            this.currentBufferTime -= Time.deltaTime;
        }

        //If our current roll time is above 0, we count down and prevent actions from being taken
        if(this.currentRollTime > 0)
        {
            this.currentRollTime -= Time.deltaTime;

            //Finding the number of degrees to rotate this frame based on the amount of time since the last frame
            float rotToAdd = Time.deltaTime / this.timeDuringRoll;
            rotToAdd = rotToAdd * this.totalRollDegrees;
            //Multiplying by our rotation direction int so if we go backwards, it becomes negative
            rotToAdd = rotToAdd * this.rollDirection;

            //Adding the rotation difference to our current Z rotation
            this.ourShip.zGyroscope.localEulerAngles += new Vector3(0, 0, rotToAdd);
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
            //Telling our player ship that we're tilting/rolling
            this.ourShip.isShipTilting = true;
            
            //If the player is wanting to move right
            if (rightTilt_Hold)
            {
                //If the player just clicked the right tilt button and we're within the buffer time for the double tap roll, we can roll right
                if (rightTilt_Press && this.currentBufferTime > 0)
                {
                    this.StartRoll(true);
                }
                //If the player just clicked the right tilt button and we're not in the buffer time, we start the buffer time and tilt
                else if(rightTilt_Press)
                {
                    this.currentBufferTime = this.doubleTapBufferTime;
                    //Rotating our ship transform's Z position
                    this.ourShip.zGyroscope.transform.localEulerAngles = new Vector3(this.ourShip.zGyroscope.transform.localEulerAngles.x,
                                                                            this.ourShip.zGyroscope.transform.localEulerAngles.y,
                                                                            this.ourShip.zGyroscope.transform.localEulerAngles.z - this.tiltDegreesPerFrame);

                    //Getting a corrected version of our Z rotation, because Unity handles euiler rotation in a weird way
                    float correctedZRot = this.ourShip.zGyroscope.transform.localEulerAngles.z;
                    if(correctedZRot > 180)
                    {
                        correctedZRot -= 360;
                    }

                    //Making sure the ship doesn't rotate past the maximum rotation point
                    if (correctedZRot < -this.maxTiltAngle)
                    {
                        this.ourShip.zGyroscope.transform.localEulerAngles = new Vector3(this.ourShip.zGyroscope.transform.localEulerAngles.x,
                                                                            this.ourShip.zGyroscope.transform.localEulerAngles.y,
                                                                            -this.maxTiltAngle);
                    }
                }
                //Otherwise, if the player is just holding the button and not clicking, we just tilt
                else
                {
                    //Rotating our ship transform's Z position
                    this.ourShip.zGyroscope.transform.localEulerAngles = new Vector3(this.ourShip.zGyroscope.transform.localEulerAngles.x,
                                                                            this.ourShip.zGyroscope.transform.localEulerAngles.y,
                                                                            this.ourShip.zGyroscope.transform.localEulerAngles.z - this.tiltDegreesPerFrame);

                    //Getting a corrected version of our Z rotation, because Unity handles euiler rotation in a weird way
                    float correctedZRot = this.ourShip.zGyroscope.transform.localEulerAngles.z;
                    if (correctedZRot > 180)
                    {
                        correctedZRot -= 360;
                    }

                    //Making sure the ship doesn't rotate past the maximum rotation point
                    if (correctedZRot < -this.maxTiltAngle)
                    {
                        this.ourShip.zGyroscope.transform.localEulerAngles = new Vector3(this.ourShip.zGyroscope.transform.localEulerAngles.x,
                                                                            this.ourShip.zGyroscope.transform.localEulerAngles.y,
                                                                            -this.maxTiltAngle);
                    }
                }
            }
            //If the player is wanting to move left
            else if (leftTilt_Hold)
            {
                //If the player just clicked the left tilt button and we're within the buffer time for the double tap roll, we can roll left
                if (leftTilt_Press && this.currentBufferTime > 0)
                {
                    this.StartRoll(false);
                }
                //If the player just clicked the left tilt button and we're not in the buffer time, we start the buffer time and tilt
                else if(leftTilt_Press)
                {
                    this.currentBufferTime = this.doubleTapBufferTime;
                    //Rotating our ship transform's Z position
                    this.ourShip.zGyroscope.transform.localEulerAngles = new Vector3(this.ourShip.zGyroscope.transform.localEulerAngles.x,
                                                                            this.ourShip.zGyroscope.transform.localEulerAngles.y,
                                                                            this.ourShip.zGyroscope.transform.localEulerAngles.z + this.tiltDegreesPerFrame);

                    //Getting a corrected version of our Z rotation, because Unity handles euiler rotation in a weird way
                    float correctedZRot = this.ourShip.zGyroscope.transform.localEulerAngles.z;
                    if (correctedZRot > 180)
                    {
                        correctedZRot -= 360;
                    }

                    //Making sure the ship doesn't rotate past the maximum rotation point
                    if (correctedZRot > this.maxTiltAngle)
                    {
                        this.ourShip.zGyroscope.transform.localEulerAngles = new Vector3(this.ourShip.zGyroscope.transform.localEulerAngles.x,
                                                                            this.ourShip.zGyroscope.transform.localEulerAngles.y,
                                                                            this.maxTiltAngle);
                    }
                }
                //Otherwise, if the player is just holding the button and not clicking, we just tilt
                else
                {
                    //Rotating our ship transform's Z position
                    this.ourShip.zGyroscope.transform.localEulerAngles = new Vector3(this.ourShip.zGyroscope.transform.localEulerAngles.x,
                                                                            this.ourShip.zGyroscope.transform.localEulerAngles.y,
                                                                            this.ourShip.zGyroscope.transform.localEulerAngles.z + this.tiltDegreesPerFrame);

                    //Getting a corrected version of our Z rotation, because Unity handles euiler rotation in a weird way
                    float correctedZRot = this.ourShip.zGyroscope.transform.localEulerAngles.z;
                    if (correctedZRot > 180)
                    {
                        correctedZRot -= 360;
                    }

                    //Making sure the ship doesn't rotate past the maximum rotation point
                    if (correctedZRot > this.maxTiltAngle)
                    {
                        this.ourShip.zGyroscope.transform.localEulerAngles = new Vector3(this.ourShip.zGyroscope.transform.localEulerAngles.x,
                                                                            this.ourShip.zGyroscope.transform.localEulerAngles.y,
                                                                            this.maxTiltAngle);
                    }
                }
            }
        }
        //Otherwise we tell our player ship that we're not tilting
        else
        {
            this.ourShip.isShipTilting = false;
        }
    }


    //Function called from Update to start spinning. True => Right, False => Left
    private void StartRoll(bool spinRight_)
    {
        //Setting our current roll time to the max
        this.currentRollTime = this.timeDuringRoll;

        //Setting our initial total roll degrees (this will be changed in a moment
        this.totalRollDegrees = 360 * numberOfRolls;

        //Getting our corrected Z rotation because Unity goes between 0 and 360. We need it to be between -180 and 180
        float correctedZRot = this.ourShip.zGyroscope.transform.localEulerAngles.z;
        if(correctedZRot > 180)
        {
            correctedZRot -= 360;
        }

        //If we spin right
        if(spinRight_)
        {
            //Our roll direction is set to -1 so we spin in negative degrees
            this.rollDirection = -1;

            //Adding our Z rotation to the total roll degrees so we offset back to horizontal
            this.totalRollDegrees += correctedZRot;
        }
        //If we spin left
        else
        {
            //Our roll direction is set to 1 so we spin in positive degrees
            this.rollDirection = 1;

            //Subtracting our Z rotation from the total roll degrees so we offset back to horizontal
            this.totalRollDegrees -= correctedZRot;
        }
    }


    //Function called from Update to spin our ship

}

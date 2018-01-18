using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipRotationLogic : MonoBehaviour
{
    //The enum designating which player ship this component belongs to
    public Players playerShipID = Players.P1;
    //The reference to our designated player ship
    private PlayerShipController ourShip;

    [Space(8)]

    //The max position we can go left and right in the local X coordinates
    public float maxLocalLeftRightPos = 2.5f;
    //The max position we can go up in the local Y coordinates
    public float maxLocalUpPos = 2.5f;
    //The max position we can go down in the local Y coordinates
    public float maxLocalDownPos = 2.5f;

    [Space(8)]

    //The maximum amount that we interpolate left and right
    [Range(0.01f, 0.99f)]
    public float maxLeftRightInterpSpeed = 0.85f;
    //The maximum amount that we interpolate up 
    [Range(0.01f, 0.99f)]
    public float maxUpInterpSpeed = 0.85f;
    //The maximum amount that we interpolate down
    [Range(0.01f, 0.99f)]
    public float maxDownInterpSpeed = 0.85f;

    [Space(8)]

    //Float from -1f to 1f. Tracks how long the player X input is positive or negative
    [Range(-1f, 1f)]
    private float xInputTracker = 0;
    //How fast the X input tracker increases every frame
    [Range(0, 0.5f)]
    public float xTrackerSpeed = 0.1f;

    //Float from -1f to 1f. Tracks how long the player Y input is positive or negative
    [Range(-1f, 1f)]
    private float yInputTracker = 0;
    //How fast Y the input tracker increases every frame
    [Range(0, 0.5f)]
    public float yTrackerSPeed = 0.1f;




	// Use this for initialization
	private void Start ()
    {
	    //Getting the reference to our player ship based on the ID given
        switch(this.playerShipID)
        {
            case Players.P1:
                this.ourShip = PlayerShipController.p1ShipRef;
                break;
            case Players.P2:
                this.ourShip = PlayerShipController.p2ShipRef;
                break;
            default:
                this.ourShip = PlayerShipController.p1ShipRef;
                break;
        }
	}
	

	// Update is called once per frame
	private void Update ()
    {
        //If the game is paused, nothing happens
        if (PauseGame.isGamePaused)
        {
            return;
        }

        //Getting the player inputs from the controller or keyboard
        Vector2 playerInputs = this.GetXYMoveInput();

        //Getting the multipliers for the interpolators
        this.FindInterpMultipliers(playerInputs);

        //Finding the position we need to interpolat to based on the controller inputs
        Vector3 targetInterpPos = this.FindTargetPos(playerInputs);

        //Finding the difference in position between our current pos and the target pos
        Vector3 positionDiff = targetInterpPos - this.transform.localPosition;

        //Setting our position to an interpolated offset if we're moving up
        if (playerInputs.y > 0)
        {
            this.transform.localPosition += new Vector3(positionDiff.x * (this.maxLeftRightInterpSpeed * Mathf.Abs(this.xInputTracker)),
                                                        positionDiff.y * (this.maxUpInterpSpeed * Mathf.Abs(this.yInputTracker)),
                                                        0);
        }
        //If we're moving down
        else
        {
            this.transform.localPosition += new Vector3(positionDiff.x * (this.maxLeftRightInterpSpeed * Mathf.Abs(this.xInputTracker)),
                                                        positionDiff.y * (this.maxDownInterpSpeed * Mathf.Abs(this.yInputTracker)),
                                                        0);
        }
	}


    //Function called from Update to get the player controller inputs in terms of XY
    private Vector2 GetXYMoveInput()
    {
        //Vector 2 to hold the XY directions that the player wants to move our ship
        Vector2 movementInput = new Vector2(0, 0);

        //Getting the controller input for our movement if the controller exists
        if (this.ourShip.ourController != null)
        {
            movementInput.x = this.ourShip.ourController.CheckStickValue(this.ourShip.ourCustomInputs.moveLeftRight_Controller);
            movementInput.y = this.ourShip.ourController.CheckStickValue(this.ourShip.ourCustomInputs.moveUpDown_Controller);
        }

        //If the left keyboard button is held, we move left
        if (Input.GetKey(this.ourShip.ourCustomInputs.moveLeft_Keyboard))
        {
            //If there's no left/right input from our controller, this is the only input taken
            if (movementInput.x > -0.1 && movementInput.x < 0.1)
            {
                movementInput.x = -1;
            }
            //If there's already stick input from our controller, we average them
            else
            {
                movementInput.x = (-1 + movementInput.x) / 2;
            }
        }
        //If the right keyboard button is held, we move right
        else if (Input.GetKey(this.ourShip.ourCustomInputs.moveRight_Keyboard))
        {
            //If there's no left/right input from our controller, this is the only input taken
            if (movementInput.x > -0.1 && movementInput.x < 0.1)
            {
                movementInput.x = 1;
            }
            //If there's already stick input from our controller, we average them
            else
            {
                movementInput.x = (1 + movementInput.x) / 2;
            }
        }

        //If the up keyboard button is held, we move up
        if (Input.GetKey(this.ourShip.ourCustomInputs.moveUp_Keyboard))
        {
            //If there's no up/down input from our controller, this is the only input taken
            if (movementInput.y > -0.1 && movementInput.y < 0.1)
            {
                movementInput.y = 1;
            }
            //If there's already stick input from our controller, we average them
            else
            {
                movementInput.y = (1 + movementInput.y) / 2;
            }
        }
        //If the down keyboard button is held, we move down
        else if (Input.GetKey(this.ourShip.ourCustomInputs.moveDown_Keyboard))
        {
            //If there's no up/down input from our controller, this is the only input taken
            if (movementInput.y > -0.1 && movementInput.y < 0.1)
            {
                movementInput.y = -1;
            }
            //If there's already stick input from our controller, we average them
            else
            {
                movementInput.y = (-1 + movementInput.y) / 2;
            }
        }

        //If our Y axis is inverted, we invert it
        if (!this.ourShip.ourCustomInputs.invertYMovement)
        {
            movementInput.y = movementInput.y * -1;
        }

        //Returning the normalized 
        return movementInput;
    }


    //Function called from Update to find the multipliers for our interpolators
    private void FindInterpMultipliers(Vector2 playerInputs_)
    {
        //If the X input is positive and outside the deadzone
        if(playerInputs_.x > 0.05f)
        {
            //If the X input tracker is already positive, we add to it
            if(this.xInputTracker >= 0)
            {
                this.xInputTracker += this.xTrackerSpeed;
                //Making sure we don't go past the limit of 1
                if(this.xInputTracker > 1)
                {
                    this.xInputTracker = 1;
                }
            }
            //If the X input tracker is negative, we zero it out
            else
            {
                this.xInputTracker = 0;
            }
        }
        //If the X input is negative and outside the deadzone
        else if(playerInputs_.x < -0.05f)
        {
            //If the X input tracker is already negative, we add to it
            if(this.xInputTracker <= 0)
            {
                this.xInputTracker -= this.xTrackerSpeed;
                //Making sure we don't go past the limit of -1
                if(this.xInputTracker < -1)
                {
                    this.xInputTracker = -1;
                }
            }
            //If the X input tracker is positive, we zero it out
            else
            {
                this.xInputTracker = 0;
            }
        }

        //If the Y input is positive and outside the deadzone
        if(playerInputs_.y > 0.05f)
        {
            //If the Y input tracker is already positive, we add to it
            if(this.yInputTracker >= 0)
            {
                this.yInputTracker += this.yTrackerSPeed;
                //Making sure we don't go past our limit of 1
                if(this.yInputTracker > 1)
                {
                    this.yInputTracker = 1;
                }
            }
            //If the Y input tracker is negative, we zero it out
            else
            {
                this.yInputTracker = 0;
            }
        }
        //If the Y input is negative and outside the deadzone
        else if(playerInputs_.y < -0.05f)
        {
            //If the Y input tracker is already negative, we add to it
            if(this.yInputTracker <= 0)
            {
                this.yInputTracker -= this.yTrackerSPeed;
                //Making sure we don't go past our limit of -1
                if(this.yInputTracker < -1)
                {
                    this.yInputTracker = -1;
                }
            }
            //If the Y input tracker is positive, we zero it out
            else
            {
                this.yInputTracker = 0;
            }
        }
    }


    //Function called from Update to get the target position that we need to interpolate to in local space
    private Vector3 FindTargetPos(Vector2 playerInputs_)
    {
        float xPos = 0;
        //If the X input is outside the deadzone, we find the max left or right position
        if(playerInputs_.x > 0.05f || playerInputs_.x < -0.05f)
        {
            xPos = playerInputs_.x * this.maxLocalLeftRightPos;
        }

        float yPos = 0;
        //If the input is going up, we find the max up position
        if(playerInputs_.y > 0.05f)
        {
            yPos = playerInputs_.y * this.maxLocalUpPos;
        }
        //If the input is going down, we find the max down position
        else if(playerInputs_.y < -0.05f)
        {
            yPos = playerInputs_.y * this.maxLocalDownPos;
        }

        //Returning the positions with the local Z unchanged
        Vector3 targetPos = new Vector3(xPos, yPos, this.transform.localPosition.z);
        return targetPos;
    }
}

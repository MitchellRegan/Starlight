using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RailMovementFlight : MonoBehaviour
{
    //The player controller that gives us input
    [HideInInspector]
    public PlayerShipController ourShip;
    //Reference to our rigidbody component
    private Rigidbody ourRigidbody;

    //The position of the collider in space so we can get this ship's relative position
    private Vector3 colliderPosition;
    //The forward direction that we're moving along
    private Vector3 railForwardDirection;
    //The up and right directions for the rail so we can have an orientation
    private Vector3 railUpDirection;
    private Vector3 railRightDirection;

    //The width and height of the designated flight zone of our current region
    private Vector2 flightBoundingBox;

    //The forward thrust multiplier for this rail zone
    private float zoneThrustMultiplier = 0;


    
    //Function called when this object is created
    private void Awake()
    {
        //Getting the reference to our rigidbody component
        this.ourRigidbody = this.GetComponent<Rigidbody>();

        //Setting our orientation directions by default
        this.railUpDirection = this.transform.up;
        this.railForwardDirection = this.transform.forward;
        this.railRightDirection = this.transform.right;
    }


    //Function called from PlayerShipController.OnTriggerEnter to change our movement to match the new region
    public void SetNewRailDirection(Collider newRegionCollider_)
    {
        //Disabling the collider's component so we can't accidentally hit it multiple times
        newRegionCollider_.enabled = false;

        //Setting our collider position
        this.colliderPosition = newRegionCollider_.transform.position;

        //Setting our forward direction to that of the collider's game object
        this.railForwardDirection = newRegionCollider_.transform.forward;

        //Setting our upward direction to that of the collider's game object
        this.railUpDirection = newRegionCollider_.transform.up;

        //Setting our right direction to that of the collider's game object
        this.railRightDirection = newRegionCollider_.transform.right;

        //Setting our flight bounding box based on the width and height of the collider
        this.flightBoundingBox = new Vector2(newRegionCollider_.transform.localScale.x, newRegionCollider_.transform.localScale.y);

        //Setting the minimum forward thrust that this rail zone requires
        this.zoneThrustMultiplier = newRegionCollider_.GetComponent<RegionZone>().thrustMultiplier;

        //Rotating this object to face the forward direction
        Quaternion lookDirection = new Quaternion();
        lookDirection.SetLookRotation(this.railForwardDirection, this.railUpDirection);
        this.transform.rotation = lookDirection;
    }


    //Function called every frame
    private void Update()
    {
        //Moving the ship with the player inputs
        this.MoveShip();
        //Making sure we're not going outside the bounding box of the zone
        this.StayWithinBoundingBox();
    }


    //Function called from Update to make sure this ship is within the rail zone's bounding box
    private void StayWithinBoundingBox()
    {
        //Getting our position in space relative to the rail zone collider
        Vector3 ourRelativePos = this.transform.InverseTransformPoint(this.colliderPosition);
        //Bool to determine if we're out of bounds
        bool isOutOfBounds = false;

        //If our relative X position is to the right of the bounding box
        if (ourRelativePos.x > this.colliderPosition.x + (this.flightBoundingBox.x / 2))
        {
            Debug.Log("Left");
            isOutOfBounds = true;
            ourRelativePos = new Vector3(this.colliderPosition.x + (this.flightBoundingBox.x / 2),
                                        ourRelativePos.y,
                                        ourRelativePos.z);
        }
        //If our relative X position is to the left of the bounding box
        else if (ourRelativePos.x < this.colliderPosition.x - (this.flightBoundingBox.x / 2))
        {
            Debug.Log("Right");
            isOutOfBounds = true;
            ourRelativePos = new Vector3(this.colliderPosition.x - (this.flightBoundingBox.x / 2),
                                        ourRelativePos.y,
                                        ourRelativePos.z);
        }


        //If our relative Y position is above the bounding box
        if (ourRelativePos.y > this.colliderPosition.y + (this.flightBoundingBox.y / 2))
        {
            Debug.Log("Up");
            isOutOfBounds = true;
            ourRelativePos = new Vector3(ourRelativePos.x,
                                        this.colliderPosition.y + (this.flightBoundingBox.y / 2),
                                        ourRelativePos.z);
        }
        //If our relative Y position is below the bounding box
        else if (ourRelativePos.y < this.colliderPosition.y - (this.flightBoundingBox.y / 2))
        {
            Debug.Log("Down");
            isOutOfBounds = true;
            ourRelativePos = new Vector3(ourRelativePos.x,
                                        this.colliderPosition.y - (this.flightBoundingBox.y / 2),
                                        ourRelativePos.z);
        }

        //Setting our transform to the corrected relative pos is we're out of bounds
        if (isOutOfBounds)
        {
            this.transform.position = ourRelativePos;
        }
    }


    //Function called from Update to handle player input
    private void MoveShip()
    {
        //Vector 2 to hold the XY movement input
        Vector2 movementInput = this.GetXYMoveInput();

        //Float to hold the forward thrust that the player wants to move at
        float thrustInput = this.GetZThrustInput();

        //Vector 3 to hold our XYZ thrust that we'll get using our input and the ship wings/engines
        Vector3 velocities = new Vector3();

        //Looping through all of the wings on our ship
        foreach(ShipWingLogic wing in this.ourShip.shipWings)
        {
            //If we're moving left
            if (movementInput.x < 0)
            {
                velocities.x += wing.currentLeftSpeed * movementInput.x;
            }
            //If we're moving right
            else if(movementInput.x > 0)
            {
                velocities.x += wing.currentRightSpeed * movementInput.x;
            }

            //If we're moving up
            if(movementInput.y > 0)
            {
                velocities.y += wing.currentUpSpeed * movementInput.y;
            }
            //If we're moving down
            else if(movementInput.y < 0)
            {
                velocities.y += wing.currentDownSpeed * movementInput.y;
            }

            //Adding any drift that the wing is causing
            velocities.x += wing.currentDrift * wing.damagedDriftDirection.x;
            velocities.y += wing.currentDrift * wing.damagedDriftDirection.y;
        }

        //Looping through all of the engines on our ship
        foreach(ShipEngineLogic engine in this.ourShip.shipEngines)
        {
            //If our thrust input is greater than 0.1, we're boosting forward
            if(thrustInput > 0.1)
            {
                velocities.z += engine.currentRailVelocity.z;
            }
            //If our thrust input is less than -0.1, we're breaking
            else if(thrustInput < -0.1)
            {
                velocities.z += engine.currentRailVelocity.x;
            }
            //Otherwise we're going normal speed
            else
            {
                velocities.z += engine.currentRailVelocity.y;
            }
        }

        //Multiplying our forward velocity by the zone's velocity multiplier
        velocities.z = velocities.z * this.zoneThrustMultiplier;

        //Vector 3 to hold our velocities in the correct orientations based on our region
        Vector3 orientationVelocities = new Vector3();
        orientationVelocities += velocities.x * this.railRightDirection;
        orientationVelocities += velocities.y * this.railUpDirection;
        orientationVelocities += velocities.z * this.railForwardDirection;

        //Setting the movement and thrust velocities based on our relative direction
        this.ourRigidbody.velocity = orientationVelocities;
    }


    //Function called from MoveShip to get the Vector 2 player inputs for XY movement
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
        if(this.ourShip.ourCustomInputs.invertYMovement)
        {
            movementInput.y = movementInput.y * -1;
        }

        //Returning the normalized 
        return movementInput.normalized;
    }


    //Function called from MoveShip to get the player inputs for forward movement
    private float GetZThrustInput()
    {
        //Float to hold the forward thrust that the player wants to move at
        float thrustInput = 0;

        //Getting the input based on the boost input
        if (this.ourShip.ourController.CheckButtonDown(this.ourShip.ourCustomInputs.boostButton_Controller) ||
            Input.GetKey(this.ourShip.ourCustomInputs.boostButton_Keyboard))
        {
            thrustInput += 1;
        }

        //Getting the input based on the break input
        if(this.ourShip.ourController.CheckButtonDown(this.ourShip.ourCustomInputs.breakButton_Controller) ||
            Input.GetKey(this.ourShip.ourCustomInputs.breakButton_Keyboard))
        {
            thrustInput -= 1;
        }

        return thrustInput;
    }
}

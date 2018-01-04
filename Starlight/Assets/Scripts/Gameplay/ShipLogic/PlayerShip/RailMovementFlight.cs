using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RailMovementFlight : MonoBehaviour
{
    //The player controller that gives us input
    [HideInInspector]
    public PlayerShipController ourShip;

    //The position of the collider in space so we can get this ship's relative position
    private Vector3 colliderPosition;
    //The forward direction that we're moving along
    private Vector3 railForwardDirection;
    //The up direction for the rail so we can have an orientation
    private Vector3 railUpDirection;

    //The width and height of the designated flight zone of our current region
    private Vector2 flightBoundingBox;


    
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

        //Setting our flight bounding box based on the width and height of the collider
        this.flightBoundingBox = new Vector2(newRegionCollider_.transform.localScale.x, newRegionCollider_.transform.localScale.y);
    }


    //Function called every frame
    private void Update()
    {
        //Making sure we're not going outside the bounding box of the zone
        this.StayWithinBoundingBox();
        //Moving the ship with the player inputs
        this.MoveShip();
    }


    //Function called from Update to make sure this ship is within the rail zone's bounding box
    private void StayWithinBoundingBox()
    {
        //Getting our position in space relative to the rail zone collider
        Vector3 ourRelativePos = this.transform.InverseTransformPoint(this.colliderPosition);

        //If our relative X position is to the right of the bounding box
        if (ourRelativePos.x > this.colliderPosition.x + (this.flightBoundingBox.x / 2))
        {
            ourRelativePos = new Vector3(this.colliderPosition.x + (this.flightBoundingBox.x / 2),
                                        ourRelativePos.y,
                                        ourRelativePos.z);
        }
        //If our relative X position is to the left of the bounding box
        else if (ourRelativePos.x < this.colliderPosition.x - (this.flightBoundingBox.x / 2))
        {
            ourRelativePos = new Vector3(this.colliderPosition.x - (this.flightBoundingBox.x / 2),
                                        ourRelativePos.y,
                                        ourRelativePos.z);
        }


        //If our relative Y position is above the bounding box
        if (ourRelativePos.y > this.colliderPosition.y + (this.flightBoundingBox.y / 2))
        {
            ourRelativePos = new Vector3(ourRelativePos.x,
                                        this.colliderPosition.y + (this.flightBoundingBox.y / 2),
                                        ourRelativePos.z);
        }
        //If our relative Y position is below the bounding box
        else if (ourRelativePos.y < this.colliderPosition.y - (this.flightBoundingBox.y / 2))
        {
            ourRelativePos = new Vector3(ourRelativePos.x,
                                        this.colliderPosition.y - (this.flightBoundingBox.y / 2),
                                        ourRelativePos.z);
        }
    }


    //Function called from Update to handle player input
    private void MoveShip()
    {
        Vector2 stickInput = new Vector2(0,0);

        //Getting the controller input for our movement if the controller exists
        if (this.ourShip.ourController != null)
        {
            stickInput.x = this.ourShip.ourController.CheckStickValue(this.ourShip.ourCustomInputs.moveLeftRight_Controller);
            stickInput.y = this.ourShip.ourController.CheckStickValue(this.ourShip.ourCustomInputs.moveUpDown_Controller);
        }

        //Getting the keyboard input for our movement
        Vector2 keyboardInput = new Vector2(0,0);

        //If the left keyboard button is held, we move left
        if(Input.GetKey(this.ourShip.ourCustomInputs.moveLeft_Keyboard))
        {
            //If there's no left/right input from our controller, this is the only input taken
            if (stickInput.x > -0.1 && stickInput.x < 0.1)
            {
                keyboardInput.x = -1;
            }
            //If there's already stick input from our controller, we average them
            else
            {
                keyboardInput.x = (-1 + stickInput.x) / 2;
            }
        }
        //If the right keyboard button is held, we move right
        else if(Input.GetKey(this.ourShip.ourCustomInputs.moveRight_Keyboard))
        {
            //If there's no left/right input from our controller, this is the only input taken
            if (stickInput.x > -0.1 && stickInput.x < 0.1)
            {
                keyboardInput.x = 1;
            }
            //If there's already stick input from our controller, we average them
            else
            {
                keyboardInput.x = (1 + stickInput.x) / 2;
            }
        }

        //If the up keyboard button is held, we move up
        if(Input.GetKey(this.ourShip.ourCustomInputs.moveUp_Keyboard))
        {
            //If there's no up/down input from our controller, this is the only input taken
            if (stickInput.y > -0.1 && stickInput.y < 0.1)
            {
                keyboardInput.y = 1;
            }
            //If there's already stick input from our controller, we average them
            else
            {
                keyboardInput.y = (1 + stickInput.y) / 2;
            }
        }
        //If the down keyboard button is held, we move down
        else if(Input.GetKey(this.ourShip.ourCustomInputs.moveDown_Keyboard))
        {
            //If there's no up/down input from our controller, this is the only input taken
            if (stickInput.y > -0.1 && stickInput.y < 0.1)
            {
                keyboardInput.y = -1;
            }
            //If there's already stick input from our controller, we average them
            else
            {
                keyboardInput.y = (-1 + stickInput.y) / 2;
            }
        }


        //
    }
}

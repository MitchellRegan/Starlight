using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailMovementFlight : MonoBehaviour
{
    //The player controller that gives us input
    [HideInInspector]
    public PlayerShipController ourController;

    //The position of the collider in space so we can get this ship's relative position
    private Vector3 colliderPosition;
    //The forward direction that we're moving along
    private Vector3 railForwardDirection;
    //The up direction for the rail so we can have an orientation
    private Vector3 railUpDirection;

    //The width and height of the designated flight zone of our current region
    private Vector2 flightBoundingBox;


    //The controller stick for moving left and right
    public ControllerSticks moveLeftRightStick = ControllerSticks.Left_Stick_X;
    //The controller stick for moving up and down
    public ControllerSticks moveUpDownStick = ControllerSticks.Left_Stick_Y;

    [Space(8)]

    //The controller stick for aiming left and right
    public ControllerSticks aimLeftRightStick = ControllerSticks.Right_Stick_X;
    //The controller stick for aiming up and down
    public ControllerSticks aimUpDownStick = ControllerSticks.Right_Stick_Y;

    [Space(8)]

    //The controller button used to boost forward
    public ControllerButtons boostButton_Controller = ControllerButtons.Right_Trigger;
    //The keyboard/mouse button used to boost forward
    public KeyCode boostButton_Keyboard = KeyCode.Space;

    [Space(8)]

    //The controller button used to break
    public ControllerButtons breakButton_Controller = ControllerButtons.Left_Trigger;
    //The keyboard/mouse button used to break
    public KeyCode breakButton_Keyboard = KeyCode.LeftShift;



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
        //Getting our position in space relative to the rail zone collider
        Vector3 ourRelativePos = this.transform.InverseTransformPoint(this.colliderPosition);

        //If our relative X position is to the right of the bounding box
        if(ourRelativePos.x > this.colliderPosition.x + (this.flightBoundingBox.x / 2))
        {
            ourRelativePos = new Vector3(this.colliderPosition.x + (this.flightBoundingBox.x / 2),
                                        ourRelativePos.y,
                                        ourRelativePos.z);
        }
        //If our relative X position is to the left of the bounding box
        else if(ourRelativePos.x < this.colliderPosition.x - (this.flightBoundingBox.x / 2))
        {
            ourRelativePos = new Vector3(this.colliderPosition.x - (this.flightBoundingBox.x / 2),
                                        ourRelativePos.y,
                                        ourRelativePos.z);
        }


        //If our relative Y position is above the bounding box
        if(ourRelativePos.y > this.colliderPosition.y + (this.flightBoundingBox.y / 2))
        {
            ourRelativePos = new Vector3(ourRelativePos.x,
                                        this.colliderPosition.y + (this.flightBoundingBox.y / 2),
                                        ourRelativePos.z);
        }
        //If our relative Y position is below the bounding box
        else if(ourRelativePos.y < this.colliderPosition.y - (this.flightBoundingBox.y / 2))
        {
            ourRelativePos = new Vector3(ourRelativePos.x,
                                        this.colliderPosition.y - (this.flightBoundingBox.y / 2),
                                        ourRelativePos.z);
        }
    }
}

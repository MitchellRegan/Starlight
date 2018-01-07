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

    //Reference to our rail parent object's rigid body component
    public Rigidbody railParentObj;

    //The position of the collider in space so we can get this ship's relative position
    private Vector3 colliderPosition;

    //The forward, up, and right directions that we're moving along
    private Vector3 railForwardDirection = Vector3.forward;
    private Vector3 railUpDirection = Vector3.up;
    private Vector3 railRightDirection = Vector3.right;

    //The width and height of the designated flight zone of our current region
    private Vector2 flightBoundingBox;

    //The forward thrust multiplier for this rail zone
    private float zoneThrustMultiplier = 0;
    
    //Bool that determines if we're currently interpolating
    private bool areWeInterping = false;
    //Float that determines how fast we interp positions
    private float interpSpeed = 0.8f;
    //The transform of the region we're entering
    private Transform nextRegionTransform;

    //The forward, up, and right directions that we were moving along in our previous rail zone
    private Vector3 prevRailForward = new Vector3();
    private Vector3 prevRailUp = new Vector3();
    private Vector3 prevRailRight = new Vector3();
    //The rotation that we were facing in our previous rail zone
    private Quaternion prevRotation = new Quaternion();

    //The forward, up, and right directions that we will be moving along in our next rail zone
    private Vector3 nextRailForward = new Vector3();
    private Vector3 nextRailUp = new Vector3();
    private Vector3 nextRailRight = new Vector3();
    //The rotation that we will be facing in our next rail zone
    private Quaternion nextRotation = new Quaternion();


    
    //Function called when this object is created
    private void Awake()
    {
        //Getting the reference to our rigidbody component
        this.ourRigidbody = this.GetComponent<Rigidbody>();

        //Setting our orientation directions by default
        this.railUpDirection = this.transform.up;
        this.railForwardDirection = this.transform.forward;
        this.railRightDirection = this.transform.right;
        
        this.areWeInterping = false;
    }


    //Function called when this component is enabled
    private void OnEnable()
    {
        //Enabling our rail parent object
        this.railParentObj.gameObject.SetActive(true);
        //Unparenting our rail parent object from this ship
        this.railParentObj.transform.SetParent(this.transform.parent);
        //Setting our rail parent object's position and rotation to match our ships
        this.railParentObj.transform.position = this.transform.position;
        this.railParentObj.transform.rotation = this.transform.rotation;
        //Parenting our ship to be parented to our rail parent object
        this.transform.SetParent(this.railParentObj.transform);
    }


    //Function called from PlayerShipController.OnTriggerEnter before this component is disabled
    public void BeforeDisable()
    {
        //Unparenting our ship from the rail parent object
        this.transform.SetParent(this.railParentObj.transform.parent);
        //Parenting the rail parent object to our ship
        this.railParentObj.transform.SetParent(this.transform);
        //Setting our rail parent object's local position to be centered on our ship
        this.railParentObj.transform.localPosition = new Vector3();
        this.areWeInterping = false;
    }


    //Function called from PlayerShipController.OnTriggerEnter to change our movement to match the new region
    public void SetNewRailDirection(Collider newRegionCollider_)
    {
        //Disabling this region's collider so we don't hit it multiple times
        newRegionCollider_.enabled = false;

        //Saving this region's transform for interpolation
        this.nextRegionTransform = newRegionCollider_.transform;

        //Unparenting our ship from our rail parent object
        this.transform.SetParent(this.transform.parent.parent);
        //Finding the center XY position in relative space of the new region collider that's at the beginning of the collision Z distance
        this.railParentObj.transform.SetParent(newRegionCollider_.transform.parent);
        this.railParentObj.transform.localPosition = new Vector3(0, 0, -newRegionCollider_.transform.localScale.z / 2);
        this.railParentObj.transform.SetParent(this.transform.parent);
        //Parenting our ship to the newly centered rail parent's position
        this.transform.SetParent(this.railParentObj.transform);

        //Setting our collider position
        this.colliderPosition = newRegionCollider_.transform.position;

        //The rotation for the next zone we're entering so we can interp to match it
        this.nextRotation = newRegionCollider_.transform.rotation;

        //Setting our flight bounding box based on the width and height of the collider
        this.flightBoundingBox = new Vector2(newRegionCollider_.transform.localScale.x, newRegionCollider_.transform.localScale.y);

        //Setting the minimum forward thrust that this rail zone requires
        this.zoneThrustMultiplier = newRegionCollider_.GetComponent<RegionZone>().thrustMultiplier;

        //Rotating our rail parent object to face the forward direction
        Quaternion lookDirection = new Quaternion();
        lookDirection.SetLookRotation(this.railForwardDirection, this.railUpDirection);
        this.railParentObj.transform.rotation = lookDirection;

        //Starting our interpolation
        this.areWeInterping = true;
    }


    //Function called every frame
    private void Update()
    {
        //If our interpolator is still interpolating, we change the direction we're facing
        if(this.areWeInterping)
        {
            //Speed to rotatefor our slerp
            float rotationSpeed = 0;

            //If we're accelerating and not breaking, our rotation speed is increased
            if(this.ourShip.ourController.CheckButtonDown(this.ourShip.ourCustomInputs.boostButton_Controller) || Input.GetKey(this.ourShip.ourCustomInputs.boostButton_Keyboard) &&
                !this.ourShip.ourController.CheckButtonDown(this.ourShip.ourCustomInputs.breakButton_Controller) && !Input.GetKey(this.ourShip.ourCustomInputs.breakButton_Keyboard))
            {
                //Variables to hold the sum of our ship engines normal speed and boost speed
                float sumNormalSpeed = 0;
                float sumBoostSpeed = 0;

                //Looping through all of our ship's engines
                foreach(ShipEngineLogic engine in this.ourShip.shipEngines)
                {
                    sumNormalSpeed += engine.currentRailVelocity.y;
                    sumBoostSpeed += engine.currentRailVelocity.z;
                }

                rotationSpeed = rotationSpeed * (sumBoostSpeed / sumNormalSpeed);
            }
            //If we're breaking and not accelerating, our rotation speed is reduced
            else if (!this.ourShip.ourController.CheckButtonDown(this.ourShip.ourCustomInputs.boostButton_Controller) && !Input.GetKey(this.ourShip.ourCustomInputs.boostButton_Keyboard) &&
                this.ourShip.ourController.CheckButtonDown(this.ourShip.ourCustomInputs.breakButton_Controller) || Input.GetKey(this.ourShip.ourCustomInputs.breakButton_Keyboard))
            {
                //Variables to hold the sum of our ship engines normal speed and break speed
                float sumNormalSpeed = 0;
                float sumBreakSpeed = 0;

                //Looping through all of our ship's engines
                foreach (ShipEngineLogic engine in this.ourShip.shipEngines)
                {
                    sumNormalSpeed += engine.currentRailVelocity.y;
                    sumBreakSpeed += engine.currentRailVelocity.x;
                }

                rotationSpeed = rotationSpeed * (sumBreakSpeed / sumNormalSpeed);
                Debug.Log(rotationSpeed);
            }

            //Interpolating our rail parent's rotation to match the rotation of the zone we're entering
            this.railParentObj.transform.rotation = Quaternion.Slerp(this.railParentObj.transform.rotation, this.nextRotation, Time.time * 0.025f);

            //Setting our directions based on the rail parent's current rotation
            this.railForwardDirection = this.railParentObj.transform.forward;
            this.railUpDirection = this.railParentObj.transform.up;
            this.railRightDirection = this.railParentObj.transform.right;

            //Unparenting our ship from our rail parent object
            this.transform.SetParent(this.transform.parent.parent);
            //Finding the center XY position in relative space of the new region collider that's at the beginning of the collision Z distance
            this.railParentObj.transform.SetParent(this.nextRegionTransform.parent);
            this.railParentObj.transform.localPosition = new Vector3(this.railParentObj.transform.localPosition.x * this.interpSpeed, 
                                                                     this.railParentObj.transform.localPosition.y * this.interpSpeed,
                                                                     this.railParentObj.transform.localPosition.z);
            this.railParentObj.transform.SetParent(this.transform.parent);
            //Parenting our ship to the newly centered rail parent's position
            this.transform.SetParent(this.railParentObj.transform);

            //If our interp's progress is done we signal that we're done
            if (this.railParentObj.transform.rotation == this.nextRotation)
            {
                this.areWeInterping = false;
            }
        }

        //Moving the ship with the player inputs
        this.MoveShip();
        //Making sure we're not going outside the bounding box of the zone
        this.StayWithinBoundingBox();
    }


    //Function called from Update to make sure this ship is within the rail zone's bounding box
    private void StayWithinBoundingBox()
    {
        //Getting our position in space relative to the rail zone collider
        Vector3 ourRelativePos = new Vector3(this.transform.localPosition.x, this.transform.localPosition.y, 0);

        //If our relative X position is to the right of the bounding box
        if (ourRelativePos.x > this.flightBoundingBox.x / 2)
        {
            ourRelativePos = new Vector3((this.flightBoundingBox.x / 2) + ((ourRelativePos.x - (this.flightBoundingBox.x / 2)) * this.interpSpeed),
                                        ourRelativePos.y,
                                        ourRelativePos.z);
        }
        //If our relative X position is to the left of the bounding box
        else if (ourRelativePos.x < -this.flightBoundingBox.x / 2)
        {
            ourRelativePos = new Vector3((-this.flightBoundingBox.x / 2) + ((ourRelativePos.x - (-this.flightBoundingBox.x / 2)) * this.interpSpeed),
                                        ourRelativePos.y,
                                        ourRelativePos.z);
        }


        //If our relative Y position is above the bounding box
        if (ourRelativePos.y > this.flightBoundingBox.y / 2)
        {
            ourRelativePos = new Vector3(ourRelativePos.x,
                                        (this.flightBoundingBox.y / 2) + ((ourRelativePos.y - (this.flightBoundingBox.y / 2)) * this.interpSpeed),
                                        ourRelativePos.z);
        }
        //If our relative Y position is below the bounding box
        else if (ourRelativePos.y < -this.flightBoundingBox.y / 2)
        {
            ourRelativePos = new Vector3(ourRelativePos.x,
                                        (-this.flightBoundingBox.y / 2) + ((ourRelativePos.y - (-this.flightBoundingBox.y / 2)) * this.interpSpeed),
                                        ourRelativePos.z);
        }

        //Setting our transform to the corrected relative pos is we're out of bounds
        //Debug.Log("Current Pos: " + this.transform.InverseTransformPoint(this.colliderPosition) + ",      Relative Pos: " + ourRelativePos);
        this.transform.localPosition = ourRelativePos;
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

        //Vector 3 to hold our XY velocities in the correct orientations based on our region
        Vector3 XYorientationVelocities = new Vector3();
        XYorientationVelocities += velocities.x * this.railRightDirection;
        XYorientationVelocities += velocities.y * this.railUpDirection;
        //XYorientationVelocities += velocities.z * this.railForwardDirection;

        //Vector 3 to hold our Z velocity in the correct orientation based on our region
        Vector3 ZorientationVelocity = new Vector3();
        ZorientationVelocity += velocities.z * this.railForwardDirection;

        //Setting the movement and thrust velocities based on our relative direction
        this.ourRigidbody.velocity = XYorientationVelocities;
        //Applying the forward thrust velocity to our rail parent object's rigid body
        this.railParentObj.velocity = ZorientationVelocity;
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

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
    public RailParentCollisionLogic railParentObj;

    //Multipliers to our XY speed for when we're rolling during rail movement
    [HideInInspector]
    public Vector2 railRollXYMultiplier = new Vector2(1, 1);

    [Space(8)]

    //Float for how fast we change bounding box sizes when changing regions
    public float changeBoundsTime = 1;
    private float currentBoundChangeTime = 0;
    private Vector2 prevBounds = new Vector2();
    private Vector2 nextBounds = new Vector2();

    [Space(8)]

    //The speed multiplier for breaking
    [Range(0.01f, 0.9f)]
    public float breakSpeedMultiplier = 0.6f;
    //The speed multiplier for boosting
    [Range(1.1f, 3f)]
    public float boostSpeedMultiplier = 1.5f;

    //The interpolation speed for changing the forward velocity
    [Range(0.01f, 0.99f)]
    public float forwardVelocityInterp = 0.9f;

    [Space(8)]

    //The game object that we move to aim the player ship
    public Transform targetRotationObj;
    //The max XY distances the target rotation can move
    public Vector2 targetRotObjMaxXY = new Vector2();
    //The speed that we move the target rotation object
    public float targetRotObjXSpeed = 1;
    public Vector2 targetRotObjUpDownSpeed = new Vector2(1.3f, 0.7f);

    //Variables for the max of rotation the player ship turns based on player input
    public Vector3 maxShipRotation = new Vector3();
    //The max amount of rotation change each frame
    public Vector3 maxRotationChange = new Vector3();
    //The amount that the ship is rotated when no input for that axis is given
    public Vector3 levelOutRotation = new Vector3();

    [Space(8)]

    //The Max velocities for Left and Right on the X axis
    public float maxLeftRightVelocity = 10;
    //The Max velocities for Up and Down on the Y axis
    public Vector2 maxUpDownVelocities = new Vector2(10, 5);

    //The velocity drag multiplier for each axis when no input is given
    [Range(0.1f, 0.99f)]
    public float xVelocityDrag = 0.9f;
    [Range(0.1f, 0.99f)]
    public float yVelocityDrag = 0.9f;

    //The width and height of the designated flight zone of our current region
    private Vector2 flightBoundingBox;
    
    //Bool that determines if we're currently interpolating
    private bool areWeInterping = false;
    //Float that determines how fast we interp positions
    private float interpSpeed = 0.8f;
    //The rotation that we will be facing in our next rail zone
    private Quaternion nextRotation = new Quaternion();
    
    //Float to hold the area where the controller inputs no longer register movement
    private float deadzone = 0.00f;



    //Function called when this object is created
    private void Awake()
    {
        //Getting the reference to our rigidbody component
        this.ourRigidbody = this.GetComponent<Rigidbody>();
        
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


    //Function called from RailParentCollisionLogic.OnTriggerEnter to change our movement to match the new region
    public void SetNewRailDirection(Collider newRegionCollider_)
    {
        //Setting the time for how long we'll be changing our bounding box
        this.currentBoundChangeTime = this.changeBoundsTime;
        //Saving the bounding size of the region we just left and the region we're entering
        this.prevBounds = this.flightBoundingBox;
        this.nextBounds = new Vector2(newRegionCollider_.transform.localScale.x, newRegionCollider_.transform.localScale.y);

        //Finding the length of the region so we know how far back the collider is from the center point
        float zDistFromCenter = (newRegionCollider_.transform.localScale.z / 2) * -1;
        //Finding the position where our ship needs to meet up with the collider's center
        this.railParentObj.transform.position = newRegionCollider_.transform.position + (zDistFromCenter * newRegionCollider_.transform.forward);

        //The rotation for the next zone we're entering so we can interp to match it
        this.nextRotation = newRegionCollider_.transform.rotation;

        //Starting our interpolation
        this.areWeInterping = true;
    }

    
    //Function called every frame
    private void Update()
    {
        //If the game is paused, nothing happens
        if(PauseGame.isGamePaused)
        {
            return;
        }

        //If we're changing bounding box size, we adjust its scale
        if(this.currentBoundChangeTime > 0)
        {
            //Subtracting from the time remaining
            this.currentBoundChangeTime -= Time.deltaTime;

            //Getting the percent that we are through the change
            float changePercent = (this.changeBoundsTime - this.currentBoundChangeTime) / this.changeBoundsTime;

            //If we're at or over 1 (100%) the change is finished
            if(changePercent >= 1)
            {
                this.flightBoundingBox = this.nextBounds;
            }
            //If we're not finished, we keep updating the bounding box limits
            else
            {
                this.flightBoundingBox = this.prevBounds + ((this.nextBounds - this.prevBounds) * changePercent);
            }
        }

        //If our interpolator is still interpolating, we change the direction we're facing
        if(this.areWeInterping)
        {
            //Speed to rotatefor our slerp
            float rotationSpeed = 0;

            //If we're accelerating and not breaking, our rotation speed is increased
            if(this.ourShip.isShipBoosting)
            {
                //If we have enough energy to boost, we continue
                if (this.ourShip.ourEnergy.CanUseEnergy(this.ourShip.boostEnergyCost))
                {
                    //Variables to hold the sum of our ship engines normal speed and boost speed
                    float sumNormalSpeed = 0;
                    float sumBoostSpeed = 0;

                    //Looping through all of our ship's engines
                    foreach (ShipEngineLogic engine in this.ourShip.shipEngines)
                    {
                        sumNormalSpeed += engine.currentRailVelocity.y;
                        sumBoostSpeed += engine.currentRailVelocity.z;
                    }

                    rotationSpeed = rotationSpeed * (sumBoostSpeed / sumNormalSpeed);
                }
            }
            //If we're breaking and not accelerating, our rotation speed is reduced
            else if (this.ourShip.isShipBreaking)
            {
                //If we have enough energy to break, we continue
                if (this.ourShip.ourEnergy.CanUseEnergy(this.ourShip.breakEnergyCost))
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
                }
            }

            //Interpolating our rail parent's rotation to match the rotation of the zone we're entering
            this.railParentObj.transform.rotation = Quaternion.Slerp(this.railParentObj.transform.rotation, this.nextRotation, Time.time * 0.025f);

            //Unparenting our ship from our rail parent object
            this.transform.SetParent(this.transform.parent.parent);
            //Finding the center XY position in relative space of the new region collider that's at the beginning of the collision Z distance
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
            ourRelativePos = new Vector3((this.flightBoundingBox.x / 2),
                                        ourRelativePos.y,
                                        ourRelativePos.z);
        }
        //If our relative X position is to the left of the bounding box
        else if (ourRelativePos.x < -this.flightBoundingBox.x / 2)
        {
            ourRelativePos = new Vector3((-this.flightBoundingBox.x / 2),
                                        ourRelativePos.y,
                                        ourRelativePos.z);
        }


        //If our relative Y position is above the bounding box
        if (ourRelativePos.y > this.flightBoundingBox.y / 2)
        {
            ourRelativePos = new Vector3(ourRelativePos.x,
                                        (this.flightBoundingBox.y / 2),
                                        ourRelativePos.z);
        }
        //If our relative Y position is below the bounding box
        else if (ourRelativePos.y < -this.flightBoundingBox.y / 2)
        {
            ourRelativePos = new Vector3(ourRelativePos.x,
                                        (-this.flightBoundingBox.y / 2),
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

        //Rotating our ship's model to display the player inputs
        this.RotateShipModel(movementInput);

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

        //Applying our thrust input to our rail parent to determine how fast we move along the rail
        this.railParentObj.ourSplineMoveRB.speedMultiplier += (thrustInput - this.railParentObj.ourSplineMoveRB.speedMultiplier) * this.forwardVelocityInterp;

        //Vector 3 to hold our XY velocities in the correct orientations based on our region
        Vector3 XYorientationVelocities = new Vector3();
        XYorientationVelocities += velocities.x * this.railParentObj.transform.right * this.railRollXYMultiplier.x;
        XYorientationVelocities += velocities.y * this.railParentObj.transform.up * this.railRollXYMultiplier.y;
        
        //Setting the movement and thrust velocities based on our relative direction
        this.ourRigidbody.AddForce(XYorientationVelocities);

        //Creating a variable to hold our rigidbody's velocity values relative to our transform's local space
        Vector3 localVelocity = this.ourRigidbody.transform.InverseTransformDirection(this.ourRigidbody.velocity);

        //If there's no X input, we apply drag to the X velocity
        if(movementInput.x < 0.1f && movementInput.x > -0.1f)
        {
            localVelocity.x *= this.xVelocityDrag;
            this.ourRigidbody.velocity = this.ourRigidbody.transform.TransformDirection(localVelocity);
        }
        //If there's no Y input, we apply drag to the Y velocity
        if(movementInput.y < 0.1f && movementInput.y > -0.1f)
        {
            localVelocity.y *= this.yVelocityDrag;
            this.ourRigidbody.velocity = this.ourRigidbody.transform.TransformDirection(localVelocity);
        }

        //Making sure the X velocity is within the min/max
        localVelocity = this.ourRigidbody.transform.InverseTransformDirection(this.ourRigidbody.velocity);
        if(localVelocity.x > this.maxLeftRightVelocity)
        {
            localVelocity.x = this.maxLeftRightVelocity;
            this.ourRigidbody.velocity = this.ourRigidbody.transform.TransformDirection(localVelocity);
        }
        else if(localVelocity.x < -this.maxLeftRightVelocity)
        {
            localVelocity.x = -this.maxLeftRightVelocity;
            this.ourRigidbody.velocity = this.ourRigidbody.transform.TransformDirection(localVelocity);
        }
        //Making sure the Y velocity is within the min/max
        if (localVelocity.y > this.maxUpDownVelocities.x)
        {
            localVelocity.y = this.maxUpDownVelocities.x;
            this.ourRigidbody.velocity = this.ourRigidbody.transform.TransformDirection(localVelocity);
        }
        else if(localVelocity.y < -this.maxUpDownVelocities.y)
        {
            localVelocity.y = -this.maxUpDownVelocities.y;
            this.ourRigidbody.velocity = this.ourRigidbody.transform.TransformDirection(localVelocity);
        }
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
        if (!this.ourShip.ourCustomInputs.invertYMovement)
        {
            movementInput.y = movementInput.y * -1;
        }

        //Returning the normalized 
        return movementInput;
    }


    //Function called from MoveShip to get the player inputs for forward movement
    private float GetZThrustInput()
    {
        //Float to hold the forward thrust that the player wants to move at
        float thrustInput = 1;

        //Getting the input based on the boost input
        if (this.ourShip.isShipBoosting)
        {
            //thrustInput += 1;
            thrustInput = this.boostSpeedMultiplier;
        }
        //Getting the input based on the break input
        else if(this.ourShip.isShipBreaking)
        {
            //thrustInput -= 1;
            thrustInput = this.breakSpeedMultiplier;
        }

        return thrustInput;
    }


    //Function called from MoveShip to rotate the player ship's model
    private void RotateShipModel(Vector2 playerInputs_)
    {
        //If we're not rolling or tilting, we rotate our ship's Z rotation
        if (!this.ourShip.isShipTilting)
        {
            //Float to hold the player ship's Z rotation (roll)
            float zRot = 0;

            //Getting the angle of our z rotation between -180 and 180 (Unity handles it between 0 and 359.99 which throws things off)
            float correctedZRotation = this.ourShip.zGyroscope.localEulerAngles.z;
            if (correctedZRotation > 180)
            {
                correctedZRotation -= 360;
            }

            //If our Z input (X stick axis) is outside the input deadzone (-0.1 - 0.1), we rotate the ship
            if (playerInputs_.x > this.deadzone || playerInputs_.x < -this.deadzone)
            {
                zRot = -this.maxRotationChange.z * playerInputs_.x;

                //If the ship's Z rotation is above the max, we cap it off
                if (correctedZRotation > this.maxShipRotation.z)
                {
                    //If the difference is greater than the level out angle, we angle back to the max
                    if (correctedZRotation - this.maxShipRotation.z <= this.levelOutRotation.z)
                    {
                        zRot = this.maxShipRotation.z - correctedZRotation;
                    }
                    //Otherwise we zero out as much as possible
                    else
                    {
                        zRot = -this.levelOutRotation.z;
                    }
                }
                //If the added z rotation will put our ship's Z rotation above the max, we make sure it doesn't
                else if(correctedZRotation + zRot > this.maxShipRotation.z)
                {
                    zRot = this.maxShipRotation.z - correctedZRotation;
                }
                //If the ship's Z rotation is below the min, we cap it off
                else if (correctedZRotation < -this.maxShipRotation.z)
                {
                    //If the difference is less than the negative level out angle, we angle back to the max
                    if (correctedZRotation + this.maxShipRotation.z >= -this.levelOutRotation.z)
                    {
                        zRot = -this.maxShipRotation.z - correctedZRotation;
                    }
                    //Otherwise we zero out as much as possible
                    else
                    {
                        zRot = this.levelOutRotation.z;
                    }
                }
                //If the added z rotation will put our ship's Z rotation below the max, we make sure it doesn't
                else if(correctedZRotation + zRot < -this.maxShipRotation.z)
                {
                    zRot = -this.maxShipRotation.z - correctedZRotation;
                }
            }
            //If our Z input is inside the input deadzone and our gyroscope's z rotation isn't 0, we need to correct it
            else if (correctedZRotation != 0)
            {
                //If our z rotation is above 0, we need to ease it down
                if (correctedZRotation > 0)
                {
                    //If the angle is greater than our level out rotation, we use all of the rotation we can
                    if (correctedZRotation > this.levelOutRotation.z)
                    {
                        zRot = -this.levelOutRotation.z;
                    }
                    //If the angle is less than our level out rotation, we make sure not to over correct
                    else
                    {
                        zRot = -correctedZRotation;
                    }
                }
                //If our z rotation is below 0, we need to ease it up
                else
                {
                    //If the angle is greater than our level out rotation, we use all of the rotation we can
                    if (correctedZRotation < this.levelOutRotation.z)
                    {
                        zRot = this.levelOutRotation.z;
                    }
                    //If the angle is less than our level out rotation, we make sure not to over correct
                    else
                    {
                        zRot = correctedZRotation;
                    }
                }
            }

            //Adjusting our ship's Z gyroscope to rotate with these inputs
            this.ourShip.zGyroscope.localEulerAngles += new Vector3(0, 0, zRot);


            //Updating our corrected rotation
            correctedZRotation = this.ourShip.zGyroscope.localEulerAngles.z;
            if (correctedZRotation > 180)
            {
                correctedZRotation -= 360;
            }
        }

        //Rotating our ship to face the target rotation object
        Quaternion lookRotation = Quaternion.LookRotation(this.targetRotationObj.position - this.ourShip.xGyroscope.position);
        this.ourShip.xGyroscope.transform.rotation = Quaternion.Lerp(this.ourShip.xGyroscope.rotation, lookRotation, 1);
        this.ourShip.xGyroscope.localEulerAngles = new Vector3(this.ourShip.xGyroscope.localEulerAngles.x, this.ourShip.xGyroscope.localEulerAngles.y, 0);
    }
}

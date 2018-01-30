using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Weapon))]
[RequireComponent(typeof(HealthAndArmor))]
public class EnemyTurret : MonoBehaviour
{
    //This enemy's weapon reference
    private Weapon ourWeapon;

    //Enum to determine which player this weapon targets
    public enum EnemyTarget { Player1, Player2, Closest };
    public EnemyTarget targetType = EnemyTarget.Closest;

    //Reference to the player that we're attacking
    private PlayerShipController targetPlayer;

    //The number of shots in a clip
    public int clipSize = 5;
    //The current amount of shots remaining
    private int currentClipSize = 0;

    //The amount of time between each shot
    public float timeBetweenShots = 0.5f;
    //The current amount of time between shots
    private float currentTimeBetweenShots = 0;

    //The cooldown time after each clip is empty
    public float cooldownAfterClip = 2f;
    //The current amount of time we have to wait after our clip is empty
    private float currentCooldown = 0;

    //Class used to rotate different parts of this turret to face the target player
    [System.Serializable]
    public class ObjToRotate
    {
        //The transform that we rotate
        public Transform objToRotate;
        //The speed that this object rotates to face the target
        public float rotationSpeed = 2;
        //The bools for each axis that we rotate
        public bool rotateX = false;
        public bool rotateY = false;
        public bool rotateZ = false;
    }

    //List for all of the objects to rotate so that we face the player correctly
    public List<ObjToRotate> rotationObjects;



	// Use this for initialization
	private void Start ()
    {
        //Getting our weapon reference
        this.ourWeapon = this.GetComponent<Weapon>();

        //If we always target player 1, we set our target to the player 1 ship
        if(this.targetType == EnemyTarget.Player1)
        {
            this.targetPlayer = PlayerShipController.p1ShipRef;
        }
        //If we always target player 2, we set our target to the player 2 ship
        else if(this.targetType == EnemyTarget.Player2)
        {
            //If the player 2 ship exists, we target it
            if(PlayerShipController.p2ShipRef != null)
            {
                this.targetPlayer = PlayerShipController.p2ShipRef;
            }
            //If the player 2 ship doesn't exist, we disable this object
            else
            {
                this.gameObject.SetActive(false);
            }
        }
	}
	

	// Update is called once per frame
	private void Update ()
    {
		//If our target type focuses on the closest enemy, we find the closest
        if(this.targetType == EnemyTarget.Closest)
        {
            //If the player 2 ship doesn't exist, we go with player 1 by default
            if(PlayerShipController.p2ShipRef == null)
            {
                this.targetPlayer = PlayerShipController.p1ShipRef;
            }
            //Otherwise we actually have to find the closest
            else
            {
                //Floats to hold the distances to each ship
                float p1Dist = Vector3.Distance(this.transform.position, PlayerShipController.p1ShipRef.transform.position);
                float p2Dist = Vector3.Distance(this.transform.position, PlayerShipController.p2ShipRef.transform.position);

                //If the player 1 distance is closer, we use that ship
                if(p1Dist < p2Dist)
                {
                    this.targetPlayer = PlayerShipController.p1ShipRef;
                }
                //If the player 2 distance is closer, we use that ship
                else
                {
                    this.targetPlayer = PlayerShipController.p2ShipRef;
                }
            }
        }

        //Variable to hold the offset of the player position to target
        Vector3 posToShoot = this.LeadShipPosition();


        //Looping through all of our rotation objects so they face our target player
        foreach(ObjToRotate objR in this.rotationObjects)
        {
            //If we rotate all of the axis, then we can rotate using quaternions
            if(objR.rotateX && objR.rotateY && objR.rotateZ)
            {
                //Getting the quaternion rotation to face the player position
                Quaternion newRot = Quaternion.LookRotation(posToShoot - objR.objToRotate.position);
                //Rotating to face the new rotation given our rotation speed
                objR.objToRotate.rotation = Quaternion.Lerp(objR.objToRotate.rotation, newRot, objR.rotationSpeed);
            }
            //Otherwise, we rotate for each designated axis
            else
            {
                //If we rotate the X axis
                if(objR.rotateX)
                {
                    //Getting the quaternion rotation to only move the X rotation to face the player position
                    //Vector3 xLookPos = new Vector3(posToShoot.x - objR.objToRotate.position.x, 0, 0);
                    Vector3 xLookPos = new Vector3(0, posToShoot.y - objR.objToRotate.position.y, posToShoot.z - objR.objToRotate.position.z);
                    Quaternion newXRot = Quaternion.LookRotation(xLookPos);
                    //Rotating the X direction to face the new rotation given our speed
                    objR.objToRotate.rotation = Quaternion.Lerp(objR.objToRotate.rotation, newXRot, objR.rotationSpeed);

                    //If this object ONLY rotates the X value, we clamp the YZ rotations
                    if(objR.rotateX && !objR.rotateY && !objR.rotateZ)
                    {
                        objR.objToRotate.localEulerAngles = new Vector3(objR.objToRotate.localEulerAngles.x, 0, 0);
                    }
                }

                //If we rotate the Y axis
                if(objR.rotateY)
                {
                    //Getting the quaternion rotation to only move the Y rotation to face the player position
                    //Vector3 yLookPos = new Vector3(0, posToShoot.y - objR.objToRotate.position.y, 0);
                    Vector3 yLookPos = new Vector3(posToShoot.x - objR.objToRotate.position.x, 0, posToShoot.z - objR.objToRotate.position.z);
                    Quaternion newYRot = Quaternion.LookRotation(yLookPos);
                    //Rotating the Y direction to face the new rotation given our speed
                    objR.objToRotate.rotation = Quaternion.Lerp(objR.objToRotate.rotation, newYRot, objR.rotationSpeed);

                    //If this object ONLY rotates the Y value, we clamp the XZ rotations
                    if (!objR.rotateX && objR.rotateY && !objR.rotateZ)
                    {
                        objR.objToRotate.localEulerAngles = new Vector3(0, objR.objToRotate.localEulerAngles.y, 0);
                    }
                }

                //If we rotate the Z axis
                if(objR.rotateZ)
                {
                    //Getting the quaternion rotation to only move the Z rotation to face the player position
                    //Vector3 zLookPos = new Vector3(0, 0, posToShoot.z - objR.objToRotate.position.z);
                    Vector3 zLookPos = new Vector3(posToShoot.x - objR.objToRotate.position.x, posToShoot.y - objR.objToRotate.position.y, 0);
                    Quaternion newZRot = Quaternion.LookRotation(zLookPos);
                    //Rotating the X direction to face the new rotation given our speed
                    objR.objToRotate.rotation = Quaternion.Lerp(objR.objToRotate.rotation, newZRot, objR.rotationSpeed);

                    //If this object ONLY rotates the Z value, we clamp the XY rotations
                    if (!objR.rotateX && !objR.rotateY && objR.rotateZ)
                    {
                        objR.objToRotate.localEulerAngles = new Vector3(0, 0, objR.objToRotate.localEulerAngles.z);
                    }
                }
            }
        }


        //Counting down our cooldown times if they exist
        if(this.currentCooldown > 0)
        {
            this.currentCooldown -= Time.deltaTime;

            //If our cooldown between firing is up, we reload our current clip
            this.currentClipSize = this.clipSize;
        }
        if(this.currentTimeBetweenShots > 0)
        {
            this.currentTimeBetweenShots -= Time.deltaTime;
        }


        //Firing at the player whenever possible
        if(this.currentCooldown <= 0)
        {
            //If our time between shots is up, we can fire
            if(this.currentTimeBetweenShots <= 0)
            {
                //Firing our weapon
                this.ourWeapon.FireWeapon(true, false, false);
                //Reducing the number of shots in our clip
                this.currentClipSize -= 1;

                //If our clip is empty, we start our cooldown
                if(this.currentClipSize < 1)
                {
                    this.currentCooldown = this.cooldownAfterClip;
                }
                //Otherwise we start our cooldown between shots
                else
                {
                    this.currentTimeBetweenShots = this.timeBetweenShots;
                }
            }
        }
    }


    //Function called from Update to get the position where the player ship will be
    private Vector3 LeadShipPosition()
    {
        //The vec3 position that we return
        Vector3 targetPos = this.targetPlayer.transform.position;

        //Float that holds the position between our muzzle and the player ship
        float distToTarget = Vector3.Distance(this.ourWeapon.muzzleAudio.transform.position, targetPos);

        //Finding the amount of time it would take for our weapon's projectile would take to cover that distance
        //Using the formula Velocity = Distance / Time  ===>   Time = Distance / Velocity
        float projectileTime = distToTarget / this.ourWeapon.firedProjectile.forwardVelocity;

        //Now that we know about how long it will take for the projectile to reach the player's CURRENT position,
        //We need to guess how far along they will be using their current velocity

        //If the ship is in a free movement zone, we use their forward velocity
        if (this.targetPlayer.ourFreeMovement.enabled)
        {
            //Getting the direction and magnitude that the velocity is facing
            Vector3 velocityOffset = this.targetPlayer.ourRailMovement.railParentObj.ourRigidbody.velocity;
            //Multiplying the velocity 
            velocityOffset = velocityOffset * projectileTime;
            //Adding the forward velocity offset to the target position
            targetPos += velocityOffset;
        }
        //If the ship is in a rail movement zone, we need to use the rail position
        else if(this.targetPlayer.ourRailMovement.enabled)
        {
            //Getting the reference to the spline that the target ship is moving on
            BezierSpline shipSpline = this.targetPlayer.ourRailMovement.railParentObj.ourSplineMoveRB.splineToFollow;

            //Getting the current amount of time that the ship has already traveled
            float currentSplineTime = this.targetPlayer.ourRailMovement.railParentObj.ourSplineMoveRB.CurrentSplineTime;
            //Getting the speed multiplier that the player is moving at
            float speedMultiplier = this.targetPlayer.ourRailMovement.railParentObj.ourSplineMoveRB.speedMultiplier;
            //Adding the projectile time to the current spline time so we get the position where the player will be
            currentSplineTime += projectileTime * (1f / speedMultiplier);

            //Getting the total time that the player will have to travel along the spline
            float totalSplineTime = this.targetPlayer.ourRailMovement.railParentObj.ourSplineMoveRB.timeToComplete;

            //Getting the position along the spline that the ship will be at when taking into account the projectile time
            targetPos = shipSpline.GetPoint(currentSplineTime / totalSplineTime);

            //Adding the offset that the player ship is from the rail parent
            //targetPos
        }

        //Returning our target pos
        return targetPos;
    }
}



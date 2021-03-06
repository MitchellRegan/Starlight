﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class RailParentCollisionLogic : MonoBehaviour
{
    //The reference to our ship's RailMovementFlight component
    public PlayerShipController ourShipController;

    //The reference to this object's rigidbody component
    public Rigidbody ourRigidbody;

    //The reference to this object's MoveAlongSplineRigidBody component
    [HideInInspector]
    public MoveAlongSplineRigidBody ourSplineMoveRB;
	


    //Function called on the first frame this object is alive
    private void Awake()
    {
        //Getting the reference to this object's rigid body and move along spline rigid body component
        this.ourRigidbody = this.GetComponent<Rigidbody>();
        this.ourSplineMoveRB = this.GetComponent<MoveAlongSplineRigidBody>();
    }


    //Function called when we hit a trigger collider
    private void OnTriggerEnter(Collider collider_)
    {
        //If the object hit has a RegionZone.cs component, we change our movement behaviors
        if (collider_.gameObject.GetComponent<RegionZone>())
        {
            //Reference to the region zone component
            RegionZone newRegion = collider_.gameObject.GetComponent<RegionZone>();

            //If the region zone effects this player
            if ((newRegion.affectPlayer1 && this.ourShipController.playerController == Players.P1) ||
                (newRegion.affectPlayer2 && this.ourShipController.playerController == Players.P2))
            {
                //If the region has rail movement
                if (newRegion.movementType == RegionZone.RegionMovement.Rail)
                {
                    //We disable our free movement controls and enable our rail movement controls
                    this.ourShipController.ourFreeMovement.enabled = false;
                    this.ourShipController.ourRailMovement.enabled = true;

                    //Enabling our spline move object and telling it to use our new region's designated spline
                    this.ourSplineMoveRB.enabled = true;
                    this.ourSplineMoveRB.SetSplineToFollow(newRegion.railZoneSplineToFollow);

                    //Setting the new direction for our rail movement
                    this.ourShipController.ourRailMovement.SetNewRailDirection(collider_);
                }
            }
        }
        //If the object hit has a ChangeRailBoundingBox.cs component, we change our bounding box
        else if(collider_.gameObject.GetComponent<ChangeRailBoundingBox>())
        {
            //Making sure this ship is using the rail movement, because the bounding box means nothing in free
            if(this.ourShipController.ourRailMovement.enabled)
            {
                ChangeRailBoundingBox newBounds = collider_.gameObject.GetComponent<ChangeRailBoundingBox>();

                //If the bounding box affects this ship
                if ((newBounds.affectPlayer1 && this.ourShipController.playerController == Players.P1) ||
                    (newBounds.affectPlayer2 && this.ourShipController.playerController == Players.P2))
                {
                    //Setting our ship's new bounding box size
                    this.ourShipController.ourRailMovement.SetNewBoundingBox(collider_.gameObject.GetComponent<ChangeRailBoundingBox>().newBoundingBox);
                }
            }
        }
    }
}

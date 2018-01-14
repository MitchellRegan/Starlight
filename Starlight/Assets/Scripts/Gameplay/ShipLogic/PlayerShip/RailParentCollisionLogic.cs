﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RailParentCollisionLogic : MonoBehaviour
{
    //The reference to our ship's RailMovementFlight component
    public PlayerShipController ourShipController;

	

    //Function called when we hit a trigger collider
    private void OnTriggerEnter(Collider collider_)
    {
        //If the object hit has a RegionZone.cs component, we change our movement behaviors
        if (collider_.gameObject.GetComponent<RegionZone>())
        {
            //If the region zone effects this player
            if ((collider_.gameObject.GetComponent<RegionZone>().effectPlayer1 && this.ourShipController.playerController == Players.P1) ||
                (collider_.gameObject.GetComponent<RegionZone>().effectPlayer2 && this.ourShipController.playerController == Players.P2))
            {
                //If the region has rail movement
                if (collider_.gameObject.GetComponent<RegionZone>().movementType == RegionZone.RegionMovement.Rail)
                {
                    //We disable our free movement controls and enable our rail movement controls
                    this.ourShipController.ourFreeMovement.enabled = false;
                    this.ourShipController.ourRailMovement.enabled = true;

                    //Setting the new direction for our rail movement
                    this.ourShipController.ourRailMovement.SetNewRailDirection(collider_);
                }
                //If the region has free movement
                /*else if (collider_.gameObject.GetComponent<RegionZone>().movementType == RegionZone.RegionMovement.Free)
                {
                    //We disable our rail movement controls and enable our free movement controls
                    this.ourShipController.ourRailMovement.BeforeDisable();
                    this.ourShipController.ourRailMovement.enabled = false;
                    this.ourShipController.ourFreeMovement.enabled = true;
                }*/
            }
        }
    }


    //Function called when we stop hitting a trigger collider
    private void OnTriggerExit(Collider collider_)
    {
        //If the hit object is a Region Zone and that region zone wants us to interpolate to another region
        if(collider_.gameObject.GetComponent<RegionZone>())
        {
            //if the region zone wants us to interpolate to another region, we tell our ship's RailMovementFlight.cs component
            if (collider_.gameObject.GetComponent<RegionZone>().interpToRegion != null)
            {
                this.ourShipController.ourRailMovement.InterpToNextRegion(this.transform, collider_.GetComponent<RegionZone>().interpToRegion);
            }

            //Turning off the region's collider component so we don't hit it again
            collider_.enabled = false;
        }
    }
}
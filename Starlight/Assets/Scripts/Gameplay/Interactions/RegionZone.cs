using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RegionZone : MonoBehaviour
{
    //Enum that determines if this region is free movement or on a rail
    public enum RegionMovement { Free, Rail };
    public RegionMovement movementType = RegionMovement.Rail;

    //Bools for which player this region effects
    public bool affectPlayer1 = true;
    public bool affectPlayer2 = true;

    //If this is a rail zone, we need to designate which rail zone the player will follow
    public BezierSpline railZoneSplineToFollow;
}

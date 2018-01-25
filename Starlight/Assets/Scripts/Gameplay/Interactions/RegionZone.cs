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
    public bool effectPlayer1 = true;
    public bool effectPlayer2 = true;

    //If this is a rail zone, we need to designate which rail zone the player will follow
    public BezierSpline railZoneSplineToFollow;
    //The amount of time that it takes to complete this rail zone
    public float timeToFinishSpline = 10;

    //UnityEvent called when the player hits this zone's collider
    public UnityEvent onCollisionEvent;

    //The next Region Zone that the player will interpolate to after leaving this one. If null, nothing happens
    public RegionZone interpToRegion;
}

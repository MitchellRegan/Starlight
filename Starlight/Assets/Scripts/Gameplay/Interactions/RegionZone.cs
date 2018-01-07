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

    //The multiplier to forward thrust that player ships move at in this zone
    [Range(0.1f, 2)]
    public float thrustMultiplier = 1;

    //UnityEvent called when the player hits this zone's collider
    public UnityEvent onCollisionEvent;
}

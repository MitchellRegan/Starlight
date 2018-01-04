using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionZone : MonoBehaviour
{
    //Enum that determines if this region is free movement or on a rail
    public enum RegionMovement { Free, Rail };
    public RegionMovement movementType = RegionMovement.Rail;

    //Enum for which player this region effects
    public Players effectedPlayer = Players.P1;



	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ChangeRailBoundingBox : MonoBehaviour
{
    //The new bounding box dimensions for the rail zone
    public Vector2 newBoundingBox = new Vector2();

    //Bools that determine which player is affected
    public bool affectPlayer1 = true;
    public bool affectPlayer2 = true;
}

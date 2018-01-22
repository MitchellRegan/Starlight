using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplineCurve : MonoBehaviour
{
    //The starting point for this spline
    public Transform startPoint;
    //The end point for this spline
    public Transform endPoint;

    [Space(8)]

    //The number of segments that make up this cure
    [Range(0, 20)]
    public int segments = 3;
}



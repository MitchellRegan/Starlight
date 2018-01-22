using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve : MonoBehaviour
{
    //Color for the curve
    public Color curveColor = Color.red;

    //Color for the line connecting the handles
    public Color handleLineColor = Color.green;

    //Color for the velocity lines
    public Color velocityLineColor = Color.yellow;

    //The list of control points that make up this curve
    public List<Vector3> points;



    //Function called externally to reset our curve to the default
    public void Reset()
    {
        this.points = new List<Vector3>()
        {
            new Vector3(1f, 0f, 0f),
            new Vector3(2f, 0f, 0f),
            new Vector3(3f, 0f, 0f),
            new Vector3(4f, 0f, 0f)
        };
    }


    //Function called externally from BezierCurveInspector.cs to access a specific handle point on our curve
    public Vector3 GetPoint(float t_)
    {
        return this.transform.TransformPoint(Bezier.GetPoint(this.points[0], this.points[1], this.points[2], this.points[3], t_));
    }


    //Function called externally from BezierCurveInspector.cs to get the tangent lines to the curve
    public Vector3 GetVelocity(float t_)
    {
        return this.transform.TransformPoint(Bezier.GetFirstDerivative(this.points[0], this.points[1], this.points[2], this.points[3], t_)) - this.transform.position;
    }


    //Function called externally from BezierCurveInspector.cs to get the normalized velocity
    public Vector3 GetDirection(float t_)
    {
        return this.GetVelocity(t_).normalized;
    }
}


//Static class used by BezierCurve.cs and BezierCurveInspector.cs
public static class Bezier
{
    //Function to get the point along the line between p0 and p2 based on the time t
    public static Vector3 GetPoint(Vector3 p0_, Vector3 p1_, Vector3 p2_, Vector3 p3_, float t_)
    {
        //Making sure the time value given is between 0 and 1
        t_ = Mathf.Clamp01(t_);
        float oneMinusT = 1f - t_;
        return (oneMinusT * oneMinusT * oneMinusT * p0_) + 
               (3f * oneMinusT * oneMinusT * t_ * p1_) +
               (3f * oneMinusT * t_ * t_ * p2_) + 
               (t_ * t_ * t_ * p3_);
    }


    //Function to get the rate of change of the curve
    public static Vector3 GetFirstDerivative(Vector3 p0_, Vector3 p1_, Vector3 p2_, Vector3 p3_, float t_)
    {
        //Making sure the time value given is between 0 and 1
        t_ = Mathf.Clamp01(t_);
        float oneMinusT = 1f - t_;
        return (3f * oneMinusT * oneMinusT * (p1_ - p0_)) +
               (6f * oneMinusT * t_ * (p2_ - p1_)) +
               (3f * t_ * t_ * (p3_ - p2_));
    }
}
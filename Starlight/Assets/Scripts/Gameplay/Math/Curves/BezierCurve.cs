using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve : MonoBehaviour
{
    //The list of control points that make up this curve
    public List<Vector3> points;



    //Function called externally to reset our curve to the default
    public void Reset()
    {
        this.points = new List<Vector3>()
        {
            new Vector3(1f, 0f, 0f),
            new Vector3(2f, 0f, 0f),
            new Vector3(3f, 0f, 0f)
        };
    }


    //Function called externally from BezierCurveInspector.cs to access a specific handle point on our curve
    public Vector3 GetPoint(float t_)
    {
        return this.transform.TransformPoint(Bezier.GetPoint(this.points[0], this.points[1], this.points[2], t_));
    }


    public Vector3 GetVelocity(float t_)
    {
        return this.transform.TransformPoint(Bezier.GetFirstDerivative(this.points[0], this.points[1], this.points[2], t_)) - this.transform.position;
    }
}


//Static class used by BezierCurve.cs and BezierCurveInspector.cs
public static class Bezier
{
    public static Vector3 GetPoint(Vector3 p0_, Vector3 p1_, Vector3 p2_, float t_)
    {
        //return Vector3.Lerp(Vector3.Lerp(p0_, p1_, t_), Vector3.Lerp(p1_, p2_, t_), t_);
        t_ = Mathf.Clamp01(t_);
        float oneMinusT = 1f - t_;
        return (oneMinusT * oneMinusT * p0_) + (2f * oneMinusT * t_ * p1_) + (t_ * t_ * p2_);
    }


    public static Vector3 GetFirstDerivative(Vector3 p0_, Vector3 p1_, Vector3 p2_, float t_)
    {
        return (2f * (1f - t_) * (p1_ - p0_)) + (2f * t_ * (p2_ - p1_));
    }
}
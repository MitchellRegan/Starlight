using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierSpline : MonoBehaviour
{
    //Color for the spline
    public Color splineColor = Color.red;
    public float splineWidth = 5;

    //Color for the line connecting the handles
    public Color handleLineColor = Color.green;

    //Color for the velocity lines
    public Color velocityLineColor = Color.yellow;

    //The list of control points that make up this spline
    public List<Vector3> points;

    //Int that holds the number of bezier curves along this spline
    public int CurveCount
    {
        get
        {
            return (this.points.Count - 1) / 3;
        }
    }



    //Function called externally to reset our spline to the default
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


    //Function called externally from BezierSplineInspector.cs to access a specific handle point on our spline based on the percentage of the way through the spline (t_)
    public Vector3 GetPoint(float t_)
    {
        //Int to hold the index of the starting point of the curve we're going to find the point along
        int i;

        //If the percent along the spline is above 100%, we find the starting point of the last curve
        if(t_ >= 1f)
        {
            t_ = 1f;
            i = this.points.Count - 4;
        }
        //Otherwise we find the starting point of the curve that's at the percent given
        else
        {
            //Finding which curve is at the percent
            t_ = Mathf.Clamp01(t_) * this.CurveCount;
            i = (int)t_;
            //Finding the index of the first point along that curve
            t_ -= i;
            i *= 3;
        }

        //Getting the point along our curve at the given percent
        return this.transform.TransformPoint(Bezier.GetPoint(this.points[i], 
                                                             this.points[i + 1], 
                                                             this.points[i + 2], 
                                                             this.points[3], 
                                                             t_));
    }


    //Function called externally from BezierSplineInspector.cs to get the tangent lines to the spline
    public Vector3 GetVelocity(float t_)
    {
        //Int to hold the index of the starting point of the curve we're going to find the point along
        int i;

        //If the percent along the spline is above 100%, we find the starting point of the last curve
        if (t_ >= 1f)
        {
            t_ = 1f;
            i = this.points.Count - 4;
        }
        //Otherwise we find the starting point of the curve that's at the percent given
        else
        {
            //Finding which curve is at the percent
            t_ = Mathf.Clamp01(t_) * this.CurveCount;
            i = (int)t_;
            //Finding the index of the first point along that curve
            t_ -= i;
            i *= 3;
        }

        //Getting the rate of change of the bezier at the given percent
        return this.transform.TransformPoint(Bezier.GetFirstDerivative(this.points[i], 
                                                                       this.points[i + 1], 
                                                                       this.points[i + 2], 
                                                                       this.points[i + 3], 
                                                                       t_)) - this.transform.position;
    }


    //Function called externally from BezierSplineInspector.cs to get the normalized velocity
    public Vector3 GetDirection(float t_)
    {
        return this.GetVelocity(t_).normalized;
    }


    //Function called externally from BezierSplineInspector to add 3 new points to our spline curve
    public void AddCurve()
    {
        //Getting the last point on our spline
        Vector3 point = this.points[this.points.Count - 1];
        //Adding the first new point
        this.points.Add(new Vector3(point.x + 1, 0, 0));
        //Adding the second new point
        this.points.Add(new Vector3(point.x + 2, 0, 0));
        //Adding the third new point
        this.points.Add(new Vector3(point.x + 3, 0, 0));
    }
}

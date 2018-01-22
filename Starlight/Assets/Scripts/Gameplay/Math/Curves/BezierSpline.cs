using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    //[SerializeField]
    private Vector3[] points;

    //Enum for what mode the control points will be in
    public enum BezierControlPointMode
    {
        Free,
        Aligned,
        Mirrored
    }

    //An array of BezierControlPointMode enums for each of our control points
    private BezierControlPointMode[] modes;



    //Accessor function to get the length of our points list
    public int ControlPointCount
    {
        get
        {
            return this.points.Length;
        }
    }


    //Int that holds the number of bezier curves along this spline
    public int CurveCount
    {
        get
        {
            return (this.points.Length - 1) / 3;
        }
    }
    

    //Accessor function to get the position of a control point at the given index
    public Vector3 GetControlPoint(int index_)
    {
        return this.points[index_];
    }


    //Accessor function to set the position of a control point at the given index
    public void SetControlPoint(int index_, Vector3 point_)
    {
        this.points[index_] = point_;
        //Enforcing the control point's mode
        this.EnforceMode(index_);
    }


    //Accessor function to get the control point mode for the point at the given index
    public BezierControlPointMode GetControlPointMode(int index_)
    {
        return this.modes[(index_ + 1) / 3];
    }


    //Accessor function to set the control point mode for the point at the given index
    public void SetControlPointMode(int index_, BezierControlPointMode mode_)
    {
        this.modes[(index_ + 1) / 3] = mode_;
        //Enforcing the control point's mode
        this.EnforceMode(index_);
    }


    //Function called externally to reset our spline to the default
    public void Reset()
    {
        //Setting our default control point positions
        this.points = new Vector3[]
        {
            new Vector3(0f, 0f, 0f),
            new Vector3(1f, 0f, 0f),
            new Vector3(2f, 0f, 0f),
            new Vector3(3f, 0f, 0f)
        };

        //Setting our default control modes for the newly added points
        this.modes = new BezierControlPointMode[]
        {
            BezierControlPointMode.Free,
            BezierControlPointMode.Free
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
            i = this.points.Length - 4;
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
                                                             this.points[i + 3], 
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
            i = this.points.Length - 4;
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


    //
    private void EnforceMode(int index_)
    {
        //Getting the index of our mode for the given control point
        int modeIndex = (index_ + 1) / 3;
        BezierControlPointMode mode = this.modes[modeIndex];

        //If the control mode is free or the control point is either the start or end points, nothing happens
        if(mode == BezierControlPointMode.Free || modeIndex == 0 || modeIndex == modes.Length - 1)
        {
            return;
        }

        //Ints to hold the indices of the points that are fixed (not moving) and the enforced point (made to follow the control point mode)
        int middleIndex = modeIndex * 3;
        int fixedIndex;
        int enforcedIndex;

        //Finding the index of the fixed point and the point that will be enforced
        if(index_ <= middleIndex)
        {
            fixedIndex = middleIndex - 1;
            enforcedIndex = middleIndex + 1;
        }
        else
        {
            fixedIndex = middleIndex + 1;
            enforcedIndex = middleIndex - 1;
        }

        Vector3 middle = this.points[middleIndex];
        Vector3 enforcedTangent = middle - this.points[fixedIndex];
        this.points[enforcedIndex] = middle + enforcedTangent;

        //If the control mode is on alligned, we make sure the new tangent has the same length as the old one
        if(mode == BezierControlPointMode.Aligned)
        {
            enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, this.points[enforcedIndex]);
        }
        this.points[enforcedIndex] = middle + enforcedTangent;
    }


    //Function called externally from BezierSplineInspector to add 3 new points to our spline curve
    public void AddCurve()
    {
        //Getting the last point on our spline
        Vector3 point = this.points[this.points.Length - 1];

        //Resizing our points array to add 3 more
        Array.Resize(ref this.points, this.points.Length + 3);

        //Making it so that the newly added points are offset from the previously last point
        this.points[this.points.Length - 3].x = point.x + 1;
        this.points[this.points.Length - 2].x = point.x + 2;
        this.points[this.points.Length - 1].x = point.x + 3;

        //Adding a new mode control type to the newly added point
        Array.Resize(ref this.modes, this.modes.Length + 1);
        this.modes[this.modes.Length - 1] = this.modes[modes.Length - 2];
    }
}

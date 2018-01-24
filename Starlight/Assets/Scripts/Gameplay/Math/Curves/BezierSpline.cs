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
    [HideInInspector]
    [SerializeField]
    private Vector3[] points =
    {
        new Vector3(0f, 0f, 0f),
        new Vector3(3f, 0f, 0f),
        new Vector3(6f, 0f, 0f),
        new Vector3(9f, 0f, 0f)
    };

    //Enum for what mode the control points will be in
    public enum BezierControlPointMode
    {
        Free,
        Aligned,
        Mirrored
    }

    //An array of BezierControlPointMode enums for each of our control points
    [HideInInspector]
    [SerializeField]
    private BezierControlPointMode[] modes =
    {
        BezierControlPointMode.Free,
        BezierControlPointMode.Free
    };

    //An array of points that determine the Up direction for objects traveling along this spline
    [HideInInspector]
    [SerializeField]
    private Vector3[] upRotationPoints =
    {
        new Vector3(0f, 1f, 0f),
        new Vector3(9f, 1f, 0f)
    };

    //Bool that determines if this spline loops back around to the start point
    private bool loop = false;

    

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
        //If the point at the given index is a center control point
        if(index_ % 3 == 0)
        {
            //Finding the distance that this point is being moved
            Vector3 delta = point_ - this.points[index_];

            //If this spline loops back around
            if (this.loop)
            {
                //If this control point is the first point in the spline
                if(index_ == 0)
                {
                    //Moving the next control point (this point's handle), the last control point in the spline, and the last point's handle
                    this.points[1] += delta;
                    this.points[this.points.Length - 1] = point_;
                    this.points[this.points.Length - 2] += delta;
                    this.upRotationPoints[0] += delta;
                }
                //If this control point is the last point in the spline
                else if(index_ == this.points.Length - 1)
                {
                    //Moving the previous control point (this point's handle), the first control point in the spline, and the first point's handle
                    this.points[0] = point_;
                    this.points[1] += delta;
                    this.points[this.points.Length - 1] += delta;
                    this.upRotationPoints[this.upRotationPoints.Length - 1] += delta;
                }
                //If this control point is somewhere between the start and end points
                else
                {
                    //Moving both of these control point's handles
                    this.points[index_ - 1] += delta;
                    this.points[index_ + 1] += delta;
                    this.upRotationPoints[(index_ + 1) / 3] += delta;
                }
            }
            //If this spline doesn't loop
            else
            {
                //If the point isn't the starting point of the spline
                if (index_ > 0)
                {
                    //We move the previous control point handle the same distance as this control point
                    this.points[index_ - 1] += delta;
                }
                //If the point isn't the end point of the spline
                if (index_ + 1 < this.points.Length)
                {
                    //We move the next control point handle the same distance as this control point
                    this.points[index_ + 1] += delta;
                }

                this.upRotationPoints[(index_ + 1) / 3] += delta;
            }
        }

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
        //Getting the index for the point that controls this point's mode
        int modeIndex = (index_ + 1) / 3;
        this.modes[modeIndex] = mode_;

        //If this spline loops
        if(this.loop)
        {
            //If this control point is the first node in the spline
            if(modeIndex == 0)
            {
                //We set the last control point's mode to the same mode as this one
                this.modes[this.modes.Length - 1] = mode_;
            }
            //If this control point is the last node in the spline
            else if(modeIndex == this.modes.Length - 1)
            {
                //We set the first control point's mode to the same mode as this one
                this.modes[0] = mode_;
            }
        }

        //Enforcing the control point's mode
        this.EnforceMode(index_);
    }


    //Accessor function to get the Up rotation point at the given index
    public Vector3 GetUpRotationPoint(int index_)
    {
        return this.upRotationPoints[index_ / 3];
    }


    //Accessor function to set the Up rotatino point at the given index
    public void SetUpRotationPoint(int index_, Vector3 point_)
    {
        //Finding the control point at the given index so we can find out the direction it's facing
        int controlPointIndex = (index_ + 1) / 3;

        //Getting the % progress that this control point is along the spline
        float progress = (float)controlPointIndex / (float)(this.points.Length - 1);

        //Getting the forward direction of the control point along the spline
        Vector3 forward = this.GetDirection(progress);

        this.upRotationPoints[controlPointIndex] = point_;//Vector3.ProjectOnPlane(point_, forward);
    }


    //Accessor function to designate if this spline should loop or not
    public bool Loop
    {
        get
        {
            return this.loop;
        }
        set
        {
            this.loop = value;
            //If we should loop this spline
            if(value == true)
            {
                //We set the last control point mode to the same as the first point's mode
                this.modes[this.modes.Length - 1] = this.modes[0];
                this.SetControlPoint(0, this.points[0]);
            }
        }
    }


    //Function called externally to reset our spline to the default
    public void Reset()
    {
        //Setting our default control point positions
        this.points = new Vector3[]
        {
            new Vector3(0f, 0f, 0f),
            new Vector3(3f, 0f, 0f),
            new Vector3(6f, 0f, 0f),
            new Vector3(9f, 0f, 0f)
        };

        //Setting our default control modes for the newly added points
        this.modes = new BezierControlPointMode[]
        {
            BezierControlPointMode.Aligned,
            BezierControlPointMode.Aligned
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


    //Function called from AddCurve, SetControlPoint, and SetControlPointMode to make sure the handles next to each control point are behaving correctly
    private void EnforceMode(int index_)
    {
        //Getting the index of our mode for the given control point
        int modeIndex = (index_ + 1) / 3;
        BezierControlPointMode mode = this.modes[modeIndex];

        //If the control mode is free or the control point is either the start or end points, nothing happens
        if(mode == BezierControlPointMode.Free || !loop && (modeIndex == 0 || modeIndex == modes.Length - 1))
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
            if(fixedIndex < 0)
            {
                fixedIndex = this.points.Length - 2;
            }
            enforcedIndex = middleIndex + 1;
            if(enforcedIndex >= this.points.Length)
            {
                enforcedIndex = 1;
            }
        }
        else
        {
            fixedIndex = middleIndex + 1;
            if(fixedIndex >= this.points.Length)
            {
                fixedIndex = 1;
            }
            enforcedIndex = middleIndex - 1;
            if(fixedIndex < 0)
            {
                enforcedIndex = this.points.Length - 2;
            }
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
        this.points[this.points.Length - 3].x = point.x + 3;
        this.points[this.points.Length - 2].x = point.x + 6;
        this.points[this.points.Length - 1].x = point.x + 9;

        //Adding a new mode control type to the newly added point
        Array.Resize(ref this.modes, this.modes.Length + 1);
        this.modes[this.modes.Length - 1] = this.modes[modes.Length - 2];

        //Adding a new up rotation point
        Array.Resize(ref this.upRotationPoints, this.upRotationPoints.Length + 1);
        //Getting the distance difference that the previously last up rotation point is from the previously last control point
        Vector3 distDiff = this.upRotationPoints[this.upRotationPoints.Length - 2] - this.points[this.points.Length - 4];
        this.upRotationPoints[this.upRotationPoints.Length - 1] = this.points[this.points.Length -1] + distDiff;

        //Making sure the added handles are behaving correctly
        this.EnforceMode(this.points.Length - 4);

        //If we loop this spline, we make sure the new last point matches the first
        if(this.loop)
        {
            this.points[this.points.Length - 1] = this.points[0];
            this.modes[this.modes.Length - 1] = this.modes[0];
            this.upRotationPoints[this.upRotationPoints.Length - 1] = this.upRotationPoints[0];
            this.EnforceMode(0);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BezierSpline : MonoBehaviour
{
    //The width that this spline renders
    public float splineWidth = 5;
    //The radius of the rotation handle when shown
    public float rotationHandleRadius = 3;

    //Color for the spline
    public Color splineColor = Color.red;

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
        new Vector3(0f, 0f, 3f),
        new Vector3(0f, 0f, 6f),
        new Vector3(0f, 0f, 9f)
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
        new Vector3(0f, 3f, 0f),
        new Vector3(9f, 3f, 0f)
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
        return this.upRotationPoints[(index_ + 1) / 3];
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
        //Getting the forward direction of the last point
        Vector3 lastPointForward = this.GetDirection(1);

        //Getting the last point on our spline
        Vector3 point = this.points[this.points.Length - 1];

        //Resizing our points array to add 3 more
        Array.Resize(ref this.points, this.points.Length + 3);

        //Making it so that the newly added points are offset from the previously last point using the forward direction
        this.points[this.points.Length - 3] = point + (lastPointForward * 3);
        this.points[this.points.Length - 2] = point + (lastPointForward * 6);
        this.points[this.points.Length - 1] = point + (lastPointForward * 9);

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


    //Function called externally from BezierSplineInspector to remove a control point from our spline curve
    public void RemoveControlPoint(int index_)
    {
        //If our array of points only has 4 points (1 curve) we don't do anything because we shouldn't have a spline with only 1 point
        if(this.points.Length < 5)
        {
            return;
        }

        //If we're removing the first control point in the spline
        if(index_ == 0)
        {
            //Creating a new array of points that will not have the removed points
            Vector3[] newPointsArray = { };
            Array.Resize(ref newPointsArray, this.points.Length - 3);
            //Looping through our current array of points and copying all but the first 3
            for(int p = 0; p < newPointsArray.Length; ++p)
            {
                Vector3 pointPos = new Vector3(this.points[p + 3].x, this.points[p + 3].y, this.points[p + 3].z);
                newPointsArray[p] = pointPos;
            }
            //Setting our points array to the new, shorter array
            this.points = newPointsArray;

            //Creating a new array of modes that will not have the removed modes
            BezierControlPointMode[] newModesArray = { };
            Array.Resize(ref newModesArray, this.modes.Length - 1);
            //Looping through our current array of modes and copying all but the last one
            for(int m = 0; m < newModesArray.Length; ++m)
            {
                BezierControlPointMode newMode = this.modes[m + 1];
                newModesArray[m] = newMode;
            }
            //Setting our modes array to the new, shorter array
            this.modes = newModesArray;

            //Creating a new array of up direction points that will not have the removed point
            Vector3[] newUpDirectionArray = { };
            Array.Resize(ref newUpDirectionArray, this.upRotationPoints.Length - 1);
            //Looping through our current array of up direction points and copying all but the last one
            for (int u = 0; u < newUpDirectionArray.Length; ++u)
            {
                Vector3 newUpDirection = new Vector3(this.upRotationPoints[u + 1].x,
                                                     this.upRotationPoints[u + 1].y,
                                                     this.upRotationPoints[u + 1].z);
                newUpDirectionArray[u] = newUpDirection;
            }
            //Setting our up direction points array to the new, shorter array
            this.upRotationPoints = newUpDirectionArray;
        }
        //If we're removing the last control point in the spline
        else if(index_ == this.points.Length - 1)
        {
            //Removing the last three points in our array (the control point, its handle, and the handle before it)
            Array.Resize(ref this.points, this.points.Length - 3);

            //Removing the last mode for the bezier control mode
            Array.Resize(ref this.modes, this.points.Length - 1);

            //Removing the last up rotation point
            Array.Resize(ref this.upRotationPoints, this.points.Length - 1);
        }
        //If we're removing a control point somewhere in the middle
        else
        {
            //Creating a new array of points that will not have the removed points
            Vector3[] newPointsArray = { };
            Array.Resize(ref newPointsArray, this.points.Length - 3);
            //Looping through our current array of points and copying all but the 3 removed ones
            int pointOffset = 0;
            for (int p = 0; p + pointOffset < this.points.Length; ++p)
            {
                //If the current point is one of the ones we're removing we ignore it and increase the offset
                if (p + pointOffset == index_ || p + pointOffset == index_ - 1 || p + pointOffset == index_ + 1)
                {
                    pointOffset += 1;
                    p -= 1;
                }
                //If the current point isn't one of the ones we're removing, we add it to the new array
                else
                {
                    Vector3 pointPos = new Vector3(this.points[p + pointOffset].x,
                                                   this.points[p + pointOffset].y,
                                                   this.points[p + pointOffset].z);
                    newPointsArray[p] = pointPos;
                }
            }
            //Setting our points array to the new, shorter array
            this.points = newPointsArray;

            //Creating a new array of modes that will not have the removed modes
            BezierControlPointMode[] newModesArray = { };
            Array.Resize(ref newModesArray, this.modes.Length - 1);
            //Looping through our current array of modes and copying all but the removed one
            int modeOffset = 0;
            for (int m = 0; m + modeOffset < this.modes.Length; ++m)
            {
                //If the current mode is the one we're removing, we ignore it and increase the offset
                if (m + modeOffset == (index_ + 1) / 3)
                {
                    modeOffset += 1;
                    m -= 1;
                }
                //If the current mode isn't the one we're removing, we add it to the new array
                else
                {
                    BezierControlPointMode newMode = this.modes[m + modeOffset];
                    newModesArray[m] = this.modes[m + modeOffset];
                }
            }
            //Setting our modes array to the new, shorter array
            this.modes = newModesArray;

            //Creating a new array of up direction points that will not have the removed point
            Vector3[] newUpDirectionArray = { };
            Array.Resize(ref newUpDirectionArray, this.upRotationPoints.Length - 1);
            //Looping through our current array of up direction points and copying all but the removed one
            int upRotOffset = 0;
            for (int u = 0; u + upRotOffset < this.upRotationPoints.Length; ++u)
            {
                //If the current up rotation point is the one we're removing, we ignore it and increase the offset
                if (u + upRotOffset == (index_ + 1) / 3)
                {
                    upRotOffset += 1;
                    u -= 1;
                }
                //If the current up rotation point isn't the one we're removing, we add it to the new array
                else
                {
                    newUpDirectionArray[u] = this.upRotationPoints[u + upRotOffset];
                }
            }
            //Setting our up direction points array to the new, shorter array
            this.upRotationPoints = newUpDirectionArray;
        }
    }
}

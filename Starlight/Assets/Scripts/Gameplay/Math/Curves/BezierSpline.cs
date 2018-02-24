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
    //The size of the control point handles when shown
    public float controlPointHandleSize = 0.06f;

    [Space(8)]

    //The distance that added control points are set to when created
    public float addedPointDistance = 10;
    //The bounding box width and height for BezierSplineInspector.cs to display
    public Vector2 boundingBoxDisplay = new Vector2(30, 12);
    //The number of bounding boxes that BezierSplineInspector.cs will display
    [Range(1, 100)]
    public int numBoundingBoxToDisplay = 15;

    [Space(8)]

    //The time increment along the spline for BezierSplineInspector.cs to draw a node on
    public float timeIncrementDisplay = 6;

    [Space(8)]

    //Color for the spline
    public Color splineColor = Color.red;

    //Color for the line connecting the handles
    public Color handleLineColor = Color.green;

    //Color for the rotation handle lines
    public Color rotationLineColor = Color.blue;

    //Color for the time increment nodes
    public Color timeIncrementColor = Color.black;

    //Color for the bounding box that is drawn
    public Color boundingBoxColor = new Color(1, 0, 1, 1);
    public Material boundingBoxMaterial;

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
    private Quaternion[] controlPointOrientations =
    {
        new Quaternion(0,1,0,0),
        new Quaternion(0,1,0,0)
    };

    //An array of floats that determine the amount of time from each control point to the next
    [HideInInspector]
    [SerializeField]
    private float[] timeToNextPoint =
    {
        1,
        0
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
    

    //Function to get the time of the 
    public float TotalSplineTime
    {
        get
        {
            float totalTime = 0;
            foreach(float t in this.timeToNextPoint)
            {
                totalTime += t;
            }
            return totalTime;
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
                }
                //If this control point is the last point in the spline
                else if(index_ == this.points.Length - 1)
                {
                    //Moving the previous control point (this point's handle), the first control point in the spline, and the first point's handle
                    this.points[0] = point_;
                    this.points[1] += delta;
                    this.points[this.points.Length - 1] += delta;
                }
                //If this control point is somewhere between the start and end points
                else
                {
                    //Moving both of these control point's handles
                    this.points[index_ - 1] += delta;
                    this.points[index_ + 1] += delta;
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


    //Accessor function to get the orientation at the given index
    public Quaternion GetPointOrientation(int index_)
    {
        return this.controlPointOrientations[(index_ + 1) / 3] * this.transform.rotation;
    }


    //Accessor function to set the orientation at the given index
    public void SetPointOrientation(int index_, Quaternion newOrientation_)
    {
        //Finding the control point at the given index so we can find out the direction it's facing
        int controlPointIndex = (index_ + 1) / 3;

        //Getting the % progress that this control point is along the spline
        float progress = (float)controlPointIndex / (float)(this.points.Length - 1);

        //Getting the forward direction of the control point along the spline
        Vector3 forward = this.GetDirection(progress);
        //Getting the up direction of the control point along the spline
        Vector3 up = newOrientation_ * Vector3.up;

        //If the control point isn't the first one, we use the PREVIOUS control point
        if (controlPointIndex > 0)
        {
            //Debug.Log("First forward: " + forward + ", Second: " + (this.points[(controlPointIndex)] - this.points[(controlPointIndex - 1)]));
            //forward = Vector3.Cross(up, forward);
        }
        //If the control point index is 0, we need to use the NEXT control point
        else
        {
            //Debug.Log("First forward: " + forward + ", Second: " + (this.points[(controlPointIndex + 1)] - this.points[(controlPointIndex)]));
            //forward = Vector3.Cross(up, forward);
        }

        Quaternion newOrientation = Quaternion.LookRotation(forward, up);

        this.controlPointOrientations[controlPointIndex] = newOrientation;
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
        //If the percent along the spline is at or below 0%, we find the starting point of the first curve
        else if(t_ <= 0)
        {
            t_ = 0f;
            i = 0;
            return this.transform.TransformPoint(this.points[0]);
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


    //Function called externally from BezierSplineInspector.cs to get the orientation at the given percent
    public Quaternion GetOrientationAtPercent(float t_)
    {
        //Float to hold the percent of the starting point of the curve we're going to find the point along
        float firstPointPercent;
        //Float to hold the percent of the end point of the curve we're going to find the point along
        float secondPointPercent;

        //Getting the orientations for each of the control points on the curve
        Quaternion firstOrientation;
        Quaternion secondOrientation;

        //If the percent along the spline is above 100%, we find the starting point of the last curve
        if (t_ >= 1f)
        {
            t_ = 1f;
            //Getting the percent points along the spline that our given percent is between
            firstPointPercent = (1f * (this.points.Length - 4)) / (1f * (this.points.Length - 1));
            secondPointPercent = 1f;
            //Getting the orientations for each of the control points in the curve
            firstOrientation = this.GetPointOrientation(this.points.Length - 4);
            secondOrientation = this.GetPointOrientation(this.points.Length - 1);
        }
        //Otherwise we find the starting point of the curve that's at the percent given
        else
        {
            //Finding which curve is at the percent
            t_ = Mathf.Clamp01(t_) * this.CurveCount;
            int firstPoint = (int)t_;
            //Finding the index of the first point along that curve
            t_ -= firstPoint;
            firstPoint *= 3;

            //Getting the percent points along the spline that our given percent is between
            firstPointPercent = (1f * firstPoint) / (1f * (this.points.Length - 1));
            secondPointPercent = (1f * (firstPoint + 3)) / (1f * (this.points.Length - 1));

            //Getting the orientations for each of the control points in the curve
            firstOrientation = this.GetPointOrientation(firstPoint);
            secondOrientation = this.GetPointOrientation(firstPoint + 3);
        }

        //Getting the percent that the given point is between these two rotations
        float orientationPercent = (t_ - firstPointPercent) / (secondPointPercent - firstPointPercent);

        //Getting the Quaternion orientation between the two points at the orientation percent
        return Quaternion.Lerp(firstOrientation, secondOrientation, orientationPercent);
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
        int middleIndex = modeIndex * 3; //Index of the control point
        int fixedIndex = -1;
        int enforcedIndex = -1;

        //If the index of the handle we're moving is behind the control point
        if(index_ <= middleIndex)
        {
            fixedIndex = middleIndex - 1;
            //If we have to loop back around, we select the handle for the last control point
            if(fixedIndex < 0 && this.loop)
            {
                fixedIndex = this.points.Length - 2;
            }
            enforcedIndex = middleIndex + 1;
            //If we have to loop back around, we select the handle for the first control point
            if(enforcedIndex >= this.points.Length)
            {
                if (this.loop)
                {
                    enforcedIndex = 1;
                }
                else
                {
                    enforcedIndex = -1;
                }
            }
        }
        //If the index of the handle we're moving is in front of the control point
        else
        {
            fixedIndex = middleIndex + 1;
            //If we have to loop back around, we select the handle for the first control point
            if(fixedIndex >= this.points.Length && this.loop)
            {
                fixedIndex = 1;
            }
            enforcedIndex = middleIndex - 1;
            //If we have to loop back around, we select the handle for the last control point
            if(enforcedIndex < 0)
            {
                if (this.loop)
                {
                    enforcedIndex = this.points.Length - 2;
                }
                else
                {
                    enforcedIndex = -1;
                }
            }
        }
        
        //If the fixed index is invalid due to not looping, we do nothing
        if (fixedIndex < 0 || enforcedIndex < 0)
        {
            return;
        }

        //Getting the position of the control point
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
        Vector3 lastPointForward = (this.points[this.points.Length - 1]);
        lastPointForward -= (this.points[this.points.Length - 2]);
        lastPointForward = Vector3.Normalize(lastPointForward);

        //Getting the last point on our spline
        Vector3 point = this.points[this.points.Length - 1];

        //Resizing our points array to add 3 more
        Array.Resize(ref this.points, this.points.Length + 3);

        //Making it so that the newly added points are offset from the previously last point using the forward direction
        this.points[this.points.Length - 3] = point + (lastPointForward * this.addedPointDistance);
        this.points[this.points.Length - 2] = point + (lastPointForward * this.addedPointDistance * 2);
        this.points[this.points.Length - 1] = point + (lastPointForward * this.addedPointDistance * 3);

        //Adding a new mode control type to the newly added point
        Array.Resize(ref this.modes, this.modes.Length + 1);
        this.modes[this.modes.Length - 1] = BezierControlPointMode.Aligned;

        //Adding a new orientation for the added point
        Array.Resize(ref this.controlPointOrientations, this.controlPointOrientations.Length + 1);
        //Setting the orientation for the added point to face the forward direction we were just using
        this.controlPointOrientations[this.controlPointOrientations.Length - 1] = Quaternion.LookRotation(this.GetControlPoint(this.points.Length - 1) - this.GetControlPoint(this.points.Length - 2));

        //Adding a new time to the next control point
        Array.Resize(ref this.timeToNextPoint, this.timeToNextPoint.Length + 1);
        this.timeToNextPoint[this.timeToNextPoint.Length - 2] = this.timeToNextPoint[this.timeToNextPoint.Length - 3];
        this.timeToNextPoint[this.timeToNextPoint.Length - 1] = 0;

        //Making sure the added handles are behaving correctly
        this.EnforceMode(this.points.Length - 4);

        //If we loop this spline, we make sure the new last point matches the first
        if(this.loop)
        {
            this.points[this.points.Length - 1] = this.points[0];
            this.modes[this.modes.Length - 1] = this.modes[0];
            this.controlPointOrientations[this.controlPointOrientations.Length - 1] = this.controlPointOrientations[0];
            this.EnforceMode(0);
        }
    }


    //Function called externally from BezierSplineInspector to add a new control point to our spline after the given existing point
    public void AddControlPointBetweenPoints(int selectedPoint_)
    {
        //Creating a new array of points that we'll use for our points
        Vector3[] newPoints = {};
        Array.Resize(ref newPoints, this.ControlPointCount + 3);

        //Creating a new array of control point modes
        BezierControlPointMode[] newModes = { };
        Array.Resize(ref newModes, this.modes.Length + 1);

        //Creating a new arry of orientation rotations for control points
        Quaternion[] newOrientations = { };
        Array.Resize(ref newOrientations, this.controlPointOrientations.Length + 1);


        //Looping through each point in our current list of curve points until we find the points to add between
        int indexOffset = 0;
        for (int p = 0; p < this.ControlPointCount; ++p)
        {
            //If we run into the index where we need to add the new point, we need to add in the new points
            if(p == selectedPoint_ + 2)
            {
                //Finding the time along the curve that's between the selected control point and the one after it
                float midpointTime = (selectedPoint_ * 1f) / ((this.points.Length - 1) * 1f);
                midpointTime += (3f / ((this.points.Length - 1) * 1f)) / 2;

                //Setting the position of the next control point
                newPoints[p + 1] = this.transform.InverseTransformPoint(this.GetPoint(midpointTime));

                //Getting the difference between the control points on either side of the added point and normalizing the direction
                Vector3 handleDirection = this.points[selectedPoint_ + 3] - this.points[selectedPoint_];
                handleDirection = Vector3.Normalize(handleDirection);

                //Setting the position of the handle behind the new control point
                newPoints[p] = newPoints[p + 1] - (handleDirection * this.addedPointDistance);

                //Setting the position of the handle ahead of the new control point
                newPoints[p + 2] = newPoints[p + 1] + (handleDirection * this.addedPointDistance);

                //Creating the new control point alignment and rotation for the created point
                newModes[(p / 3) + 1] = BezierControlPointMode.Aligned;
                newOrientations[(p / 3) + 1] = new Quaternion();

                //Enforcing the handle mode for the newly created handles
                this.EnforceMode(p);
                this.EnforceMode(p + 2);

                //Adding to the offset for which point index we change
                indexOffset = 3;
            }

            //Adding the current curve point to the new list of points
            newPoints[p + indexOffset] = this.points[p];
            
            //If the current point is a control point, we add the orientation and mode to the new arrays
            if(p % 3 == 0)
            {
                newModes[(p + indexOffset) / 3] = this.modes[p / 3];
                newOrientations[(p + indexOffset) / 3] = this.controlPointOrientations[p / 3];
            }
        }

        //Setting this spline's new control points, orientations, and modes
        this.points = newPoints;
        this.modes = newModes;
        this.controlPointOrientations = newOrientations;
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
            //Looping through our current array of modes and copying all but the first one
            for(int m = 0; m < newModesArray.Length; ++m)
            {
                BezierControlPointMode newMode = this.modes[m + 1];
                newModesArray[m] = newMode;
            }
            //Setting our modes array to the new, shorter array
            this.modes = newModesArray;

            //Creating a new array of up direction points that will not have the removed point
            Quaternion[] newOrientationArray = { };
            Array.Resize(ref newOrientationArray, this.controlPointOrientations.Length - 1);
            //Looping through our current array of up direction points and copying all but the first one
            for (int u = 0; u < newOrientationArray.Length; ++u)
            {
                Quaternion newUpDirection = this.controlPointOrientations[u + 1];
                newOrientationArray[u] = newUpDirection;
            }
            //Setting our up direction points array to the new, shorter array
            this.controlPointOrientations = newOrientationArray;

            //Creating a new array of times to the next control point that will not have the removed point
            float[] newTimeToNextPoint = { };
            Array.Resize(ref newTimeToNextPoint, this.timeToNextPoint.Length - 1);
            //Looping through our current array of time to next points and copying all but the first one
            for(int t = 0; t < newTimeToNextPoint.Length; ++t)
            {
                float newTime = this.timeToNextPoint[t + 1];
                newTimeToNextPoint[t] = newTime;
            }
        }
        //If we're removing the last control point in the spline
        else if(index_ == this.points.Length - 1)
        {
            //Removing the last three points in our array (the control point, its handle, and the handle before it)
            Array.Resize(ref this.points, this.points.Length - 3);

            //Removing the last mode for the bezier control mode
            Array.Resize(ref this.modes, this.modes.Length - 1);

            //Removing the last up rotation point
            Array.Resize(ref this.controlPointOrientations, this.controlPointOrientations.Length - 1);

            //Removing the last time to next point
            Array.Resize(ref this.timeToNextPoint, this.timeToNextPoint.Length - 1);
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
            Quaternion[] newOrientationArray = { };
            Array.Resize(ref newOrientationArray, this.controlPointOrientations.Length - 1);
            //Looping through our current array of up direction points and copying all but the removed one
            int upRotOffset = 0;
            for (int u = 0; u + upRotOffset < this.controlPointOrientations.Length; ++u)
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
                    newOrientationArray[u] = this.controlPointOrientations[u + upRotOffset];
                }
            }
            //Setting our up direction points array to the new, shorter array
            this.controlPointOrientations = newOrientationArray;

            //Creating a new array of times to the next control points that will not have the removed time
            float[] newTimeToNextPoint = { };
            Array.Resize(ref newTimeToNextPoint, this.timeToNextPoint.Length - 1);
            //Looping through our current array of times to next points and copying all but the removed one
            int timeOffset = 0;
            for(int t = 0; t + timeOffset < this.timeToNextPoint.Length; ++t)
            {
                //If the current time point is the one we're removing, we ignore it and increase the offset
                if(t + timeOffset == (index_ + 1) / 3)
                {
                    timeOffset += 1;
                    t -= 1;
                }
                //If the current time point isn't the one we're removing, we add it to the new array
                else
                {
                    newTimeToNextPoint[t] = this.timeToNextPoint[t + timeOffset];
                }
            }
            //Setting our time to next points array to the new, shorter array
            this.timeToNextPoint = newTimeToNextPoint;
        }
    }
}

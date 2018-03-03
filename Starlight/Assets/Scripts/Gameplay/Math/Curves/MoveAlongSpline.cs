using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAlongSpline : MonoBehaviour
{
    //The reference to the spline we move along
    public BezierSpline splineToFollow;

    //The amount of time it takes for us to get from one point on the spline to another
    public float timeToComplete = 5f;

    //The current amount of time that this object has progressed along the spline
    private float currentTime = 0;

    //Multiplier for moving along this spline faster or slower
    public float speedMultiplier = 1f;

    //Bool that determines if this object rotates to face the direction of the spline
    public bool rotateToFollowSpline = true;

    //Enum to determine what happens when this object reaches the end of the spline
    public enum SplineEndBehavior
    {
        Stop,
        Loop,
        PingPong
    }
    public SplineEndBehavior endBehavior = SplineEndBehavior.Stop;

    //Bool for if this object is progressing forward or backwards along this spline
    private bool isMovingForward = true;

        

	// Update is called once per frame
	private void Update ()
    {
        //If this object is moving forward along the spline
        if (this.isMovingForward)
        {
            //Increasing our current time based on our speed multiplier
            this.currentTime += Time.deltaTime * this.speedMultiplier;

            //If we reach the time to complete, we make sure we don't go over
            if (this.currentTime > this.timeToComplete)
            {
                //If our end behavior is "Stop", we stop moving
                if (this.endBehavior == SplineEndBehavior.Stop)
                {
                    this.currentTime = this.timeToComplete;
                }
                //If our end behavior is "Loop", we cycle back to the beginning
                else if(this.endBehavior == SplineEndBehavior.Loop)
                {
                    this.currentTime -= this.timeToComplete;
                }
                //If our end behavior is "PingPong", we reverse direction
                else
                {
                    this.currentTime = (2 * this.timeToComplete) - this.currentTime;
                    this.isMovingForward = false;
                }
            }
        }
        //If this object is moving backward along the spline
        else
        {
            //Decreasing our current time
            this.currentTime -= Time.deltaTime;

            //If we reach time 0, we reverse direction
            if(this.currentTime < 0)
            {
                this.currentTime = -this.currentTime;
                this.isMovingForward = true;
            }
        }

        //Setting our transform to the correct percent along the spline based on the time completed
        //this.transform.position += (this.splineToFollow.GetPoint(this.currentTime / this.timeToComplete) - this.transform.position) * this.interpSpeed;
        this.transform.position = this.splineToFollow.GetPoint(this.currentTime / this.timeToComplete);

        //If we rotate to face the direction of the spline path
        if(this.rotateToFollowSpline)
        {
            this.transform.LookAt(this.transform.position + this.splineToFollow.GetDirection(this.currentTime / this.timeToComplete));
        }
	}


    //Function called externally to change the spline that this object follows
    public void ChangeSplineToFollow(BezierSpline newSpline_)
    {
        this.splineToFollow = newSpline_;
        this.timeToComplete = newSpline_.TotalSplineTime;
        this.currentTime = 0;
    }
}

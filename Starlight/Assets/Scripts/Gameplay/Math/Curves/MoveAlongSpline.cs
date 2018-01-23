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

    //Bool that determines if this object rotates to face the direction of the spline
    public bool rotateToFollowSpline = true;

        

	// Update is called once per frame
	private void Update ()
    {
        //Increasing our current time
        this.currentTime += Time.deltaTime;

        //If we reach the time to complete, we make sure we don't go over
        if(this.currentTime > this.timeToComplete)
        {
            this.currentTime = this.timeToComplete;
        }

        //Setting our transform to the correct percent along the spline based on the time completed
        this.transform.position = this.splineToFollow.GetPoint(this.currentTime / this.timeToComplete);

        //If we rotate to face the direction of the spline path
        if(this.rotateToFollowSpline)
        {
            this.transform.LookAt(this.transform.position + this.splineToFollow.GetDirection(this.currentTime / this.timeToComplete));
        }
	}
}

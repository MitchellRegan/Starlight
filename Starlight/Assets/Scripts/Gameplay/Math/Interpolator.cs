using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interpolator
{
    //The way that this Interpolator reaches 1.0
    public EaseType ease = EaseType.Linear;
    //The length of time it takes this Interpolator to reach 1.0
    private float duration = 1.0f;
    //The current amount of time this interpolator has spent interpolating
    private float currentTime = 0;
    /*Number of times a full Sine wave is completed by the time this Interpolator reaches 1.0
    Only used with Ease types that say Jitter */
    private int numberOfJitters = 1;



    //Constructor function for this Interpolator class
    public Interpolator(EaseType ease_ = EaseType.Linear, float duration_ = 1.0f)
    {
        this.ease = ease_;
        this.SetDuration(duration_);
    }


    //Function called externally. Sets the amount of time that the interpolation happens between
    public void SetDuration(float duration_)
    {
        this.duration = duration_;

        //The time can't be negative
        if (this.duration < 0)
        {
            this.duration = 0;
        }
    }


    //Function called externally. Sets the number of times a full Sine wave is completed over the interpolation
    public void SetNumberOfJitters(int numJitters_)
    {
        //Sets the NumberOfJitters as long as it's not negative
        if (numJitters_ >= 0)
        {
            this.numberOfJitters = numJitters_;
        }
    }


    //Function called externally. Adds (or reduces) time to the interpolator's current time
    public void AddTime(float timeDiff_)
    {
        this.currentTime = this.currentTime + timeDiff_;

        //Makes sure the time can't go above the current duration
        if (this.currentTime > this.duration)
        {
            this.currentTime = this.duration;
        }
        //Makes sure the time can't go below 0 (because time doesn't work that way)
        else if (this.currentTime < 0)
        {
            this.currentTime = 0;
        }
    }


    //Function called externally. Resets the current time to 0
    public void ResetTime()
    {
        this.currentTime = 0;
    }


    //Function called externally. Returns the progress in percent (0 - 1) without ease 
    public float GetPercent()
    {
        return (this.currentTime / this.duration);
    }


    //Function called externally. Returns the progress of this interpolator
    public float GetProgress()
    {
        //If the current timer is at the duration, it returns the max value
        if (this.currentTime == this.duration)
        {
            if (this.ease != EaseType.JitterBetweenStay && this.ease != EaseType.JitterOutsideStay)
            {
                return 1.0f;
            }
        }
        //If the current timer is at 0, it returns the lowest value
        else if (this.currentTime == 0)
        {
            return 0;
        }

        float progress = this.currentTime / this.duration;
        float difference = 0;
        float offset = 0;

        //Otherwise, the progress needs to be calculated based on the ease type
        switch (this.ease)
        {
            //Moves at a constant speed
            case EaseType.Linear:
                return progress;

            //Moves slow at first then speeds up
            case EaseType.SineIn:
                difference = Mathf.PI / 2;
                offset = Mathf.PI * 1.5f;
                return Mathf.Sin((difference * progress) + offset) + 1;

            //Moves fast at first then slows to a stop
            case EaseType.SineOut:
                difference = Mathf.PI / 2;
                return Mathf.Sin(difference * progress);

            //Moves slow at first, speeds up, then slows down to a stop
            case EaseType.SineInOut:
                difference = Mathf.PI;
                offset = Mathf.PI * 1.5f;
                return (Mathf.Sin((difference * progress) + offset) * 0.5f) + 0.5f;

            //Moves slow at first then speeds up. Faster than SineIn
            case EaseType.CubeIn:
                return Mathf.Pow(progress, 3);

            //Moves fast at first then slows to a stop. Faster than SineOut
            case EaseType.CubeOut:
                return 1 + Mathf.Pow((progress - 1), 3);

            //Moves slow at first, speeds up, then slows down to a stop. Faster than SineInOut
            case EaseType.CubeInOut:
                if (progress <= 0.5)
                {
                    return Mathf.Pow((2 * progress), 3) * 0.5f;
                }
                else
                {
                    return ((1 + Mathf.Pow(((progress - 1) * 2), 3)) * 0.5f) + 0.5f;
                }

            //Goes back and forth from 0 - 1 and ends at 1
            case EaseType.JitterBetweenIn:
                float jitterBI = (this.numberOfJitters * (2 * Mathf.PI)) + (Mathf.PI);
                return (Mathf.Sin((1.5f * Mathf.PI) + jitterBI * progress) * 0.5f) + 0.5f;

            //Goes back and forth from 0 - 1 and ends at 0
            case EaseType.JitterBetweenStay:
                float jitterBS = (this.numberOfJitters * (2 * Mathf.PI));
                return (Mathf.Sin((1.5f * Mathf.PI) + jitterBS * progress) * 0.5f) + 0.5f;

            //Goes back and forth from -1 - 1 and ends at 1
            case EaseType.JitterOutsideIn:
                float jitterOI = (this.numberOfJitters * (2 * Mathf.PI)) + (0.5f * Mathf.PI);
                return Mathf.Sin(jitterOI * progress);

            //Goes back and forth from -1 - 1 and ends at 0
            case EaseType.JitterOutsideStay:
                float jitterOS = (this.numberOfJitters * (2 * Mathf.PI));
                return Mathf.Sin(jitterOS * progress);

            //Returns linear by default
            default:
                return (this.currentTime / this.duration);
        }
    }
}


//Enums used in Interpolator.cs to determine the type of ease 
public enum EaseType
{
    Linear,
    SineIn,
    SineOut,
    SineInOut,
    CubeIn,
    CubeOut,
    CubeInOut,
    JitterBetweenStay,
    JitterOutsideStay,
    JitterBetweenIn,
    JitterOutsideIn
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraWeight : MonoBehaviour
{
    //Determines if the distance between objects tracks the Z plane
    public bool track2D = true;

    //If this is on, player1's camera will always follow it regardless of the Add or Drop distance
    public bool alwaysRemainOnCam = false;

    //Amount of priority for the camera to follow this object
    public int weight = 1;
    //Distance from the camera object that this object is added to the weight system
    public float addDistance = 10f;
    //Distance away from the camera that this object is dropped from the weight system.
    //This should ALWAYS be greater than the AddDistance
    public float dropDistance = 11f;

    //References to the FollowCameraWeights.cs camera and if this object is currently on the camera's screen
    private GameObject cameraRef;
    private bool isOnScreen = false;



    // Use this for initialization
    private void Start()
    {
        //Finds the static references of all cameras
        this.cameraRef = FollowCameraWeights.globalReference.gameObject;

        //If this object always needs to be tracked by a player, adds their weight to the designated camera
        if (this.alwaysRemainOnCam && this.cameraRef != null)
        {
            FollowCameraWeights.AddWeightedObject(this);
        }

        //Makes sure the Drop Distance is greater than the Add Distance to prevent errors with the weight system
        if (this.dropDistance <= this.addDistance)
        {
            this.dropDistance = this.addDistance + 1;
        }
    }

    
    // Update is called once per frame
    private void FixedUpdate()
    {
        //Determines if this object should be added to each of the cameras. We don't care if they're null, because HandleCamera handles that for us
        this.isOnScreen = HandleCamera(this.cameraRef, this.isOnScreen);
    }


    //Adds and drops this object from the designated camera when it gets in or out of range and returns a bool based on if this object is being tracked by the camera
    private bool HandleCamera(GameObject cameraObj_, bool onCamera_)
    {
        //If the designated camera doesn't exist, nothing happens
        if (cameraObj_ == null)
        {
            return false;
        }

        bool isOnScreen = onCamera_;

        //If this object is already being tracked by the designated camera, finds out if it should be dropped
        if (onCamera_)
        {
            //Tracks the distance between only X and Y coords
            if (this.track2D)
            {
                Vector2 thisObj = new Vector2(transform.position.x, transform.position.y);
                Vector2 camObj = new Vector2(cameraObj_.transform.position.x, cameraObj_.transform.position.y);
                float dist = Vector2.Distance(thisObj, camObj);

                //If the distance is greater than the drop distance, it won't be tracked anymore
                if (dist >= this.dropDistance)
                {
                    FollowCameraWeights.DropWeightedObject(this);
                    isOnScreen = false;
                }
            }
            //Tracks the distance between X, Y, and Z coords
            else
            {
                float dist = Vector3.Distance(transform.position, cameraObj_.transform.position);

                //If the distance is greater than the drop distance it won't be tracked anymore
                if (dist >= this.dropDistance)
                {
                    FollowCameraWeights.DropWeightedObject(this);
                    isOnScreen = false;
                }
            }
        }
        //If this object isn't being tracked by the designated camera, finds out if it should be added
        else
        {
            //Tracks the distance between only X and Y coords
            if (this.track2D)
            {
                Vector2 thisObj = new Vector2(transform.position.x, transform.position.y);
                Vector2 camObj = new Vector2(cameraObj_.transform.position.x, cameraObj_.transform.position.y);
                float dist = Vector2.Distance(thisObj, camObj);

                //If the distance is less than the add distance, it will be tracked
                if (dist <= this.addDistance)
                {
                    FollowCameraWeights.AddWeightedObject(this);
                    isOnScreen = true;
                }
            }
            //Tracks the distance between X, Y, and Z coords
            else
            {
                float dist = Vector3.Distance(transform.position, cameraObj_.transform.position);

                //If the distance is less than the add distance it will be tracked 
                if (dist <= this.addDistance)
                {
                    FollowCameraWeights.AddWeightedObject(this);
                    isOnScreen = true;
                }
            }
        }

        return isOnScreen;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FollowCameraWeights : MonoBehaviour
{
    //Static ref to the cameras for each player
    public static FollowCameraWeights p1GlobalReference;
    public static FollowCameraWeights p2GlobalReference;

    //The player that this camera follows weights for
    public Players playerToFollow = Players.P1;

    //The game object that's moved to follow
    public GameObject rootPosObj;

    //The game object that our root pos object rotates to match
    private GameObject rotationFollowObj;
    //The amount that we rotate each frame to match the rotation follow obj
    public float rotationSpeed = 0.05f;

    //An array to hold all of the objects that have the Camera_Weight component on them
    private List<GameObject> weightObjects;
    //The sum of all weights of objects in the WeightObjects array
    private int weightSum = 0;
    //The rate that this camera interps to the weight position
    [Range(0, 1.0f)]
    public float interpSpeed = 0.95f;

    //Reference to this object's camera component
    private Camera weightCamera;
    //The furthest out the camera can zoom based on object distance
    public float maxZoom = 60;
    //The furthest in the camera can zoom based on object distance
    public float minZoom = 30;
    //The furthest distance that affects zoom
    public float maxZoomDist = 6;
    //The closest distance that affects zoom
    public float minZoomDist = 0;



    // Use this for initialization
    private void Awake()
    {
        //If this camera follows player 2
        if (this.playerToFollow == Players.P2)
        {
            //Sets the static reference to this camera if one doesn't already exist
            if (p2GlobalReference == null)
            {
                p2GlobalReference = this;
                //Setting our rotation object to follow as the p2 ship
                if (PlayerShipController.p1ShipRef != null)
                {
                    this.rotationFollowObj = PlayerShipController.p2ShipRef.gameObject;
                }
                //If the p2 ship is null, we disable this obj
                else
                {
                    this.enabled = false;
                }
            }
            //If there's already a static reference
            else
            {
                //If the static reference is disabled, this camera becomes the static reference
                if (!p2GlobalReference.gameObject.activeInHierarchy)
                {
                    p2GlobalReference = this;
                    //Setting our rotation object to follow as the p2 ship
                    if (PlayerShipController.p1ShipRef != null)
                    {
                        this.rotationFollowObj = PlayerShipController.p2ShipRef.gameObject;
                    }
                    //If the p2 ship is null, we disable this obj
                    else
                    {
                        this.enabled = false;
                    }
                }
                //If the static reference is still awake, this camera becomes disabled
                else
                {
                    this.enabled = false;
                }
            }
        }
        //If this camera follows anything but player 2, it's set to player 1
        else
        {
            //Sets the static reference to this camera if one doesn't already exist
            if(p1GlobalReference == null)
            {
                p1GlobalReference = this;
                //Setting our rotation object to follow as the p1 ship
                if (PlayerShipController.p1ShipRef != null)
                {
                    this.rotationFollowObj = PlayerShipController.p1ShipRef.gameObject;
                }
                //If the p1 ship is null, we disable this obj
                else
                {
                    this.enabled = false;
                }
            }
            //If there's already a static reference
            else
            {
                //If the static reference is disabled, this camera becomes the static reference
                if(!p1GlobalReference.gameObject.activeInHierarchy)
                {
                    p1GlobalReference = this;
                    //Setting our rotation object to follow as the p1 ship
                    if(PlayerShipController.p1ShipRef != null)
                    {
                        this.rotationFollowObj = PlayerShipController.p1ShipRef.gameObject;
                    }
                    //If the p1 ship is null, we disable this obj
                    else
                    {
                        this.enabled = false;
                    }
                }
                //If the static reference is still awake, this camera becomes disabled
                else
                {
                    this.enabled = false;
                }
            }
        }

        //Initializes the list to hold weighted objects
        this.weightObjects = new List<GameObject>();
        //Makes sure the sum of weights starts at 0
        this.weightSum = 0;

        //Stores the reference to this object's camera component
        this.weightCamera = GetComponent<Camera>();
    }


    //Function called by objects with the Camera_Weight component when they come into range
    public static void AddWeightedObject(CameraWeight objToAdd_, Players playerToFollow_)
    {
        //If the player to follow is p1
        if (playerToFollow_ == Players.P1)
        {
            //Adds the weighted object to the list of game objects
            p1GlobalReference.weightObjects.Add(objToAdd_.gameObject);
            //Adds the weighted object's weight to the sum of all of them
            p1GlobalReference.weightSum += objToAdd_.weight;
        }
        //If the player to follow is p2
        else if(playerToFollow_ == Players.P2)
        {
            //Adds the weighted object to the list of game objects
            p2GlobalReference.weightObjects.Add(objToAdd_.gameObject);
            //Adds the weighted object's weight to the sum of all of them
            p2GlobalReference.weightSum += objToAdd_.weight;
        }
    }


    //Function called by objects with Camera_Weight component when they drop out of range
    public static void DropWeightedObject(CameraWeight objToDrop_, Players playerToDrop_)
    {
        //If the player to drop is p1
        if (playerToDrop_ == Players.P1)
        {
            //makes sure the object being dropped is in the list of game objects in the first place
            if (p1GlobalReference.weightObjects.Contains(objToDrop_.gameObject))
            {
                //Removes the weighted object from the list of game objects
                p1GlobalReference.weightObjects.Remove(objToDrop_.gameObject);
                //Subtracts the weighted object's weight from the sum of all of them
                p1GlobalReference.weightSum -= objToDrop_.weight;
            }
        }
        //If the player to drop is p2
        else if(playerToDrop_ == Players.P2)
        {
            //makes sure the object being dropped is in the list of game objects in the first place
            if (p2GlobalReference.weightObjects.Contains(objToDrop_.gameObject))
            {
                //Removes the weighted object from the list of game objects
                p2GlobalReference.weightObjects.Remove(objToDrop_.gameObject);
                //Subtracts the weighted object's weight from the sum of all of them
                p2GlobalReference.weightSum -= objToDrop_.weight;
            }
        }
    }

    
    // Update is called once per frame
    private void FixedUpdate()
    {
        //Doesn't update unless there's an object to follow
        if (this.weightSum == 0)
        {
            return;
        }

        //Gets the position that this camera needs to go to
        Vector3 interpPos = FindWeightPosition();

        //Finds the difference position
        Vector3 diff = new Vector3(interpPos.x - this.transform.position.x,
                            interpPos.y - this.transform.position.y,
                            interpPos.z - this.transform.position.z);

        //Sets this camera's position so that it moves in the direction of the interpPos
        this.rootPosObj.transform.position = new Vector3(this.transform.position.x + (diff.x * this.interpSpeed),
                                                                        this.transform.position.y + (diff.y * this.interpSpeed),
                                                                        this.transform.position.z + (diff.z * this.interpSpeed));

        //Sets the camera's field of view based on the furthest tracked object's distance
        this.weightCamera.orthographicSize += (this.FindZoom() - this.weightCamera.orthographicSize) * this.interpSpeed;

        //Sets the camera's rotation to match our rotation object
        this.rootPosObj.transform.rotation = Quaternion.Slerp(this.rootPosObj.transform.rotation, this.rotationFollowObj.transform.rotation, Time.time * this.rotationSpeed);
    }

    
    //Determines the location that this camera goes to each frame
    private Vector3 FindWeightPosition()
    {
        //The position that this function returns
        Vector3 returnPos = new Vector3(0, 0, 0);

        //Loops through each object in the list and adds its position to the returnPos a number of times equal to its weight
        for (int o = 0; o < this.weightObjects.Count; ++o)
        {
            returnPos += (this.weightObjects[o].transform.position * this.weightObjects[o].GetComponent<CameraWeight>().weight);
        }

        //Divides the position by the sum of weights
        returnPos = returnPos / this.weightSum;

        return returnPos;
    }


    //Sets the Field of View based on the furthest object's distance each frame
    private float FindZoom()
    {
        float zoom = 0;
        float furthestDist = 0;
        float currentDist = 0;

        //Loops through each tracked object to find the one furthest from this camera
        for (int z = 0; z < this.weightObjects.Count; ++z)
        {
            //Stores the distance between the camera and the current object in the list
            currentDist = Vector2.Distance(new Vector2(this.transform.position.x, this.transform.position.y),
                                            new Vector2(this.weightObjects[z].transform.position.x, this.weightObjects[z].transform.position.y));

            //If this current distance is the furthest away so far, we store it
            if (currentDist > furthestDist)
            {
                furthestDist = currentDist;
            }
        }

        //If the furthest object is at or beyond the max zoom distance, the zoom is set to the highest allowed
        if (furthestDist >= this.maxZoomDist)
        {
            zoom = this.maxZoom;
        }
        //If the furthest object is at or closer than the min zoom distance, the zoom is set to the lowest allowed
        else if (furthestDist <= this.minZoomDist)
        {
            zoom = this.minZoom;
        }
        //If the furthest object is between the min and max zoom distance, we find the middleground based on the difference
        else
        {
            float zoomDiff = this.maxZoom - this.minZoom;
            float distDiff = this.maxZoomDist - this.minZoomDist;
            float distPercent = (furthestDist / this.minZoomDist) / distDiff;

            zoom = (distPercent * zoomDiff) + this.minZoom;
        }


        return zoom;
    }
}

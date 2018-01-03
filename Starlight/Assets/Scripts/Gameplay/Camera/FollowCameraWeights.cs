using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCameraWeights : MonoBehaviour
{
    //Static ref to the main camera (this camera)
    public static FollowCameraWeights globalReference;

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
        //Sets the static reference to this camera if one doesn't already exist
        if (globalReference == null)
        {
            globalReference = this;
        }
        //If there's already a static reference
        else
        {
            //If the static reference is disabled, this camera becomes the static reference
            if(!globalReference.gameObject.activeInHierarchy)
            {
                globalReference = this;
            }
            //If the static reference is still awake, this camera becomes disabled
            else
            {
                this.enabled = false;
            }
        }

        //Initializes the list to hold weighted objects
        globalReference.weightObjects = new List<GameObject>();
        //Makes sure the sum of weights starts at 0
        globalReference.weightSum = 0;

        //Stores the reference to this object's camera component
        globalReference.weightCamera = GetComponent<Camera>();
    }


    //Function called by objects with the Camera_Weight component when they come into range
    public static void AddWeightedObject(CameraWeight objToAdd_)
    {
        //Adds the weighted object to the list of game objects
        globalReference.weightObjects.Add(objToAdd_.gameObject);
        //Adds the weighted object's weight to the sum of all of them
        globalReference.weightSum += objToAdd_.weight;
    }


    //Function called by objects with Camera_Weight component when they drop out of range
    public static void DropWeightedObject(CameraWeight objToDrop_)
    {
        //makes sure the object being dropped is in the list of game objects in the first place
        if (globalReference.weightObjects.Contains(objToDrop_.gameObject))
        {
            //Removes the weighted object from the list of game objects
            globalReference.weightObjects.Remove(objToDrop_.gameObject);
            //Subtracts the weighted object's weight from the sum of all of them
            globalReference.weightSum -= objToDrop_.weight;
        }
    }

    
    // Update is called once per frame
    private void FixedUpdate()
    {
        //Doesn't update unless there's an object to follow
        if (globalReference.weightSum == 0)
        {
            return;
        }

        //Gets the position that this camera needs to go to
        Vector3 interpPos = FindWeightPosition();

        //Finds the difference position
        Vector2 diff = new Vector3(interpPos.x - globalReference.transform.position.x,
                            interpPos.y - globalReference.transform.position.y);

        //Sets this camera's position so that it moves in the direction of the interpPos
        //thisCam.transform.position = thisCam.transform.position + (diff * thisCam.interpSpeed);

        globalReference.transform.localPosition = new Vector3(globalReference.transform.localPosition.x + (diff.x * globalReference.interpSpeed),
                                                                globalReference.transform.localPosition.y + (diff.y * globalReference.interpSpeed),
                                                                globalReference.transform.localPosition.z);

        //Sets the camera's field of view based on the furthest tracked object's distance
        globalReference.weightCamera.orthographicSize += (this.FindZoom() - globalReference.weightCamera.orthographicSize) * globalReference.interpSpeed;
    }

    
    //Determines the location that this camera goes to each frame
    private Vector3 FindWeightPosition()
    {
        //The position that this function returns
        Vector3 returnPos = new Vector3(0, 0, 0);

        //Loops through each object in the list and adds its position to the returnPos a number of times equal to its weight
        for (int o = 0; o < globalReference.weightObjects.Count; ++o)
        {
            returnPos += (globalReference.weightObjects[o].transform.position * globalReference.weightObjects[o].GetComponent<CameraWeight>().weight);
        }

        //Divides the position by the sum of weights
        returnPos = returnPos / globalReference.weightSum;

        return returnPos;
    }


    //Sets the Field of View based on the furthest object's distance each frame
    private float FindZoom()
    {
        float zoom = 0;
        float furthestDist = 0;
        float currentDist = 0;

        //Loops through each tracked object to find the one furthest from this camera
        for (int z = 0; z < globalReference.weightObjects.Count; ++z)
        {
            //Stores the distance between the camera and the current object in the list
            currentDist = Vector2.Distance(new Vector2(globalReference.transform.position.x, globalReference.transform.position.y),
                                            new Vector2(globalReference.weightObjects[z].transform.position.x, globalReference.weightObjects[z].transform.position.y));

            //If this current distance is the furthest away so far, we store it
            if (currentDist > furthestDist)
            {
                furthestDist = currentDist;
            }
        }

        //If the furthest object is at or beyond the max zoom distance, the zoom is set to the highest allowed
        if (furthestDist >= globalReference.maxZoomDist)
        {
            zoom = globalReference.maxZoom;
        }
        //If the furthest object is at or closer than the min zoom distance, the zoom is set to the lowest allowed
        else if (furthestDist <= globalReference.minZoomDist)
        {
            zoom = globalReference.minZoom;
        }
        //If the furthest object is between the min and max zoom distance, we find the middleground based on the difference
        else
        {
            float zoomDiff = globalReference.maxZoom - globalReference.minZoom;
            float distDiff = globalReference.maxZoomDist - globalReference.minZoomDist;
            float distPercent = (furthestDist / globalReference.minZoomDist) / distDiff;

            zoom = (distPercent * zoomDiff) + globalReference.minZoom;
        }


        return zoom;
    }
}

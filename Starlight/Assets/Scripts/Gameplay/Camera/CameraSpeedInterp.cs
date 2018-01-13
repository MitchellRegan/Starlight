using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraSpeedInterp : MonoBehaviour
{
    //Enum for which player this camera follows
    public Players playerToTrack = Players.P1;
    //Reference to the player ship we're tracking
    private PlayerShipController ourShip;
    //Reference to our object's camera component
    private Camera ourCamera;

    [Space(8)]

    //The default Z distance this camera will be when we aren't boosting or breaking
    public float defaultZDist = 5;
    //The local Z distance this camera will be when boosting
    public float boostZDist = 8;
    //The local Z distance this camera will be when breaking
    public float breakZDist = 3.5f;
    //The interp multiplier for changing between Z distances
    [Range(0.01f, 0.99f)]
    public float interpZDistSpeed = 0.9f;

    [Space(8)]

    //The default field of view for the camera when we aren't boosting or breaking
    public float defaultFOV = 60;
    //The field of view for the camera when we're boosting
    public float boostFOV = 80;
    //The field of view for the camera when we're breaking
    public float breakFOV = 40;
    //The interp multiplier for changing between FOV
    [Range(0.01f, 0.99f)]
    public float interpFOVSpeed = 0.8f;


	// Use this for initialization
	private void Start ()
    {
		//Finding our player ship reference
        switch(this.playerToTrack)
        {
            case Players.P1:
                this.ourShip = PlayerShipController.p1ShipRef;
                break;

            case Players.P2:
                this.ourShip = PlayerShipController.p2ShipRef;
                break;

            default:
                this.ourShip = PlayerShipController.p1ShipRef;
                break;
        }

        //If our ship reference is null, we disable this component
        if(this.ourShip == null)
        {
            this.enabled = false;
        }

        //Getting our camera component reference
        this.ourCamera = this.GetComponent<Camera>();
	}
	

	// Update is called once per frame
	private void Update ()
    {
        //The Z distance and FOV variables that we need to interpolate to
        float targetZDist = 0;
        float targetFOV = 0;

        //If the player is boosting
        if (this.ourShip.isShipBoosting)
        {
            targetZDist = this.boostZDist;
            targetFOV = this.boostFOV;
        }
        //If the player is breaking
        else if (this.ourShip.isShipBreaking)
        {
            targetZDist = this.breakZDist;
            targetFOV = this.breakFOV;
        }
        //If the player is neither boosting or breaking
        else
        {
            targetZDist = this.defaultZDist;
            targetFOV = this.defaultFOV;
        }

        //Setting our Z position to interp to the target Z distance
        this.transform.localPosition = new Vector3(0, 0, this.transform.localPosition.z + ((targetZDist - this.transform.localPosition.z) * this.interpZDistSpeed));
        //Setting our FOV to interp to the target FOV
        this.ourCamera.fieldOfView = this.ourCamera.fieldOfView + ((targetFOV - this.ourCamera.fieldOfView) * this.interpFOVSpeed);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITargetingReticle : MonoBehaviour
{
    //The player ID for which player to track
    public Players ourPlayer = Players.P1;

    //The images on the UI that this script moves around
    public Image closeTargetImage;
    public Image farTargetImage;

    //The references to the transforms that we track to find the target positions
    private Transform closeObj;
    private Transform farObj;

    //The reference to the camera for our canvas
    private Camera ourCam;



	// Use this for initialization
	private void Start ()
    {
		//Getting our close and far objects to track
        switch(this.ourPlayer)
        {
            case Players.P1:
                this.closeObj = TargetPoint.p1Close;
                this.farObj = TargetPoint.p1Far;
                break;

            case Players.P2:
                this.closeObj = TargetPoint.p2Close;
                this.farObj = TargetPoint.p2Far;
                break;

            default:
                this.closeObj = TargetPoint.p1Close;
                this.farObj = TargetPoint.p1Far;
                break;
        }
	}
	

	// Update is called once per frame
	private void Update ()
    {
        //Moving our target images to the screen positions where our objects would be
        this.closeTargetImage.transform.position = Camera.main.WorldToScreenPoint(this.closeObj.position);
        this.farTargetImage.transform.position = Camera.main.WorldToScreenPoint(this.farObj.position);
	}
}

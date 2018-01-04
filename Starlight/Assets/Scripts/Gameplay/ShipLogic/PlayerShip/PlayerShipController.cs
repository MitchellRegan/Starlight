using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FreeMovementFlight))]
[RequireComponent(typeof(RailMovementFlight))]
public class PlayerShipController : MonoBehaviour
{
    //Enum to determine which player controls this ship
    public Players playerController = Players.P1;
    //The controller input that we use for this ship
    [HideInInspector]
    public ControllerInput ourController;

    //The input settings for this player
    [HideInInspector]
    public PlayerInputs ourCustomInputs;

    //The camera that follows this ship
    public Camera ourCamera;

    [Space(8)]

    //References to this ship's different movement mechanic scripts
    private FreeMovementFlight ourFreeMovement;
    private RailMovementFlight ourRailMovement;

    //The main weapon for this ship
    public Weapon mainWeapon;
    //The secondary weapon for this ship
    public Weapon secondaryWeapon;

    [Space(8)]

    //The list of all wing objects that are attached to this ship
    public List<ShipWingLogic> shipWings;

    [Space(8)]

    //The list of all engine objects that are attached to this ship
    public List<ShipEngineLogic> shipEngines;




    //Function called when this object is created
    private void Awake()
    {
        //Getting the movement component references
        this.ourFreeMovement = this.GetComponent<FreeMovementFlight>();
        this.ourRailMovement = this.GetComponent<RailMovementFlight>();

        //Getting our controller input based on which player this is
        switch(this.playerController)
        {
            case Players.P1:
                this.ourController = ControllerInputManager.P1Controller;
                this.ourCustomInputs = CustomInputSettings.globalReference.p1Inputs;
                break;

            case Players.P2:
                this.ourController = ControllerInputManager.P2Controller;
                this.ourCustomInputs = CustomInputSettings.globalReference.p2Inputs;
                break;

            default:
                this.ourController = ControllerInputManager.P1Controller;
                this.ourCustomInputs = CustomInputSettings.globalReference.p1Inputs;
                break;
        }

        //Passing our controller input to our movement mechanic scripts
        this.ourFreeMovement.ourShip = this;
        this.ourRailMovement.ourShip = this;
    }


	// Update is called once per frame
	private void Update ()
    {
		//If we have a main weapon, we pass it the controller input and keyboard input for the main fire
        if(this.mainWeapon != null)
        {
            this.mainWeapon.FireWeapon(this.ourController.CheckButtonPressed(this.ourCustomInputs.mainFireButton_Controller),
                                        this.ourController.CheckButtonDown(this.ourCustomInputs.mainFireButton_Controller),
                                        this.ourController.CheckButtonReleased(this.ourCustomInputs.mainFireButton_Controller));

            this.mainWeapon.FireWeapon(Input.GetKeyDown(this.ourCustomInputs.mainFireButton_Keyboard),
                                        Input.GetKey(this.ourCustomInputs.mainFireButton_Keyboard),
                                        Input.GetKeyUp(this.ourCustomInputs.mainFireButton_Keyboard));
        }

        //If we have a secondary weapon, we pass it the controller input for the secondary fire
        if(this.secondaryWeapon != null)
        {
            this.secondaryWeapon.FireWeapon(this.ourController.CheckButtonPressed(this.ourCustomInputs.secondaryFireButton_Controller),
                                        this.ourController.CheckButtonDown(this.ourCustomInputs.secondaryFireButton_Controller),
                                        this.ourController.CheckButtonReleased(this.ourCustomInputs.secondaryFireButton_Controller));

            this.secondaryWeapon.FireWeapon(Input.GetKeyDown(this.ourCustomInputs.secondaryFireButton_Keyboard),
                                        Input.GetKey(this.ourCustomInputs.secondaryFireButton_Keyboard),
                                        Input.GetKeyUp(this.ourCustomInputs.secondaryFireButton_Keyboard));
        }
	}


    //Function called when this object hits a trigger collider
    private void OnTriggerEnter(Collider collider_)
    {
        //If the object hit has a RegionZone.cs component, we change our movement behaviors
        if(collider_.gameObject.GetComponent<RegionZone>())
        {
            //If the region zone effects this player
            if (collider_.gameObject.GetComponent<RegionZone>().effectedPlayer == this.playerController)
            {
                //If the region has rail movement
                if (collider_.gameObject.GetComponent<RegionZone>().movementType == RegionZone.RegionMovement.Rail)
                {
                    //We disable our free movement controls and enable our rail movement controls
                    this.ourFreeMovement.enabled = false;
                    this.ourRailMovement.enabled = true;

                    //Setting the new direction for our rail movement
                    this.ourRailMovement.SetNewRailDirection(collider_);
                }
                //If the region has free movement
                else if (collider_.gameObject.GetComponent<RegionZone>().movementType == RegionZone.RegionMovement.Free)
                {
                    //We disable our rail movement controls and enable our free movement controls
                    this.ourRailMovement.enabled = false;
                    this.ourFreeMovement.enabled = true;
                }
            }
        }
    }
}

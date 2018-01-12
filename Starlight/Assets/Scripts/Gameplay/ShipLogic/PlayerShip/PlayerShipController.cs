using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FreeMovementFlight))]
[RequireComponent(typeof(RailMovementFlight))]
public class PlayerShipController : MonoBehaviour
{
    //Enum to determine which player controls this ship
    public Players playerController = Players.P1;

    //References to the public static ship controllers for each ship
    public static PlayerShipController p1ShipRef;
    public static PlayerShipController p2ShipRef;

    //The controller input that we use for this ship
    [HideInInspector]
    public ControllerInput ourController;

    //The input settings for this player
    [HideInInspector]
    public PlayerInputs ourCustomInputs;

    [Space(8)]

    //References to this ship's different movement mechanic scripts
    [HideInInspector]
    public FreeMovementFlight ourFreeMovement;
    [HideInInspector]
    public RailMovementFlight ourRailMovement;

    //The main weapon for this ship
    public Weapon mainWeapon;
    //The secondary weapon for this ship
    public Weapon secondaryWeapon;

    [Space(8)]

    //The game objects that are used as a gyroscope to pivot our ship model without having to deal with annoying rotation problems
    public Transform xGyroscope;
    public Transform yGyroscope;
    public Transform zGyroscope;

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
                //Making sure there's not already a static reference to the p1 ship
                if (p1ShipRef == null)
                {
                    p1ShipRef = this;
                    this.ourController = ControllerInputManager.P1Controller;
                    this.ourCustomInputs = CustomInputSettings.globalReference.p1Inputs;
                }
                //If there's already a static reference for the p1 ship, we disable this object
                else
                {
                    this.gameObject.SetActive(false);
                }
                break;

            case Players.P2:
                //Making sure there's not already a static reference to the p2 ship
                if (p2ShipRef == null)
                {
                    p2ShipRef = this;
                    this.ourController = ControllerInputManager.P2Controller;
                    this.ourCustomInputs = CustomInputSettings.globalReference.p2Inputs;
                }
                //If there's already a static reference for the p2 ship, we disable this object
                else
                {
                    this.gameObject.SetActive(false);
                }
                break;

            default:
                //Making sure there's not already a static reference to the p1 ship
                if (p1ShipRef == null)
                {
                    p1ShipRef = this;
                    this.ourController = ControllerInputManager.P1Controller;
                    this.ourCustomInputs = CustomInputSettings.globalReference.p1Inputs;
                }
                //If there's already a static reference for the p1 ship, we disable this object
                else
                {
                    this.gameObject.SetActive(false);
                }
                break;
        }

        //Passing our controller input to our movement mechanic scripts
        this.ourFreeMovement.ourShip = this;
        this.ourRailMovement.ourShip = this;

        //Looping through all of our weapons, wings and engines to tell them what player ID we are
        if (this.playerController == Players.P1)
        {
            this.mainWeapon.objectIDType = AttackerID.Player1;
            this.secondaryWeapon.objectIDType = AttackerID.Player1;
        }
        else
        {
            this.mainWeapon.objectIDType = AttackerID.Player2;
            this.secondaryWeapon.objectIDType = AttackerID.Player2;
        }
        foreach (ShipWingLogic wing in this.shipWings)
        {
            if(this.playerController == Players.P1)
            {
                wing.objectIDType = AttackerID.Player1;
            }
            else if (this.playerController == Players.P2)
            {
                wing.objectIDType = AttackerID.Player2;
            }
        }
        foreach(ShipEngineLogic engine in this.shipEngines)
        {
            if (this.playerController == Players.P1)
            {
                engine.objectIDType = AttackerID.Player1;
            }
            else if (this.playerController == Players.P2)
            {
                engine.objectIDType = AttackerID.Player2;
            }
        }
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

        //If the player presses the button to invert Y movement controls
        if(this.ourController.CheckButtonPressed(this.ourCustomInputs.invertY_Controller) || Input.GetKeyDown(this.ourCustomInputs.invertY_Keyboard))
        {
            this.ourCustomInputs.invertYMovement = !this.ourCustomInputs.invertYMovement;
        }
	}


    //Function called when this object hits a trigger collider
    private void OnTriggerEnter(Collider collider_)
    {
        //If the object hit has a RegionZone.cs component, we change our movement behaviors
        if(collider_.gameObject.GetComponent<RegionZone>())
        {
            //If the region zone effects this player
            if ((collider_.gameObject.GetComponent<RegionZone>().effectPlayer1 && this.playerController == Players.P1) ||
                (collider_.gameObject.GetComponent<RegionZone>().effectPlayer2 && this.playerController == Players.P2))
            {
                //If the region has rail movement
                /*if (collider_.gameObject.GetComponent<RegionZone>().movementType == RegionZone.RegionMovement.Rail)
                {
                    //We disable our free movement controls and enable our rail movement controls
                    this.ourFreeMovement.enabled = false;
                    this.ourRailMovement.enabled = true;

                    //Setting the new direction for our rail movement
                    this.ourRailMovement.SetNewRailDirection(collider_);
                }*/
                //If the region has free movement
                if (collider_.gameObject.GetComponent<RegionZone>().movementType == RegionZone.RegionMovement.Free)
                {
                    //We disable our rail movement controls and enable our free movement controls
                    this.ourRailMovement.BeforeDisable();
                    this.ourRailMovement.enabled = false;
                    this.ourFreeMovement.enabled = true;
                }
            }
        }
    }
}

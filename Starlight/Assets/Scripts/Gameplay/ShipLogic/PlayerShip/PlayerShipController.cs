using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(FreeMovementFlight))]
[RequireComponent(typeof(RailMovementFlight))]
public class PlayerShipController : MonoBehaviour
{
    //Enum to determine which player controls this ship
    public Players playerController = Players.P1;
    //The controller input that we use for this ship
    [HideInInspector]
    public ControllerInput ourController;

    //References to this ship's different movement mechanic scripts
    private FreeMovementFlight ourFreeMovement;
    private RailMovementFlight ourRailMovement;

    //The main weapon for this ship
    public Weapon mainWeapon;
    //The secondary weapon for this ship
    public Weapon secondaryWeapon;

    //The controller button used to fire the main weapon
    public ControllerButtons mainFireButton_Controller = ControllerButtons.A_Button;
    //The keyboard/mouse button used to fire the main weapon
    public KeyCode mainFireButton_Keyboard = KeyCode.Mouse0;

    [Space(8)]

    //The controller button used to fire the secondary weapon
    public ControllerButtons secondaryFireButton_Controller = ControllerButtons.B_Button;
    //The keyboard/mouse button used to fire the secondary weapon
    public KeyCode secondaryFireButton_Keyboard = KeyCode.Mouse1;

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
                break;

            case Players.P2:
                this.ourController = ControllerInputManager.P2Controller;
                break;

            case Players.P3:
                this.ourController = ControllerInputManager.P3Controller;
                break;

            case Players.P4:
                this.ourController = ControllerInputManager.P4Controller;
                break;
        }

        //Passing our controller input to our movement mechanic scripts
        this.ourFreeMovement.ourController = this;
        this.ourRailMovement.ourController = this;
    }


	// Update is called once per frame
	private void Update ()
    {
		//If we have a main weapon, we pass it the controller input and keyboard input for the main fire
        if(this.mainWeapon != null)
        {
            this.mainWeapon.FireWeapon(this.ourController.CheckButtonPressed(this.mainFireButton_Controller),
                                        this.ourController.CheckButtonDown(this.mainFireButton_Controller),
                                        this.ourController.CheckButtonReleased(this.mainFireButton_Controller));

            this.mainWeapon.FireWeapon(Input.GetKeyDown(this.mainFireButton_Keyboard),
                                        Input.GetKey(this.mainFireButton_Keyboard),
                                        Input.GetKeyUp(this.mainFireButton_Keyboard));
        }

        //If we have a secondary weapon, we pass it the controller input for the secondary fire
        if(this.secondaryWeapon != null)
        {
            this.secondaryWeapon.FireWeapon(this.ourController.CheckButtonPressed(this.secondaryFireButton_Controller),
                                        this.ourController.CheckButtonDown(this.secondaryFireButton_Controller),
                                        this.ourController.CheckButtonReleased(this.secondaryFireButton_Controller));

            this.secondaryWeapon.FireWeapon(Input.GetKeyDown(this.secondaryFireButton_Keyboard),
                                        Input.GetKey(this.secondaryFireButton_Keyboard),
                                        Input.GetKeyUp(this.secondaryFireButton_Keyboard));
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

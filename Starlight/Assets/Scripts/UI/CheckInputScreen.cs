using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckInputScreen : MonoBehaviour
{
    //Button to activate if the player can progress
    public Button progressButton;

    //The UI element to enable if the player has no controller plugged in
    public GameObject noController;

    //The UI element to enable if the player has the p1 controller plugged in
    public GameObject p1ControllerPluggedIn;
    //The UI element to enable if the player has the p2 controller plugged in
    public GameObject p2ControllerPluggedIn;

    
	
	// Update is called once per frame
	private void Update ()
    {
        //Checking to see if the player 1 
        string[] controllers = Input.GetJoystickNames();

        //If there's no controller connected, we display the noController object
        if(controllers.Length == 0)
        {
            //Making sure the p1 controller is using default controls
            this.SetP1ControlAsP1();
            //Making sure the UI is displayed for no controllers plugged in
            this.noController.SetActive(true);
            this.p1ControllerPluggedIn.SetActive(false);
            this.p2ControllerPluggedIn.SetActive(false);

            //Making sure the progress button is disabled so the players can't do co-op mode with 1 player
            this.progressButton.interactable = false;
        }
        //If there's 1 controller connected, we display the p1 controller plugged in obj
        else if(controllers.Length == 1)
        {
            //Making sure the p1 controller is being used for p2 controls
            this.SetP1ControlAsP2();
            //Making sure the UI is displayed for 1 controller plugged in
            this.noController.SetActive(false);
            this.p1ControllerPluggedIn.SetActive(true);
            this.p2ControllerPluggedIn.SetActive(false);

            //Making sure the progress button is enabled so we can go to the next screen
            this.progressButton.interactable = true;
        }
        //If there's at least 2 controllers connected, we display both controller plugged in obj
        else if(controllers.Length > 1)
        {
            //Making sure the p1 controller is using default controls
            this.SetP1ControlAsP1();
            //Making sure the UI is displayed for both controllers plugged in
            this.noController.SetActive(false);
            this.p1ControllerPluggedIn.SetActive(false);
            this.p2ControllerPluggedIn.SetActive(true);

            //Making sure the progress button is enabled so we can go to the next screen
            this.progressButton.interactable = true;
        }
	}


    //Function called externally through UI events to set the player 1 controller as the player 2 controller
    public void SetP1ControlAsP2()
    {
        ControllerInputManager.P1Controller.SetPlayerID(Players.P2);
        ControllerInputManager.P2Controller.SetPlayerID(Players.P1);
    }


    //Function called externally through UI events to set the player 1 controller back to player 1 input
    public void SetP1ControlAsP1()
    {
        ControllerInputManager.P1Controller.SetPlayerID(Players.P1);
        ControllerInputManager.P2Controller.SetPlayerID(Players.P2);
    }
}

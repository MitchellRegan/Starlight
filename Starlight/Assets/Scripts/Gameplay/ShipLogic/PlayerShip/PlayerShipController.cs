using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShipController : MonoBehaviour
{
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

    //The controller button used to boost forward
    public ControllerButtons boostButton_Controller = ControllerButtons.Right_Trigger;
    //The keyboard/mouse button used to boost forward
    public KeyCode boostButton_Keyboard = KeyCode.Space;

    [Space(8)]

    //The controller button used to break
    public ControllerButtons breakButton_Controller = ControllerButtons.Left_Trigger;
    //The keyboard/mouse button used to break
    public KeyCode breakButton_Keyboard = KeyCode.LeftShift;



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	private void Update ()
    {
		//If 
	}
}

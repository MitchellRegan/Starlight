using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomInputSettings : MonoBehaviour
{
    //The static reference for this component
    static public CustomInputSettings globalReference;

    //Player inputs for each player
    public PlayerInputs p1Inputs;
    public PlayerInputs p2Inputs;



    // Use this for initialization
    private void Awake()
    {
        //If the global reference is null, this component becomes the global reference
        if(globalReference == null)
        {
            globalReference = this;
        }
        //If there's already a global reference, this component is destroyed
        else
        {
            Destroy(this);
        }

        //Creating new inputs for the players
        this.p1Inputs = new PlayerInputs();
        this.p2Inputs = new PlayerInputs();
    }
}

//Class used by CustomInputSettings to hold all input buttons for a given player
[System.Serializable]
public class PlayerInputs
{
    //~~~~~~~~~~~~~~~~~~~~~~~CONTROLLER INPUT~~~~~~~~~~~~~~~~~~~~~~~~~
    //The controller stick for moving left and right
    public ControllerSticks moveLeftRight_Controller = ControllerSticks.Left_Stick_X;
    //The controller stick for moving up and down
    public ControllerSticks moveUpDown_Controller = ControllerSticks.Left_Stick_Y;
    //The controller stick for aiming left and right
    public ControllerSticks aimLeftRightStick = ControllerSticks.Right_Stick_X;
    //The controller stick for aiming up and down
    public ControllerSticks aimUpDownStick = ControllerSticks.Right_Stick_Y;

    [Space(8)]

    //The controller button used to fire the main weapon
    public ControllerButtons mainFireButton_Controller = ControllerButtons.A_Button;
    //The controller button used to fire the secondary weapon
    public ControllerButtons secondaryFireButton_Controller = ControllerButtons.B_Button;

    [Space(8)]

    //The controller button used to boost forward
    public ControllerButtons boostButton_Controller = ControllerButtons.Right_Trigger;
    //The controller button used to break
    public ControllerButtons breakButton_Controller = ControllerButtons.Left_Trigger;


    [Space(18)]

    //~~~~~~~~~~~~~~~~~~~~~~~KEYBOARD/MOUSE INPUT~~~~~~~~~~~~~~~~~~~~~~~~~
    //The keyboard input for moving left
    public KeyCode moveLeft_Keyboard = KeyCode.A;
    //The keyboard input for moving right
    public KeyCode moveRight_Keyboard = KeyCode.D;
    //The keyboard input for moving up
    public KeyCode moveUp_Keyboard = KeyCode.W;
    //The keyboard input for moving down
    public KeyCode moveDown_Keyboard = KeyCode.S;

    [Space(8)]

    //The keyboard/mouse button used to fire the main weapon
    public KeyCode mainFireButton_Keyboard = KeyCode.Mouse0;
    //The keyboard/mouse button used to fire the secondary weapon
    public KeyCode secondaryFireButton_Keyboard = KeyCode.Mouse1;

    [Space(8)]
    
    //The keyboard/mouse button used to boost forward
    public KeyCode boostButton_Keyboard = KeyCode.Space;
    //The keyboard/mouse button used to break
    public KeyCode breakButton_Keyboard = KeyCode.LeftShift;
}
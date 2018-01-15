using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerHilight : MonoBehaviour
{
    //Enum to determine which player we take input from
    public Players playerInput = Players.P1;

    //Reference to our player controller and custom input settings
    ControllerInput ourController;
    PlayerInputs ourCustomInputs;

    //The list of buttons that this script interacts with
    public List<Button> buttonList;

    //The index of the button that we're currently on
    private int currentButtonIndex = 0;

    //The amount of cooldown time between moving between different buttons
    private float moveDelay = 0.1f;
    //The current amount of time remaining for our move delay
    private float currentDelayTime = 0;



    //Function called on initialize
    private void Start()
    {
        //Getting the reference to the player input for this selected player
        switch(this.playerInput)
        {
            case Players.P1:
                this.ourController = ControllerInputManager.P1Controller;
                this.ourCustomInputs = GlobalData.globalReference.GetComponent<CustomInputSettings>().p1Inputs;
                break;

            case Players.P2:
                this.ourController = ControllerInputManager.P2Controller;
                this.ourCustomInputs = GlobalData.globalReference.GetComponent<CustomInputSettings>().p2Inputs;
                break;

            default:
                this.ourController = ControllerInputManager.P1Controller;
                this.ourCustomInputs = GlobalData.globalReference.GetComponent<CustomInputSettings>().p1Inputs;
                break;
        }
    }


	//Function called when this component is enabled
    private void OnEnable()
    {
        //Setting our current button index to the first one
        this.currentButtonIndex = 0;
    }
	

	// Update is called once per frame
	private void Update ()
    {
        //If the player presses the A button on the controller or the spacebar or enter buttons on keyboard, the player presses the current button
        if(this.ourController.CheckButtonPressed(ControllerButtons.A_Button) || Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            this.buttonList[this.currentButtonIndex].onClick.Invoke();
        }

        //If our current delay time is above 0, we need to count it down
        if (this.currentDelayTime > 0)
        {
            this.currentDelayTime -= Time.deltaTime;
            return;
        }
        //If we don't have to wait for the delay, we can take directional input
        else
        {
            //If our player presses the up button on the D-pad, the left joystick is up, or W or Up arrow on the keyboard are pressed
            if (this.ourController.CheckButtonPressed(ControllerButtons.D_Pad_Up) ||
                this.ourController.CheckStickValue(ControllerSticks.Left_Stick_Y) > 0.1f ||
                Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                //If our current button has a UI connection up
                if (this.buttonList[this.currentButtonIndex].navigation.selectOnUp != null)
                {
                    .//need to navigate
                }
            }
            //If our player presses the down button on the D-pad, the left joystick is down, or S or Down arrow on the keyboard are pressed
            else if (this.ourController.CheckButtonPressed(ControllerButtons.D_Pad_Down) ||
                this.ourController.CheckStickValue(ControllerSticks.Left_Stick_Y) < -0.1f ||
                Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                //If our current button has a UI connection down
                if (this.buttonList[this.currentButtonIndex].navigation.selectOnDown != null)
                {

                }
            }
            //If our player presses the left button on the D-pad, the left joystick is left, or A or Left arrow on the keyboard are pressed
            else if (this.ourController.CheckButtonPressed(ControllerButtons.D_Pad_Left) ||
                this.ourController.CheckStickValue(ControllerSticks.Left_Stick_X) < -0.1f ||
                Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                //If our current button has a UI connection left
                if (this.buttonList[this.currentButtonIndex].navigation.selectOnLeft != null)
                {

                }
            }
            //If our player presses the right button on the D-pad, the left joystick is right, or D or Right arrow on the keyboard are pressed
            else if (this.ourController.CheckButtonPressed(ControllerButtons.D_Pad_Right) ||
                this.ourController.CheckStickValue(ControllerSticks.Left_Stick_X) > 0.1f ||
                Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                //If our current button has a UI connection right
                if (this.buttonList[this.currentButtonIndex].navigation.selectOnRight != null)
                {

                }
            }
        }
    }
}

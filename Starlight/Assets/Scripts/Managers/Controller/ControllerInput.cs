using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInput
{
    //Determines if this controller gets input
    private bool IsDisabled = false;

    //Player ID of this controller
    public Players PlayerID = Players.P1;

    //The player prefix used when getting input from Unity's default input manager
    private string Prefix = "P1";
    private string JoystickNum = "1";

    //X and Y range from -1 to 1
    public Vector2 LeftStick;
    //X and Y range from -1 to 1
    public Vector2 RightStick;
    //X and Y range from -1 to 1
    public Vector2 DPad;
    //Vector to hold DPad's values from the previous frame to compare to
    private Vector2 PrevDPad;

    //The speed that this player's camera rotates
    private float LookSensitivity = 0.5f;

    //Multipliers that are set to -1 if a stick is inverted
    private float LeftInvertY = 1.0f;
    private float RightInvertY = 1.0f;

    //Float ranging from 0 to 1 showing how far the left trigger is pressed
    public float LeftTrigger = 0;
    public float LeftTriggerPrevious = 0;
    //Float ranging from 0 to 1 showing how far the right trigger is pressed
    public float RightTrigger = 0;
    public float RightTriggerPrevious = 0;

    public bool A_Button_Pressed = false;
    public bool B_Button_Pressed = false;
    public bool X_Button_Pressed = false;
    public bool Y_Button_Pressed = false;
    public bool Start_Button_Pressed = false;
    public bool Back_Button_Pressed = false;
    public bool Left_Bumper_Pressed = false;
    public bool Right_Bumper_Pressed = false;
    public bool Left_Stick_Button_Pressed = false;
    public bool Right_Stick_Button_Pressed = false;
    public bool DPad_Up_Pressed = false;
    public bool DPad_Down_Pressed = false;
    public bool DPad_Left_Pressed = false;
    public bool DPad_Right_Pressed = false;
    public bool Left_Trigger_Pressed = false;
    public bool Right_Trigger_Pressed = false;

    public bool A_Button_Down = false;
    public bool B_Button_Down = false;
    public bool X_Button_Down = false;
    public bool Y_Button_Down = false;
    public bool Start_Button_Down = false;
    public bool Back_Button_Down = false;
    public bool Left_Bumper_Down = false;
    public bool Right_Bumper_Down = false;
    public bool Left_Stick_Button_Down = false;
    public bool Right_Stick_Button_Down = false;
    public bool DPad_Up_Down = false;
    public bool DPad_Down_Down = false;
    public bool DPad_Left_Down = false;
    public bool DPad_Right_Down = false;
    public bool Left_Trigger_Down = false;
    public bool Right_Trigger_Down = false;

    public bool A_Button_Released = false;
    public bool B_Button_Released = false;
    public bool X_Button_Released = false;
    public bool Y_Button_Released = false;
    public bool Start_Button_Released = false;
    public bool Back_Button_Released = false;
    public bool Left_Bumper_Released = false;
    public bool Right_Bumper_Released = false;
    public bool Left_Stick_Button_Released = false;
    public bool Right_Stick_Button_Released = false;
    public bool DPad_Up_Released = false;
    public bool DPad_Down_Released = false;
    public bool DPad_Left_Released = false;
    public bool DPad_Right_Released = false;
    public bool Left_Trigger_Released = false;
    public bool Right_Trigger_Released = false;



    // Use this for initialization
    void Start()
    {
        //Initializes the new vectors
        LeftStick = new Vector2();
        RightStick = new Vector2();
        DPad = new Vector2();
        PrevDPad = new Vector2();
    }


    // Update is called once per frame
    public void LogicUpdate()
    {
        //Does nothing if this controller's input is disabled
        if (IsDisabled)
            return;


        //Stores the XY input for the left stick, right stick, and Dpad
        LeftStick.x = Input.GetAxis(Prefix + "_LeftJoystick_X");
        LeftStick.y = Input.GetAxis(Prefix + "_LeftJoystick_Y") * LeftInvertY;

        RightStick.x = Input.GetAxis(Prefix + "_RightJoystick_X");
        RightStick.y = Input.GetAxis(Prefix + "_RightJoystick_Y") * RightInvertY;

        DPad.x = Input.GetAxis(Prefix + "_DPad_X");
        DPad.y = Input.GetAxis(Prefix + "_DPad_Y");

        //Stores the previous values for the triggers
        LeftTriggerPrevious = LeftTrigger;
        RightTriggerPrevious = RightTrigger;

        //Stores the trigger ranges for this controller
        LeftTrigger = Input.GetAxis(Prefix + "_LeftTrigger");
        RightTrigger = Input.GetAxis(Prefix + "_RightTrigger");

        //Finds the states for all controller buttons
        A_Button_Pressed = CheckButtonPressed(ControllerButtons.A_Button);
        A_Button_Down = CheckButtonDown(ControllerButtons.A_Button);
        A_Button_Released = CheckButtonReleased(ControllerButtons.A_Button);

        B_Button_Pressed = CheckButtonPressed(ControllerButtons.B_Button);
        B_Button_Down = CheckButtonDown(ControllerButtons.B_Button);
        B_Button_Released = CheckButtonReleased(ControllerButtons.B_Button);

        X_Button_Pressed = CheckButtonPressed(ControllerButtons.X_Button);
        X_Button_Down = CheckButtonDown(ControllerButtons.X_Button);
        X_Button_Released = CheckButtonReleased(ControllerButtons.X_Button);

        Y_Button_Pressed = CheckButtonPressed(ControllerButtons.Y_Button);
        Y_Button_Down = CheckButtonDown(ControllerButtons.Y_Button);
        Y_Button_Released = CheckButtonReleased(ControllerButtons.Y_Button);

        Start_Button_Pressed = CheckButtonPressed(ControllerButtons.Start_Button);
        Start_Button_Down = CheckButtonDown(ControllerButtons.Start_Button);
        Start_Button_Released = CheckButtonReleased(ControllerButtons.Start_Button);

        Back_Button_Pressed = CheckButtonPressed(ControllerButtons.Back_Button);
        Back_Button_Down = CheckButtonDown(ControllerButtons.Back_Button);
        Back_Button_Pressed = CheckButtonReleased(ControllerButtons.Back_Button);

        Left_Bumper_Pressed = CheckButtonPressed(ControllerButtons.Left_Bumper);
        Left_Bumper_Down = CheckButtonDown(ControllerButtons.Left_Bumper);
        Left_Bumper_Released = CheckButtonReleased(ControllerButtons.Left_Bumper);

        Right_Bumper_Pressed = CheckButtonPressed(ControllerButtons.Right_Bumper);
        Right_Bumper_Down = CheckButtonDown(ControllerButtons.Right_Bumper);
        Right_Bumper_Released = CheckButtonReleased(ControllerButtons.Right_Bumper);

        Left_Stick_Button_Pressed = CheckButtonPressed(ControllerButtons.Left_Stick_Click);
        Left_Stick_Button_Down = CheckButtonDown(ControllerButtons.Left_Stick_Click);
        Left_Stick_Button_Released = CheckButtonReleased(ControllerButtons.Left_Stick_Click);

        Right_Stick_Button_Pressed = CheckButtonPressed(ControllerButtons.Right_Stick_Click);
        Right_Stick_Button_Down = CheckButtonDown(ControllerButtons.Right_Stick_Click);
        Right_Stick_Button_Released = CheckButtonReleased(ControllerButtons.Right_Stick_Click);

        Left_Trigger_Pressed = CheckButtonPressed(ControllerButtons.Left_Trigger);
        Left_Trigger_Down = CheckButtonDown(ControllerButtons.Left_Trigger);
        Left_Trigger_Released = CheckButtonReleased(ControllerButtons.Left_Trigger);

        Right_Trigger_Pressed = CheckButtonPressed(ControllerButtons.Right_Trigger);
        Right_Trigger_Down = CheckButtonDown(ControllerButtons.Right_Trigger);
        Right_Trigger_Released = CheckButtonReleased(ControllerButtons.Right_Trigger);

        CheckDPadPressed();
        CheckDPadDown();
        CheckDPadReleased();

        //Stores the DPad values to compare them next frame
        PrevDPad = DPad;
    }



    //Function called by Manager_InputManager to make this script get the correct player input
    public void SetPlayerID(Players playerID_)
    {
        PlayerID = playerID_;

        switch (PlayerID)
        {
            case Players.P1:
                Prefix = "P1";
                JoystickNum = "1";
                break;
            case Players.P2:
                Prefix = "P2";
                JoystickNum = "2";
                break;
            case Players.P3:
                Prefix = "P3";
                JoystickNum = "3";
                break;
            case Players.P4:
                Prefix = "P4";
                JoystickNum = "4";
                break;
            default:
                Prefix = "P1";
                JoystickNum = "1";
                Debug.LogError("ERROR! Misc_ControllerInput.SetPlayerID, Player ID not allowed");
                break;
        }
    }



    //Checks to see if a controller button was pressed this frame based on the ControllerButtons enum given
    public bool CheckButtonPressed(ControllerButtons buttonID_)
    {
        //Getting the input based on the button ID
        switch(buttonID_)
        {
            case ControllerButtons.Left_Trigger:
                //If the previous left trigger input is below 50% and the current input is above, it was pressed
                if (LeftTriggerPrevious < 0.5f && LeftTrigger >= 0.5f)
                {
                    return true;
                }
                //If the current input is still above 50% it's not released
                else
                {
                    return false;
                }

            case ControllerButtons.Right_Trigger:
                //If the previous right trigger input is below 50% and the current input is above, it was pressed
                if (RightTriggerPrevious < 0.5f && RightTrigger >= 0.5f)
                {
                    return true;
                }
                //Otherwise it wasn't pressed
                else
                {
                    return false;
                }

            case ControllerButtons.D_Pad_Up:
                if (DPad.y > 0.5 && PrevDPad.y < 0.5)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case ControllerButtons.D_Pad_Down:
                if (DPad.y < -0.5 && PrevDPad.y > -0.5)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case ControllerButtons.D_Pad_Left:
                if (DPad.x < -0.5 && PrevDPad.x > -0.5)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case ControllerButtons.D_Pad_Right:
                if (DPad.x > 0.5 && PrevDPad.x < 0.5)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            default:
                return Input.GetKeyDown("joystick " + JoystickNum + " button " + ((int)buttonID_));
        }
    }



    //Checks to see if a controller button was held this frame
    public bool CheckButtonDown(ControllerButtons buttonID_)
    {
        //Getting the input based on the button ID
        switch (buttonID_)
        {
            case ControllerButtons.Left_Trigger:
                //If the current input is above 50% it's down
                if (LeftTrigger >= 0.5f)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case ControllerButtons.Right_Trigger:
                //If the current input is above above 50% it's down
                if (RightTrigger >= 0.5f)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case ControllerButtons.D_Pad_Up:
                if (DPad.y > 0.5)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case ControllerButtons.D_Pad_Down:
                if (DPad.y < -0.5)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case ControllerButtons.D_Pad_Left:
                if (DPad.x < -0.5)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case ControllerButtons.D_Pad_Right:
                if (DPad.x > 0.5)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            default:
                return Input.GetKey("joystick " + JoystickNum + " button " + ((int)buttonID_));
        }
    }



    //Checks to see if a controller button was released this frame
    public bool CheckButtonReleased(ControllerButtons buttonID_)
    {
        //Getting the input based on the button ID
        switch(buttonID_)
        {
            case ControllerButtons.Left_Trigger:
                //If the previous left trigger input is above 50% and the current input is below, it was released
                if(LeftTriggerPrevious >= 0.5f && LeftTrigger < 0.5f)
                {
                    return true;
                }
                //If the current input is still above 50% it's not released
                else
                {
                    return false;
                }

            case ControllerButtons.Right_Trigger:
                //If the previous right trigger input is above 50% and the current input is below, it was released
                if (RightTriggerPrevious >= 0.5f && RightTrigger < 0.5f)
                {
                    return true;
                }
                //If the current input is still above 50% it's not released
                else
                {
                    return false;
                }

            case ControllerButtons.D_Pad_Up:
                if (DPad.y < 0.5 && PrevDPad.y > 0.5)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case ControllerButtons.D_Pad_Down:
                if (DPad.y > -0.5 && PrevDPad.y < -0.5)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case ControllerButtons.D_Pad_Left:
                if (DPad.x > -0.5 && PrevDPad.x < -0.5)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case ControllerButtons.D_Pad_Right:
                if (DPad.x < 0.5 && PrevDPad.x > 0.5)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            default:
                return Input.GetKeyUp("joystick " + JoystickNum + " button " + ((int)buttonID_));
        }
    }



    //Checks and sets all 4 DPad buttons to see if they were pressed this frame
    private void CheckDPadPressed()
    {
        //Gets input for Dpad up
        if (DPad.y > 0.5 && PrevDPad.y < 0.5)
            DPad_Up_Pressed = true;
        else
            DPad_Up_Pressed = false;

        //Gets input for Dpad down
        if (DPad.y < -0.5 && PrevDPad.y > -0.5)
            DPad_Down_Pressed = true;
        else
            DPad_Down_Pressed = false;

        //Gets input for Dpad left
        if (DPad.x < -0.5 && PrevDPad.x > -0.5)
            DPad_Left_Pressed = true;
        else
            DPad_Left_Pressed = false;

        //Gets input for Dpad right
        if (DPad.x > 0.5 && PrevDPad.x < 0.5)
            DPad_Right_Pressed = true;
        else
            DPad_Right_Pressed = false;
    }



    //Checks and sets all 4 DPad buttons to see if they were held this frame
    private void CheckDPadDown()
    {
        //Gets input for Dpad Up
        if (DPad.y > 0.5)
            DPad_Up_Down = true;
        else
            DPad_Up_Down = false;

        //Gets input for Dpad down
        if (DPad.y < -0.5)
            DPad_Down_Down = true;
        else
            DPad_Down_Down = false;

        //Gets input for Dpad left
        if (DPad.x < -0.5)
            DPad_Left_Down = true;
        else
            DPad_Left_Down = false;

        //Gets input for Dpad right
        if (DPad.x > 0.5)
            DPad_Right_Down = true;
        else
            DPad_Right_Down = false;
    }


    //Checks the value of a controller stick
    public float CheckStickValue(ControllerSticks stickID_)
    {
        switch(stickID_)
        {
            case ControllerSticks.Left_Stick_X:
                return this.LeftStick.x;

            case ControllerSticks.Left_Stick_Y:
                return this.LeftStick.y * this.LeftInvertY;

            case ControllerSticks.Right_Stick_X:
                return this.RightStick.x;

            case ControllerSticks.Right_Stick_Y:
                return this.RightStick.y * this.RightInvertY;

            case ControllerSticks.Left_Trigger:
                return this.LeftTrigger;

            case ControllerSticks.Right_Trigger:
                return this.RightTrigger;

            case ControllerSticks.D_Pad_X:
                return this.DPad.x;

            case ControllerSticks.D_Pad_Y:
                return this.DPad.y;

            default:
                return 0;
        }
    }


    //Checks and sets all 4 DPad buttons to see if they were released this frame
    private void CheckDPadReleased()
    {
        //Gets input for Dpad up
        if (DPad.y < 0.5 && PrevDPad.y > 0.5)
            DPad_Up_Released = true;
        else
            DPad_Up_Released = false;

        //Gets input for Dpad down
        if (DPad.y > -0.5 && PrevDPad.y < -0.5)
            DPad_Down_Released = true;
        else
            DPad_Down_Released = false;

        //Gets input for Dpad left
        if (DPad.x > -0.5 && PrevDPad.x < -0.5)
            DPad_Left_Released = true;
        else
            DPad_Left_Released = false;

        //Gets input for Dpad right
        if (DPad.x < 0.5 && PrevDPad.x > 0.5)
            DPad_Right_Released = true;
        else
            DPad_Right_Released = false;
    }



    //Makes it so that this controller can no longer get new input until it is re-enabled
    public void DisableInput()
    {
        IsDisabled = true;

        LeftStick.x = 0;
        LeftStick.y = 0;
        RightStick.x = 0;
        RightStick.y = 0;
        DPad.x = 0;
        DPad.y = 0;
        PrevDPad.x = 0;
        PrevDPad.y = 0;

        LeftTrigger = 0;
        RightTrigger = 0;

        A_Button_Pressed = false;
        B_Button_Pressed = false;
        X_Button_Pressed = false;
        Y_Button_Pressed = false;
        Start_Button_Pressed = false;
        Back_Button_Pressed = false;
        Left_Bumper_Pressed = false;
        Right_Bumper_Pressed = false;
        Left_Stick_Button_Pressed = false;
        Right_Stick_Button_Pressed = false;
        DPad_Up_Pressed = false;
        DPad_Down_Pressed = false;
        DPad_Left_Pressed = false;
        DPad_Right_Pressed = false;

        A_Button_Down = false;
        B_Button_Down = false;
        X_Button_Down = false;
        Y_Button_Down = false;
        Start_Button_Down = false;
        Back_Button_Down = false;
        Left_Bumper_Down = false;
        Right_Bumper_Down = false;
        Left_Stick_Button_Down = false;
        Right_Stick_Button_Down = false;
        DPad_Up_Down = false;
        DPad_Down_Down = false;
        DPad_Left_Down = false;
        DPad_Right_Down = false;

        A_Button_Released = false;
        B_Button_Released = false;
        X_Button_Released = false;
        Y_Button_Released = false;
        Start_Button_Released = false;
        Back_Button_Released = false;
        Left_Bumper_Released = false;
        Right_Bumper_Released = false;
        Left_Stick_Button_Released = false;
        Right_Stick_Button_Released = false;
        DPad_Up_Released = false;
        DPad_Down_Released = false;
        DPad_Left_Released = false;
        DPad_Right_Released = false;
    }



    //Makes it so that this controller can take input again after being disabled
    public void EnableInput()
    {
        IsDisabled = false;
    }



    //Inverses this controller's Left Stick Y
    public void InvertLeftY(bool inverted_)
    {
        if (inverted_)
            LeftInvertY = -1.0f;
        else
            LeftInvertY = 1.0f;
    }



    //Inverses this controller's Right Stick Y
    public void InvertRightY(bool inverted_)
    {
        if (inverted_)
            RightInvertY = -1.0f;
        else
            RightInvertY = 1.0f;
    }



    //Sets the look sensitivity for this controller. The range is from 0.1 to 1.0
    public void SetLookSensitivity(float newSensitivity)
    {
        if (newSensitivity > 1)
            LookSensitivity = 1;
        else if (newSensitivity < 0.1f)
            LookSensitivity = 0.1f;
        else
            LookSensitivity = newSensitivity;
    }
}

//Enum used to define buttons on the controller
public enum ControllerButtons
{
    A_Button,
    B_Button,
    X_Button,
    Y_Button,

    Start_Button,
    Back_Button,

    Left_Bumper,
    Right_Bumper,

    Left_Stick_Click,
    Right_Stick_Click,

    Left_Trigger,
    Right_Trigger,

    D_Pad_Up,
    D_Pad_Down,
    D_Pad_Left,
    D_Pad_Right
}


//Enum used to define stick inputs on the controller
public enum ControllerSticks
{
    Left_Stick_X,
    Left_Stick_Y,

    Right_Stick_X,
    Right_Stick_Y,

    D_Pad_X,
    D_Pad_Y,

    Left_Trigger,
    Right_Trigger
}
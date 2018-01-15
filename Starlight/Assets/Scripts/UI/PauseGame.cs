using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseGame : MonoBehaviour
{
    //Static bool that lets the gameplay objects know if time is paused
    public static bool isGamePaused = false;

    //The game object to enable/disable for the player 1 pause screen
    public GameObject p1PauseScreen;
    //The game object to enable/disable for the player 2 pause screen
    public GameObject p2PauseScreen;

    //Bool that determines if player 1 paused the game
    [HideInInspector]
    public bool isGamePausedP1 = false;
    //Bool that determines if player 2 paused the game
    [HideInInspector]
    public bool isGamePausedP2 = false;

    
	
	// Update is called once per frame
	private void Update ()
    {
		//Checking to see if player 1 pressed the pause button for keyboard or controller
        if(PlayerShipController.p1ShipRef.ourController.CheckButtonPressed(PlayerShipController.p1ShipRef.ourCustomInputs.pause_Controller) ||
            Input.GetKeyDown(PlayerShipController.p1ShipRef.ourCustomInputs.pause_Keyboard))
        {
            //If the game isn't paused, we can pause
            if(!this.isGamePausedP1 && !this.isGamePausedP2)
            {
                this.P1Pause();
            }
            //If the game is paused because of player 1, we unpause
            else if(this.isGamePausedP1)
            {
                this.P1Unpause();
            }
            //If player 2 paused the game, nothing happens
        }
        //Checking to see if player 2 exists and pressed the pause button on their controller (don't care about keyboard since p1 should only have it)
        else if(PlayerShipController.p2ShipRef != null &&
                PlayerShipController.p2ShipRef.ourController.CheckButtonPressed(PlayerShipController.p2ShipRef.ourCustomInputs.pause_Controller))
        {
            //If the game isn't paused, we can pause
            if (!this.isGamePausedP1 && !this.isGamePausedP2)
            {
                this.P2Pause();
            }
            //If the game is paused because of player 2, we unpause
            else if (this.isGamePausedP2)
            {
                this.P2Unpause();
            }
            //If player 1 paused the game, nothing happens
        }
    }


    //Function that can be called from Update or externally to pause for player 1
    public void P1Pause()
    {
        //If player 2 has paused the game, nothing happens
        if(this.isGamePausedP2)
        {
            return;
        }

        //Marking that player 1 is the one who paused the game
        this.isGamePausedP1 = true;
        //Freezing the game time
        Time.timeScale = 0;
        //Displaying the player 1 pause screen
        this.p1PauseScreen.SetActive(true);
        //Updating our static bool
        isGamePaused = true;
    }


    //Function that can be called from Update or externally to unpause for player 1
    public void P1Unpause()
    {
        //If player 2 has paused the game, nothing happens
        if (this.isGamePausedP2)
        {
            return;
        }

        //Marking that player 1 is no longer paused
        this.isGamePausedP1 = false;
        //Setting the game time back to normal
        Time.timeScale = 1;
        //Hiding the player 1 pause screen
        this.p1PauseScreen.SetActive(false);
        //Updating our static bool
        isGamePaused = false;
    }


    //Function that can be called from Update or externally to pause for player 2
    public void P2Pause()
    {
        //If player 1 has paused the game, nothing happens
        if (this.isGamePausedP1)
        {
            return;
        }

        //Marking that player 2 is the one who paused the game
        this.isGamePausedP2 = true;
        //Freezing the game time
        Time.timeScale = 0;
        //Displaying the player 2 pause screen
        this.p2PauseScreen.SetActive(true);
        //Updating our static bool
        isGamePaused = true;
    }


    //Function that can be called from Update or externally to unpause for player 2
    public void P2Unpause()
    {
        //If player 1 has paused the game, nothing happens
        if (this.isGamePausedP2)
        {
            return;
        }

        //Marking that player 2 is no longer paused
        this.isGamePausedP2 = false;
        //Setting the game time back to normal
        Time.timeScale = 1;
        //Hiding the player 2 pause screen
        this.p2PauseScreen.SetActive(false);
        //Updating our static bool
        isGamePaused = false;
    }
}

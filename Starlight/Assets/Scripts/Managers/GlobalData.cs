using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData : MonoBehaviour
{
    //A reference to the object that stores data between scenes
    public static GlobalData globalReference;

    //Determines if the mouse cursor is visible or hidden
    public bool ShowMouseCursor = true;

    public Color P1HilightColor = new Color(1, 0f, 0f, 1);
    public Color P2HilightColor = new Color(0, 0.5f, 1, 1);

    //Colors used for player ships
    public PlayerColorSlots p1Colors;
    public PlayerColorSlots p2Colors;

    //Bool that determines if the game mode is single player or co-op. True == 1 player, False == 2 player
    public bool singlePlayerMode = false;

    //The prefab for the ship that's spawned for player 1
    public PlayerShipController player1Ship;
    //The prefab for the ship that's spawned for player 2
    public PlayerShipController player2Ship;



    // Use this for initialization
    void Awake()
    {
        //If there isn't already a static reference to this global data object, creates a new one
        if (globalReference == null)
        {
            DontDestroyOnLoad(this.gameObject);
            globalReference = this;
        }
        //Otherwise, we already have a global data object created and we can't make a new one
        else if (globalReference != this)
        {
            Destroy(this.gameObject);
        }

        //Determines if the mouse cursor is visible or hidden
        UnityEngine.Cursor.visible = ShowMouseCursor;
    }


    //Function called externally at the Main Menu scene to determine if the game is in single player or co-op mode
    public void SetSinglePlayerMode(bool isSinglePlayer_)
    {
        this.singlePlayerMode = isSinglePlayer_;
    }


    //Closes the application
    public void QuitGame()
    {
        Application.Quit();
    }
}


//Class used in GlobalData.cs to store the different color slots for player ships
[System.Serializable]
public class PlayerColorSlots
{
    //Primary color
    public Color slot1 = Color.red;
    //Secondary color
    public Color slot2 = Color.red;
    //Tertiary colors
    public Color slot3 = Color.red;
    public Color slot4 = Color.red;
    public Color slot5 = Color.red;

    //Color used for optional decals
    public Color decal = Color.black;
}
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

    //Bool that determines if the game mode is single player or co-op. True == 1 player, False == 2 player
    public static bool singlePlayerMode = true;

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


    //Closes the application
    public void QuitGame()
    {
        Application.Quit();
    }
}
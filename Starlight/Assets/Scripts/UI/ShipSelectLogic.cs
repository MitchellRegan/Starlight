using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSelectLogic : MonoBehaviour
{
    //The player who is currently selecting their ship
    public Players player = Players.P1;

    //Bool for if the player is currently changing their ship
    private bool isChangingShip = false;

    //The location where we display the current ship
    public RectTransform shipDisplayPos;
    //The locations off screen where new ships move to the display position
    public RectTransform leftTransitionPos;
    public RectTransform rightTransitionPos;

    //The list of ship prefabs that the player can select
    public List<GameObject> shipPrefabs;

    //The index of the currently selected ship
    private int selectedShipIndex = 0;

    //Reference to the displayed ship game object
    private GameObject displayedShip;

    //Reference to the ship that transitions to the displayed ship position
    private GameObject transitionShip;


    
    //Function called when this object is enabled
    public void OnEnable()
    {
        .//Need to loop through all of the ship prefabs to get the one in Global data that the player has and our index
    }


	// Update is called once per frame
	private void Update ()
    {
		//If the player isn't changing their ship, nothing happens
        if(!this.isChangingShip)
        {
            return;
        }

        .//Need to get input for left and right to shift the selected ship index
        .//If the player hits the "select" button we need to invoke the unity event to go back to the options menu
	}
}

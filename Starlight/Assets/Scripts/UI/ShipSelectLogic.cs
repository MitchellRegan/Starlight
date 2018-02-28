using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShipSelectLogic : MonoBehaviour
{
    //The player who is currently selecting their ship
    public Players player = Players.P1;

    //Bool for if the player is currently changing their ship
    private bool isChangingShip = false;

    //The list of ship prefabs that the player can select
    public List<SelectableShipInfo> shipPrefabs;

    //The index of the currently selected ship
    private int selectedShipIndex = 0;


    //The delay between switching between ships
    public float changeShipDelay = 0.4f;
    //The current delay time between switching ships
    private float currentDelayTime = 0;

    //The direction that the ships are moving during a transition
    private bool transitioningLeft = true;

    //Reference to the displayed ship game object
    private GameObject displayedShip;

    //Reference to the ship that transitions to the displayed ship position
    private GameObject transitionShip;

    //Event called when the player confirms the ship selection
    public UnityEvent confirmSelectEvent;

    [Space(16)]

    //The location where we display the current ship
    public RectTransform shipDisplayPos;
    //The locations off screen where new ships move to the display position
    public RectTransform leftTransitionPos;
    public RectTransform rightTransitionPos;

    //Reference to the game object to show for locked ships
    public GameObject lockedScreenObj;

    //Text box references for the selected ship's name and description
    public Text shipNameText;
    public Text shipDescriptionText;

    //Reference to the sliders for the selected ship's stats
    public Slider healthSlider;
    public Slider armorSlider;
    public Slider maneuverabilitySlider;
    public Slider damageSlider;
    public Slider attackSpeedSlider;



    //Function called when this object is enabled
    public void OnEnable()
    {
        //Looping through all of the ship prefabs to get the one in Global data that the player has and our index
        for(int i = 0; i < this.shipPrefabs.Count; ++i)
        {
            //If we're checking the prefab for player 1
            if(this.player == Players.P1)
            {
                //If the ship prefab at this index matches the designated p1 ship in GlobalData, we need to display it
                if(this.shipPrefabs[i].shipPrefab == GlobalData.globalReference.player1Ship)
                {
                    this.selectedShipIndex = i;
                }
            }
            //If we're checking the prefab for player 2
            else
            {
                //If the ship prefab at this index matches the designated p2 ship in GlobalData, we need to display it
                if (this.shipPrefabs[i].shipPrefab == GlobalData.globalReference.player2Ship)
                {
                    this.selectedShipIndex = i;
                }
            }
        }
    }


	// Update is called once per frame
	private void Update ()
    {
		//If the player isn't changing their ship, nothing happens
        if(!this.isChangingShip)
        {
            return;
        }
        
        
        //If we're changing the displayed ship
        if(this.currentDelayTime > 0)
        {
            //Counting down the delay time
            this.currentDelayTime -= Time.deltaTime;

            //If the current delay time is below 0, we need to cap it at 0 so the percent var below works
            if(this.currentDelayTime < 0)
            {
                this.currentDelayTime = 0;
            }

            //Finding the percent along the transition that the ships are across the screen
            float percent = (this.changeShipDelay - this.currentDelayTime) / this.changeShipDelay;

            //If we're transitioning the ship left, we have to move the selected ship positions left
            if(this.transitioningLeft)
            {
                //Getting the position for the transition ship to go offscreen
                Vector3 newTransitionPos = this.shipDisplayPos.position - this.rightTransitionPos.position;
                newTransitionPos *= percent;
                newTransitionPos += this.shipDisplayPos.position;

                //Getting the position for the displayed ship to come on screen
                Vector3 newDisplayPos = this.leftTransitionPos.position - this.shipDisplayPos.position;
                newDisplayPos *= percent;
                newDisplayPos += this.leftTransitionPos.position;

                //Setting the positions of the transition and display ships
                this.transitionShip.transform.position = newTransitionPos;
                this.displayedShip.transform.position = newDisplayPos;
            }
            //If we're transitioning the ship right, we have to move the selected ship positions right
            else
            {
                //Getting the position for the transition ship to go offscreen
                Vector3 newTransitionPos = this.shipDisplayPos.position - this.leftTransitionPos.position;
                newTransitionPos *= percent;
                newTransitionPos += this.shipDisplayPos.position;

                //Getting the position for the displayed ship to come on screen
                Vector3 newDisplayPos = this.rightTransitionPos.position - this.shipDisplayPos.position;
                newDisplayPos *= percent;
                newDisplayPos += this.rightTransitionPos.position;

                //Setting the positions of the transition and display ships
                this.transitionShip.transform.position = newTransitionPos;
                this.displayedShip.transform.position = newDisplayPos;
            }
        }
        //Otherwise we check to see if the player is cycling the selected ship left or right
        else if(this.currentDelayTime <= 0)
        {
            //Getting the input for player 1
            if(this.player == Players.P1)
            {
                //If the player hits the "select" button we need to invoke the unity event to go back to the options menu
                if ((this.player == Players.P1 && Input.GetKeyDown(KeyCode.Return)) || (this.player == Players.P1 && Input.GetKeyDown(KeyCode.Space)) ||
                    ControllerInputManager.P1Controller.CheckButtonPressed(ControllerButtons.A_Button))
                {
                    //Making it so we stop selecting ships
                    this.isChangingShip = false;

                    //Invoking our confirm selection event
                    this.confirmSelectEvent.Invoke();
                }
                //Checking for input to move left
                else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || ControllerInputManager.P1Controller.CheckButtonDown(ControllerButtons.D_Pad_Left) ||
                    ControllerInputManager.P1Controller.CheckStickValue(ControllerSticks.Left_Stick_X) > 0.3f)
                {

                }
                //Checking for input to move right
                else if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || ControllerInputManager.P1Controller.CheckButtonDown(ControllerButtons.D_Pad_Right) ||
                    ControllerInputManager.P1Controller.CheckStickValue(ControllerSticks.Left_Stick_X) < -0.3f)
                {

                }
            }
            //Getting the input for player 2
            else
            {
                //If the player hits the "select" button we need to invoke the unity event to go back to the options menu
                if (ControllerInputManager.P2Controller.CheckButtonPressed(ControllerButtons.A_Button))
                {
                    //Making it so we stop selecting ships
                    this.isChangingShip = false;

                    //Invoking our confirm selection event
                    this.confirmSelectEvent.Invoke();
                }
                //Checking for input to move left
                else if (ControllerInputManager.P2Controller.CheckButtonDown(ControllerButtons.D_Pad_Left) ||
                    ControllerInputManager.P2Controller.CheckStickValue(ControllerSticks.Left_Stick_X) > 0.3f)
                {

                }
                //Checking for input to move right
                else if (ControllerInputManager.P2Controller.CheckButtonDown(ControllerButtons.D_Pad_Right) ||
                    ControllerInputManager.P2Controller.CheckStickValue(ControllerSticks.Left_Stick_X) < -0.3f)
                {

                }
            }
        }
	}


    //Function called from update to start the transition to the next ship
    private void StartTransition(bool transitionLeft_)
    {
        //Setting the delay time to prevent player input
        this.currentDelayTime = this.changeShipDelay;

        //Setting the transition direction
        this.transitioningLeft = transitionLeft_;

        //If the currently selected ship is locked
        if (this.shipPrefabs[this.selectedShipIndex].locked)
        {
            //We display the locked screen icon
            this.lockedScreenObj.SetActive(true);

            //Displaying the ship's name and hiding all of the other stats
            this.shipNameText.text = this.shipPrefabs[this.selectedShipIndex].shipName;
            this.shipDescriptionText.text = "??????????";

            this.healthSlider.value = 0;
            this.armorSlider.value = 0;
            this.maneuverabilitySlider.value = 0;
            this.damageSlider.value = 0;
            this.attackSpeedSlider.value = 0;
        }
        //If the currently selected ship is unlocked
        else
        {
            //We hide the locked screen icon
            this.lockedScreenObj.SetActive(false);

            //Displaying the ship's name and all of the other stats
            this.shipNameText.text = this.shipPrefabs[this.selectedShipIndex].shipName;
            this.shipDescriptionText.text = this.shipPrefabs[this.selectedShipIndex].shipDescription;

            this.healthSlider.value = this.shipPrefabs[this.selectedShipIndex].health;
            this.armorSlider.value = this.shipPrefabs[this.selectedShipIndex].armor;
            this.maneuverabilitySlider.value = this.shipPrefabs[this.selectedShipIndex].maneuvarability;
            this.damageSlider.value = this.shipPrefabs[this.selectedShipIndex].damage;
            this.attackSpeedSlider.value = this.shipPrefabs[this.selectedShipIndex].attackSpeed;
        }
        

        //Creating an instance of the prefab that we'll be selecting
        GameObject newShip = GameObject.Instantiate(this.shipPrefabs[this.selectedShipIndex].shipPrefab.gameObject);

        //Setting the new ship as the displayed ship and the currently displayed ship as the transition ship
        this.transitionShip = this.displayedShip;
        this.displayedShip = newShip;


        //If we're transitioning left, we need to change our selected ship index down
        if (this.transitioningLeft)
        {
            this.selectedShipIndex -= 1;

            //If we go below index 0, we need to loop back around
            if(this.selectedShipIndex < 0)
            {
                this.selectedShipIndex = this.shipPrefabs.Count - 1;
            }

            //Setting the new ship's position to the left display position
            newShip.transform.position = this.leftTransitionPos.position;
        }
        //If we're transitioning right, we need to change our selected ship index up
        else
        {
            this.selectedShipIndex += 1;

            //If we go above the last index, we need to loop back around
            if (this.selectedShipIndex > this.shipPrefabs.Count - 1)
            {
                this.selectedShipIndex = 0;
            }

            //Setting the new ship's position to the right display position
            newShip.transform.position = this.rightTransitionPos.position;
        }
    }
}


//Class used by ShipSelectLogic.cs to hold info for each selectable player ship
[System.Serializable]
public class SelectableShipInfo
{
    //Bool for if this ship is locked off
    public bool locked = false;

    //Gameplay prefab for this ship
    public PlayerShipController shipPrefab;
    //The name of this ship
    public string shipName;
    //String for the description of this ship
    public string shipDescription;

    //Sliders for ship maneuverability, armor, health, damage and attack speed
    [Range(0, 1)]
    public float health = 1;
    [Range(0, 1)]
    public float armor = 1;
    [Range(0, 1)]
    public float maneuvarability = 1;
    [Range(0, 1)]
    public float damage = 1;
    [Range(0, 1)]
    public float attackSpeed = 1;
}
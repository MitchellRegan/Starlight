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

    [Space(16)]

    //The index of the currently selected ship
    private int selectedShipIndex = 0;


    //The delay between switching between ships
    public float changeShipDelay = 0.4f;
    //The current delay time between switching ships
    private float currentDelayTime = 0;

    //The size of the display ship
    public Vector3 displayShipScale = new Vector3(1, 1, 1);
    //The size of the ships when they first transition in
    public Vector3 transitionShipScale = new Vector3(0.5f, 0.5f, 0.5f);

    //The speed that the stat sliders transition to the correct value
    [Range(0.01f, 0.99f)]
    public float sliderTransitionSpeed = 0.9f;

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
                    break;
                }
            }
            //If we're checking the prefab for player 2
            else
            {
                //If the ship prefab at this index matches the designated p2 ship in GlobalData, we need to display it
                if (this.shipPrefabs[i].shipPrefab == GlobalData.globalReference.player2Ship)
                {
                    this.selectedShipIndex = i;
                    break;
                }
            }
        }

        //We hide the locked screen icon
        this.lockedScreenObj.SetActive(false);


        //Creating an instance of the prefab that we'll be selecting
        GameObject newShip = GameObject.Instantiate(this.shipPrefabs[this.selectedShipIndex].shipPrefab.gameObject);

        //Applying the ship's textures for the model
        newShip.GetComponent<CustomShipTextures>().SetPlayerShipID(this.player);

        //Disabling specific components on the spawned ship so it doesn't fly off...
        newShip.GetComponent<PlayerShipController>().enabled = false;
        newShip.GetComponent<CameraWeight>().enabled = false;
        newShip.GetComponent<RailMovementFlight>().enabled = false;
        newShip.GetComponent<FreeMovementFlight>().enabled = false;
        newShip.GetComponent<ShipEnergy>().enabled = false;
        newShip.GetComponent<ShipTiltAndRoll>().enabled = false;
        newShip.GetComponent<RailMovementFlight>().railParentObj.gameObject.SetActive(false);
        newShip.GetComponent<ShipTiltAndRoll>().rollTrails.SetActive(false);

        //Setting the display position and rotation of the display ship
        newShip.transform.position = this.shipDisplayPos.transform.position;
        newShip.transform.rotation = this.shipDisplayPos.transform.rotation;
        newShip.transform.localScale = this.displayShipScale;

        //Activating the displayed ship after the components are disabled so we don't get errors on Awake or Start
        newShip.gameObject.SetActive(true);
        this.displayedShip = newShip;

        //Displaying the ship's name and all of the other stats
        this.shipNameText.text = this.shipPrefabs[this.selectedShipIndex].shipName;
        this.shipDescriptionText.text = this.shipPrefabs[this.selectedShipIndex].shipDescription;

        this.healthSlider.value = this.shipPrefabs[this.selectedShipIndex].health;
        this.armorSlider.value = this.shipPrefabs[this.selectedShipIndex].armor;
        this.maneuverabilitySlider.value = this.shipPrefabs[this.selectedShipIndex].maneuvarability;
        this.damageSlider.value = this.shipPrefabs[this.selectedShipIndex].damage;
        this.attackSpeedSlider.value = this.shipPrefabs[this.selectedShipIndex].attackSpeed;
    }


    //Function called when this object is disabled
    public void OnDisable()
    {
        //If our display ship isn't null, we destroy it
        if(this.displayedShip != null)
        {
            Destroy(this.displayedShip);
        }

        //If our transition ship isn't null, we destroy it
        if(this.transitionShip != null)
        {
            Destroy(this.transitionShip);
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
                newTransitionPos += this.rightTransitionPos.position;

                //Getting the position for the displayed ship to come on screen
                Vector3 newDisplayPos = this.leftTransitionPos.position - this.shipDisplayPos.position;
                newDisplayPos *= percent;
                newDisplayPos += this.shipDisplayPos.position;

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
                newTransitionPos += this.leftTransitionPos.position;

                //Getting the position for the displayed ship to come on screen
                Vector3 newDisplayPos = this.rightTransitionPos.position - this.shipDisplayPos.position;
                newDisplayPos *= percent;
                newDisplayPos += this.shipDisplayPos.position;

                //Setting the positions of the transition and display ships
                this.transitionShip.transform.position = newTransitionPos;
                this.displayedShip.transform.position = newDisplayPos;
            }

            //Finding the correct scale for the display ship
            Vector3 newDisplayScale = (percent * (this.transitionShipScale - this.displayShipScale)) + this.displayShipScale;
            //Finding the correct scale for the transition ship
            Vector3 newTransitionScale = (percent * (this.displayShipScale - this.transitionShipScale)) + this.transitionShipScale;

            //Setting the new ship scales
            this.displayedShip.transform.localScale = newDisplayScale;
            this.transitionShip.transform.localScale = newTransitionScale;


            //If the current ship isn't locked, we interpolate all of the stat sliders to the correct values
            if(!this.shipPrefabs[this.selectedShipIndex].locked)
            {
                this.healthSlider.value = (this.sliderTransitionSpeed * (this.shipPrefabs[this.selectedShipIndex].health - this.healthSlider.value)) + this.healthSlider.value;
                this.armorSlider.value = (this.sliderTransitionSpeed * (this.shipPrefabs[this.selectedShipIndex].armor - this.armorSlider.value)) + this.armorSlider.value;
                this.maneuverabilitySlider.value = (this.sliderTransitionSpeed * (this.shipPrefabs[this.selectedShipIndex].maneuvarability - this.maneuverabilitySlider.value)) + this.maneuverabilitySlider.value;
                this.damageSlider.value = (this.sliderTransitionSpeed * (this.shipPrefabs[this.selectedShipIndex].damage - this.damageSlider.value)) + this.damageSlider.value;
                this.attackSpeedSlider.value = (this.sliderTransitionSpeed * (this.shipPrefabs[this.selectedShipIndex].attackSpeed - this.attackSpeedSlider.value)) + this.attackSpeedSlider.value;
            }
            //If the current ship is locked, we interpolate all of the stat sliders to 0
            else
            {
                this.healthSlider.value *= (1 - this.sliderTransitionSpeed);
                this.armorSlider.value *= (1 - this.sliderTransitionSpeed);
                this.maneuverabilitySlider.value *= (1 - this.sliderTransitionSpeed);
                this.damageSlider.value *= (1 - this.sliderTransitionSpeed);
                this.attackSpeedSlider.value *= (1 - this.sliderTransitionSpeed);
            }


            //If the time is 0, we delete the transition ship object and make sure the display ship is in the right position
            if(this.currentDelayTime <= 0)
            {
                this.displayedShip.transform.position = this.shipDisplayPos.transform.position;
                this.displayedShip.transform.localScale = this.displayShipScale;
                Destroy(this.transitionShip);

                //If the currently selected ship is locked, we set all the slider values to 0
                if (this.shipPrefabs[this.selectedShipIndex].locked)
                {
                    this.healthSlider.value = 0;
                    this.armorSlider.value = 0;
                    this.maneuverabilitySlider.value = 0;
                    this.damageSlider.value = 0;
                    this.attackSpeedSlider.value = 0;
                }
                //If the currently selected ship is unlocked, we make sure all the slider values are correct
                else
                {
                    this.healthSlider.value = this.shipPrefabs[this.selectedShipIndex].health;
                    this.armorSlider.value = this.shipPrefabs[this.selectedShipIndex].armor;
                    this.maneuverabilitySlider.value = this.shipPrefabs[this.selectedShipIndex].maneuvarability;
                    this.damageSlider.value = this.shipPrefabs[this.selectedShipIndex].damage;
                    this.attackSpeedSlider.value = this.shipPrefabs[this.selectedShipIndex].attackSpeed;
                }
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
                    //Making sure the selected ship isn't locked
                    if (!this.shipPrefabs[this.selectedShipIndex].locked)
                    {
                        //Making it so we stop selecting ships
                        this.isChangingShip = false;

                        //Invoking our confirm selection event
                        this.confirmSelectEvent.Invoke();
                    }
                }
                //Checking for input to move left
                else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || ControllerInputManager.P1Controller.CheckButtonDown(ControllerButtons.D_Pad_Left) ||
                    ControllerInputManager.P1Controller.CheckStickValue(ControllerSticks.Left_Stick_X) > 0.6f)
                {
                    this.StartTransition(true);
                }
                //Checking for input to move right
                else if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || ControllerInputManager.P1Controller.CheckButtonDown(ControllerButtons.D_Pad_Right) ||
                    ControllerInputManager.P1Controller.CheckStickValue(ControllerSticks.Left_Stick_X) < -0.6f)
                {
                    this.StartTransition(false);
                }
            }
            //Getting the input for player 2
            else
            {
                //If the player hits the "select" button we need to invoke the unity event to go back to the options menu
                if (ControllerInputManager.P2Controller.CheckButtonPressed(ControllerButtons.A_Button))
                {
                    //Making sure the selected ship isn't locked
                    if (!this.shipPrefabs[this.selectedShipIndex].locked)
                    {
                        //Making it so we stop selecting ships
                        this.isChangingShip = false;

                        //Invoking our confirm selection event
                        this.confirmSelectEvent.Invoke();
                    }
                }
                //Checking for input to move left
                else if (ControllerInputManager.P2Controller.CheckButtonDown(ControllerButtons.D_Pad_Left) ||
                    ControllerInputManager.P2Controller.CheckStickValue(ControllerSticks.Left_Stick_X) > 0.6f)
                {
                    this.StartTransition(true);
                }
                //Checking for input to move right
                else if (ControllerInputManager.P2Controller.CheckButtonDown(ControllerButtons.D_Pad_Right) ||
                    ControllerInputManager.P2Controller.CheckStickValue(ControllerSticks.Left_Stick_X) < -0.6f)
                {
                    this.StartTransition(false);
                }
            }
        }
	}


    //Function called externally to start the ship selection
    public void StartShipSelection()
    {
        this.isChangingShip = true;
    }


    //Function called from update to start the transition to the next ship
    public void StartTransition(bool transitionLeft_)
    {
        //If we're already transitioning, nothing happens
        if(this.currentDelayTime > 0)
        {
            return;
        }

        //Setting the delay time to prevent player input
        this.currentDelayTime = this.changeShipDelay;

        //Setting the transition direction
        this.transitioningLeft = transitionLeft_;


        //If we're transitioning left, we need to change our selected ship index down
        if (this.transitioningLeft)
        {
            this.selectedShipIndex -= 1;

            //If we go below index 0, we need to loop back around
            if (this.selectedShipIndex < 0)
            {
                this.selectedShipIndex = this.shipPrefabs.Count - 1;
            }
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
        }

        //If the currently selected ship is locked
        if (this.shipPrefabs[this.selectedShipIndex].locked)
        {
            //We display the locked screen icon
            this.lockedScreenObj.SetActive(true);

            //Displaying the ship's name and hiding the description
            this.shipNameText.text = this.shipPrefabs[this.selectedShipIndex].shipName;
            this.shipDescriptionText.text = "??????????";
        }
        //If the currently selected ship is unlocked
        else
        {
            //We hide the locked screen icon
            this.lockedScreenObj.SetActive(false);

            //Displaying the ship's name and the description
            this.shipNameText.text = this.shipPrefabs[this.selectedShipIndex].shipName;
            this.shipDescriptionText.text = this.shipPrefabs[this.selectedShipIndex].shipDescription;
        }
        

        //Creating an instance of the prefab that we'll be selecting
        GameObject newShip = GameObject.Instantiate(this.shipPrefabs[this.selectedShipIndex].shipPrefab.gameObject);

        //Applying the ship's textures for the model
        newShip.GetComponent<CustomShipTextures>().SetPlayerShipID(this.player);

        //Disabling the player ship controller component
        newShip.GetComponent<PlayerShipController>().enabled = false;

        //Disabling specific components on the spawned ship so it doesn't fly off...
        newShip.GetComponent<PlayerShipController>().enabled = false;
        newShip.GetComponent<CameraWeight>().enabled = false;
        newShip.GetComponent<RailMovementFlight>().enabled = false;
        newShip.GetComponent<FreeMovementFlight>().enabled = false;
        newShip.GetComponent<ShipEnergy>().enabled = false;
        newShip.GetComponent<ShipTiltAndRoll>().enabled = false;
        newShip.GetComponent<RailMovementFlight>().railParentObj.gameObject.SetActive(false);
        newShip.GetComponent<ShipTiltAndRoll>().rollTrails.SetActive(false);

        //Setting the ship's rotation and scale to the correct value to begin the transition
        newShip.transform.rotation = this.shipDisplayPos.transform.rotation;
        newShip.transform.localScale = this.transitionShipScale;

        //Enabling the game object after all of the correct components are disabled so we don't get errors on Awake or Start
        newShip.gameObject.SetActive(true);

        //Setting the new ship as the displayed ship and the currently displayed ship as the transition ship
        this.transitionShip = this.displayedShip;
        this.displayedShip = newShip;

        //If we transition left, we set the new ship's position to the left position pos
        if(this.transitioningLeft)
        {
            //Setting the new ship's position to the left display position
            newShip.transform.position = this.leftTransitionPos.position;
        }
        //If we transition right, we set the new ship's position to the right position pos
        else
        {
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
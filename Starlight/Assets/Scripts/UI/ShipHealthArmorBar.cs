using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipHealthArmorBar : MonoBehaviour
{
    //Enum to determine which player we're displaying the health of
    public Players playerID = Players.P1;
    //The reference to the health and armor component that this script tracks
    private HealthAndArmor ourHealthAndArmor;

    //The reference to the slider component to display health
    public Slider healthSlider;
    //The reference to the slider component to display shilds
    public Slider shieldSlider;



    // Use this for initialization
    private void Start()
    {
        //Getting the reference to the health and armor component for our player
        switch (this.playerID)
        {
            case Players.P1:
                this.ourHealthAndArmor = PlayerShipController.p1ShipRef.ourHealth;
                break;

            case Players.P2:
                this.ourHealthAndArmor = PlayerShipController.p2ShipRef.ourHealth;
                break;

            default:
                this.ourHealthAndArmor = PlayerShipController.p1ShipRef.ourHealth;
                break;
        }

        this.UpdateSliders();
    }


    // Update is called once per frame
    private void Update()
    {
        //Making sure our sliders show the accurate values
        this.UpdateSliders();
    }


    //Function called from Start and Update to make sure our sliders are accurate
    private void UpdateSliders()
    {
        //Setting the values of our sliders
        this.healthSlider.maxValue = this.ourHealthAndArmor.maxHealth;
        this.healthSlider.value = this.ourHealthAndArmor.currentHealth;

        this.shieldSlider.maxValue = this.ourHealthAndArmor.maxShield;
        this.shieldSlider.value = this.ourHealthAndArmor.currentShields;
    }
}
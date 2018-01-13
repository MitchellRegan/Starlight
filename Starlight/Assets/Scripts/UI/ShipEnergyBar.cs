using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipEnergyBar : MonoBehaviour
{
    //Enum to determine which player we're displaying the health of
    public Players playerID = Players.P1;
    //The ship energy component that this script tracks
    private ShipEnergy ourEnergy;

    //The list of sliders that we update to show the energy amount
    public List<Slider> energySliders;



    // Use this for initialization
    private void Start()
    {
        //Getting the reference to the health and armor component for our player
        switch (this.playerID)
        {
            case Players.P1:
                this.ourEnergy = PlayerShipController.p1ShipRef.ourEnergy;
                break;

            case Players.P2:
                this.ourEnergy = PlayerShipController.p2ShipRef.ourEnergy;
                break;

            default:
                this.ourEnergy = PlayerShipController.p1ShipRef.ourEnergy;
                break;
        }

        //Looping through all of our energy sliders to make sure they show the correct max value
        foreach(Slider energySlider in this.energySliders)
        {
            energySlider.maxValue = this.ourEnergy.maxEnergy;
        }
    }


    // Update is called once per frame
    private void Update ()
    {
		//Looping through all of our energy sliders to show their current value
        foreach(Slider energySlider in this.energySliders)
        {
            energySlider.value = this.ourEnergy.currentEnergy;
        }
	}
}

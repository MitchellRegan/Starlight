using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipEnergy : MonoBehaviour
{
    //Float for the maximum amount of energy this ship has
    public float maxEnergy = 100;
    //Float to hold the current amount of energy we have
    public float currentEnergy = 0;

    //The length of time before we can start recharging energy
    public float rechargeDelay = 1.5f;
    //The current time we're waiting before recharging
    private float currentRechargeTime = 0;
    //Bool that determines if our energy is COMPLETELY depleted and must fully recharge before being used
    private bool fullyDepleted = false;

    //The amount of energy restored each frame while recharging
    public float rechargePerFrame = 1;
    


    //Function called on initialization
    private void Awake()
    {
        //Setting our current recharge time to full so it charges at the start of the level
        this.currentRechargeTime = this.rechargeDelay;
    }

	
	// Update is called once per frame
	private void Update ()
    {
        //If the game is paused, nothing happens
        if(GlobalData.globalReference.isGamePaused)
        {
            return;
        }

		//If we're waiting to recharge, we reduce our recharge time
        if(this.currentRechargeTime > 0)
        {
            this.currentRechargeTime -= Time.deltaTime;
        }
        //Otherwise, if our current energy level isn't at max, we increase it
        else if(this.currentEnergy < this.maxEnergy)
        {
            //Adding our recharge per frame to our current energy amount
            this.currentEnergy += this.rechargePerFrame;

            //Making sure our current energy doesn't exceed our max
            if(this.currentEnergy >= this.maxEnergy)
            {
                this.currentEnergy = this.maxEnergy;

                //If we were fully depleted, we let the player use energy again
                this.fullyDepleted = false;
            }
        }
	}


    //Function called externally to use energy from a separate script. Returns false if there's not enough
    public bool CanUseEnergy(float energyToUse_)
    {
        //If we have enough energy for the amount asked and we aren't fully depleted, we use it
        if(!this.fullyDepleted && this.currentEnergy >= energyToUse_)
        {
            //Removing all of the energy that was asked for
            this.currentEnergy -= energyToUse_;
            //Refreshing our recharge time to full
            this.currentRechargeTime = this.rechargeDelay;

            //If our energy is too low, we make the player wait until we're back at full energy before they can use it
            if(this.currentEnergy < 1)
            {
                this.fullyDepleted = true;
            }

            //Returning true so the script that asked for energy knows that it will get it
            return true;
        }

        //If we don't have enough energy for the amount asked, we return false
        return false;
    }
}

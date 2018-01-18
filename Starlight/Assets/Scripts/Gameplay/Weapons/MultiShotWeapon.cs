using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiShotWeapon : Weapon
{
    //The current amount of time we're waiting for cooldowns
    private float currentCooldown = 0;

    //The extra muzzles that projectiles are fired from
    public List<ExtraSoundEmitterSettings> extraMuzzles;

    [Space(8)]

    //Bool to determine if we alternate between muzzles or fire from all at once
    public bool alternateMuzzles = true;
    //Int to track which muzzle we're currently on
    private int currentMuzzleIndex = 0;

    [Space(8)]

    //Bool to determine if we keep firing while held
    public bool continuousFire = false;



    //Function called every frame
    private void Update()
    {
        //If our current cooldown time is above 0, we reduce the time remaining
        if (this.currentCooldown > 0)
        {
            this.currentCooldown -= Time.deltaTime;
        }
    }


    //Function called externally to perform the main fire
    public override void FireWeapon(bool pressed_, bool held_, bool released_)
    {
        //If we're not pressing the fire button OR if we use continuous fire and the fire button isn't held, nothing happens
        if ((!pressed_ && !this.continuousFire) || (!held_ && this.continuousFire))
        {
            return;
        }

        //If our current cooldown time is above 0 or there's no ammo, we can't fire
        if (this.currentCooldown > 0 || (this.currentAmmo <= 0 && !this.unlimitedAmmo))
        {
            return;
        }

        //If we don't alternate firing from different muzzles, we fire from all of them
        if (!this.alternateMuzzles)
        {
            //First, we create an instance of the fired projectile at our default muzzle location
            GameObject projectile = GameObject.Instantiate(this.firedProjectile.gameObject, this.muzzleAudio.transform.position, this.muzzleAudio.transform.rotation);
            //Setting the projectile's fire data
            projectile.GetComponent<WeaponProjectile>().SetProjectileInfo(this.objectIDType);

            //Then we loop through all of the extra muzzles to fire from each of them
            foreach(ExtraSoundEmitterSettings muzzle in this.extraMuzzles)
            {
                //Creating an instance of the fired projectile at our current extra muzzle location
                GameObject newProjectile = GameObject.Instantiate(this.firedProjectile.gameObject, muzzle.transform.position, muzzle.transform.rotation);
                //Setting the projectile's fire data
                newProjectile.GetComponent<WeaponProjectile>().SetProjectileInfo(this.objectIDType);
            }
        }
        //If we do alternate firing from different muzzles, we go in order
        else
        {
            //Finding the muzzle that we fire from this time
            GameObject currentMuzzle;

            //If our current muzzle index is within the length of our list of extra muzzles
            if(this.currentMuzzleIndex < this.extraMuzzles.Count)
            {
                currentMuzzle = this.extraMuzzles[this.currentMuzzleIndex].gameObject;
            }
            //Otherwise we use the default muzzle
            else
            {
                currentMuzzle = this.muzzleAudio.gameObject;
            }

            //Creating an instance of the fired projectile at our current extra muzzle location
            GameObject newProjectile = GameObject.Instantiate(this.firedProjectile.gameObject, currentMuzzle.transform.position, currentMuzzle.transform.rotation);
            //Setting the projectile's fire data
            newProjectile.GetComponent<WeaponProjectile>().SetProjectileInfo(this.objectIDType);

            //Moving to the next muzzle in line
            this.currentMuzzleIndex += 1;
            if(this.currentMuzzleIndex > this.extraMuzzles.Count)
            {
                this.currentMuzzleIndex = 0;
            }
        }

        

        //Setting our weapon cooldown
        this.currentCooldown = this.weaponCooldown;

        //Playing the muzzle's audio source
        this.muzzleAudio.ownerAudio.Play();

        //Subtracting from our current ammo supply (if it's not unlimited)
        if (!this.unlimitedAmmo)
        {
            this.currentAmmo -= 1;
        }
    }


    //Function called externally to add ammo to this weapon
    public override void RefillAmmo(int amountToAdd_)
    {
        //Adding the amount to our current ammo supply
        this.currentAmmo += amountToAdd_;

        //If we have more ammo than our max allows, we set it to the max
        if (this.currentAmmo > this.maxAmmo)
        {
            this.currentAmmo = this.maxAmmo;
        }
    }
}

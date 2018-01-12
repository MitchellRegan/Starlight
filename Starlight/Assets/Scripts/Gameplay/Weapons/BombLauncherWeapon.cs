using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombLauncherWeapon : Weapon
{
    //Reference to the bomb projectile this weapon launches
    private BombProjectile ourBomb = null;
    //The current amount of time we're waiting for cooldowns
    private float currentCooldown = 0;


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
        //If we're not pressing the fire button, nothing happens
        if (!pressed_)
        {
            return;
        }

        //If our current cooldown time is above 0 or there's no ammo, we can't fire
        if (this.ourBomb == null)
        {
            if (this.currentCooldown > 0 || (this.currentAmmo <= 0 && !this.unlimitedAmmo))
            {
                return;
            }
        }

        //If we don't already have a bomb launched, we fire one
        if (this.ourBomb == null)
        {
            //Otherwise we create an instance of the fired projectile at our muzzle location
            GameObject projectile = GameObject.Instantiate(this.firedProjectile.gameObject, this.muzzleAudio.transform.position, this.muzzleAudio.transform.rotation);

            //Setting the projectile's fire data
            projectile.GetComponent<WeaponProjectile>().SetProjectileInfo(this.objectIDType);

            //If the projectile we fire is a bomb, we save the component reference
            if(projectile.GetComponent<BombProjectile>())
            {
                this.ourBomb = projectile.GetComponent<BombProjectile>();
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
        //If we already have a bomb launched, we detonate it
        else
        {
            this.ourBomb.DetonateBomb();
            this.ourBomb = null;
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

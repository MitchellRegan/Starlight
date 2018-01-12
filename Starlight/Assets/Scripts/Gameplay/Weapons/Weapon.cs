using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    //The attacker ID
    [HideInInspector]
    public AttackerID objectIDType = AttackerID.Enemy;

    //The projectile that's fired by this weapon's main fire
    public WeaponProjectile firedProjectile;

    //The cooldown after this weapon fires the main projectile
    public float weaponCooldown = 0.5f;
    //The current amount of time we're waiting for cooldowns
    private float currentCooldownTime = 0;

    //The maximum amount of ammo this weapon starts with
    public int maxAmmo = 5;
    //The current amount of ammo this weapon has
    public int currentAmmo = 0;
    //If true, this weapon has unlimited ammo
    public bool unlimitedAmmo = false;

    //The audio emitter that is played when this weapon is fired
    public ExtraSoundEmitterSettings muzzleAudio;




    //Function called every frame
    private void Update()
    {
        //If our current cooldown time is above 0, we reduce the time remaining
        if(this.currentCooldownTime > 0)
        {
            this.currentCooldownTime -= Time.deltaTime;
        }
    }


    //Function called externally to perform the main fire
    public virtual void FireWeapon(bool pressed_, bool held_, bool released_)
    {
        //If we're not pressing the fire button, nothing happens
        if(!pressed_)
        {
            return;
        }

        //If our current cooldown time is above 0 or there's no ammo, we can't fire
        if (this.currentCooldownTime > 0 || (this.currentAmmo <= 0 && !this.unlimitedAmmo))
        {
            return;
        }

        //Otherwise we create an instance of the fired projectile at our muzzle location
        GameObject projectile = GameObject.Instantiate(this.firedProjectile.gameObject, this.muzzleAudio.transform.position, this.muzzleAudio.transform.rotation);

        //Setting the projectile's fire data
        projectile.GetComponent<WeaponProjectile>().SetProjectileInfo(this.objectIDType);

        //Setting our weapon cooldown
        this.currentCooldownTime = this.weaponCooldown;

        //Playing the muzzle's audio source
        this.muzzleAudio.ownerAudio.Play();

        //Subtracting from our current ammo supply (if it's not unlimited)
        if(!this.unlimitedAmmo)
        {
            this.currentAmmo -= 1;
        }
    }

    
    //Function called externally to add ammo to this weapon
    public void RefillAmmo(int amountToAdd_)
    {
        //Adding the amount to our current ammo supply
        this.currentAmmo += amountToAdd_;

        //If we have more ammo than our max allows, we set it to the max
        if(this.currentAmmo > this.maxAmmo)
        {
            this.currentAmmo = this.maxAmmo;
        }
    }
}
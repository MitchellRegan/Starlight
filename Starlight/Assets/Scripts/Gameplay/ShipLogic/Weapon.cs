using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    //The projectile that's fired by this weapon's main fire
    public GameObject firedProjectile;

    //The cooldown after this weapon fires the main projectile
    public float weaponCooldown = 0.5f;

    //The audio emitter that is played when this weapon is fired
    public AudioSource muzzleAudio;

    //The current amount of time we're waiting for cooldowns
    private float currentCooldownTime = 0;



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
    public virtual void FireMainWeapon()
    {
        //If our current cooldown time is above 0, we can't fire
        if(this.currentCooldownTime > 0)
        {
            return;
        }

        //Otherwise we create an instance of the fired projectile at our muzzle location
        GameObject projectile = GameObject.Instantiate(this.firedProjectile, this.muzzleAudio.transform.position, this.muzzleAudio.transform.rotation);

        //Setting our weapon cooldown
        this.currentCooldownTime = this.weaponCooldown;

        //Playing the muzzle's audio source
        this.muzzleAudio.Play();
    }
}
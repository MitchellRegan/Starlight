using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthAndArmor : MonoBehaviour
{
    //This object's ID type so projectiles know if they're hitting a player or an enemy
    [HideInInspector]
    public AttackerID objectIDType = AttackerID.Enemy;

    //The maximum health that this object has
    public int maxHealth = 100;
    //The current amount of health this object has
    public int currentHealth = 100;
    //Bool that determines if this object is invulnerable
    public bool isInvulnerable = false;

    [Space(8)]

    //The maximum shield value this object has
    public int maxShield = 100;
    //The current amount of shields this object has
    public int currentShields = 0;

    [Space(8)]

    //The Unity Event called when this object dies
    public UnityEvent onDeathEvent;



    //Function called externally to heal this object
    public virtual void RestoreHealth(int amountToRestore_)
    {
        //If the amount is less than 1, nothing happens because we shouldn't deal damage
        if(amountToRestore_ < 1)
        {
            return;
        }

        //Adding the amount to our current health
        this.currentHealth += amountToRestore_;

        //If we've gone over our maximum health, we cap it off
        if(this.currentHealth > this.maxHealth)
        {
            this.currentHealth = this.maxHealth;
        }
    }


    //Function called externally to replenish shields on this object
    public virtual void RestoreShields(int amountToRestore_)
    {
        //If the amount is less than 1, nothing happens because we shouldn't deal damage
        if (amountToRestore_ < 1)
        {
            return;
        }

        //Adding the amount to our current shields
        this.currentShields += amountToRestore_;

        //If we've gone over our maximum shields, we cap it off
        if (this.currentShields > this.maxShield)
        {
            this.currentShields = this.maxShield;
        }
    }


    //Function called externally to damage this object
    public virtual void DealDamage(int amountOfDamage_)
    {
        //If the amount is less than 1 or we're invulnerable, nothing happens
        if(amountOfDamage_ < 1 || this.isInvulnerable)
        {
            return;
        }

        //If our shields can absorb all of the damage, they do
        if(this.currentShields >= amountOfDamage_)
        {
            this.currentShields -= amountOfDamage_;
        }
        //If our shields can absorb only some of the damage, some damage is dealt to health
        else
        {
            int damage = amountOfDamage_;
            //Depleting damage using the shields and reducing them to 0
            damage -= this.currentShields;
            this.currentShields = 0;
            //Dealing the rest of the damage to health
            this.currentHealth -= damage;
        }

        //If this object has no health left, it is dead
        if(this.currentHealth < 1)
        {
            this.currentHealth = 0;
            //Invoking our unity death event
            this.onDeathEvent.Invoke();
        }
    }


    //Function that can be called externally (possibly by our onDeathEvent) to destroy this game object
    public void DestroyThisObject()
    {
        Destroy(this.gameObject);
    }
}

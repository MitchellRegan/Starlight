using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class WeaponProjectile : MonoBehaviour
{
    //Enum for the ID of the character that fired this projectile
    [HideInInspector]
    public AttackerID attackerID = AttackerID.Enemy;

    //The damage that's inflicted to the target this projectile hits
    public int damageDealt = 10;
    //The forward velocity of this projectile
    public float forwardVelocity = 10;
    //Bool for if this attack causes friendly fire
    public bool causeFriendlyFire = false;

    //The length of time this projectile is alive before dying
    public float lifetime = 5;
    
	
    
    //Function called externally from Weapon.cs to set this projectile's attacker ID and forward direction
    public virtual void SetProjectileInfo(AttackerID projectileID_)
    {
        //Setting the ID of the attacker that fired this projectile
        this.attackerID = projectileID_;
        //Setting our velocity
        this.GetComponent<Rigidbody>().velocity = this.gameObject.transform.forward * this.forwardVelocity;
    }


    //Function called every frame
    private void Update()
    {
        //Reducing our lifetime
        this.lifetime -= Time.deltaTime;

        //If our lifetime goes below 0, we destroy this object
        if(this.lifetime <= 0)
        {
            Destroy(this.gameObject);
        }
    }


    //Function called when this object's collider hits something
    private void OnCollisionStart(Collider collider_)
    {
        //If the object we hit has a health and armor component, we might be able to damage it
        if (collider_.gameObject.GetComponent<HealthAndArmor>())
        {
            //We damage the object if friendly fire is on or it has a different ID from this projectile's attacker
            if (this.causeFriendlyFire || this.attackerID != collider_.gameObject.GetComponent<HealthAndArmor>().objectIDType)
            {
                collider_.gameObject.GetComponent<HealthAndArmor>().DealDamage(this.damageDealt);
                Destroy(this.gameObject);
            }
        }
        //If we hit something without a health and armor component, this is destroyed
        else
        {
            Destroy(this.gameObject);
        }
    }


    //Function called when this object's collider is triggered by something
    private void OnTriggerEnter(Collider collider_)
    {
        //If the object we hit has a health and armor component, we might be able to damage it
        if (collider_.gameObject.GetComponent<HealthAndArmor>())
        {
            //We damage the object if friendly fire is on or it has a different ID from this projectile's attacker
            if (this.causeFriendlyFire || this.attackerID != collider_.gameObject.GetComponent<HealthAndArmor>().objectIDType)
            {
                collider_.gameObject.GetComponent<HealthAndArmor>().DealDamage(this.damageDealt);
                Destroy(this.gameObject);
            }
        }
    }
}


//Enum used in WeaponProjectile.cs and HealthAndArmor.cs to determine if something is a player or an enemy
public enum AttackerID
{
    Player1,
    Player2,
    Enemy
};
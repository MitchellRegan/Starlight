using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombProjectile : WeaponProjectile
{
    //The explosion game object spawned when this bomb explodes
    public ExplosionLogic explosionToSpawn;



    //Function inherited from WeaponProjectile.cs to set this projectile's attack ID
    public override void SetProjectileInfo(AttackerID projectileID_)
    {
        base.SetProjectileInfo(projectileID_);
    }


    // Update is called once per frame
    private void Update ()
    {
        //Reducing our lifetime
        this.lifetime -= Time.deltaTime;

        //If our lifetime goes below 0, we detonate this bomb
        if(this.lifetime <= 0)
        {
            this.DetonateBomb();
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
                collider_.gameObject.GetComponent<HealthAndArmor>().DealDamage(this.damageDealt, this.ignoreIFrames);
            }
        }

        //If we hit something, this bomb is detonated
        this.DetonateBomb();
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
                collider_.gameObject.GetComponent<HealthAndArmor>().DealDamage(this.damageDealt, this.ignoreIFrames);
                this.DetonateBomb();
            }
        }
        //If the object we hit has an explosion logic component, we detonate this bomb
        else if(!collider_.isTrigger || collider_.gameObject.GetComponent<ExplosionLogic>())
        {
            this.DetonateBomb();
        }
    }


    //Function called to destroy this bomb and cause an explosion
    public void DetonateBomb()
    {
        //Creating our explosion object on our position and then destroying this bomb
        GameObject explosion = GameObject.Instantiate(this.explosionToSpawn.gameObject, this.transform.position, new Quaternion());
        explosion.GetComponent<ExplosionLogic>().attackerID = this.attackerID;
        Destroy(this.gameObject);
    }
}

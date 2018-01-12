using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombProjectile : WeaponProjectile
{
    //Reference to the ship of the player who fired this bomb
    private PlayerShipController ourShip;
    //The controller and keyboard buttons that are used to detonate this bomb prematurely
    private ControllerButtons detonateButton_Controller = ControllerButtons.A_Button;
    private KeyCode detonateButton_Keyboard = KeyCode.Mouse0;

    //The explosion game object spawned when this bomb explodes
    public ExplosionLogic explosionToSpawn;



    //Function inherited from WeaponProjectile.cs to set this projectile's attack ID
    public override void SetProjectileInfo(AttackerID projectileID_)
    {
        base.SetProjectileInfo(projectileID_);

        //Finding our detonation buttons
        this.FindDetonateButtons();
    }


    //Function called from SetProjectileInfo to find out which buttons will detonate our bomb
    private void FindDetonateButtons()
    {
        //If our attacker ID is player 1, we get the reference to the player 1 ship
        if (this.attackerID == AttackerID.Player1)
        {
            this.ourShip = PlayerShipController.p1ShipRef;
        }
        //If our attacker ID is player 2, we get the reference to the player 2 ship
        else if (this.attackerID == AttackerID.Player2)
        {
            this.ourShip = PlayerShipController.p2ShipRef;
        }

        //If our ship's main weapon fires this bomb
        if(this.ourShip.mainWeapon.firedProjectile.GetType() == this.GetType())
        {
            this.detonateButton_Controller = this.ourShip.ourCustomInputs.mainFireButton_Controller;
            this.detonateButton_Keyboard = this.ourShip.ourCustomInputs.mainFireButton_Keyboard;
        }
        //If our ship's secondary weapon fires this bomb
        else if(this.ourShip.secondaryWeapon.firedProjectile.GetType() == this.GetType())
        {
            this.detonateButton_Controller = this.ourShip.ourCustomInputs.secondaryFireButton_Controller;
            this.detonateButton_Keyboard = this.ourShip.ourCustomInputs.secondaryFireButton_Keyboard;
        }
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
            return;
        }

        //If the player presses the detonate button, we detonate
        if(this.ourShip.ourController.CheckButtonPressed(this.detonateButton_Controller) || Input.GetKeyDown(this.detonateButton_Keyboard))
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
                collider_.gameObject.GetComponent<HealthAndArmor>().DealDamage(this.damageDealt);
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
                collider_.gameObject.GetComponent<HealthAndArmor>().DealDamage(this.damageDealt);
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
    private void DetonateBomb()
    {
        //Creating our explosion object on our position and then destroying this bomb
        GameObject explosion = GameObject.Instantiate(this.explosionToSpawn.gameObject, this.transform.position, new Quaternion());
        explosion.GetComponent<ExplosionLogic>().attackerID = this.attackerID;
        Destroy(this.gameObject);
    }
}

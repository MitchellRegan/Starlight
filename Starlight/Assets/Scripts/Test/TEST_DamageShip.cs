using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerShipController))]
public class TEST_DamageShip : MonoBehaviour
{
    //The reference to the player ship to damage
    private PlayerShipController ourShip;

    //The button to press to deal damage
    public KeyCode damageEverything = KeyCode.Keypad0;
    public KeyCode damageCockpit = KeyCode.Keypad1;
    public KeyCode damageWings = KeyCode.Keypad2;
    public KeyCode damageEngines = KeyCode.Keypad3;

    //The amount of damage to deal
    public int damageToDeal = 10;



	// Use this for initialization
	private void Start ()
    {
        //Getting our ship component reference
        this.ourShip = this.GetComponent<PlayerShipController>();
	}
	

	// Update is called once per frame
	private void Update ()
    {
	    //If we press our damage cockpit button, we deal damage to the cockpit
        if(Input.GetKeyDown(this.damageCockpit))
        {
            this.ourShip.shipCockpit.DealDamage(this.damageToDeal);
        }	

        //If we press our damage wings button, we deal damage to a random wing
        if(Input.GetKeyDown(this.damageWings))
        {
            //Getting a random wing index
            int wingIndex = Random.Range(0, this.ourShip.shipWings.Count);
            //Damaging the wing at the index
            this.ourShip.shipWings[wingIndex].DealDamage(this.damageToDeal);
        }

        //If we press our damage engine button, we deal damage to a random engine
        if(Input.GetKeyDown(this.damageEngines))
        {
            //Getting a random engine index
            int engineIndex = Random.Range(0, this.ourShip.shipEngines.Count);
            //Damaging the wing at the index
            this.ourShip.shipEngines[engineIndex].DealDamage(this.damageToDeal);
        }

        //If we press our damage everything button, we deal damage to everything on the ship
        if(Input.GetKeyDown(this.damageEverything))
        {
            this.ourShip.shipCockpit.DealDamage(this.damageToDeal);

            foreach(ShipWingLogic wing in this.ourShip.shipWings)
            {
                wing.DealDamage(this.damageToDeal);
            }

            foreach(ShipEngineLogic engine in this.ourShip.shipEngines)
            {
                engine.DealDamage(this.damageToDeal);
            }
        }
	}
}

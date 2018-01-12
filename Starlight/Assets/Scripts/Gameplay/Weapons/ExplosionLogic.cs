using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class ExplosionLogic : MonoBehaviour
{
    //The attack ID of this explosion
    [HideInInspector]
    public AttackerID attackerID = AttackerID.Enemy;

    //Reference to our object's sphere collider
    private SphereCollider ourCollider;
    //The list of objects hit by this explosion so we don't hit them multiple times
    private List<GameObject> hitObjects;

    //The amount of damage dealt to hit objects
    public int damageDealt = 10;
    //Bool that determines if this explosion deals friendly fire
    public bool causeFriendlyFire = false;

    //The starting and ending collider radius of this explosion
    public Vector2 startEndRadius = new Vector2(1, 5);

    //The lifetime of the explosion
    public float lifetime = 0.5f;
    //The current lifetime of the explosion
    private float currentLifetime = 0;



	// Use this for initialization
	private void Awake ()
    {
        //Getting our sphere collider reference
        this.ourCollider = this.GetComponent<SphereCollider>();
        //Initializing our list of hit objects
        this.hitObjects = new List<GameObject>();
        //Setting our collider's radius to the starting size
        //this.ourCollider.radius = this.startEndRadius.x;
        this.transform.localScale = new Vector3(this.startEndRadius.x, this.startEndRadius.x, this.startEndRadius.x);
	}
	

	// Update is called once per frame
	private void Update ()
    {
        //Increasing the current lifetime counter
        this.currentLifetime += Time.deltaTime;

        //Setting our sphere collider's radius to the correct size based on how far along we are in the lifetime
        //this.ourCollider.radius = this.startEndRadius.x + ((this.startEndRadius.y - this.startEndRadius.x) * (this.currentLifetime / this.lifetime));
        float newRadius = this.startEndRadius.x + ((this.startEndRadius.y - this.startEndRadius.x) * (this.currentLifetime / this.lifetime));
        this.transform.localScale = new Vector3(newRadius, newRadius, newRadius);

        //If our current lifetime is greater than the max lifetime, we destroy this explosion
        if(this.currentLifetime >= this.lifetime)
        {
            Destroy(this.gameObject);
        }
	}


    //Function called when this object's collider hits something
    private void OnCollisionStart(Collider collider_)
    {
        //If the object hit is already in our list of hit objects, nothing happens
        if(this.hitObjects.Contains(collider_.gameObject))
        {
            return;
        }
        
        //If the object we hit has a health and armor component, we might be able to damage it
        if (collider_.gameObject.GetComponent<HealthAndArmor>())
        {
            //We damage the object if friendly fire is on or it has a different ID from this projectile's attacker
            if (this.causeFriendlyFire || this.attackerID != collider_.gameObject.GetComponent<HealthAndArmor>().objectIDType)
            {
                collider_.gameObject.GetComponent<HealthAndArmor>().DealDamage(this.damageDealt);

                //Adding this object to our list of hit objects so we don't damage it again
                this.hitObjects.Add(collider_.gameObject);
            }
        }
    }


    //Function called when this object's collider is triggered by something
    private void OnTriggerEnter(Collider collider_)
    {
        //If the object hit is already in our list of hit objects, nothing happens
        if (this.hitObjects.Contains(collider_.gameObject))
        {
            return;
        }

        //If the object we hit has a health and armor component, we might be able to damage it
        if (collider_.gameObject.GetComponent<HealthAndArmor>())
        {
            //We damage the object if friendly fire is on or it has a different ID from this projectile's attacker
            if (this.causeFriendlyFire || this.attackerID != collider_.gameObject.GetComponent<HealthAndArmor>().objectIDType)
            {
                collider_.gameObject.GetComponent<HealthAndArmor>().DealDamage(this.damageDealt);

                //Adding this object to our list of hit objects so we don't damage it again
                this.hitObjects.Add(collider_.gameObject);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class CollisionEvent : MonoBehaviour
{
    //Enum that determines if this collider event happens on collision, trigger, or both
    public enum CollisionType { CollisionStart, CollisionEnd, TriggerStart, TriggerEnd, EitherStart, EitherEnd};
    public CollisionType collisionType = CollisionType.EitherStart;

    //If true, this object is disabled after the event happens
    public bool disableOnCollision = false;

    //The unity event that's triggered
    public UnityEvent eventTriggered;



    //Function called when this object's collider hits something
	private void OnCollisionEnter(Collision collision_)
    {
        //Does nothing if this collider doesn't collide on start
        if(this.collisionType != CollisionType.CollisionStart && this.collisionType != CollisionType.EitherStart)
        {
            return;
        }

        //Triggering the event
        this.eventTriggered.Invoke();

        //If this object is supposed to be disabled on collision, we disable it
        if(this.disableOnCollision)
        {
            this.gameObject.SetActive(false);
        }
    }


    //Function called when this object's collider stops hitting something
    private void OnCollisionExit(Collision collision_)
    {
        //Does nothing if this collider doesn't collide on end
        if (this.collisionType != CollisionType.CollisionEnd && this.collisionType != CollisionType.EitherEnd)
        {
            return;
        }

        //Triggering the event
        this.eventTriggered.Invoke();

        //If this object is supposed to be disabled on collision, we disable it
        if (this.disableOnCollision)
        {
            this.gameObject.SetActive(false);
        }
    }


    //Function called when something triggers this object's collider
    private void OnTriggerEnter(Collider collision_)
    {
        //Does nothing if this collider doesn't trigger on start
        if (this.collisionType != CollisionType.TriggerStart && this.collisionType != CollisionType.EitherStart)
        {
            return;
        }

        //Triggering the event
        this.eventTriggered.Invoke();

        //If this object is supposed to be disabled on collision, we disable it
        if (this.disableOnCollision)
        {
            this.gameObject.SetActive(false);
        }
    }


    //Function called when something stops triggering this object's collider
    private void OnTriggerExit(Collider collision_)
    {
        //Does nothing if this collider doesn't trigger on start
        if (this.collisionType != CollisionType.TriggerEnd && this.collisionType != CollisionType.EitherEnd)
        {
            return;
        }

        //Triggering the event
        this.eventTriggered.Invoke();

        //If this object is supposed to be disabled on collision, we disable it
        if (this.disableOnCollision)
        {
            this.gameObject.SetActive(false);
        }
    }
}

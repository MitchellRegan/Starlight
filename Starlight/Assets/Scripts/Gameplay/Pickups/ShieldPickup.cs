using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(ExtraSoundEmitterSettings))]
public class ShieldPickup : MonoBehaviour
{
    //The amount of shield that this pickup restores
    public int shieldToRestore = 40;

    //Bool that lets us know that we should perform the animation for when a player hits this object
    private bool isAnimating = false;

    //The transform of the player ship that hits this pickup
    private Transform playerShipTransform;

    //The amount of time it takes to animate and destroy this pickup after being hit by a player
    public float animationTime = 2;
    private float currentAnimationTime = 0;

    //The amount that's added to this object's rotation each frame while animating
    public Vector3 rotationToAdd = new Vector3();

    //The animation curve for the scale of this object while animating
    public AnimationCurve scaleCurve = new AnimationCurve();

    //The default scale of this object
    private Vector3 defaultScale = new Vector3();
    


    //Function called when this object collides with another collider
	private void OnTriggerEnter(Collider collision_)
    {
        //Checking to see if the object that hit this pickup is a player ship
        if(collision_.gameObject.GetComponent<HealthAndArmor>())
        {
            //Creating a variable to hold the health and armor component that we hit
            HealthAndArmor hitArmor = collision_.gameObject.GetComponent<HealthAndArmor>();

            //If the hit object is for player 1 ship
            if (hitArmor.objectIDType == AttackerID.Player1)
            {
                //Telling the player 1 ship to restore shields by our amount to restore
                PlayerShipController.p1ShipRef.shipShield.RestoreShields(this.shieldToRestore);

                //Disabling our collider component
                this.GetComponent<Collider>().enabled = false;

                //Setting the player ship to follow
                this.playerShipTransform = PlayerShipController.p1ShipRef.transform;

                //Setting our default scale
                this.defaultScale = this.transform.lossyScale;

                //Starting the animation
                this.isAnimating = true;

                //Telling our sound emitter to play the pickup sound
                this.GetComponent<ExtraSoundEmitterSettings>().ownerAudio.Play();

                //If the player ship's shield collider is disabled, we enable it again
                if (!PlayerShipController.p1ShipRef.shipShield.gameObject.activeInHierarchy)
                {
                    PlayerShipController.p1ShipRef.shipShield.gameObject.SetActive(true);
                }
            }
            //If the hit object is for player 2 ship
            else if(hitArmor.objectIDType == AttackerID.Player2)
            {
                //Telling the player 2 ship to restore shields by our amount to restore
                PlayerShipController.p2ShipRef.shipShield.RestoreShields(this.shieldToRestore);

                //Disabling our collider component
                this.GetComponent<Collider>().enabled = false;

                //Setting the player ship to follow
                this.playerShipTransform = PlayerShipController.p2ShipRef.transform;

                //Setting our default scale
                this.defaultScale = this.transform.lossyScale;

                //Starting the animation
                this.isAnimating = true;

                //Telling our sound emitter to play the pickup sound
                this.GetComponent<ExtraSoundEmitterSettings>().ownerAudio.Play();

                //If the player ship's shield collider is disabled, we enable it again
                if (!PlayerShipController.p2ShipRef.shipShield.gameObject.activeInHierarchy)
                {
                    PlayerShipController.p2ShipRef.shipShield.gameObject.SetActive(true);
                }
            }
            //If an enemy hits this component nothing happens
        }
    }


    //Function called every frame
    private void Update()
    {
        //if we're not animating, nothing happens
        if(!this.isAnimating)
        {
            return;
        }

        //Counting up the current time for the animation
        this.currentAnimationTime += Time.deltaTime;

        //If the current animation time is above the total animation time, this object is destroyed
        if(this.currentAnimationTime >= this.animationTime)
        {
            Destroy(this.gameObject);
            return;
        }

        //Following the player ship's location
        this.transform.position = this.playerShipTransform.position;

        //Adding to our local rotation
        this.transform.localEulerAngles += this.rotationToAdd;

        //Setting our global scale based on our scale curve
        float newScale = this.currentAnimationTime / this.animationTime;
        newScale = scaleCurve.Evaluate(newScale);
        this.transform.localScale = new Vector3(newScale * defaultScale.x, newScale * defaultScale.y, newScale * defaultScale.z);
    }
}

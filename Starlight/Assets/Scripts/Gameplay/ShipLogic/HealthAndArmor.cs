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

    //Bool that determines if this object is currently having I-frames temporary invincibility
    [HideInInspector]
    public bool inIFrames = false;

    //The number of seconds this object has I-frames after being hit
    public float damageIFrameTime = 0;
    private float currentIFrameTime = 0f;

    [Space(8)]

    //The maximum shield value this object has
    public int maxShield = 100;
    //The current amount of shields this object has
    public int currentShields = 0;

    [Space(8)]

    //The Unity Event called when this object dies
    public UnityEvent onDeathEvent;

    [Space(8)]

    //The audio emitter for health sound effects
    public ExtraSoundEmitterSettings ourAudio;

    //The audio sound for taking shield damage
    public AudioClip shieldDamageSound;
    //The volume for taking shield damage
    [Range(0, 1)]
    public float shieldDamageVolume = 1;
    //The pitch for taking shield damage
    public Vector2 shieldDamagePitch = new Vector2(0.9f, 1.1f);

    [Space(8)]

    //The audio sound for when shields go down
    public AudioClip shieldDownSound;
    //The volume for when shields go down
    [Range(0, 1)]
    public float shieldDownVolume = 1;
    //The pitch for when shields go down
    [Range(0, 3)]
    public float shieldDownPitch = 1;

    [Space(8)]

    //The audio sound for taking health damage
    public AudioClip healthDamageSound;
    //The volume for taking health damage
    [Range(0, 1)]
    public float healthDamageVolume = 1;
    //The range of the pitch for taking health damage
    public Vector2 healthDamagePitch = new Vector2(0.9f, 1.1f);

    [Space(8)]

    //The health warning sound emitter when we're at low health
    public ExtraSoundEmitterSettings lowHealthSoundEmitter;
    //The health percent that we activate the low health sound
    [Range(0.01f, 0.5f)]
    public float warningHealthPercent = 0.1f;



    //Function called every frame
    private void Update()
    {
        //If our I-frame time is above 0, we count it down
        if(this.currentIFrameTime > 0)
        {
            this.currentIFrameTime -= Time.deltaTime;

            //If the time is still above 0, this object has I-frames
            if(this.currentIFrameTime > 0)
            {
                this.inIFrames = true;
            }
            //If the time is below 0, this object no longer has I-frames
            else
            {
                this.inIFrames = false;
            }
        }
    }


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

        //If we have the low health sound emitter, we check to see if we should disable it
        if(this.lowHealthSoundEmitter != null)
        {
            //Getting the health percent to check against our low health percent
            float healthPercent = (1f * this.currentHealth) / (1f * this.maxHealth);
            if(healthPercent > this.warningHealthPercent)
            {
                this.lowHealthSoundEmitter.gameObject.SetActive(false);
            }
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
    public virtual void DealDamage(int amountOfDamage_, bool ignoreIFrames_)
    {
        //If the amount is less than 1, we're invulnerable, or we're in I-frames that aren't ignored, nothing happens
        if(amountOfDamage_ < 1 || this.isInvulnerable || (this.inIFrames && !ignoreIFrames_))
        {
            return;
        }

        //If our shields can absorb all of the damage, they do
        if(this.currentShields >= amountOfDamage_)
        {
            this.currentShields -= amountOfDamage_;

            //Playing the sound for shields taking damage
            float randomPitch = Random.Range(this.shieldDamagePitch.x, this.shieldDamagePitch.y);
            this.PlaySoundEffects(this.shieldDamageSound, this.shieldDamageVolume, randomPitch);
        }
        //If our shields can absorb only some of the damage, some damage is dealt to health
        else
        {
            //If we have some shields left, they drop so we play the shield down sound
            if(this.currentShields > 0)
            {
                //Playing the sound for shields dropping
                this.PlaySoundEffects(this.shieldDownSound, this.shieldDownVolume, this.shieldDownPitch);
            }
            //If we don't have shields, we just take health damage so we play the health damage sound
            else
            {
                //Playing the sound for shields dropping
                float randomPitch = Random.Range(this.healthDamagePitch.x, this.healthDamagePitch.y);
                this.PlaySoundEffects(this.healthDamageSound, this.healthDamageVolume, randomPitch);
            }

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
            return;
        }
        //If we have a low health emitter we check to see if we're at low health
        else if(this.lowHealthSoundEmitter != null)
        {
            //Getting the health percent
            float healthPercent = (1f * this.currentHealth) / (1f * this.maxHealth);

            //If our health percent is below the low health percent, we activate the sound emitter
            if(healthPercent <= this.warningHealthPercent)
            {
                this.lowHealthSoundEmitter.gameObject.SetActive(true);
            }
        }

        //Setting the number of I-frames after taking damage
        this.currentIFrameTime = this.damageIFrameTime;
    }


    //Function that can be called externally (possibly by our onDeathEvent) to destroy this game object
    public void DestroyThisObject()
    {
        Destroy(this.gameObject);
    }


    //Function called to play the different sound effects
    private void PlaySoundEffects(AudioClip clipToPlay_, float volume_, float pitch_, bool loopSound_ = false)
    {
        //If there's no way to play the audio, nothing happens
        if(this.ourAudio == null || clipToPlay_ == null)
        {
            return;
        }
        //If we already have a sound effect playing, we don't override it unless we're over 20% through the sound
        if(this.ourAudio.ownerAudio.isPlaying)
        {
            if(this.ourAudio.ownerAudio.time / this.ourAudio.ownerAudio.clip.length < 0.2f)
            {
                return;
            }
        }

        //Setting our audio source's volume and pitch before playing the clip
        this.ourAudio.ownerAudio.volume = volume_;
        this.ourAudio.ownerAudio.pitch = pitch_;

        //Determining if the sound should be looped
        this.ourAudio.ownerAudio.loop = loopSound_;

        //Playing the designated sound clip
        this.ourAudio.ownerAudio.clip = clipToPlay_;
        this.ourAudio.ownerAudio.Play();
    }
}

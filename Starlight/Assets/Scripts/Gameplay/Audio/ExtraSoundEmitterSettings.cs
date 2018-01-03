using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraSoundEmitterSettings : MonoBehaviour
{
    //Reference to this object's Audio Source component
    [HideInInspector]
    public AudioSource ownerAudio;

    //Type of sound this emitter plays
    public enum SoundType { Music, Dialogue, SFX}
    public SoundType soundEmitterType = SoundType.SFX;

    //Event that listens to the event manager to see if the sound settings changed
    private DelegateEvent<EVTData> soundChangeListener;



    //Function called when this object is created
    private void Awake()
    {
        this.soundChangeListener = new DelegateEvent<EVTData>(SettingsChanged);
    }


    //Starts listening for the sound change event
    private void OnEnable()
    {
        EventManager.StartListening("SoundSettingsChanged", this.soundChangeListener);
    }


    //Stops listening for the sound change event 
    private void OnDisable()
    {
        EventManager.StopListening("SoundSettingsChanged", this.soundChangeListener);
    }


    // Use this for initialization
    private void Start()
    {
        this.ownerAudio = gameObject.GetComponent<AudioSource>();
        this.SettingsChanged(new EVTData());
    }


    //Called from the Event Manager when the sound settings have been changed
    private void SettingsChanged(EVTData data)
    {
        float emitterTypeVol = 1.0f;

        //Finds the volume of the emitter type based on what kind of sound it emits
        switch (this.soundEmitterType)
        {
            case SoundType.Dialogue:
                if (AudioSettings.globalReference.muteDialogue)
                {
                    emitterTypeVol = 0;
                }
                else
                {
                    emitterTypeVol = AudioSettings.globalReference.dialogueVolume;
                }
                break;
            case SoundType.Music:
                if (AudioSettings.globalReference.muteMusic)
                {
                    emitterTypeVol = 0;
                }
                else
                {
                    emitterTypeVol = AudioSettings.globalReference.musicVolume;
                }
                break;
            case SoundType.SFX:
                if (AudioSettings.globalReference.muteSoundEffects)
                {
                    emitterTypeVol = 0;
                }
                else
                {
                    emitterTypeVol = AudioSettings.globalReference.soundEffectVolume;
                }
                break;
            //Uses music by default
            default:
                if (AudioSettings.globalReference.muteMusic)
                {
                    emitterTypeVol = 0;
                }
                else
                {
                    emitterTypeVol = AudioSettings.globalReference.musicVolume;
                }
                break;
        }

        //Sets this owner's sound emitter volume based on the settings
        this.ownerAudio.volume = emitterTypeVol;
    }
}

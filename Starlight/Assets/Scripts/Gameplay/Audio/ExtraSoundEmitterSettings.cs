using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraSoundEmitterSettings : MonoBehaviour
{
    //Reference to this object's Audio Source component
    private AudioSource OwnerAudio;

    //Type of sound this emitter plays
    public enum SoundType { Music, Dialogue, SFX}
    public SoundType SoundEmitterType = SoundType.SFX;

    //Slider for the Headphone volume, going from mute to the max
    [Range(0.0f, 1.0f)]
    public float HeadphoneVol = 1.0f;
    //Slider for the Computer speaker volume, going from mute to the max
    [Range(0.0f, 1.0f)]
    public float CompSpeakerVol = 1.0f;
    //Slider for the  Room speaker volume, going from mute to the max
    [Range(0.0f, 1.0f)]
    public float RoomspeakerVol = 1.0f;

    //Event that listens to the event manager to see if the sound settings changed
    private DelegateEvent<EVTData> SoundChangeListener;



    //Function called when this object is created
    private void Awake()
    {
        SoundChangeListener = new DelegateEvent<EVTData>(SettingsChanged);
    }


    //Starts listening for the sound change event
    private void OnEnable()
    {
        EventManager.StartListening("SoundSettingsChanged", SoundChangeListener);
    }


    //Stops listening for the sound change event 
    private void OnDisable()
    {
        EventManager.StopListening("SoundSettingsChanged", SoundChangeListener);
    }


    // Use this for initialization
    private void Start()
    {
        OwnerAudio = gameObject.GetComponent<AudioSource>();
        SettingsChanged(new EVTData());
    }


    //Called from the Event Manager when the sound settings have been changed
    private void SettingsChanged(EVTData data)
    {
        float emitterTypeVol = 1.0f;

        //Finds the volume of the emitter type based on what kind of sound it emits
        switch (SoundEmitterType)
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
        OwnerAudio.volume = emitterTypeVol;
    }
}

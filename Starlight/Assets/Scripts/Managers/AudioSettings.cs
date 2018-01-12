using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSettings : MonoBehaviour
{
    //A reference to this manager that can be accessed anywhere
    public static AudioSettings globalReference;

    //Bool and slider for the music audio volume
    public bool muteMusic = false;
    [Range(0, 1.0f)]
    public float musicVolume = 1.0f;

    //Bool and slider for the sound effect volume
    public bool muteSoundEffects = false;
    [Range(0, 1.0f)]
    public float soundEffectVolume = 1.0f;

    //Bool and slider for the dialogue volume
    public bool muteDialogue = false;
    [Range(0, 1.0f)]
    public float dialogueVolume = 1.0f;

    //Bool and slider for the global volume
    public bool muteAll = false;
    [Range(0, 1.0f)]
    public float globalVolume = 1.0f;



    //Function called when this object is created
    void Awake()
    {
        //If there isn't already a static reference to this manager, this instance becomes the static reference
        if (globalReference == null)
        {
            globalReference = this;
        }
        //If there's already a static reference to this manager, we destroy this component
        else
        {
            Destroy(this);
        }
    }
    
    
    //Changes the global volume
    public void ChangeGlobalVolume(float newVolume_)
    {
        //Setting the global volume to the volume given
        globalReference.globalVolume = newVolume_;

        //Making sure the new volume setting is between 0 and 1 or else things get odd
        if (globalReference.globalVolume > 1)
        {
            globalReference.globalVolume = 1.0f;
        }
        else if (globalReference.globalVolume < 0)
        {
            globalReference.globalVolume = 0;
        }

        this.DispatchSoundChangeEvt();
    }


    //Changes the music volume
    public void ChangeMusicVolume(float newVolume_)
    {
        //Setting the music volume to the volume given
        globalReference.musicVolume = newVolume_;

        //Making sure the new volume setting is between 0 and 1
        if (globalReference.musicVolume > 1)
        {
            globalReference.musicVolume = 1.0f;
        }
        else if (globalReference.musicVolume < 0)
        {
            globalReference.musicVolume = 0;
        }

        this.DispatchSoundChangeEvt();
    }


    //Changes the sound effect volume
    public void ChangeSoundEffectVolume(float newVolume_)
    {
        //Setting the sound effect volume to the volume given
        globalReference.soundEffectVolume = newVolume_;

        //Making sure the new volume setting is between 0 and 1
        if (globalReference.soundEffectVolume > 1)
        {
            globalReference.soundEffectVolume = 1.0f;
        }
        else if (globalReference.soundEffectVolume < 0)
        {
            globalReference.soundEffectVolume = 0;
        }

        this.DispatchSoundChangeEvt();
    }


    //Changes the dialogue volume
    public void ChangeDialogueVolume(float newVolume_)
    {
        //Setting the dialogue volume to the volume given
        globalReference.dialogueVolume = newVolume_;

        //Making sure the new volume setting is between 0 and 1
        if (globalReference.dialogueVolume > 1)
        {
            globalReference.dialogueVolume = 1.0f;
        }
        else if (globalReference.dialogueVolume < 0)
        {
            globalReference.dialogueVolume = 0;
        }

        this.DispatchSoundChangeEvt();
    }


    //If true, mutes all audio, if false unmutes
    public void ToggleMuteAll(bool isMuted_)
    {
        globalReference.muteAll = isMuted_;
        this.DispatchSoundChangeEvt();
    }


    //If true, mutes all music, if false unmutes
    public void ToggleMuteMusic(bool isMuted_)
    {
        globalReference.muteMusic = isMuted_;
        this.DispatchSoundChangeEvt();
    }


    //If true, mutes all dialogue, if false unmutes
    public void ToggleMuteDialogue(bool isMuted_)
    {
        globalReference.muteDialogue = isMuted_;
        this.DispatchSoundChangeEvt();
    }


    //If true, mutes all sound effects, if false unmutes
    public void ToggleMuteSFX(bool isMuted_)
    {
        globalReference.muteSoundEffects = isMuted_;
        this.DispatchSoundChangeEvt();
    }


    //Function called from all of our functions to update audio emitters
    private void DispatchSoundChangeEvt()
    {
        EventManager.TriggerEvent("SoundSettingsChanged");
    }
}

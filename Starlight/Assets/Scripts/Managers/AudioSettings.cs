using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSettings : MonoBehaviour
{
    //A reference to this manager that can be accessed anywhere
    public static AudioSettings globalReference;

    //Bool and slider for the music audio volume
    public bool MuteMusic = false;
    [Range(0, 1.0f)]
    public float MusicVolume = 1.0f;

    //Bool and slider for the sound effect volume
    public bool MuteSoundEffects = false;
    [Range(0, 1.0f)]
    public float SoundEffectVolume = 1.0f;

    //Bool and slider for the dialogue volume
    public bool MuteDialogue = false;
    [Range(0, 1.0f)]
    public float DialogueVolume = 1.0f;

    //Bool and slider for the global volume
    public bool MuteAll = false;
    [Range(0, 1.0f)]
    public float GlobalVolume = 1.0f;



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
        globalReference.GlobalVolume = newVolume_;

        //Making sure the new volume setting is between 0 and 1 or else things get odd
        if (globalReference.GlobalVolume > 1)
        {
            globalReference.GlobalVolume = 1.0f;
        }
        else if (globalReference.GlobalVolume < 0)
        {
            globalReference.GlobalVolume = 0;
        }

        DispatchSoundChangeEvt();
    }


    //Changes the music volume
    public void ChangeMusicVolume(float newVolume_)
    {
        //Setting the music volume to the volume given
        globalReference.MusicVolume = newVolume_;

        //Making sure the new volume setting is between 0 and 1
        if (globalReference.MusicVolume > 1)
        {
            globalReference.MusicVolume = 1.0f;
        }
        else if (globalReference.MusicVolume < 0)
        {
            globalReference.MusicVolume = 0;
        }

        DispatchSoundChangeEvt();
    }


    //Changes the sound effect volume
    public void ChangeSoundEffectVolume(float newVolume_)
    {
        //Setting the sound effect volume to the volume given
        globalReference.SoundEffectVolume = newVolume_;

        //Making sure the new volume setting is between 0 and 1
        if (globalReference.SoundEffectVolume > 1)
        {
            globalReference.SoundEffectVolume = 1.0f;
        }
        else if (globalReference.SoundEffectVolume < 0)
        {
            globalReference.SoundEffectVolume = 0;
        }

        DispatchSoundChangeEvt();
    }


    //Changes the dialogue volume
    public void ChangeDialogueVolume(float newVolume_)
    {
        //Setting the dialogue volume to the volume given
        globalReference.DialogueVolume = newVolume_;

        //Making sure the new volume setting is between 0 and 1
        if (globalReference.DialogueVolume > 1)
        {
            globalReference.DialogueVolume = 1.0f;
        }
        else if (globalReference.DialogueVolume < 0)
        {
            globalReference.DialogueVolume = 0;
        }

        DispatchSoundChangeEvt();
    }


    //If true, mutes all audio, if false unmutes
    public void ToggleMuteAll(bool isMuted_)
    {
        globalReference.MuteAll = isMuted_;
        DispatchSoundChangeEvt();
    }


    //If true, mutes all music, if false unmutes
    public void ToggleMuteMusic(bool isMuted_)
    {
        globalReference.MuteMusic = isMuted_;
        DispatchSoundChangeEvt();
    }


    //If true, mutes all dialogue, if false unmutes
    public void ToggleMuteDialogue(bool isMuted_)
    {
        globalReference.MuteDialogue = isMuted_;
        DispatchSoundChangeEvt();
    }


    //If true, mutes all sound effects, if false unmutes
    public void ToggleMuteSFX(bool isMuted_)
    {
        globalReference.MuteSoundEffects = isMuted_;
        DispatchSoundChangeEvt();
    }


    //Function called from all of our functions to update audio emitters
    private void DispatchSoundChangeEvt()
    {
        EventManager.TriggerEvent("SoundSettingsChanged");
    }
}

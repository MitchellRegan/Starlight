using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AdvanceMenuButton : MonoBehaviour
{
    //Unity event called if the game is set to single player
    public UnityEvent singlePlayerAdvanceEvent;

    //Unity event called if the game is set to co-op
    public UnityEvent coOpAdvanceEvent;

	

    //Function called externally to advance the menu based on how many players there are
    public void AdvanceMenu()
    {
        //If we're in single player mode, we call the single player event
        if(GlobalData.globalReference.singlePlayerMode)
        {
            this.singlePlayerAdvanceEvent.Invoke();
        }
        //If we're in co-op mode, we call the co-op event
        else
        {
            this.coOpAdvanceEvent.Invoke();
        }
    }
}

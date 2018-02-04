using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateCombatUI : MonoBehaviour
{
    //The game object for the single player UI
    public GameObject singlePlayerUIObj;
    //The game object for the co-op UI
    public GameObject coOpUIObj;



	// Use this for initialization
	private void Awake()
    {
        //If the game is single player, we only activate the p1 UI
        if(GlobalData.singlePlayerMode)
        {
            this.singlePlayerUIObj.SetActive(true);
            this.coOpUIObj.SetActive(false);
        }
        //If the game is co-op, we activate both p1 and p2 UI
        else
        {
            this.singlePlayerUIObj.SetActive(false);
            this.coOpUIObj.SetActive(true);
        }
    }
}

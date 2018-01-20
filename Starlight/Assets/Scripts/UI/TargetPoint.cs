using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPoint : MonoBehaviour
{
    //The reference to this point's parent ship
    public PlayerShipController ourShip;

    //Bool that determines if this is a close target or far
    public bool isCloseTarget = true;

    //Static transforms for each type of target point there are
    public static Transform p1Close;
    public static Transform p1Far;

    public static Transform p2Close;
    public static Transform p2Far;

	

    //Function called when this object is created
    private void Awake()
    {
        //Setting the correct target point transform based on which one this is
        switch(this.ourShip.playerController)
        {
            case Players.P1:
                if (this.isCloseTarget)
                {
                    p1Close = this.transform;
                }
                else
                {
                    p1Far = this.transform;
                }
                break;

            case Players.P2:
                if (this.isCloseTarget)
                {
                    p2Close = this.transform;
                }
                else
                {
                    p2Far = this.transform;
                }
                break;

            default:
                if (this.isCloseTarget)
                {
                    p1Close = this.transform;
                }
                else
                {
                    p1Far = this.transform;
                }
                break;
        }
    }
}

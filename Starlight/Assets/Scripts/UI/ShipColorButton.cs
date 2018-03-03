using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ShipColorButton : MonoBehaviour
{
    //Enum for which player this is setting the color for
    public Players player = Players.P1;

    //Enum for which slot this color is for
    public enum ColorSlot { Color1, Color2, Color3, Color4, Color5, Decal };
    public ColorSlot slot = ColorSlot.Color1;

    //The ship select logic component that we use to update the displayed ship
    public ShipSelectLogic shipSelectRef;



    //Function called externally from UI button Unity Event to set this player's color slot
    public void SetPlayerColor()
    {
        //Getting a reference to the color that we're setting
        Color newColor = this.GetComponent<Image>().color;

        //Reference to the selected player color slots to change
        PlayerColorSlots selectedSlots;

        //If we're changing the player 1 colors
        if(this.player == Players.P1)
        {
            selectedSlots = GlobalData.globalReference.p1Colors;
        }
        //If we're changing the player 2 colors
        else
        {
            selectedSlots = GlobalData.globalReference.p2Colors;
        }

        //Switch statement to change the correct color slot
        switch(this.slot)
        {
            case ColorSlot.Color1:
                selectedSlots.slot1 = newColor;
                break;

            case ColorSlot.Color2:
                selectedSlots.slot2 = newColor;
                break;

            case ColorSlot.Color3:
                selectedSlots.slot3 = newColor;
                break;

            case ColorSlot.Color4:
                selectedSlots.slot4 = newColor;
                break;

            case ColorSlot.Color5:
                selectedSlots.slot5 = newColor;
                break;

            case ColorSlot.Decal:
                selectedSlots.decal = newColor;
                break;
        }

        //Updating the displayed ship's colors
        this.shipSelectRef.UpdateDisplayShipColors();
    }
}

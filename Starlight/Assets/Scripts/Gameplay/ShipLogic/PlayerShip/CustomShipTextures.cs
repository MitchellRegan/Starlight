using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerShipController))]
public class CustomShipTextures : MonoBehaviour
{
    //The player index that this ship uses to assign textures
    public Players player = Players.P1;

    //Reference to the mesh that this ship uses
    public MeshRenderer shipMesh;

    //Alpha maps for each of this ship's material layers
    public Texture color1Alpha;
    public Texture color2Alpha;
    public Texture color3Alpha;
    public Texture color4Alpha;
    public Texture color5Alpha;

    //The list of different alpha maps that this ship can use for the decal material
    public List<Texture> decalAlphas;



    //Function called from PlayerShipController.cs to tell this script which player texture to use
    public void SetPlayerShipID(Players ourPlayer_)
    {
        //Setting our player ID to the one given
        this.player = ourPlayer_;

        if(this.player != Players.P1)
        {
            this.player = Players.P2;
        }

        //Making sure the current ship decal index is within the correct bounds
        if(this.player == Players.P1 && GlobalData.globalReference.p1Colors.decalIndex >= this.decalAlphas.Count)
        {
            GlobalData.globalReference.p1Colors.decalIndex = 0;
        }
        else if (this.player == Players.P2 && GlobalData.globalReference.p2Colors.decalIndex >= this.decalAlphas.Count)
        {
            GlobalData.globalReference.p2Colors.decalIndex = 0;
        }

        //Applying our ship textures
        this.ApplyShipTextures();
    }


    //Function called externally to set this ship's textures
    public void ApplyShipTextures()
    {
        Debug.Log("Applying " + this.player + " ship texture");
    }


    //Function called externally to change this ship's decal alpha
    public void ChangeDecal(bool nextDecal_)
    {
        //If we shift to the next decal
        if(nextDecal_)
        {
            //If this is the p1 ship
            if (this.player == Players.P1)
            {
                GlobalData.globalReference.p1Colors.decalIndex += 1;

                //If we go past the last index in range, we loop back around to 0
                if (GlobalData.globalReference.p1Colors.decalIndex >= this.decalAlphas.Count - 1)
                {
                    GlobalData.globalReference.p1Colors.decalIndex = 0;
                }
            }
            //If this is the p2 ship
            else
            {
                GlobalData.globalReference.p2Colors.decalIndex += 1;

                //If we go past the last index in range, we loop back around to 0
                if (GlobalData.globalReference.p2Colors.decalIndex >= this.decalAlphas.Count - 1)
                {
                    GlobalData.globalReference.p2Colors.decalIndex = 0;
                }
            }
        }
        //If we shift to the previous decal
        else
        {
            //If this is the p1 ship
            if (this.player == Players.P1)
            {
                GlobalData.globalReference.p1Colors.decalIndex -= 1;

                //If we go below index 0, we loop back around to the last index
                if (GlobalData.globalReference.p1Colors.decalIndex < 0)
                {
                    GlobalData.globalReference.p1Colors.decalIndex = this.decalAlphas.Count - 1;
                }
            }
            //If this is the p2 ship
            else
            {
                GlobalData.globalReference.p2Colors.decalIndex -= 1;

                //If we go below index 0, we loop back around to the last index
                if (GlobalData.globalReference.p2Colors.decalIndex < 0)
                {
                    GlobalData.globalReference.p2Colors.decalIndex = this.decalAlphas.Count - 1;
                }
            }
        }

        //Updating our ship's textures so we can see the difference
        this.ApplyShipTextures();
    }
}

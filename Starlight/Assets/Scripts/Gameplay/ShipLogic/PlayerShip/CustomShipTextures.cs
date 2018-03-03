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
    public Texture2D color1Alpha;
    public Texture2D color2Alpha;
    public Texture2D color3Alpha;
    public Texture2D color4Alpha;
    public Texture2D color5Alpha;
    public Texture2D exhaustAlpha;

    //The list of different alpha maps that this ship can use for the decal material
    public List<Texture2D> decalAlphas;



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
        //Getting the reference to this ship model's material
        Material[] shipMats = new Material[7];

        //Setting the shader materials
        shipMats[0] = new Material(Shader.Find("Transparent/Cutout/Specular"));
        shipMats[1] = new Material(Shader.Find("Transparent/Cutout/Specular"));
        shipMats[2] = new Material(Shader.Find("Transparent/Cutout/Specular"));
        shipMats[3] = new Material(Shader.Find("Transparent/Cutout/Specular"));
        shipMats[4] = new Material(Shader.Find("Transparent/Cutout/Specular"));
        shipMats[5] = new Material(Shader.Find("Transparent/Cutout/Specular"));
        shipMats[6] = new Material(Shader.Find("Transparent/Cutout/Specular"));

        //Setting the shader textures
        shipMats[6].mainTexture = this.color1Alpha;
        shipMats[1].mainTexture = this.color2Alpha;
        shipMats[2].mainTexture = this.color3Alpha;
        shipMats[3].mainTexture = this.color4Alpha;
        shipMats[4].mainTexture = this.color5Alpha;
        shipMats[5].mainTexture = this.exhaustAlpha;

        //If we're setting the player 1 colors
        if (this.player == Players.P1)
        {
            shipMats[0].color = GlobalData.globalReference.p1Colors.decal;
            shipMats[1].color = GlobalData.globalReference.p1Colors.slot2;
            shipMats[2].color = GlobalData.globalReference.p1Colors.slot3;
            shipMats[3].color = GlobalData.globalReference.p1Colors.slot4;
            shipMats[4].color = GlobalData.globalReference.p1Colors.slot5;
            shipMats[5].color = Color.black;
            shipMats[6].color = GlobalData.globalReference.p1Colors.slot1;

            shipMats[0].mainTexture = this.decalAlphas[GlobalData.globalReference.p1Colors.decalIndex];
        }
        //If we're setting the player 2 colors
        else
        {
            shipMats[0].color = GlobalData.globalReference.p2Colors.decal;
            shipMats[1].color = GlobalData.globalReference.p2Colors.slot2;
            shipMats[2].color = GlobalData.globalReference.p2Colors.slot3;
            shipMats[3].color = GlobalData.globalReference.p2Colors.slot4;
            shipMats[4].color = GlobalData.globalReference.p2Colors.slot5;
            shipMats[5].color = Color.black;
            shipMats[6].color = GlobalData.globalReference.p2Colors.slot1;

            shipMats[0].mainTexture = this.decalAlphas[GlobalData.globalReference.p2Colors.decalIndex];
        }

        //Setting our ship model's texture array to the one we just created
        this.shipMesh.materials = shipMats;
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
                if (GlobalData.globalReference.p1Colors.decalIndex > this.decalAlphas.Count - 1)
                {
                    GlobalData.globalReference.p1Colors.decalIndex = 0;
                }
            }
            //If this is the p2 ship
            else
            {
                GlobalData.globalReference.p2Colors.decalIndex += 1;

                //If we go past the last index in range, we loop back around to 0
                if (GlobalData.globalReference.p2Colors.decalIndex > this.decalAlphas.Count - 1)
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

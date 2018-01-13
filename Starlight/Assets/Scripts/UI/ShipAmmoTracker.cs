using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipAmmoTracker : MonoBehaviour
{
    //Enum to determine which player we're displaying the ammo of
    public Players playerID = Players.P1;
    //The reference to the main and secondary weapon components that this script tracks
    private Weapon ourMainWeapon;
    private Weapon ourSecondaryWeapon;

    //Reference to the text component to display main weapon ammo
    public Text mainAmmoText;
    //Reference to the text component to display secondary weapon ammo
    public Text secondaryAmmoText;



	// Use this for initialization
	private void Start()
    {
        //Getting the reference to the weapon components for our player
        switch (this.playerID)
        {
            case Players.P1:
                this.ourMainWeapon = PlayerShipController.p1ShipRef.mainWeapon;
                this.ourSecondaryWeapon = PlayerShipController.p1ShipRef.secondaryWeapon;
                break;

            case Players.P2:
                this.ourMainWeapon = PlayerShipController.p2ShipRef.mainWeapon;
                this.ourSecondaryWeapon = PlayerShipController.p2ShipRef.secondaryWeapon;
                break;

            default:
                this.ourMainWeapon = PlayerShipController.p1ShipRef.mainWeapon;
                this.ourSecondaryWeapon = PlayerShipController.p1ShipRef.secondaryWeapon;
                break;
        }

        //If our main weapon is unlimited, we set the text to show infinite
        if(this.ourMainWeapon.unlimitedAmmo)
        {
            this.mainAmmoText.text = "8";
            this.mainAmmoText.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90)); 
        }
        //If our secondary weapon is unlimited, we set the text to show infinite
        if(this.ourSecondaryWeapon.unlimitedAmmo)
        {
            this.secondaryAmmoText.text = "8";
            this.secondaryAmmoText.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90));
        }
    }


    //Update is called once per frame
    private void Update()
    {
        //Setting the text for the main weapon if it's not unlimited
        if (!this.ourMainWeapon.unlimitedAmmo)
        {
            this.mainAmmoText.text = "" + this.ourMainWeapon.currentAmmo;
        }

        //Setting the text for the secondary weapon if it's not unlimited
        if (!this.ourSecondaryWeapon.unlimitedAmmo)
        {
            this.secondaryAmmoText.text = "" + this.ourSecondaryWeapon.currentAmmo;
        }
    }
}

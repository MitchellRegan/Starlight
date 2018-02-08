using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStartingPosition : MonoBehaviour
{
    //The player ID for which player is spawned here
    public Players playerToSpawn = Players.P1;

    //The type of movement that the player ship starts out using
    RegionZone.RegionMovement movementType = RegionZone.RegionMovement.Rail;

    //The camera object to activate
    public FollowCameraWeights cameraToActivate;



	// Use this for initialization
	private void Awake ()
    {
		//If this starting position is for player 1
        if(this.playerToSpawn == Players.P1)
        {
            //Spawning in the player 1 ship prefab at this transform position and rotation
            GameObject p1ShipObj = GameObject.Instantiate(GlobalData.globalReference.player1Ship.gameObject) as GameObject;
            p1ShipObj.transform.position = this.transform.position;
            p1ShipObj.transform.rotation = this.transform.rotation;

            //Getting the component for the player ship controller
            PlayerShipController p1Ship = p1ShipObj.GetComponent<PlayerShipController>();
            p1Ship.SetPlayerShipID(Players.P1);

            //If the starting movement type is "Rail" we tell it which rail to start on
            if(this.movementType == RegionZone.RegionMovement.Rail)
            {
                p1Ship.ourRailMovement.enabled = true;
                p1Ship.ourFreeMovement.enabled = false;
            }
            //If the starting movement type is "Free" we enable the free movement component
            else if(this.movementType == RegionZone.RegionMovement.Free)
            {
                p1Ship.ourRailMovement.enabled = false;
                p1Ship.ourFreeMovement.enabled = true;
            }

            //Enabling the camera for this player
            this.cameraToActivate.rootPosObj.SetActive(true);

            //If the game mode is single player, we make sure the camera is full-screen
            if(GlobalData.globalReference.singlePlayerMode)
            {
                this.cameraToActivate.GetComponent<Camera>().rect = new Rect(0, 0, 1, 1);
            }
            //If the game mode is co-op, we make sure the camera takes up the top half of the screen
            else
            {
                this.cameraToActivate.GetComponent<Camera>().rect = new Rect(0, 0.5f, 1, 0.5f);
            }
        }
        //If this starting position is for player 2 and the game mode is co-op
        else if(this.playerToSpawn == Players.P2 && !GlobalData.globalReference.singlePlayerMode)
        {
            //Spawning in the player 2 ship prefab at this transform position and rotation
            GameObject p2ShipObj = GameObject.Instantiate(GlobalData.globalReference.player2Ship.gameObject) as GameObject;
            p2ShipObj.transform.position = this.transform.position;
            p2ShipObj.transform.rotation = this.transform.rotation;
            
            //Getting the component for the player ship controller
            PlayerShipController p2Ship = p2ShipObj.GetComponent<PlayerShipController>();
            p2Ship.SetPlayerShipID(Players.P2);

            //If the starting movement type is "Rail" we tell it which rail to start on
            if (this.movementType == RegionZone.RegionMovement.Rail)
            {
                p2Ship.ourRailMovement.enabled = true;
                p2Ship.ourFreeMovement.enabled = false;
            }
            //If the starting movement type is "Free" we enable the free movement component
            else if (this.movementType == RegionZone.RegionMovement.Free)
            {
                p2Ship.ourRailMovement.enabled = false;
                p2Ship.ourFreeMovement.enabled = true;
            }
            
            //Enabling the camera for this player
            this.cameraToActivate.rootPosObj.SetActive(true);

            //Making sure the camera takes up the lower half of the screen
            this.cameraToActivate.GetComponent<Camera>().rect = new Rect(0, 0, 1, 0.5f);
        }
	}
}

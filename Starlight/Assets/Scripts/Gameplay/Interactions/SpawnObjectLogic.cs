using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjectLogic : MonoBehaviour
{
    //Class used by this script to designate what object should be spawned and where
    [System.Serializable]
    public class SpawnObjInfo
    {
        //The object prefab that's spawned
        public GameObject objectToSpawn;
        //The offset in local space from this object's transform
        public Vector3 localOffset = new Vector3();
        //If true, the spawned object will face the same direction as this object's transform
        public bool faceLocalDirection = false;
    }

    //The list of different objects that this script can spawn
    public List<SpawnObjInfo> objectList;



    //Function called externally to spawn the object at the given index
    public void SpawnObjAtIndex(int index_)
    {
        //If the given index is out of bounds, nothing happens
        if(index_ < 0 || index_ >= this.objectList.Count)
        {
            return;
        }

        //Creating an instance of the spawned object
        GameObject spawnedObj = GameObject.Instantiate(this.objectList[index_].objectToSpawn);

        //Setting the object's position and offset
        spawnedObj.transform.position = this.transform.TransformPoint(this.objectList[index_].localOffset);

        //If we rotate the object, we make it face the same direction as our transform
        if(this.objectList[index_].faceLocalDirection)
        {
            spawnedObj.transform.rotation = this.transform.rotation;
        }
    }
}

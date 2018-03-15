using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollUVTexture : MonoBehaviour
{
    //The model reference 
    public MeshRenderer texturedMesh;
    //The index of the material that we're scrolling
    public int materialIndex = 0;

    //The amount that we scroll the UVs each frame
    public Vector2 uVScrollRate = new Vector2(0, 0);

    //The current amount that we've scrolled the UV
    private Vector2 currentUVOffset = new Vector2(0,0);



	// Use this for initialization
	private void Start ()
    {
		//If the index is below 0 or out of range of the mesh's material array, this component is disabled
        if(this.materialIndex < 0 || this.materialIndex > this.texturedMesh.materials.Length -1)
        {
            this.enabled = false;
        }
	}
	

	// Update is called once per frame
	private void Update ()
    {
        //Adding to our current UV offset
        this.currentUVOffset += this.uVScrollRate * Time.deltaTime;

        //If the current mesh is being rendered, we shift the UVs
        if(this.texturedMesh.GetComponent<Renderer>().enabled)
        {
            this.texturedMesh.materials[this.materialIndex].mainTextureOffset = this.currentUVOffset;
        }
	}
}

using UnityEngine;
using System.Collections;

public class cubeDebug : MonoBehaviour {

	// Use this for initialization
	void Start () {
		MeshFilter mf = this.GetComponent<MeshFilter> ();
		Vector2[] uv = new Vector2[mf.mesh.vertexCount];

		uv [0] = new Vector2(0,0);
		uv [1] = new Vector2(0,0.5f);
		uv [2] = new Vector2(0.25f,0.5f);
		uv [3] = new Vector2(0.25f,0);

		uv [4] = new Vector2(0.25f,0);
		uv [5] = new Vector2(0.25f,0.5f);
		uv [6] = new Vector2(0.5f,0.5f);
		uv [7] = new Vector2(0.5f,0);


		mf.mesh.uv = uv;

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

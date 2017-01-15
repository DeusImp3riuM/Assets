using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class MapControl : MonoBehaviour {
	
	public int map_WIDTH = 9;
	public int map_HEIGHT = 9;
	public float textureCount = 4;
	public GameObject Cell = null;
	public GameObject Worker = null;
	public GameObject Hive = null;
	public Map_Cell[,] mapCells;
	public int[,] mapCellNumber;
	public aStarPoint[,] mapStarMap;
	public List<Vector3> vert = null;
	List<Vector3> verticies = new List<Vector3> ();
	int layerCount;
	public bool fullyGenerated = false;
	// Use this for initialization
	void Start () {
		mapCells = new Map_Cell[map_HEIGHT, map_WIDTH];
		mapCellNumber = new int[map_HEIGHT, map_WIDTH];
		mapStarMap = new aStarPoint[map_HEIGHT*3, map_WIDTH*3];



		int count = 0;
		for(int y=0;y<map_HEIGHT;y++){
			for(int x=0;x<map_WIDTH;x++){
				mapCells [y, x] = new Map_Cell(new Vector3 (x * 12, 6, y * 12));
				mapCells [y, x].setPosition (x, y);
				mapCells [y, x].name = "Map_Cell " + count.ToString ();
				if (x == 0 || y == 0 || x == map_WIDTH - 1 || y == map_HEIGHT - 1) {
					mapCells [y, x].ChangeCell (-1);
				} else {
					mapCells [y, x].ChangeCell (0);
				}
				count++;
			}
		}
		Vector2 center = new Vector2 (map_HEIGHT / 2, map_WIDTH / 2);
		for(int x=-3;x<4;x++){
			for(int y=-3;y<4;y++){
				if(x >= -2 && x <= 2 && y >= -2 && y <= 2)
					mapCells [(int)(center.y)+y,(int)(center.x)+x].ChangeCell(2);
				else
					mapCells [(int)(center.y)+y,(int)(center.x)+x].ChangeCell(1);
			}
		}
		for(int y=0;y<map_HEIGHT*3;y++){
			for (int x = 0; x < map_WIDTH * 3; x++) {
				mapStarMap [y, x].Location = new Vector3 ((x - 1) * 4f, 20, (y - 1) * 4f);
			}
		}
		fullyGenerated = true;
		//StaticOcclusionCulling.Compute ();

		GameObject w1 = Instantiate (Worker, mapCells[(int)center.y-1,(int)center.x-1].position+new Vector3(0,7,0), Quaternion.identity) as GameObject;
		w1.name = "Worker1";
		//*
		GameObject w2 = Instantiate (Worker, mapCells[(int)center.y-1,(int)center.x].position+new Vector3(0,7,0), Quaternion.identity) as GameObject;
		w2.name = "Worker2";

		GameObject w3 = Instantiate (Worker, mapCells[(int)center.y-1,(int)center.x+1].position+new Vector3(0,7,0), Quaternion.identity) as GameObject;
		w3.name = "Worker3";

		GameObject w4 = Instantiate (Worker, mapCells[(int)center.y,(int)center.x-1].position+new Vector3(0,7,0), Quaternion.identity) as GameObject;
		w4.name = "Worker4";
		GameObject w5 = Instantiate (Worker, mapCells[(int)center.y,(int)center.x].position+new Vector3(0,7,0), Quaternion.identity) as GameObject;
		w5.name = "Worker5";
		GameObject w6 = Instantiate (Worker, mapCells[(int)center.y,(int)center.x+1].position+new Vector3(0,7,0), Quaternion.identity) as GameObject;
		w6.name = "Worker6";
		GameObject w7 = Instantiate (Worker, mapCells[(int)center.y+1,(int)center.x-1].position+new Vector3(0,7,0), Quaternion.identity) as GameObject;
		w7.name = "Worker7";
		GameObject w8 = Instantiate (Worker, mapCells[(int)center.y+1,(int)center.x].position+new Vector3(0,7,0), Quaternion.identity) as GameObject;
		w8.name = "Worker8";
		GameObject w9 = Instantiate (Worker, mapCells[(int)center.y+1,(int)center.x+1].position+new Vector3(0,7,0), Quaternion.identity) as GameObject;
		w9.name = "Worker9";
		//*/
		GameObject hm = Instantiate (Hive,new Vector3(),Quaternion.identity) as GameObject;
		hm.name = "HiveMind";
		foreach(Map_Cell cell in mapCells){
			cell.Hive = hm.GetComponent<HiveMindAI>();
		}
		/* Mesh Test */
		generateVerticies ();
		generateMesh ();
		generateTextures ();
	}
	
	// Update is called once per frame
	void Update () {
		string Mode = "Mine";
		if (Input.GetMouseButtonDown(0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
				if (Mode.Equals ("Mine")) {
					if (hit.transform.gameObject.name.Equals ("Terrain")) {
						if (getClosestCell (hit.point).MineAble) {
							getClosestCell (hit.point).SelectCell ();
						}
					}
				} else if (Mode.Equals ("Move")) {
					if (hit.transform.gameObject.name.Contains ("Map_Cell")) {
						
					}
				}
			} else {
			}
		}
		foreach(Map_Cell cell in mapCells){
			cell.Update ();
		}
	}
	Map_Cell getClosestCell(Vector3 hitPosition){
		Map_Cell bestTarget = null;
		float closestDist = Mathf.Infinity;
		foreach(Map_Cell cell in mapCells){
			float dist = Vector3.Distance (cell.position,hitPosition);
			if(dist < closestDist){
				closestDist = dist;
				bestTarget = cell;
			}
		}
		//Debug.Log (bestTarget.name);
		return bestTarget;
	}

	public void generateMesh(){
		Mesh mesh = new Mesh ();
		mesh.name = "testMesh";
		mesh.Clear ();
		List<int> tri = new List<int> ();
		for (int y = 0; y < map_HEIGHT; y++) {
			for (int x = 0; x < map_WIDTH; x++) {
				int[] index = new int[24];
				for(int i = 0; i < 24 ; i++){
					index[i] = (i + (((x * 12) + ((y * 12) * map_WIDTH)) * 2));
				}
				if (!mapCells [y, x].Walkable) {
					//top
					tri.Add (index [0]);
					tri.Add (index [3]);
					tri.Add (index [1]);

					tri.Add (index [2]);
					tri.Add (index [1]);
					tri.Add (index [3]);

					if (x + 1 == map_WIDTH) {
						// right
						tri.Add (index [8]);
						tri.Add (index [9]);
						tri.Add (index [11]);

						tri.Add (index [10]);
						tri.Add (index [11]);
						tri.Add (index [9]);

					} else if (mapCells [y, x + 1].Walkable) {
						tri.Add (index [8]);
						tri.Add (index [9]);
						tri.Add (index [11]);

						tri.Add (index [10]);
						tri.Add (index [11]);
						tri.Add (index [9]);
					}

					if (x - 1 == -1) {
						//left
						tri.Add (index [16]);
						tri.Add (index [17]);
						tri.Add (index [19]);

						tri.Add (index [18]);
						tri.Add (index [19]);
						tri.Add (index [17]);

					} else if (mapCells [y, x - 1].Walkable) {
						tri.Add (index [16]);
						tri.Add (index [17]);
						tri.Add (index [19]);

						tri.Add (index [18]);
						tri.Add (index [19]);
						tri.Add (index [17]);
					}
					if (y - 1 == -1) {
						//front
						tri.Add (index [4]);
						tri.Add (index [5]);
						tri.Add (index [7]);

						tri.Add (index [6]);
						tri.Add (index [7]);
						tri.Add (index [5]);
					} else if (mapCells [y - 1, x].Walkable) {
						tri.Add (index [4]);
						tri.Add (index [5]);
						tri.Add (index [7]);

						tri.Add (index [6]);
						tri.Add (index [7]);
						tri.Add (index [5]);
					}
					if (y + 1 == map_HEIGHT) {
						//back
						tri.Add (index [12]);
						tri.Add (index [13]);
						tri.Add (index [15]);

						tri.Add (index [14]);
						tri.Add (index [15]);
						tri.Add (index [13]);

					} else if (mapCells [y + 1, x].Walkable) {
						tri.Add (index [12]);
						tri.Add (index [13]);
						tri.Add (index [15]);

						tri.Add (index [14]);
						tri.Add (index [15]);
						tri.Add (index [13]);
					}
				}
				else {
					tri.Add (index [20]);
					tri.Add (index [22]);
					tri.Add (index [21]);

					tri.Add (index [23]);
					tri.Add (index [22]);
					tri.Add (index [20]);
				}
			}
		}
		vert = new List<Vector3>(verticies);
		mesh.vertices = verticies.ToArray ();
		mesh.triangles = tri.ToArray ();
		mesh.RecalculateNormals ();
		mesh.RecalculateBounds ();
		MeshFilter mf = (MeshFilter)GameObject.Find ("Terrain").GetComponent (typeof(MeshFilter));
		mf.mesh = mesh;
		MeshCollider mc = (MeshCollider)GameObject.Find("Terrain").GetComponent(typeof(MeshCollider));
		mc.sharedMesh = mesh;
	}
		
	public void generateTextures(){
		Vector2[] uvs = new Vector2[verticies.Count];
		MeshFilter mf = (MeshFilter)GameObject.Find ("Terrain").GetComponent (typeof(MeshFilter));

		for (int y = 0; y < map_HEIGHT; y++) {
			for (int x = 0; x < map_WIDTH; x++) {
				int[] index = new int[24];
				for(int i = 0; i < 24 ; i++){
					index[i] = (i + (((x * 12) + ((y * 12) * map_WIDTH)) * 2));
				}
				int cell = mapCells [y, x].Cell;
				float selected = 0;

				float offset = 1 / textureCount;
				if (mapCells [y, x].Selected)
					selected = 0.5f;
				//top
				uvs[index[0]] = new Vector2 ((cell+1)*offset,0+selected);
				uvs[index[1]] = new Vector2 ((cell+1)*offset,0.5f+selected);
				uvs[index[3]] = new Vector2 (((cell+1)*offset)+offset,0f+selected);
				uvs[index[2]] = new Vector2 (((cell+1)*offset)+offset,0.5f+selected);
				//front
				uvs[index[4]] = new Vector2 ((cell+1)*offset,0+selected);
				uvs[index[5]] = new Vector2 ((cell+1)*offset,0.5f+selected);
				uvs[index[7]] = new Vector2 (((cell+1)*offset)+offset,0f+selected);
				uvs[index[6]] = new Vector2 (((cell+1)*offset)+offset,0.5f+selected);
				//right
				uvs[index[8]] = new Vector2 ((cell+1)*offset,0+selected);
				uvs[index[9]] = new Vector2 ((cell+1)*offset,0.5f+selected);
				uvs[index[11]] = new Vector2 (((cell+1)*offset)+offset,0f+selected);
				uvs[index[10]] = new Vector2 (((cell+1)*offset)+offset,0.5f+selected);
				//back
				uvs[index[12]] = new Vector2 ((cell+1)*offset,0+selected);
				uvs[index[13]] = new Vector2 ((cell+1)*offset,0.5f+selected);
				uvs[index[15]] = new Vector2 (((cell+1)*offset)+offset,0f+selected);
				uvs[index[14]] = new Vector2 (((cell+1)*offset)+offset,0.5f+selected);
				//left
				uvs[index[16]] = new Vector2 ((cell+1)*offset,0+selected);
				uvs[index[17]] = new Vector2 ((cell+1)*offset,0.5f+selected);
				uvs[index[19]] = new Vector2 (((cell+1)*offset)+offset,0f+selected);
				uvs[index[18]] = new Vector2 (((cell+1)*offset)+offset,0.5f+selected);
				//bottom
				uvs[index[20]] = new Vector2 ((cell+1)*offset,0+selected);
				uvs[index[21]] = new Vector2 ((cell+1)*offset,0.5f+selected);
				uvs[index[23]] = new Vector2 (((cell+1)*offset)+offset,0f+selected);
				uvs[index[22]] = new Vector2 (((cell+1)*offset)+offset,0.5f+selected);

			}
		}
		mf.mesh.uv = uvs;
	}

	public void generateVerticies(){
		int heightOffset = 1;
		/*
		for (int y = 0; y < map_HEIGHT; y++) {
			for (int x = 0; x < map_WIDTH; x++) {
				verticies.Add (new Vector3 (mapCells[y,x].transform.position.x,6,mapCells[y,x].transform.position.z) + new Vector3 (-6, 6+heightOffset, -6));
				verticies.Add (new Vector3 (mapCells[y,x].transform.position.x,6,mapCells[y,x].transform.position.z) + new Vector3 (+6, 6+heightOffset, -6));
				verticies.Add (new Vector3 (mapCells[y,x].transform.position.x,6,mapCells[y,x].transform.position.z) + new Vector3 (-6, 6+heightOffset, +6));
				verticies.Add (new Vector3 (mapCells[y,x].transform.position.x,6,mapCells[y,x].transform.position.z) + new Vector3 (+6, 6+heightOffset, +6));

				verticies.Add (new Vector3 (mapCells[y,x].transform.position.x,6,mapCells[y,x].transform.position.z) + new Vector3 (-6, 6+heightOffset, -6));
				verticies.Add (new Vector3 (mapCells[y,x].transform.position.x,6,mapCells[y,x].transform.position.z) + new Vector3 (+6, 6+heightOffset, -6));
				verticies.Add (new Vector3 (mapCells[y,x].transform.position.x,6,mapCells[y,x].transform.position.z) + new Vector3 (-6, 6+heightOffset, +6));
				verticies.Add (new Vector3 (mapCells[y,x].transform.position.x,6,mapCells[y,x].transform.position.z) + new Vector3 (+6, 6+heightOffset, +6));

				verticies.Add (new Vector3 (mapCells[y,x].transform.position.x,6,mapCells[y,x].transform.position.z) + new Vector3 (-6, 6+heightOffset, -6));
				verticies.Add (new Vector3 (mapCells[y,x].transform.position.x,6,mapCells[y,x].transform.position.z) + new Vector3 (+6, 6+heightOffset, -6));
				verticies.Add (new Vector3 (mapCells[y,x].transform.position.x,6,mapCells[y,x].transform.position.z) + new Vector3 (-6, 6+heightOffset, +6));
				verticies.Add (new Vector3 (mapCells[y,x].transform.position.x,6,mapCells[y,x].transform.position.z) + new Vector3 (+6, 6+heightOffset, +6));
			}
		}
		layerCount = verticies.Count;
		for (int y = 0; y < map_HEIGHT; y++) {
			for (int x = 0; x < map_WIDTH; x++) {
				verticies.Add (new Vector3 (mapCells[y,x].transform.position.x,6,mapCells[y,x].transform.position.z) + new Vector3 (-6, -6+heightOffset, -6));
				verticies.Add (new Vector3 (mapCells[y,x].transform.position.x,6,mapCells[y,x].transform.position.z) + new Vector3 (+6, -6+heightOffset, -6));
				verticies.Add (new Vector3 (mapCells[y,x].transform.position.x,6,mapCells[y,x].transform.position.z) + new Vector3 (-6, -6+heightOffset, +6));
				verticies.Add (new Vector3 (mapCells[y,x].transform.position.x,6,mapCells[y,x].transform.position.z) + new Vector3 (+6, -6+heightOffset, +6));

				verticies.Add (new Vector3 (mapCells[y,x].transform.position.x,6,mapCells[y,x].transform.position.z) + new Vector3 (-6, -6+heightOffset, -6));
				verticies.Add (new Vector3 (mapCells[y,x].transform.position.x,6,mapCells[y,x].transform.position.z) + new Vector3 (+6, -6+heightOffset, -6));
				verticies.Add (new Vector3 (mapCells[y,x].transform.position.x,6,mapCells[y,x].transform.position.z) + new Vector3 (-6, -6+heightOffset, +6));
				verticies.Add (new Vector3 (mapCells[y,x].transform.position.x,6,mapCells[y,x].transform.position.z) + new Vector3 (+6, -6+heightOffset, +6));

				verticies.Add (new Vector3 (mapCells[y,x].transform.position.x,6,mapCells[y,x].transform.position.z) + new Vector3 (-6, -6+heightOffset, -6));
				verticies.Add (new Vector3 (mapCells[y,x].transform.position.x,6,mapCells[y,x].transform.position.z) + new Vector3 (+6, -6+heightOffset, -6));
				verticies.Add (new Vector3 (mapCells[y,x].transform.position.x,6,mapCells[y,x].transform.position.z) + new Vector3 (-6, -6+heightOffset, +6));
				verticies.Add (new Vector3 (mapCells[y,x].transform.position.x,6,mapCells[y,x].transform.position.z) + new Vector3 (+6, -6+heightOffset, +6));
			}
		}
		*/
		for (int y = 0; y < map_HEIGHT; y++) {
			for (int x = 0; x < map_WIDTH; x++) {
				verticies.Add (new Vector3 (mapCells[y,x].position.x,6,mapCells[y,x].position.z) + new Vector3 (-6, 6+heightOffset, -6));
				verticies.Add (new Vector3 (mapCells[y,x].position.x,6,mapCells[y,x].position.z) + new Vector3 (+6, 6+heightOffset, -6));
				verticies.Add (new Vector3 (mapCells[y,x].position.x,6,mapCells[y,x].position.z) + new Vector3 (+6, 6+heightOffset, +6));
				verticies.Add (new Vector3 (mapCells[y,x].position.x,6,mapCells[y,x].position.z) + new Vector3 (-6, 6+heightOffset, +6));

				verticies.Add (new Vector3 (mapCells[y,x].position.x,6,mapCells[y,x].position.z) + new Vector3 (-6, 6+heightOffset, -6));
				verticies.Add (new Vector3 (mapCells[y,x].position.x,6,mapCells[y,x].position.z) + new Vector3 (+6, 6+heightOffset, -6));
				verticies.Add (new Vector3 (mapCells[y,x].position.x,6,mapCells[y,x].position.z) + new Vector3 (+6, -6+heightOffset, -6));
				verticies.Add (new Vector3 (mapCells[y,x].position.x,6,mapCells[y,x].position.z) + new Vector3 (-6, -6+heightOffset, -6));

				verticies.Add (new Vector3 (mapCells[y,x].position.x,6,mapCells[y,x].position.z) + new Vector3 (+6, 6+heightOffset, -6));
				verticies.Add (new Vector3 (mapCells[y,x].position.x,6,mapCells[y,x].position.z) + new Vector3 (+6, 6+heightOffset, +6));
				verticies.Add (new Vector3 (mapCells[y,x].position.x,6,mapCells[y,x].position.z) + new Vector3 (+6, -6+heightOffset, +6));
				verticies.Add (new Vector3 (mapCells[y,x].position.x,6,mapCells[y,x].position.z) + new Vector3 (+6, -6+heightOffset, -6));

				verticies.Add (new Vector3 (mapCells[y,x].position.x,6,mapCells[y,x].position.z) + new Vector3 (+6, 6+heightOffset, +6));
				verticies.Add (new Vector3 (mapCells[y,x].position.x,6,mapCells[y,x].position.z) + new Vector3 (-6, 6+heightOffset, +6));
				verticies.Add (new Vector3 (mapCells[y,x].position.x,6,mapCells[y,x].position.z) + new Vector3 (-6, -6+heightOffset, +6));
				verticies.Add (new Vector3 (mapCells[y,x].position.x,6,mapCells[y,x].position.z) + new Vector3 (+6, -6+heightOffset, +6));

				verticies.Add (new Vector3 (mapCells[y,x].position.x,6,mapCells[y,x].position.z) + new Vector3 (-6, 6+heightOffset, +6));
				verticies.Add (new Vector3 (mapCells[y,x].position.x,6,mapCells[y,x].position.z) + new Vector3 (-6, 6+heightOffset, -6));
				verticies.Add (new Vector3 (mapCells[y,x].position.x,6,mapCells[y,x].position.z) + new Vector3 (-6, -6+heightOffset, -6));
				verticies.Add (new Vector3 (mapCells[y,x].position.x,6,mapCells[y,x].position.z) + new Vector3 (-6, -6+heightOffset, +6));

				verticies.Add (new Vector3 (mapCells[y,x].position.x,6,mapCells[y,x].position.z) + new Vector3 (-6, -6+heightOffset, -6));
				verticies.Add (new Vector3 (mapCells[y,x].position.x,6,mapCells[y,x].position.z) + new Vector3 (+6, -6+heightOffset, -6));
				verticies.Add (new Vector3 (mapCells[y,x].position.x,6,mapCells[y,x].position.z) + new Vector3 (+6, -6+heightOffset, +6));
				verticies.Add (new Vector3 (mapCells[y,x].position.x,6,mapCells[y,x].position.z) + new Vector3 (-6, -6+heightOffset, +6));
			}
		}
	}

	aStarPoint getClosetPoint(Vector3 currentPos){
		aStarPoint bestTarget = new aStarPoint ();
		float closestDist = Mathf.Infinity;
		foreach(aStarPoint v in mapStarMap){
			float dist = Vector2.Distance (new Vector2(v.Location.x,v.Location.z),(new Vector2(currentPos.x,currentPos.z)));
			if(dist < closestDist){
				closestDist = dist;
				bestTarget = v;
			}
		}
		return bestTarget;
	}

	public void OnDrawGizmos(){
		//*
		Gizmos.color = Color.green;
		if(verticies != null){
			/*
			foreach(Vector3 v in vert){
				Gizmos.DrawWireSphere (v, 1);
			}
			*/
			/*
			for(int i = 0 ; i < 8 ; i++){
				Gizmos.DrawWireSphere (vert[i], 1);
			}
			*/
			/*
			int x = 21;
			int y = 0;
			int a = (0 + (((x * 2) + ((y * 2) * map_WIDTH)) * 2));
			int b = (1 + (((x * 2) + ((y * 2) * map_WIDTH)) * 2));
			int c = (2 + (((x * 2) + ((y * 2) * map_WIDTH)) * 2));
			int d = (3 + (((x * 2) + ((y * 2) * map_WIDTH)) * 2));
			Gizmos.DrawWireSphere (verticies[a], 1);
			Gizmos.DrawWireSphere (verticies[b], 1);
			Gizmos.DrawWireSphere (verticies[c], 1);
			Gizmos.DrawWireSphere (verticies[d], 1);
			*/
			/*
			int x = 0;
			int y = 0;
			int ind1 = (0 + (((x * 6) + ((y * 6) * map_WIDTH)) * 2));
			int ind2 = (8 + (((x * 6) + ((y * 6) * map_WIDTH)) * 2));
			ind2 += layerCount;
			Gizmos.DrawWireSphere (verticies[ind1], 1);
			Gizmos.DrawWireSphere (verticies[ind2], 2);
			*/

		}
		//*/
	}

}
public class aStarPoint : IHeapItem<aStarPoint>{
	public int x = 0;
	public int y = 0;
	public int Cell = 0;
	public Vector3 Location = new Vector3 ();
	public int g = 0;
	public float f=0,h=0;
	public bool traversable = false;
	public aStarPoint parent = null,child  = null;
	int heapIndex;


	public aStarPoint(){
		
	}
	public aStarPoint(aStarPoint a){
		this.x = a.x;
		this.y = a.y;
		this.Cell = a.Cell;
		this.Location = a.Location;
		this.g = a.g;
		this.f = a.f;
		this.h = a.h;
		this.parent = a.parent;
		this.child = a.child;
	}

	public int HeapIndex{
		get{
			return heapIndex;
		}
		set{
			heapIndex = value;
		}
	}
	public int CompareTo(aStarPoint nodeToCompare){
		int compare = this.f.CompareTo (nodeToCompare.f);
		if (compare == 0) {
			compare = this.h.CompareTo (nodeToCompare.h);
		}
		return -compare;
	}
	public void reset(){
		this.f = 0;
		this.g = 0;
		this.h = 0;
		this.parent = null;
	}
	public bool isTraversable{
		get{
			return traversable;
		}
		set{
			traversable = value;
		}
	}
	public Vector2 gridPosition(){
		return new Vector2(x,y);
	}
}

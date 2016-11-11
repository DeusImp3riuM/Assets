using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class MapControl : MonoBehaviour {
	
	public int map_WIDTH = 9;
	public int map_HEIGHT = 9;
	public GameObject Cell = null;
	public GameObject Worker = null;
	public GameObject Hive = null;
	public GameObject[,] mapCells;
	public int[,] mapCellNumber;
	public aStarPoint[,] mapStarMap;
	public List<GameObject> selectedCells;

	// Use this for initialization
	void Start () {
		selectedCells = new List<GameObject>();
		mapCells = new GameObject[map_HEIGHT, map_WIDTH];
		mapCellNumber = new int[map_HEIGHT, map_WIDTH];
		mapStarMap = new aStarPoint[map_HEIGHT*3, map_WIDTH*3];
	
		int count = 0;
		for(int y=0;y<map_HEIGHT;y++){
			for(int x=0;x<map_WIDTH;x++){
				mapCells [y, x] = Instantiate (Cell, new Vector3 (x * 12, 6, y * 12), Quaternion.identity) as GameObject;
				mapCells [y, x].GetComponent<Map_CellControl> ().setPosition (x, y);
				mapCells [y, x].name = "Map_Cell " + count.ToString ();
				mapCells [y, x].GetComponent<Map_CellControl> ().ChangeCell (0);
				count++;
			}
		}
		Vector2 center = new Vector2 (map_HEIGHT / 2, map_WIDTH / 2);
		for(int x=-2;x<3;x++){
			for(int y=-2;y<3;y++){
				mapCells [(int)(center.y)+y,(int)(center.x)+x].GetComponent<Map_CellControl> ().ChangeCell(1);
			}
		}
		for(int y=0;y<map_HEIGHT*3;y++){
			for (int x = 0; x < map_WIDTH * 3; x++) {
				mapStarMap [y, x].Location = new Vector3 ((x - 1) * 4f, 20, (y - 1) * 4f);
			}
		}
		NavMeshBuilder.BuildNavMesh();

		GameObject w1 = Instantiate (Worker, mapCells[(int)center.y-1,(int)center.x-1].transform.position+new Vector3(0,6,0), Quaternion.identity) as GameObject;
		w1.name = "Worker1";

		GameObject w2 = Instantiate (Worker, mapCells[(int)center.y-1,(int)center.x].transform.position+new Vector3(0,6,0), Quaternion.identity) as GameObject;
		w2.name = "Worker2";

		GameObject w3 = Instantiate (Worker, mapCells[(int)center.y-1,(int)center.x+1].transform.position+new Vector3(0,6,0), Quaternion.identity) as GameObject;
		w3.name = "Worker3";
		//*
		GameObject w4 = Instantiate (Worker, mapCells[(int)center.y,(int)center.x-1].transform.position+new Vector3(0,6,0), Quaternion.identity) as GameObject;
		w4.name = "Worker4";
		GameObject w5 = Instantiate (Worker, mapCells[(int)center.y,(int)center.x].transform.position+new Vector3(0,6,0), Quaternion.identity) as GameObject;
		w5.name = "Worker5";
		GameObject w6 = Instantiate (Worker, mapCells[(int)center.y,(int)center.x+1].transform.position+new Vector3(0,6,0), Quaternion.identity) as GameObject;
		w6.name = "Worker6";
		GameObject w7 = Instantiate (Worker, mapCells[(int)center.y+1,(int)center.x-1].transform.position+new Vector3(0,6,0), Quaternion.identity) as GameObject;
		w7.name = "Worker7";
		GameObject w8 = Instantiate (Worker, mapCells[(int)center.y+1,(int)center.x].transform.position+new Vector3(0,6,0), Quaternion.identity) as GameObject;
		w8.name = "Worker8";
		GameObject w9 = Instantiate (Worker, mapCells[(int)center.y+1,(int)center.x+1].transform.position+new Vector3(0,6,0), Quaternion.identity) as GameObject;
		w9.name = "Worker9";
		//*/
		GameObject hm = Instantiate (Hive,new Vector3(),Quaternion.identity) as GameObject;


	}
	
	// Update is called once per frame
	void Update () {
		
		string Mode = "Mine";
		if (Input.GetMouseButtonDown(0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
				if (Mode.Equals ("Mine")) {
					if (hit.transform.gameObject.name.Contains ("Map_Cell") && hit.transform.gameObject.GetComponent<Map_CellControl>().Cell == 0) {
						hit.transform.gameObject.GetComponent<Map_CellControl> ().SelectCell ();
					}
				}
			}
		}
	}
	public void OnDrawGizmos(){
		/*
		for (int y = 0; y < map_HEIGHT * 3; y++) {
			for (int x = 0; x < map_WIDTH * 3; x++) {
				
				if (mapStarMap [y, x].Cell == 0) {
					Gizmos.color = Color.red;
					Gizmos.DrawSphere (mapStarMap [y, x].Location, 1);
				}

			}
		}
		//*/
		//Gizmos.color = Color.green;
		//Gizmos.DrawSphere (getClosetPoint(GameObject.Find("Worker5").gameObject.transform.position), 1);
	}
	Vector3 getClosetPoint(Vector3 currentPos){
		Vector3 bestTarget = new Vector3();
		float closestDist = Mathf.Infinity;
		foreach(aStarPoint v in mapStarMap){
			float dist = Vector3.Distance (v.Location,currentPos);
			if(dist < closestDist){
				closestDist = dist;
				bestTarget = v.Location;
			}
		}
		return bestTarget;
	}

}
public class aStarPoint{
	public int Cell = 0;
	public Vector3 Location = new Vector3 ();
}

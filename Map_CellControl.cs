using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Map_CellControl : MonoBehaviour {

	public int Cell = 1;
	public int health = 10;
	public bool Selected = false;
	public Vector2 gridPosition;
	public List<string> assignedWorkerNorth = new List<string>(3);
	public List<string> assignedWorkerSouth = new List<string>(3);
	public List<string> assignedWorkerEast = new List<string>(3);
	public List<string> assignedWorkerWest = new List<string>(3);
	MapControl mapControl;
	// Use this for initialization
	void Start () {
		mapControl = GameObject.Find ("MapController").GetComponent<MapControl> ();
	}
	// Update is called once per frame
	void Update () {
		if(name.Equals("Map_Cell 283")){
			
		}
		if(health <= 0){
			mapControl.selectedCells.Remove(gameObject);
			ChangeCell (1);
			SelectCell ();
			NavMeshBuilder.BuildNavMeshAsync ();
			health = 3;
		}
	}
	public void ChangeCell(int newCell){
		Cell = newCell;
		MapControl mc = GameObject.Find ("MapController").GetComponent<MapControl> ();
		mc.mapCellNumber[(int)gridPosition.y, (int)gridPosition.x] = newCell;
		for(int i=0;i<3;i++){
			for(int j=0;j<3;j++){
				mc.mapStarMap [(int)(gridPosition.y * 3) + i, (int)(gridPosition.x * 3) + j] = new aStarPoint ();
				mc.mapStarMap[(int)(gridPosition.y*3)+i,(int)(gridPosition.x*3)+j].Cell = newCell;
			}
		}
		switch (Cell) {
		case 0:
			transform.position = new Vector3 (transform.position.x, 6, transform.position.z);
			this.GetComponent<Renderer>().material.SetColor("_Color",new Color(0,0,0));
			break;
		case 1:
			transform.position = new Vector3 (transform.position.x, -6, transform.position.z);
			this.GetComponent<Renderer>().material.SetColor("_Color",new Color(0.5F,0.25F,0.0F));

			break;
		case 2:
			transform.position = new Vector3 (transform.position.x, -6, transform.position.z);
			this.GetComponent<Renderer>().material.SetColor("_Color",new Color(0.2F,0.2F,0.2F));
			break;
		default:
			Debug.Log ("Cell Type Not Found");
			break;
		}
	}
	public void SelectCell(){
		Selected = !Selected;
		if (Selected) {
			this.GetComponent<Renderer> ().material.SetColor ("_EmissionColor", Color.blue);
			GameObject.Find ("MapController").GetComponent<MapControl> ().selectedCells.Add(this.gameObject);
		} else {
			assignedWorkerNorth.Clear();
			assignedWorkerSouth.Clear();
			assignedWorkerEast.Clear();
			assignedWorkerWest.Clear();

			this.GetComponent<Renderer> ().material.SetColor ("_EmissionColor", Color.black);
			GameObject.Find ("MapController").GetComponent<MapControl> ().selectedCells.Remove (this.gameObject);
		}
	}
	public void setPosition(int x, int y){
		gridPosition = new Vector2 (x, y);
	}
	public Vector3 assignPosition(GameObject Worker){
		if(mapControl.mapCellNumber[(int)gridPosition.y-1,(int)gridPosition.x] == 1){
			if (!assignedWorkerSouth.Contains (Worker.name) && assignedWorkerSouth.Count < 3) {
				assignedWorkerSouth.Add (Worker.name);

			}
			if (assignedWorkerSouth.Count < 3 || assignedWorkerSouth.Contains(Worker.name)) {
				switch (assignedWorkerSouth.IndexOf (Worker.name)) {

				case 0:
					return new Vector3 (transform.position.x, Worker.transform.position.y, transform.position.z) + new Vector3 (0, 0, -8f);
					break;
				case 1:
					return new Vector3 (transform.position.x, Worker.transform.position.y, transform.position.z) + new Vector3 (3.5f, 0, -8f);
					break;
				case 2:
					return new Vector3 (transform.position.x, Worker.transform.position.y, transform.position.z) + new Vector3 (-3.5f, 0, -8f);
					break;
				}
			}
		}
		if(mapControl.mapCellNumber[(int)gridPosition.y+1,(int)gridPosition.x] == 1){
			if (!assignedWorkerNorth.Contains (Worker.name) && assignedWorkerNorth.Count < 3) {
				assignedWorkerNorth.Add (Worker.name);

			}
			if (assignedWorkerNorth.Count < 3 || assignedWorkerNorth.Contains(Worker.name)) {
				switch (assignedWorkerNorth.IndexOf (Worker.name)) {

				case 0:
					return new Vector3 (transform.position.x, Worker.transform.position.y, transform.position.z) + new Vector3 (0, 0, 8f);
					break;
				case 1:
					return new Vector3 (transform.position.x, Worker.transform.position.y, transform.position.z) + new Vector3 (3.5f, 0, 8f);
					break;
				case 2:
					return new Vector3 (transform.position.x, Worker.transform.position.y, transform.position.z) + new Vector3 (-3.5f, 0, 8f);
					break;
				}
			}
		}
		if(mapControl.mapCellNumber[(int)gridPosition.y,(int)gridPosition.x+1] == 1){
			if (!assignedWorkerEast.Contains (Worker.name) && assignedWorkerEast.Count < 3) {
				assignedWorkerEast.Add (Worker.name);

			}
			if (assignedWorkerEast.Count < 3 || assignedWorkerEast.Contains(Worker.name)) {
				switch (assignedWorkerEast.IndexOf (Worker.name)) {

				case 0:
					return new Vector3 (transform.position.x, Worker.transform.position.y, transform.position.z) + new Vector3 (8f, 0, 0f);
					break;
				case 1:
					return new Vector3 (transform.position.x, Worker.transform.position.y, transform.position.z) + new Vector3 (8f, 0, 3.5f);
					break;
				case 2:
					return new Vector3 (transform.position.x, Worker.transform.position.y, transform.position.z) + new Vector3 (8f, 0, -3.5f);
					break;
				}
			}
		}
		if(mapControl.mapCellNumber[(int)gridPosition.y,(int)gridPosition.x-1] == 1){
			if (!assignedWorkerWest.Contains (Worker.name) && assignedWorkerWest.Count < 3) {
				assignedWorkerWest.Add (Worker.name);

			}
			if (assignedWorkerWest.Count < 3 || assignedWorkerWest.Contains(Worker.name)) {
				switch (assignedWorkerWest.IndexOf (Worker.name)) {

				case 0:
					return new Vector3 (transform.position.x, Worker.transform.position.y, transform.position.z) + new Vector3 (-8f, 0, 0f);
					break;
				case 1:
					return new Vector3 (transform.position.x, Worker.transform.position.y, transform.position.z) + new Vector3 (-8f, 0, 3.5f);
					break;
				case 2:
					return new Vector3 (transform.position.x, Worker.transform.position.y, transform.position.z) + new Vector3 (-8f, 0, -3.5f);
					break;
				}
			}
		}
		//assignedWorkerNorth
		//assignedWorkerEast
		//assignedWorkerWest
		return Worker.transform.position;
	}
	public int availableSpace(){
		int sides = 0;
		GameObject[,] Map = GameObject.Find ("MapController").GetComponent<MapControl> ().mapCells;
		if(Map[(int)gridPosition.y-1,(int)gridPosition.x].GetComponent<Map_CellControl>().Cell == 1){
			sides++;
		}
		if(Map[(int)gridPosition.y+1,(int)gridPosition.x].GetComponent<Map_CellControl>().Cell == 1){
			sides++;
		}
		if(Map[(int)gridPosition.y,(int)gridPosition.x-1].GetComponent<Map_CellControl>().Cell == 1){
			sides++;
		}
		if(Map[(int)gridPosition.y,(int)gridPosition.x+1].GetComponent<Map_CellControl>().Cell == 1){
			sides++;
		}
		sides *= 3; // default value 3
		sides = sides-assignedWorkerNorth.Count - assignedWorkerEast.Count - assignedWorkerSouth.Count - assignedWorkerWest.Count;
		/*
		if(assignedWorkerNorth.Contains(NPC) || assignedWorkerEast.Contains(NPC) || assignedWorkerSouth.Contains(NPC) || assignedWorkerWest.Contains(NPC)){
			sides = 1;
		}
		*/
		return sides;
	}

}

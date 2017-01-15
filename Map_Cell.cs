﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Map_Cell{

	public Vector3 position = new Vector3 ();
	public string name  = "";
	public int Cell = 1;
	public int health = 20;
	public bool Selected = false;
	public bool MineAble = false;
	public bool Walkable = false;
	public Vector2 gridPosition;
	public List<string> assignedWorkerNorth = new List<string>(3);
	public List<string> assignedWorkerSouth = new List<string>(3);
	public List<string> assignedWorkerEast = new List<string>(3);
	public List<string> assignedWorkerWest = new List<string>(3);
	public bool hasClaimer = false;
	MapControl mapControl;
	HiveMindAI HiveMind;

	// Use this for initialization
	public Map_Cell (Vector3 pos) {
		position = pos;
		mapControl = GameObject.Find ("MapController").GetComponent<MapControl> ();
		//HiveMind = GameObject.Find ("HiveMind").GetComponent<HiveMindAI> ();

	}
	// Update is called once per frame
	public void Update () {
		if(Cell == 1){
			if(isClaimable() && !HiveMind.IsClaimable.Contains(this) && !hasClaimer){
				HiveMind.IsClaimable.Add (this);
			}
		}

	}
	public void ChangeCell(int newCell){
		Cell = newCell;
		MapControl mc = GameObject.Find ("MapController").GetComponent<MapControl> ();
		mc.mapCellNumber[(int)gridPosition.y, (int)gridPosition.x] = newCell;

		for (int i = 0; i < 3; i++) {
			for (int j = 0; j < 3; j++) {
				if (mc.mapStarMap [(int)(gridPosition.y * 3) + i, (int)(gridPosition.x * 3) + j] == null) {
					mc.mapStarMap [(int)(gridPosition.y * 3) + i, (int)(gridPosition.x * 3) + j] = new aStarPoint ();
					mc.mapStarMap [(int)(gridPosition.y * 3) + i, (int)(gridPosition.x * 3) + j].x = (int)(gridPosition.x * 3) + j;
					mc.mapStarMap [(int)(gridPosition.y * 3) + i, (int)(gridPosition.x * 3) + j].y = (int)(gridPosition.y * 3) + i;
				}
			}
		}
		switch (Cell) {
		case -1:
			position = new Vector3 (position.x, 6, position.z);
			MineAble = false;
			Walkable = false;
			for (int i = 0; i < 3; i++) {
				for (int j = 0; j < 3; j++) {
					mc.mapStarMap [(int)(gridPosition.y * 3) + i, (int)(gridPosition.x * 3) + j].Cell = newCell;
					mc.mapStarMap [(int)(gridPosition.y * 3) + i, (int)(gridPosition.x * 3) + j].isTraversable = Walkable;
				}
			}
			break;
		case 0:
			position = new Vector3 (position.x, 6, position.z);
			MineAble = true;
			Walkable = false;
			for (int i = 0; i < 3; i++) {
				for (int j = 0; j < 3; j++) {
					mc.mapStarMap [(int)(gridPosition.y * 3) + i, (int)(gridPosition.x * 3) + j].Cell = newCell;
					mc.mapStarMap [(int)(gridPosition.y * 3) + i, (int)(gridPosition.x * 3) + j].isTraversable = Walkable;
				}
			}
			break;
		case 1:
			position = new Vector3 (position.x, -6, position.z);
			MineAble = false;
			Walkable = true;
			for (int i = 0; i < 3; i++) {
				for (int j = 0; j < 3; j++) {
					mc.mapStarMap [(int)(gridPosition.y * 3) + i, (int)(gridPosition.x * 3) + j].Cell = newCell;
					mc.mapStarMap [(int)(gridPosition.y * 3) + i, (int)(gridPosition.x * 3) + j].isTraversable = Walkable;
				}
			}
			break;
		case 2:
			position = new Vector3 (position.x, -6, position.z);
			MineAble = false;
			Walkable = true;
			for (int i = 0; i < 3; i++) {
				for (int j = 0; j < 3; j++) {
					mc.mapStarMap [(int)(gridPosition.y * 3) + i, (int)(gridPosition.x * 3) + j].Cell = newCell;
					mc.mapStarMap [(int)(gridPosition.y * 3) + i, (int)(gridPosition.x * 3) + j].isTraversable = Walkable;
				}
			}
			break;
		default:
			Debug.Log ("Cell Type Not Found");
			break;
		}
		if (mc.fullyGenerated) {
			mc.generateMesh ();
			mc.generateTextures ();
		}
	}
	public void SelectCell(){
		Selected = !Selected;
		if (Selected) {
			HiveMind.selectedCells.Add(this);
			mapControl.generateTextures ();
		} else {
			assignedWorkerNorth.Clear();
			assignedWorkerSouth.Clear();
			assignedWorkerEast.Clear();
			assignedWorkerWest.Clear();

			HiveMind.selectedCells.Remove (this);
			mapControl.generateTextures ();
		}
	}
	bool isClaimable(){
		if (mapControl.mapCells [(int)gridPosition.y - 1, (int)gridPosition.x].Cell == 2)
			return true;
		else if (mapControl.mapCells [(int)gridPosition.y + 1, (int)gridPosition.x].Cell == 2)
			return true;
		else if (mapControl.mapCells [(int)gridPosition.y, (int)gridPosition.x - 1].Cell == 2)
			return true;
		else if (mapControl.mapCells [(int)gridPosition.y, (int)gridPosition.x + 1].Cell == 2)
			return true;
		return false;
	}
	public void setPosition(int x, int y){
		gridPosition = new Vector2 (x, y);
	}
	public Vector3 assignPosition(GameObject Worker){
		Vector3 south = Vector3.zero;
		Vector3 north = Vector3.zero;
		Vector3 east = Vector3.zero;
		Vector3 west = Vector3.zero;
		if(mapControl.mapCells[(int)gridPosition.y-1,(int)gridPosition.x].Walkable){
//			if (!assignedWorkerSouth.Contains (Worker.name) && assignedWorkerSouth.Count < 3) {
//				assignedWorkerSouth.Add (Worker.name);
//
//			}
			if (assignedWorkerSouth.Count < 3 /*|| assignedWorkerSouth.Contains(Worker.name)*/) {
				//switch (assignedWorkerSouth.IndexOf (Worker.name)) {
				switch (assignedWorkerSouth.Count) {
				case 0:
					south = getClosetPoint(new Vector3 (position.x, Worker.transform.position.y, position.z) + new Vector3 (0, 0, -8f));
					break;
				case 1:
					south = getClosetPoint(new Vector3 (position.x, Worker.transform.position.y, position.z) + new Vector3 (3.5f, 0, -8f));
					break;
				case 2:
					south = getClosetPoint(new Vector3 (position.x, Worker.transform.position.y, position.z) + new Vector3 (-3.5f, 0, -8f));
					break;
				}
			}
		}
		if(mapControl.mapCells[(int)gridPosition.y+1,(int)gridPosition.x].Walkable){
//			if (!assignedWorkerNorth.Contains (Worker.name) && assignedWorkerNorth.Count < 3) {
//				assignedWorkerNorth.Add (Worker.name);
//
//			}
			if (assignedWorkerNorth.Count < 3 /*|| assignedWorkerNorth.Contains(Worker.name)*/) {
				//switch (assignedWorkerNorth.IndexOf (Worker.name)) {
				switch (assignedWorkerNorth.Count) {
				case 0:
					north = getClosetPoint(new Vector3 (position.x, Worker.transform.position.y, position.z) + new Vector3 (0, 0, 8f));
					break;
				case 1:
					north = getClosetPoint(new Vector3 (position.x, Worker.transform.position.y, position.z) + new Vector3 (3.5f, 0, 8f));
					break;
				case 2:
					north = getClosetPoint(new Vector3 (position.x, Worker.transform.position.y, position.z) + new Vector3 (-3.5f, 0, 8f));
					break;
				}
			}
		}
		if(mapControl.mapCells[(int)gridPosition.y,(int)gridPosition.x+1].Walkable){
//			if (!assignedWorkerEast.Contains (Worker.name) && assignedWorkerEast.Count < 3) {
//				assignedWorkerEast.Add (Worker.name);
//
//			}
			if (assignedWorkerEast.Count < 3 /*|| assignedWorkerEast.Contains(Worker.name)*/) {
				//switch (assignedWorkerEast.IndexOf (Worker.name)) {
				switch (assignedWorkerEast.Count) {
				case 0:
					east = getClosetPoint(new Vector3 (position.x, Worker.transform.position.y, position.z) + new Vector3 (8f, 0, 0f));
					break;
				case 1:
					east = getClosetPoint(new Vector3 (position.x, Worker.transform.position.y, position.z) + new Vector3 (8f, 0, 3.5f));
					break;
				case 2:
					east = getClosetPoint(new Vector3 (position.x, Worker.transform.position.y, position.z) + new Vector3 (8f, 0, -3.5f));
					break;
				}
			}
		}
		if(mapControl.mapCells[(int)gridPosition.y,(int)gridPosition.x-1].Walkable){
//			if (!assignedWorkerWest.Contains (Worker.name) && assignedWorkerWest.Count < 3) {
//				assignedWorkerWest.Add (Worker.name);
//
//			}
			if (assignedWorkerWest.Count < 3 /*|| assignedWorkerWest.Contains(Worker.name)*/) {
				//switch (assignedWorkerWest.IndexOf (Worker.name)) {
				switch (assignedWorkerWest.Count) {
				case 0:
					west = getClosetPoint(new Vector3 (position.x, Worker.transform.position.y, position.z) + new Vector3 (-8f, 0, 0f));
					break;
				case 1:
					west = getClosetPoint(new Vector3 (position.x, Worker.transform.position.y, position.z) + new Vector3 (-8f, 0, 3.5f));
					break;
				case 2:
					west = getClosetPoint(new Vector3 (position.x, Worker.transform.position.y, position.z) + new Vector3 (-8f, 0, -3.5f));
					break;
				}
			}
		}
		List<Vector3> sides = new List<Vector3>();
		if (south != Vector3.zero)
			sides.Add (south);
		if (north != Vector3.zero)
			sides.Add (north);
		if (east != Vector3.zero)
			sides.Add (east);
		if (west != Vector3.zero)
			sides.Add (west);
		//Debug.Log (sides.Count);
		if (sides.Count != 0) {
			
			sides.Sort (delegate(Vector3 x, Vector3 y) {
				return Vector3.Distance (x, Worker.transform.position).CompareTo (Vector3.Distance (y, Worker.transform.position));
			});
			if (south == sides [0])
				assignedWorkerSouth.Add (Worker.name);
			if (north == sides [0])
				assignedWorkerNorth.Add(Worker.name);
			if (east == sides [0])
				assignedWorkerEast.Add(Worker.name);
			if (west == sides [0])
				assignedWorkerWest.Add(Worker.name);
			return sides [0];
		}else
			return Worker.transform.position;
	}
	public int availableMineSpace(){
		int sides = 0;
		Map_Cell[,] Map = GameObject.Find ("MapController").GetComponent<MapControl> ().mapCells;
		if(Map[(int)gridPosition.y-1,(int)gridPosition.x].Walkable){
			sides++;
		}
		if(Map[(int)gridPosition.y+1,(int)gridPosition.x].Walkable){
			sides++;
		}
		if(Map[(int)gridPosition.y,(int)gridPosition.x-1].Walkable){
			sides++;
		}
		if(Map[(int)gridPosition.y,(int)gridPosition.x+1].Walkable){
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
	Vector3 getClosetPoint(Vector3 currentPos){
		Vector3 bestTarget = new Vector3();
		float closestDist = Mathf.Infinity;
		foreach(aStarPoint v in mapControl.mapStarMap){
			float dist = Vector3.Distance (v.Location,currentPos);
			if(dist < closestDist){
				closestDist = dist;
				bestTarget = v.Location;
			}
		}
		bestTarget = new Vector3 (bestTarget.x,currentPos.y,bestTarget.z);
		return bestTarget;
	}
	public bool Claimer{
		get{
			return hasClaimer;
		}
		set{
			hasClaimer = value;
		}
	}
	public HiveMindAI Hive{
		set{
			HiveMind = value;
		}
	}
	public void Attack(int dmg){
		health -= dmg;
		if(health <= 0){
			HiveMind.selectedCells.Remove(this);
			ChangeCell (1);
			SelectCell ();
			health = 1;
		} 
	}
}

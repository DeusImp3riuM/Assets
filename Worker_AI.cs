using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
public class Worker_AI : MonoBehaviour {

	NavMeshAgent agent;
	MapControl mapController;
	// Use this for initialization
	void Start () {
		mapController = GameObject.Find ("MapController").GetComponent<MapControl> ();
		agent = gameObject.GetComponent<NavMeshAgent> ();
		agent.autoRepath = true;
		agent.speed = 30;
	}

	// Update is called once per frame
	void Update () {
		if (mapController.selectedCells.Count != 0) {
			Vector3 destination = transform.position;
			List<GameObject> GO = new List<GameObject>();
			foreach (GameObject s in mapController.selectedCells) {
				if (findSuitable (s) != null) {
					GO.Add (s);
				}
			}
			foreach(GameObject s in GO){
				if (s.GetComponent<Map_CellControl>().availableSpace(gameObject.name) > 0) {
					findSuitableLocation (s);
					if (Vector3.Distance (transform.position, s.transform.position) <= 9) {
						MineCell (s);
					}
				}
			}
		} else {
			agent.SetDestination (transform.position);
			if (Vector3.Distance (transform.position, agent.destination) < 0.5f) {
				agent.velocity = Vector3.zero;
				agent.Stop ();
			}
		}
	}
	int attackLimit = 50;
	int attackCount = 50;
	void MineCell(GameObject cell){
		if (attackCount >= attackLimit) {
			cell.GetComponent<Map_CellControl> ().health -= 1;
			attackCount = 0;
			Debug.Log(gameObject.name+".. Pick!");
		} else {
			attackCount++;
		}
	}
	GameObject findSuitable(GameObject s){
		if (mapController.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y - 1, (int)s.GetComponent<Map_CellControl> ().gridPosition.x].GetComponent<Map_CellControl> ().Cell == 1) {
			return mapController.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y - 1, (int)s.GetComponent<Map_CellControl> ().gridPosition.x];
		} else if (mapController.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y + 1, (int)s.GetComponent<Map_CellControl> ().gridPosition.x].GetComponent<Map_CellControl> ().Cell == 1) {
			return mapController.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y + 1, (int)s.GetComponent<Map_CellControl> ().gridPosition.x];
		} else if (mapController.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y, (int)s.GetComponent<Map_CellControl> ().gridPosition.x - 1].GetComponent<Map_CellControl> ().Cell == 1) {
			return mapController.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y, (int)s.GetComponent<Map_CellControl> ().gridPosition.x - 1];
		} else if (mapController.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y, (int)s.GetComponent<Map_CellControl> ().gridPosition.x + 1].GetComponent<Map_CellControl> ().Cell == 1) {
			return mapController.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y, (int)s.GetComponent<Map_CellControl> ().gridPosition.x + 1];
		} else {
			return null;
		}
	}
	void findSuitableLocation(GameObject s){
		Vector3 destination = transform.position;
		List<string> AssignedWorkers = new List<string>();
		if (mapController.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y - 1, (int)s.GetComponent<Map_CellControl> ().gridPosition.x].GetComponent<Map_CellControl> ().Cell == 1) {
			AssignedWorkers = s.GetComponent<Map_CellControl> ().assignedWorkerSouth;
			if (AssignedWorkers.Contains (gameObject.name) || AssignedWorkers.Count < 3) {
				if(!AssignedWorkers.Contains(gameObject.name)){
					AssignedWorkers.Add (gameObject.name);
				}
				int position = AssignedWorkers.IndexOf(gameObject.name);
				switch (position) {
				case 0:
					destination = mapController.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y, (int)s.GetComponent<Map_CellControl> ().gridPosition.x].transform.position + new Vector3 (0, 0, -6.5f);
					destination.Set (destination.x, transform.position.y, destination.z);
					agent.SetDestination (destination);
					break;
				case 1:
					destination = mapController.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y, (int)s.GetComponent<Map_CellControl> ().gridPosition.x].transform.position + new Vector3 (3.5f, 0, -6.5f);
					destination.Set (destination.x, transform.position.y, destination.z);
					agent.SetDestination (destination);
					break;
				case 2:
					destination = mapController.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y, (int)s.GetComponent<Map_CellControl> ().gridPosition.x].transform.position + new Vector3 (-3.5f, 0, -6.5f);
					destination.Set (destination.x, transform.position.y, destination.z);
					agent.SetDestination (destination);
					break;
				default:
					break;
				}
			}

		}
		if (mapController.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y + 1, (int)s.GetComponent<Map_CellControl> ().gridPosition.x].GetComponent<Map_CellControl> ().Cell == 1 && destination == transform.position) {
			AssignedWorkers = s.GetComponent<Map_CellControl> ().assignedWorkerNorth;
			if (AssignedWorkers.Contains (gameObject.name) || AssignedWorkers.Count < 3) {
				if(!AssignedWorkers.Contains(gameObject.name)){
					AssignedWorkers.Add (gameObject.name);
				}
				int position = AssignedWorkers.IndexOf (gameObject.name);
				switch (position) {
				case 0:
					destination = mapController.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y, (int)s.GetComponent<Map_CellControl> ().gridPosition.x].transform.position + new Vector3 (0, 0, 6.5f);
					destination.Set (destination.x, transform.position.y, destination.z);
					agent.SetDestination (destination);
					break;
				case 1:
					destination = mapController.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y, (int)s.GetComponent<Map_CellControl> ().gridPosition.x].transform.position + new Vector3 (3.5f, 0, 6.5f);
					destination.Set (destination.x, transform.position.y, destination.z);
					agent.SetDestination (destination);
					break;
				case 2:
					destination = mapController.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y, (int)s.GetComponent<Map_CellControl> ().gridPosition.x].transform.position + new Vector3 (-3.5f, 0, 6.5f);
					destination.Set (destination.x, transform.position.y, destination.z);
					agent.SetDestination (destination);
					break;
				default:
					break;
				}
			}
			//return mapController.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y, (int)s.GetComponent<Map_CellControl> ().gridPosition.x].transform.position + new Vector3 (0,0,6.5f);
		}
		if (mapController.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y, (int)s.GetComponent<Map_CellControl> ().gridPosition.x - 1].GetComponent<Map_CellControl> ().Cell == 1 && destination == transform.position) {
			AssignedWorkers = s.GetComponent<Map_CellControl> ().assignedWorkerWest;
			if (AssignedWorkers.Contains (gameObject.name) || AssignedWorkers.Count < 3) {
				if(!AssignedWorkers.Contains(gameObject.name)){
					AssignedWorkers.Add (gameObject.name);
				}
				int position = AssignedWorkers.IndexOf (gameObject.name);
				switch (position) {
				case 0:
					destination = mapController.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y, (int)s.GetComponent<Map_CellControl> ().gridPosition.x].transform.position + new Vector3 (-6.5f, 0, 0);
					destination.Set (destination.x, transform.position.y, destination.z);
					agent.SetDestination (destination);
					break;
				case 1:
					destination = mapController.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y, (int)s.GetComponent<Map_CellControl> ().gridPosition.x].transform.position + new Vector3 (-6.5f, 0, 3.5f);
					destination.Set (destination.x, transform.position.y, destination.z);
					agent.SetDestination (destination);
					break;
				case 2:
					destination = mapController.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y, (int)s.GetComponent<Map_CellControl> ().gridPosition.x].transform.position + new Vector3 (-6.5f, 0, -3.5f);
					destination.Set (destination.x, transform.position.y, destination.z);
					agent.SetDestination (destination);
					break;
				default:
					break;
				}
			}
			//return mapController.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y, (int)s.GetComponent<Map_CellControl> ().gridPosition.x].transform.position + new Vector3 (-6.5f,0,0);
		}
		if (mapController.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y, (int)s.GetComponent<Map_CellControl> ().gridPosition.x + 1].GetComponent<Map_CellControl> ().Cell == 1 && destination == transform.position) {
			AssignedWorkers = s.GetComponent<Map_CellControl> ().assignedWorkerEast;
			if (AssignedWorkers.Contains (gameObject.name) || AssignedWorkers.Count < 3) {
				if(!AssignedWorkers.Contains(gameObject.name)){
					AssignedWorkers.Add (gameObject.name);
				}
				int position = AssignedWorkers.IndexOf (gameObject.name);
				switch (position) {
				case 0:
					destination = mapController.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y, (int)s.GetComponent<Map_CellControl> ().gridPosition.x].transform.position + new Vector3 (6.5f, 0, 0);
					destination.Set (destination.x, transform.position.y, destination.z);
					agent.SetDestination (destination);
					break;
				case 1:
					destination = mapController.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y, (int)s.GetComponent<Map_CellControl> ().gridPosition.x].transform.position + new Vector3 (6.5f, 0, 3.5f);
					destination.Set (destination.x, transform.position.y, destination.z);
					agent.SetDestination (destination);
					break;
				case 2:
					destination = mapController.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y, (int)s.GetComponent<Map_CellControl> ().gridPosition.x].transform.position + new Vector3 (6.5f, 0, -3.5f);
					destination.Set (destination.x, transform.position.y, destination.z);
					agent.SetDestination (destination);
					break;
				default:
					break;
				}
			}
			//return mapController.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y, (int)s.GetComponent<Map_CellControl> ().gridPosition.x].transform.position + new Vector3 (6.5f,0,0);
		} else {
			//return new Vector3(0,0,0);
		}
	}
}
*/
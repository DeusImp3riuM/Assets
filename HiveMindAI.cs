using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class HiveMindAI : MonoBehaviour {
	
	List<GameObject> IdleWorkers = new List<GameObject>();
	List<GameObject> AllWorkers = new List<GameObject>();
	MapControl mc = null;
	List<GameObject> ReachableCells = new List<GameObject>();
	// Use this for initialization
	void Start () {
		mc = GameObject.Find ("MapController").GetComponent<MapControl> ();
	}
	
	// Update is called once per frame
	void Update () {
		// Find all Workers.
		AllWorkers = new List<GameObject>(GameObject.FindGameObjectsWithTag ("Worker"));
		// Loop to find all workers currently idle.
		foreach(GameObject w in AllWorkers){ 
			if(w.GetComponent<Worker_AI2>().State.Equals("Idle")){
				IdleWorkers.Add (w);
			}
		}
		List<GameObject> AvailableJobs = new List<GameObject> ();
		if (mc.selectedCells.Count > 0 && IdleWorkers.Count > 0) {
			foreach (GameObject cell in mc.selectedCells.ToArray()) {
				if (findSuitable (cell) != null) {
					ReachableCells.Add (cell);
				}
			}
			foreach(GameObject cell in ReachableCells){
				if(cell.GetComponent<Map_CellControl>().availableSpace() > 0){
					AvailableJobs.Add (cell);
				}
			}



			/*
			foreach(GameObject worker in IdleWorkers.ToArray()){
				foreach (GameObject cell in ReachableCells.ToArray()) {
					if(cell.GetComponent<Map_CellControl>().availableSpace() > 0){
						AvailableJobs.Add (cell);
					}
				}
				if (AvailableJobs.Count != 0) {
					GameObject nearestJob = AvailableJobs [0];
					foreach (GameObject cell in AvailableJobs.ToArray()) {
						if (Vector3.Distance (worker.transform.position, cell.transform.position) < Vector3.Distance (worker.transform.position, nearestJob.transform.position)) {
							nearestJob = cell;
						}
					}
					worker.GetComponent<Worker_AI2> ().assignJob (nearestJob, "Mine", 0);
				}
				AvailableJobs.Clear();
			}
			*/
		}
		if(AvailableJobs.Count != 0){
			IdleWorkers.Sort (delegate(GameObject x, GameObject y) {
				return Vector3.Distance(AvailableJobs[0].transform.position,x.transform.position)
					.CompareTo(
						Vector3.Distance(AvailableJobs[0].transform.position,y.transform.position));
			});
			IdleWorkers[0].GetComponent<Worker_AI2> ().assignJob (AvailableJobs[0], "Mine", 0);
		}
		AvailableJobs.Clear ();
		ReachableCells.Clear();
		IdleWorkers.Clear ();
		AllWorkers.Clear ();
	}
	GameObject findSuitable(GameObject s){
		if (mc.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y - 1, (int)s.GetComponent<Map_CellControl> ().gridPosition.x].GetComponent<Map_CellControl> ().Cell == 1) {
			return mc.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y - 1, (int)s.GetComponent<Map_CellControl> ().gridPosition.x];
		} else if (mc.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y + 1, (int)s.GetComponent<Map_CellControl> ().gridPosition.x].GetComponent<Map_CellControl> ().Cell == 1) {
			return mc.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y + 1, (int)s.GetComponent<Map_CellControl> ().gridPosition.x];
		} else if (mc.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y, (int)s.GetComponent<Map_CellControl> ().gridPosition.x - 1].GetComponent<Map_CellControl> ().Cell == 1) {
			return mc.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y, (int)s.GetComponent<Map_CellControl> ().gridPosition.x - 1];
		} else if (mc.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y, (int)s.GetComponent<Map_CellControl> ().gridPosition.x + 1].GetComponent<Map_CellControl> ().Cell == 1) {
			return mc.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y, (int)s.GetComponent<Map_CellControl> ().gridPosition.x + 1];
		} else {
			return null;
		}
	}
	/*
	public List<Vector3> Vector3ToPoint(Vector3 Position){
		List<Vector3> test = mc.mapStarMap;
		test.Sort (delegate(Vector3 x, Vector3 y) {
			return Vector3.Distance(x,Position).CompareTo(
				Vector3.Distance(y,Position));
		});
		return test;
	}*/
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

public class HiveMindAI : MonoBehaviour {
	
	/* Diagnostics */
	//Stopwatch sw = new Stopwatch();

	/* Work Assigning */
	public List<GameObject> selectedCells = new List<GameObject> ();
	List<GameObject> IdleWorkers = new List<GameObject>();
	List<GameObject> AllWorkers = new List<GameObject>();
	MapControl mc = null;
	List<GameObject> ReachableCells = new List<GameObject>();
	public List<GameObject> IsClaimable = new List<GameObject> ();
	List<Job> AvailableJobs = new List<Job> ();

	/* Path Finding */
	public Queue PathQueue = new Queue ();
	List<PathRequest> PathRequests = new List<PathRequest> ();
	PathFinding Path = null;
	int timerLimit = 5;
	int timer = 0;

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

		if (IdleWorkers.Count > 0) {
			if (selectedCells.Count > 0 ) {
				foreach (GameObject cell in selectedCells.ToArray()) {
					if (findSuitableMine (cell) != null) {
						ReachableCells.Add (cell);
					}
				}
				foreach (GameObject cell in ReachableCells) {
					if (cell.GetComponent<Map_CellControl> ().availableMineSpace () > 0) {
						AvailableJobs.Add (new Job (cell, "Mine"));
					}
				}
			}
			if(IsClaimable.Count > 0){
				foreach(GameObject cell in IsClaimable){
					if(!cell.GetComponent<Map_CellControl>().Claimer)
						AvailableJobs.Add(new Job(cell,"ClaimGround"));
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
			//*
			IdleWorkers.Sort (delegate(GameObject x, GameObject y) {
				return Vector3.Distance(AvailableJobs[0].Cell.transform.position,x.transform.position)
					.CompareTo(
						Vector3.Distance(AvailableJobs[0].Cell.transform.position,y.transform.position));
			});
			//IdleWorkers[0].GetComponent<Worker_AI2> ().assignJob (AvailableJobs[0], "Mine", 0);
			//*/

			//*
			AvailableJobs.Sort (delegate(Job x, Job y) {
				return Vector3.Distance(IdleWorkers[0].transform.position,x.Cell.transform.position).CompareTo(
					Vector3.Distance(IdleWorkers[0].transform.position,y.Cell.transform.position));
			});
			IdleWorkers[0].GetComponent<Worker_AI2> ().assignJob (AvailableJobs[0].Cell, AvailableJobs[0].Type, 0);
			//*/
		}
		AvailableJobs.Clear ();
		ReachableCells.Clear();
		IdleWorkers.Clear ();
		AllWorkers.Clear ();

		ProcessRequests ();
		if (Path != null) {
			if(Path.Update()){
				Path = null;
			}
		}
	}

	public void ProcessRequests(){
		if (PathRequests.Count > 0 && Path == null && timer == timerLimit) {
			
			Path = new PathFinding (PathRequests [0].Worker, PathRequests [0].Position, PathRequests [0].Destination);
			PathRequests.Remove (PathRequests [0]);
			Path.Start ();
		}
		if (timer == timerLimit)
			timer = 0;
		timer++;
	}
	GameObject findSuitableMine(GameObject s){
		if (mc.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y - 1, (int)s.GetComponent<Map_CellControl> ().gridPosition.x].GetComponent<Map_CellControl> ().Walkable) {
			return mc.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y - 1, (int)s.GetComponent<Map_CellControl> ().gridPosition.x];
		} else if (mc.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y + 1, (int)s.GetComponent<Map_CellControl> ().gridPosition.x].GetComponent<Map_CellControl> ().Walkable) {
			return mc.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y + 1, (int)s.GetComponent<Map_CellControl> ().gridPosition.x];
		} else if (mc.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y, (int)s.GetComponent<Map_CellControl> ().gridPosition.x - 1].GetComponent<Map_CellControl> ().Walkable) {
			return mc.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y, (int)s.GetComponent<Map_CellControl> ().gridPosition.x - 1];
		} else if (mc.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y, (int)s.GetComponent<Map_CellControl> ().gridPosition.x + 1].GetComponent<Map_CellControl> ().Walkable) {
			return mc.mapCells [(int)s.GetComponent<Map_CellControl> ().gridPosition.y, (int)s.GetComponent<Map_CellControl> ().gridPosition.x + 1];
		} else {
			return null;
		}
	}
	public List <Vector3> PathFind(aStarPoint NodeStart, aStarPoint NodeFinish){
		Heap<aStarPoint> openList = new Heap<aStarPoint>(mc.mapStarMap.GetLength(0) * mc.mapStarMap.GetLength(1));
		List<aStarPoint> closedList = new List<aStarPoint> ();
		openList.Add (NodeStart);
		int t = 0;
		while(openList.Count > 0){
			t++;
			aStarPoint currentNode = openList.RemoveFirst();
			closedList.Add (currentNode);
			if (currentNode == NodeFinish) {
				List <Vector3> Path = new List<Vector3> ();
				aStarPoint Node = NodeFinish;
				while(Node != NodeStart){
					Path.Add (Node.Location);
					Node = Node.parent;
				}
				foreach(aStarPoint a in mc.mapStarMap){
					a.reset ();
				}
				Path.Reverse();
				return Path;
			}

			List<aStarPoint> neighbours = findValidNeighbours (currentNode);
			foreach (aStarPoint node in neighbours) {
				if (closedList.Contains (node))
					continue;
				int g = Mathf.RoundToInt (Vector2.Distance (node.gridPosition (), currentNode.gridPosition ()) * 10);
				int h = GetDistance (node, NodeFinish);
				int f = g + h;
				if (f < node.f || !openList.Contains (node)) {
					node.f = f;
					node.h = h;
					node.g = g;
					node.parent = currentNode;
					if (!openList.Contains (node)) {
						openList.Add (node);
						openList.UpdateItem (node);
					}
				}
			}
		}
		return null;
	}
	public List<aStarPoint> findValidNeighbours(aStarPoint Node){
		List<aStarPoint> neighbours = new List<aStarPoint> ();
		for(int x = -1; x < 2; x++){
			for(int y = -1; y < 2; y++){
				if (x == 0 && y == 0)
					continue;
				if (mc.mapStarMap [Node.y + y, Node.x - x].isTraversable) {
					if (x != 0 && y != 0) {
						if (!mc.mapStarMap [Node.y, Node.x - x].isTraversable || !mc.mapStarMap [Node.y + y, Node.x].isTraversable) {

						} else {
							neighbours.Add (mc.mapStarMap [Node.y + y, Node.x - x]);
						}

					} else
						neighbours.Add (mc.mapStarMap [Node.y + y, Node.x - x]);
				}
			}
		}
		return neighbours;
	}
	public int GetDistance(aStarPoint nodeA, aStarPoint nodeB){
		int x = Mathf.Abs (nodeA.x - nodeB.x);
		int y = Mathf.Abs (nodeA.y - nodeB.y);
		if (x < y)
			return 14 * y + 10 * (x - y);
		return 14 * x + 10 * (y - x);
	}
	public List<Vector3> SimplifyPath(List<Vector3> oldPath){
		List<Vector3> newPath = new List<Vector3> ();
		Vector2 dirOld = Vector2.zero;
		for(int i = 1; i < oldPath.Count;i++){
			Vector2 dirNew = new Vector2 (oldPath [i - 1].x - oldPath [i].x, oldPath [i - 1].z - oldPath [i].z);
			if (dirNew != dirOld) {
				newPath.Add (oldPath [i]);
			}
			dirOld = dirNew;
		}
		return newPath;
	}

	public void RequestPath(PathRequest Form){
		if(PathRequests.Find (x => x.Worker == Form.Worker) == null){
			if (Path != null) {
				if(Path.Worker != Form.Worker){
					PathRequests.Add (Form);
				}
			} else {
				PathRequests.Add (Form);
			}
		}
	}
		
}


public class Job{
	private GameObject cell;
	private string type;

	public Job(GameObject _cell, string _job){
		cell = _cell;
		type = _job;
	}
	public GameObject Cell{
		get{
			return cell;
		}
	}
	public string Type{
		get{
			return type;
		}
	}
}

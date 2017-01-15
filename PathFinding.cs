using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PathFinding : ThreadJob{
	
	MapControl mc = GameObject.Find("MapController").GetComponent<MapControl>();
	public GameObject Worker;
	public aStarPoint Location;
	public aStarPoint Destination;
	List<Vector3> Pathfound = null;
	public bool endedTest = false;

	public PathFinding(GameObject _Worker, aStarPoint _Location, aStarPoint _Destination){
		Worker = _Worker;
		Location = _Location;
		Destination = _Destination;
		endedTest = false;
	}

	protected override void ThreadFunction(){
		Heap<aStarPoint> openList = new Heap<aStarPoint>(mc.mapStarMap.GetLength(0) * mc.mapStarMap.GetLength(1));
		List<aStarPoint> closedList = new List<aStarPoint> ();
		openList.Add (Location);
		int t = 0;
		while(openList.Count > 0){
			t++;
			aStarPoint currentNode = openList.RemoveFirst();
			closedList.Add (currentNode);
			/*
			if (currentNode == Destination) {
				List <Vector3> Path = new List<Vector3> ();
				aStarPoint Node = Destination;
				while(Node != Location){
					Path.Add (Node.Location);
					Node = Node.parent;
				}
				foreach(aStarPoint a in mc.mapStarMap){
					a.reset ();
				}
				Path = SimplifyPath (Path);
				Path.Reverse();
				if(!Path.Contains(Destination.Location)){
					Path.Add (Destination.Location);
				}

				//sw.Stop ();
				//UnityEngine.Debug.Log (sw.ElapsedMilliseconds+"ms");
				//sw.Reset ();
				Pathfound = Path;
			}
			*/
			if (currentNode == Destination) {
				List <Vector3> Path = new List<Vector3> ();
				aStarPoint Node = Destination;
				while(Node != Location){
					Path.Add (Node.Location);
					Node = Node.parent;
				}
				foreach(aStarPoint a in mc.mapStarMap){
					a.reset ();
				}
				Path = CleanRoute (Path);
				Path.Reverse();
				if(!Path.Contains(Destination.Location)){
					Path.Add (Destination.Location);
				}
				Path.Insert (0,Location.Location);
				//SimplifyPath (Path);
				//sw.Stop ();
				//UnityEngine.Debug.Log (sw.ElapsedMilliseconds+"ms");
				//sw.Reset ();
				Pathfound = Path;
			}

			List<aStarPoint> neighbours = findValidNeighbours (currentNode);
			foreach (aStarPoint node in neighbours) {
				if (closedList.Contains (node))
					continue;
				int g = Mathf.RoundToInt (Vector2.Distance (node.gridPosition (), currentNode.gridPosition ()) * 10);
				int h = GetDistance (node, Destination);
				int f = g + h;
				if (f < node.f || !openList.Contains (node)) {
					node.f = f;
					node.h = h;
					node.g = g;
					node.parent = currentNode;
					if (!openList.Contains (node)) {
						//CalculatePosition (node,openList);
						openList.Add (node);
						openList.UpdateItem (node);
					}
				}
			}
		}
	}

	protected override void OnFinished(){
		Pathfound = SimplifyPath (Pathfound);
		Worker.GetComponent<Worker_AI2> ().destinationPath = Pathfound;
		Worker = null;
		Location = null;
		Destination = null;
		endedTest = true;

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
	public List<Vector3> CleanRoute(List<Vector3> oldPath){
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
	aStarPoint getClosetPoint(Vector3 currentPos){
		aStarPoint bestTarget = new aStarPoint ();
		float closestDist = -1f;
		//List<aStarPoint> liststar = new List<aStarPoint> ();
		foreach(aStarPoint v in mc.mapStarMap){
			//*
			float dist = Vector2.Distance (new Vector2(v.Location.x,v.Location.z),(new Vector2(currentPos.x,currentPos.z)));
			if(dist < closestDist || closestDist == -1f){
				closestDist = dist;
				bestTarget = v;
			}
			//*/
			//liststar.Add (v);
		}

		//bestTarget = liststar [0];
		//liststar.Clear ();
		return bestTarget;
	}

	public List<Vector3> SimplifyPath(List<Vector3> Path){
		LayerMask rayMask = LayerMask.GetMask ("Terrain");
		int start = 0;
		int offset = 2;
		Vector3 _loc = new Vector3 ();
		Vector3 _des = new Vector3 ();
		RaycastHit hit;
		while (start < Path.Count - 1 && start + offset < Path.Count - 1) {
			_loc = new Vector3 (Path [start].x, Worker.transform.position.y, Path [start].z);
			_des = new Vector3 (Path [start + offset].x, Worker.transform.position.y, Path [start + offset].z);
			if (Physics.Linecast (_loc, _des, out hit, rayMask)) {
				start++;
			} else {
				Path.RemoveAt (start+1);
			}

		}
		return Path;
	}
}



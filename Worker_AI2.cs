using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class Worker_AI2 : MonoBehaviour {
	
	HiveMindAI HiveMind;

	public string State = "Idle";
	public Map_Cell jobTarget = null;
	public Vector3 jobDestination = new Vector3 ();
	public Vector3 astardestination = new Vector3();
	public aStarPoint destinationPoint = null;
	public List<Vector3> destinationPath = null;
	MapControl mapController;
	public int pathPosition = 0;
	public float speed = 1;
	public int mineSpeed = 1;


	// Use this for initialization
	void Start () {
		HiveMind = GameObject.Find ("HiveMind").GetComponent<HiveMindAI>();
		mapController = GameObject.Find ("MapController").GetComponent<MapControl> ();
		//astardestination = getClosetPoint (transform.position + new Vector3 (30, 0, 35)).Location;
		astardestination = transform.position;
	}
	
	// Update is called once per frame
	bool toggle = true;
	void Update () {
		/*  Debug Code  */
		if(Input.GetKeyDown("l")){
			toggle = !toggle;
		}
		if (toggle) {
			speed = 1;
			mineSpeed = 1;
		} else {
			speed = 100;
			mineSpeed = 100;
		}	
		/*  End Of Debug Code  */

		if(State.Equals("Mine")){
			//UnityEngine.Debug.Log (destinationPath);
			if (jobTarget.Selected == false) {
				State = "Idle";
				jobTarget = null;
				astardestination = transform.position;
			} else {
				if (destinationPath == null && transform.position != new Vector3 (astardestination.x, transform.position.y, astardestination.z))
					HiveMind.RequestPath (new PathRequest (this.gameObject, getClosetPoint (transform.position), getClosetPoint (astardestination)));
					//destinationPath = PathFind (getClosetPoint (transform.position), getClosetPoint (astardestination));
				
				//astardestination = jobTarget.GetComponent<Map_CellControl>().assignPosition(gameObject);
				if (Vector3.Distance (transform.position, new Vector3 (destinationPoint.Location.x, transform.position.y, destinationPoint.Location.z)) <= 8) {
					MineCell (jobTarget);
				}
			}
			if (transform.position != astardestination) {
				if (destinationPath != null ) {
					
					try{
						if (transform.position.Equals (new Vector3 (destinationPath [pathPosition].x, transform.position.y, destinationPath [pathPosition].z))) {
							if (pathPosition < destinationPath.Count-1) {
								pathPosition++;
							}
						}
					}
					catch(System.ArgumentOutOfRangeException ex){
						Debug.Log (ex);
					}
					if (pathPosition >= destinationPath.Count) {
						pathPosition = destinationPath.Count - 1;
					}
					//Debug.Log (pathPosition + " : " + (destinationPath.Count-1));
					transform.position = Vector3.MoveTowards (transform.position, new Vector3 (destinationPath [pathPosition].x, transform.position.y, destinationPath [pathPosition].z), speed);
				}
				//	transform.position = Vector3.MoveTowards (transform.position, new Vector3 (astardestination.x, transform.position.y, astardestination.z), 1f);
			} else {
				destinationPath = null;
				pathPosition = 0;
			}
		}
		else if(State.Equals("ClaimGround")){
			
			if (jobTarget.Cell == 2) {
				State = "Idle";
				jobTarget = null;
				astardestination = transform.position;
			} else {
				
				astardestination = new Vector3 (jobTarget.position.x, transform.position.y, jobTarget.position.z);

				if (destinationPath == null && transform.position != new Vector3 (astardestination.x, transform.position.y, astardestination.z)){
					HiveMind.RequestPath (new PathRequest (this.gameObject, getClosetPoint (transform.position), getClosetPoint (astardestination)));
			}

				if (Vector3.Distance(transform.position,new Vector3(destinationPoint.Location.x,transform.position.y,destinationPoint.Location.z)) <= 8) {
					ClaimCell (jobTarget);
				}

			}
			if (transform.position != astardestination) {
				if (destinationPath != null ) {

					if (transform.position.Equals (new Vector3 (destinationPath [pathPosition].x, transform.position.y, destinationPath [pathPosition].z))) {
						if (pathPosition < destinationPath.Count - 1) {
							pathPosition++;
						}
					}
					
					if (pathPosition >= destinationPath.Count) {
						pathPosition = destinationPath.Count - 1;
					}
					transform.position = Vector3.MoveTowards (transform.position, new Vector3 (destinationPath [pathPosition].x, transform.position.y, destinationPath [pathPosition].z), speed);
				}
				//	transform.position = Vector3.MoveTowards (transform.position, new Vector3 (astardestination.x, transform.position.y, astardestination.z), 1f);
			}  else {
				destinationPath = null;
				pathPosition = 0;
			}
		
		}else if(State.Equals("Idle")){
			destinationPath = null;
			destinationPoint = null;
			astardestination = Vector3.zero;
		}
	}

	public void assignJob(Map_Cell cell, string job, int cellNumber){
		jobTarget = cell;
		State = job;
		astardestination = jobTarget.assignPosition (gameObject);
		destinationPoint = getClosetPoint (astardestination);
		if (job.Equals ("ClaimGround")) {
			jobTarget.Claimer = true;
		}
	}

	int attackLimit = 5;
	int attackCount = 5;
	void MineCell(Map_Cell cell){
		if (attackCount >= attackLimit) {
			cell.Attack(mineSpeed);
			attackCount = 0;
		} else
			attackCount++;
	}

	int claimLimit = 100;
	int claimCount = 0;
	void ClaimCell(Map_Cell cell){
		if (claimCount >= claimLimit) {
			cell.ChangeCell (2);
			claimCount = 0;
		} else
			claimCount++;
	}



	public void OnDrawGizmos(){
		/*
		Gizmos.color = Color.black;
		foreach (aStarPoint a in mapController.mapStarMap) {
			Gizmos.DrawWireSphere (a.Location, 0.5f);
		}
		//*/
		/*
		if (destinationPath != null) {
			Gizmos.color = Color.magenta;
			foreach (Vector3 node in destinationPath) {
				Gizmos.DrawWireSphere (node, 0.5f);
			}

			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere (getClosetPoint (transform.position).Location, 1.5f);
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere (getClosetPoint (astardestination).Location, 1.5f);
		}
		//*/
	}

	aStarPoint getClosetPoint(Vector3 currentPos){
		aStarPoint bestTarget = new aStarPoint ();
		float closestDist = -1f;
		//List<aStarPoint> liststar = new List<aStarPoint> ();

		foreach(aStarPoint v in mapController.mapStarMap){
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



}
public class PathRequest{
	public GameObject Worker;
	public aStarPoint Position;
	public aStarPoint Destination;

	public PathRequest(GameObject _Worker, aStarPoint _Position, aStarPoint _Destination){
		Worker = _Worker;
		Position = _Position;
		Destination = _Destination;
	}
}
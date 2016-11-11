using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Worker_AI2 : MonoBehaviour {

	public string State = "Idle";
	public GameObject jobTarget = null;
	public int jobCellNumber = 0;
	public Vector3 jobDestination = new Vector3 ();

	NavMeshAgent agent;
	MapControl mapController;
	// Use this for initialization
	void Start () {
		mapController = GameObject.Find ("MapController").GetComponent<MapControl> ();
		agent = gameObject.GetComponent<NavMeshAgent> ();
		agent.autoRepath = true;
		agent.speed = 30;
		agent.avoidancePriority = 1;
	}
	
	// Update is called once per frame
	void Update () {
		if(State.Equals("Mine")){
			if (jobTarget.GetComponent<Map_CellControl> ().Selected == false) {
				State = "Idle";
				jobTarget = null;
				jobCellNumber = 0;
				agent.SetDestination (transform.position);
			} else {
				agent.SetDestination (jobTarget.GetComponent<Map_CellControl>().assignPosition(gameObject));
				Debug.Log (Vector3.Distance(agent.destination,gameObject.transform.position));
				if(Vector3.Distance(agent.destination,transform.position) == 1){
					MineCell (jobTarget);
				}
			}
		}
		aStar ();
	}

	public void assignJob(GameObject cell, string job, int cellNumber){
		jobTarget = cell;
		State = job;
		jobCellNumber = cellNumber;
		jobDestination = jobTarget.GetComponent<Map_CellControl> ().assignPosition (gameObject);

	}

	int attackLimit = 50;
	int attackCount = 50;
	void MineCell(GameObject cell){
		if (attackCount >= attackLimit) {
			cell.GetComponent<Map_CellControl> ().health -= 1;
			attackCount = 0;
		} else {
			attackCount++;
		}
	}
	public void aStar(){
		int x = 0;
		while(x < 10){
			x++;
		}
	}
}

using UnityEngine;
using System.Collections;

public class CameraScroll : MonoBehaviour {

	int ScrollSpeed = 60;
	int ZoomSpeed = 60;
	bool canMove = false;
	// Use this for initialization
	void Start () {
		transform.Rotate (Vector3.left, 20f);
	}
	
	// Update is called once per frame
	void Update () {
		//*
		if(Input.GetKeyDown("space")){
			canMove = !canMove;
		}
		if (canMove) {
			if (Input.mousePosition.y >= Screen.height * 0.99) {
				transform.Translate (Vector3.forward * Time.deltaTime * ScrollSpeed, Space.World);
			}
			if (Input.mousePosition.y <= Screen.height - Screen.height * 0.99) {
				transform.Translate (Vector3.back * Time.deltaTime * ScrollSpeed, Space.World);
			}
			if (Input.mousePosition.x >= Screen.width * 0.99) {
				transform.Translate (Vector3.right * Time.deltaTime * ScrollSpeed, Space.World);
			}
			if (Input.mousePosition.x <= Screen.width - Screen.width * 0.99) {
				transform.Translate (Vector3.left * Time.deltaTime * ScrollSpeed, Space.World);
			}
		}
		//*/
		if(Input.GetAxis("Mouse ScrollWheel") > 0f){
			transform.Translate (Vector3.forward * Time.deltaTime*ZoomSpeed*2, Space.Self);
		}
		else if(Input.GetAxis("Mouse ScrollWheel") < 0f){
			transform.Translate (Vector3.back * Time.deltaTime*ZoomSpeed*2, Space.Self);
		}
	}
}

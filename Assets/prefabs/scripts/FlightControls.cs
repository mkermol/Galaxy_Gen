using UnityEngine;
using System.Collections;

public class FlightControls : MonoBehaviour {

	public float speed = 50.0f;
	public float sensitivity = 0.25f;
	public bool inverted = true;
	//public bool smooth = true;
	public float acceleration = 0.1f;

	private Vector3 lastMouse = new Vector3(512,256,0);
	private float actSpeed = 0.0f;
	private Vector3 lastDir = new Vector3();

	// Use this for initialization
	void Start () {
		lastMouse = Input.mousePosition;
	}
	
	// Update is called once per frame
	void Update () {
	
		//Mouse Look
		lastMouse = Input.mousePosition - lastMouse;

		if (!inverted)lastMouse.y = -lastMouse.y;
		lastMouse *= sensitivity;

		lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.y, transform.eulerAngles.y +lastMouse.x, 0);
		transform.eulerAngles = lastMouse;


		lastMouse = Input.mousePosition;

		//Movement

		Vector3 dir = new Vector3 ();

		if (Input.GetKey (KeyCode.W)) dir.z += 1.0f;        //move forward
		if (Input.GetKey (KeyCode.S)) dir.z -= 1.0f;       //move back
		if (Input.GetKey (KeyCode.A)) dir.x -= 1.0f;       //strafe left
		if (Input.GetKey (KeyCode.D)) dir.x += 1.0f;       //strafe right

		dir.Normalize ();

		if (dir != Vector3.zero) {
			if (actSpeed < 1.0f)
				actSpeed += acceleration * Time.deltaTime;
			else
				actSpeed = 1.0f;
			lastDir = dir;
		} else {
			if(actSpeed>0.0f)
				actSpeed -= acceleration *Time.deltaTime;
			else
				actSpeed = 0.0f;
		}

		transform.Translate (lastDir * actSpeed * speed * Time.deltaTime);

	}
}

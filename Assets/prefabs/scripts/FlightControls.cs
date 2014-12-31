using UnityEngine;
using System.Collections;

public class FlightControls : MonoBehaviour {

	public float speed = 50.0f;
	public float sensitivity = 0.25f;
	public bool inverted = true;
	//public bool smooth = true;
	public float acceleration = 0.1f;
	public float turnSpeed = 0.5f;

	private Vector3 deltaRotation = new Vector3(0,0,0);
	private Vector3 actSpeed = new Vector3(0,0,0);

	private Vector3 screenCenter;


	// Use this for initialization
	void Start () {

		screenCenter = new Vector3 (Screen.width*0.5f,Screen.height*0.5f,0);
	}
	
	// Update is called once per frame
	void Update () {

		//Mouse look
		screenCenter = new Vector3 (Screen.width*0.5f,Screen.height*0.5f,0);//adjust screen center 

		//calculate x & y axis rotation based on cursor distance from screen center
		if (!Input.GetKey (KeyCode.LeftAlt)) {
			deltaRotation.x = (screenCenter.y - Input.mousePosition.y) * turnSpeed * Time.deltaTime;
			deltaRotation.y = (Input.mousePosition.x - screenCenter.x) * turnSpeed * Time.deltaTime;


			//if keys pressed rotate around z axis

			deltaRotation.z = 0;//reset z axis delta rotation
			if (Input.GetKey (KeyCode.Q))
				deltaRotation.z = Screen.height * 0.5f * turnSpeed * Time.deltaTime;
	
			if (Input.GetKey (KeyCode.E))
				deltaRotation.z = -Screen.height * 0.5f * turnSpeed * Time.deltaTime;
		} else {
			deltaRotation.x=0;
			deltaRotation.y=0;
			deltaRotation.z=0;
		}
				


		transform.Rotate (deltaRotation);//apply rotation


		//speed control
		if (Input.GetKey (KeyCode.T)) speed = 0.0f;
		if (Input.GetKey (KeyCode.Y)) speed = 50.0f;
		if (Input.GetKey (KeyCode.U)) speed = 500.0f;
		if (Input.GetKey (KeyCode.I)) speed = 5000.0f;

		//Movement

		Vector3 dir = new Vector3 ();

		if (Input.GetKey (KeyCode.W)) dir.z += 1.0f;        //move forward
		if (Input.GetKey (KeyCode.S)) dir.z -= 1.0f;       //move back
		if (Input.GetKey (KeyCode.A)) dir.x -= 1.0f;       //strafe left
		if (Input.GetKey (KeyCode.D)) dir.x += 1.0f;       //strafe right

		dir.Normalize ();
		//print ("dir: "+dir);

		if (dir != Vector3.zero) {
			if (actSpeed.magnitude < 1.0f)
				actSpeed += dir * acceleration * Time.deltaTime;
			else
				actSpeed.Normalize();

		} else {
			if(actSpeed.magnitude >0.0f)
				actSpeed -= dir * acceleration *Time.deltaTime;
			else
				actSpeed = new Vector3();
		}

		//print ("actspeed: "+actSpeed);

		transform.Translate (actSpeed * speed * Time.deltaTime);

	}

	float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n){
		// angle in [0,180]
		float angle = Vector3.Angle(a,b);
		float sign = Mathf.Sign(Vector3.Dot(n,Vector3.Cross(a,b)));
		
		// angle in [-179,180]
		float signed_angle = angle * sign;
		
		// angle in [0,360]
		float angle360 =  ((signed_angle) + 360) % 360;
		
		return angle360;
	}
}

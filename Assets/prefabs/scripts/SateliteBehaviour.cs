using UnityEngine;
using System.Collections;
using System;

public class SateliteBehaviour : MonoBehaviour {

	float m,M,r,R;
	double inclination,RAAN,eccentricity,AP,V,meanA; //orbital elements
	Vector3 movement;

	const double G = 6.672* 0.000001 ;//* 0.00000000001;
	Vector3 refVector,n,position;
	private float rx,ry;



	// Use this for initialization
	void Start () {

		//inclination = 0.0;
		//RAAN = 0.0;
		//eccentricity = 0.0;
		//AP = 0.0;
		M = 1000000;
		V = (G * M*m) / (R*R);
		meanA = 0;
		movement = new Vector3 ( 0.0f, 0.0f, 0.0f );
		position = new Vector3 (0.0f, 0.0f, 0.0f);
		refVector = new Vector3 ( 1.0f, 0.0f, 0.0f );
		n = new Vector3 (0.0f, 1.0f, 0.0f);
	}
	
	// Update is called once per frame
	void Update () {


		V = (G * M*m) / (R*R); // calculate mean velocity magnitude at epoch
		meanA = SignedAngleBetween (transform.position-transform.parent.transform.position,refVector, n);//calculate mean anomally at epoch


		//calculate and correct position to stable orbit
		position.x =(float)( R * Math.Cos (Mathf.Deg2Rad * meanA));
		position.z = (float)( R * Math.Sin (Mathf.Deg2Rad * meanA));
		transform.position = position+transform.parent.transform.position;

		//calculate mean velocity vectors
		movement.x = (float)(V * -Math.Sin (Mathf.Deg2Rad*meanA)*Time.deltaTime*GlobalTimer.timeScale);
		movement.z = (float)(V * Math.Cos (Mathf.Deg2Rad*meanA)*Time.deltaTime*GlobalTimer.timeScale);
		transform.Translate (movement ,Space.Self);


		//print ("R: " + R);
		//print ("meanA: " + meanA);
		//print ("v: " + meanV);
		//print ("v.x: " + movement.x);
		//print ("v.z: " + movement.z);
		//print ("-Sin(mA): " + -Math.Sin (Mathf.Deg2Rad*meanA));
		//print ("Cos(mA): " + Math.Cos (Mathf.Deg2Rad*meanA));
	}

	public void initFields(float density,float radius1,float radius2){
		//meanA = SignedAngleBetween (transform.position,refVector, n);
		r = radius1;
		transform.localScale = new Vector3 (r, r, r);
		R = radius2;
		m = density*0.75f*Mathf.PI*r*r*r;

		//rx=
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

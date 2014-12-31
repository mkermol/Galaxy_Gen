using UnityEngine;
using System.Collections;

public class GalaxyParticles : MonoBehaviour {

	public int starsMax = 100;
	public float starSize = 0.01f;
	public float starDistance = 10.0f;
	public float r = 5.0f;


	
	private Transform tx;
	private ParticleSystem.Particle[] points;
	private float starDistanceSqr;

	// Use this for initialization
	void Start () {
		tx = transform;
		starDistanceSqr = starDistance * starDistance;


	}

	private void CreateDisc(){
		points = new ParticleSystem.Particle[starsMax];
		Random.seed = 0;

		for(int i =0; i<starsMax/2;i++){
			Vector3 pos =new Vector3();

			pos = Random.insideUnitSphere * starDistance;// + tx.position;
			pos.z *= 0.1f;
			pos.y *= 0.5f;
			pos = CartesianToPolar(pos);
			//pos.x =1/(Mathf.Log(pos.x*r));
			pos.z += r*1/Mathf.Log(pos.x);
			pos = PolarToCartesian(pos);
			//pos.z *= Mathf.Log(Mathf.Sqrt(pos.x*pos.x+pos.y*pos.y));


			points[i].position = pos;
			points[i].color = new Color(Random.value+0.5f,Random.value+0.5f,Random.value+0.5f,Random.value);
			points[i].size = starSize;
		}

		for(int i = starsMax/2; i<starsMax;i++){
			Vector3 pos =new Vector3();
			
			pos = Random.insideUnitSphere * starDistance;// + tx.position;
			
			pos = CartesianToPolar(pos);

			
			pos = PolarToCartesian(pos);
			pos.z *= 0.05f;
			
			points[i].position = pos;
			points[i].color = new Color(Random.value+0.5f,Random.value+0.5f,Random.value+0.5f,Random.value);
			points[i].size = starSize;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (points == null)	CreateDisc ();			
		particleSystem.SetParticles (points, points.Length);
	}

	private Vector3 CartesianToPolar(Vector3 cartesian){//x=r,y=lon,z=lat
		Vector3 polar = new Vector3 ();

		polar.x = Mathf.Sqrt (cartesian.x*cartesian.x + cartesian.y*cartesian.y + cartesian.z*cartesian.z );

		polar.y = Mathf.Acos (cartesian.z/Mathf.Sqrt(cartesian.x*cartesian.x + cartesian.y*cartesian.y + cartesian.z*cartesian.z ));
		if(cartesian.y<0) polar.y *= (-1);

		polar.z = Mathf.Acos(cartesian.x/Mathf.Sqrt(cartesian.x*cartesian.x + cartesian.y*cartesian.y));

		return polar;
	}

	private Vector3 PolarToCartesian(Vector3 polar){//x=r,y=lon,z=lat
		/*x = r * sin(lat) * cos(long)
			y = r * sin(lat) * sin(long)
			z = r * cos(lat)*/
		Vector3 cartesian = new Vector3 ();

		cartesian.x = polar.x * Mathf.Sin (polar.y) * Mathf.Cos (polar.z);
		cartesian.y = polar.x * Mathf.Sin (polar.y) * Mathf.Sin (polar.z);
		cartesian.z = polar.x * Mathf.Cos (polar.y);

		return cartesian;
	}
}

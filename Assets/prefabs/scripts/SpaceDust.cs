using UnityEngine;
using System.Collections;

public class SpaceDust : MonoBehaviour {

	public int dustMax = 100;
	public float dustSize = 1.0f;
	public float dustDistance = 10.0f;

	private Transform tx;
	private ParticleSystem.Particle[] points;
	private float dustDistanceSqr;

	// Use this for initialization
	void Start () {
		tx = transform;
		dustDistanceSqr = dustDistance * dustDistance;
	}

	private void CreatDust(){
		points = new ParticleSystem.Particle[dustMax];

		for(int i =0; i<dustMax;i++){

			points[i].position = Random.insideUnitSphere * dustDistance + tx.position;
			points[i].color = new Color(0,1,0,1);
			points[i].size = dustSize;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (points == null)	CreatDust ();

		for (int i=0; i<dustMax; i++) {
			if((points[i].position - tx.position).sqrMagnitude > dustDistanceSqr){
				points[i].position = Random.insideUnitSphere * dustDistance + tx.position;
			}
		}

		particleSystem.SetParticles (points, points.Length);
	}
}

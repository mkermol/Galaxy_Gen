using UnityEngine;
using System.Collections;


public class SolarSystem : MonoBehaviour {

	private float displayScale;
	public GameObject starPrefab;
	public Vector3 starPosition;
	public Quaternion starRotation;

	public GameObject planetPrefab;
	Vector3 planetPosition;
	Quaternion planetRotation;

	GameObject[] planets;
	SateliteBehaviour sateliteTemp;


	// Use this for initialization
	void Start () {
		displayScale = 1.0f;

		GameObject starInstance = (GameObject)Instantiate (starPrefab, starPosition, starRotation);
		planetPosition = starPosition;
		planetRotation = starRotation;

		planets = new GameObject[10];

		planetPosition.z = 0;

		int max = Random.Range (3, 10);

		for (int i=0; i<max; i++) {//create random planets at different orbits; make sure to place them at anomaly!=0
			planetPosition.x += 4;
			planetPosition.z += 4;

			planets[i] = (GameObject) Instantiate(planetPrefab,planetPosition,planetRotation);
			planets[i].transform.parent = starInstance.transform;
			sateliteTemp = planets[i].GetComponent<SateliteBehaviour>();
			sateliteTemp.initFields(10,Random.Range(0.3f,2.0f)+0.1f*i,Vector3.Distance(planetPosition,starPosition));
		}

	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void OnGUI(){
		displayScale = GUI.HorizontalSlider (new Rect (60, 120, 350, 30), displayScale, 0.0f, 10.0f);
	}
}

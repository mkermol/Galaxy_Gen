using UnityEngine;
using System.Collections;


public class SolarSystem : MonoBehaviour {

	private float displayScale;

	private Vector3 systemRange ;
	private Transform tx;
	private GameObject starInstance;
	private int unitLayer;
	private bool running = false;


	public Octree systemOctree;
	public GameObject starPrefab;
	public int baseSeed = 2;
	public Texture target;

	public GameObject planetPrefab;
	Vector3 planetPosition;
	Quaternion planetRotation;

	GameObject[] planets;
	SateliteBehaviour sateliteTemp;


	// Use this for initialization
	void Start () {


	}

	public bool IsRunning(){
		return running;
	}

	public void Init(int startingSeed,float range,int uScale, Color starColor){


		running = true;

		tx = transform;
		unitLayer = uScale;
		systemRange = new Vector3 (range,range,range);
		displayScale = 4.0f;
		baseSeed = startingSeed;
		Random.seed = startingSeed;
		
		starInstance = (GameObject)Instantiate (starPrefab, tx.position, tx.rotation);
		starInstance.transform.parent = tx;
		starInstance.transform.localScale = new Vector3 (displayScale, displayScale, displayScale);
		ChangeSunColor(starColor);
		
		//create octree and input the star instance

		systemOctree = ScriptableObject.CreateInstance ("Octree") as Octree;
		systemOctree.Init (systemRange, tx.position,systemOctree);

		systemOctree.InsertObject(starInstance);
		

		planetPosition = tx.position;
		planetRotation = tx.rotation;

		
		planets = new GameObject[10];
		

		
		int max = Random.Range (3, 10);
		
		for (int i=0; i<max; i++) {//create random planets at different orbits; make sure to place them at anomaly!=0
			planetPosition.x += 4*displayScale;
			planetPosition.z += 4*displayScale;
			
			planets[i] = (GameObject) Instantiate(planetPrefab,planetPosition,planetRotation);
			planets[i].transform.parent = tx.transform;
			sateliteTemp = planets[i].GetComponent<SateliteBehaviour>();
			sateliteTemp.initFields(10,Random.Range(0.3f,2.0f)+0.1f*i,Vector3.Distance(planetPosition,tx.position));

			systemOctree.InsertObject(planets[i]);

		}
		
		//create octree
	}

	public void ChangeSunColor(Color newColor){
		starInstance.GetComponent<MeshRenderer>().material.color=newColor;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool CheckPointInBounds(Vector3 pointA,Vector3 pointB, float r){
		Vector3 point = pointB - pointA;
		//print ("Checking: player" + pointA + "; system" + pointB);
		if (point.sqrMagnitude < (r * r)) {
						return true;
				} else
						return false;
	}

	public void Disable(){

		for (int i=0; i<planets.Length; i++) {
			Destroy(planets[i]);
		}
		Destroy(starInstance);
	}

	void VisNode(Octree node){
		Gizmos.DrawWireCube(node.GetOrigin(),node.GetHalfDimension()*2 );
		Gizmos.DrawCube (node.GetOrigin(),node.GetHalfDimension() * 2);
		for (int i=0; i<8; i++) {
			//print ("drawing next child...");
			if(!node.IsLeafNode()) VisNode(node.GetChild(i));
		}
	}

	void OnDrawGizmos(){
		Gizmos.color = new Color (0.0f, 1.0f, 0.0f, 0.1f);
		//VisNode(systemOctree);

		Gizmos.DrawWireCube(transform.position,systemRange*2);
		Gizmos.DrawCube (transform.position,systemRange*2);
	}
}

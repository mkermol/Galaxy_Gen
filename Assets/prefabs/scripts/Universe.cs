using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Universe : MonoBehaviour {

	public int starCount=20;
	public float universeRadius= 256.0f;
	public float starSize = 1.0f;
	public GameObject solarSystemPrefab;
	public GameObject playerPrefab;
	public float spiralCoeff = 60.0f;


	private Transform tx;
	private Octree universeOctree;
	private SimulationPlaceHolder[] stars;
	private GameObject solarSystem;
	private GameObject playerAvatar;
	private PlayerBehaviour pb;
	private int unitNumber=2;
	private ScallableUnit[] units;
	private int unitLayer=0;
	private ParticleSystem.Particle[] particleStars;
	private float fps;
	private float maxPosition = 1000.0f;
	private SimulationPlaceHolder closestStar = null;

	// Use this for initialization
	void Start () {
		Random.seed = 1;

		tx = transform;

		particleStars = new ParticleSystem.Particle[starCount];
		solarSystem = (GameObject) Instantiate (solarSystemPrefab, tx.transform.position, tx.transform.rotation);

		InitUnits ();

		stars=new SimulationPlaceHolder[starCount];
		universeOctree = ScriptableObject.CreateInstance ("Octree") as Octree;
		universeOctree.Init (new Vector3(universeRadius,universeRadius,universeRadius),new Vector3(0,0,0),universeOctree);



		for (int i=0; i<starCount; i++) {//generate star positions
			Vector3 pos;

			if(i<(starCount/2)) {
				pos = GenDisc();
			}else{
				pos = GenSpiral();
			}
			stars[i] = ScriptableObject.CreateInstance("SimulationPlaceHolder") as SimulationPlaceHolder;
			stars[i].position = pos;
			universeOctree.InsertObject(stars[i]);
			//SolarSystem script = stars[i].GetComponent<SolarSystem>();
			Random.seed = i;
			Color col = new Color(Random.value+0.5f,1,Random.value+0.5f);
			stars[i].col = col;
			//script.Init(i, 64.0f,unitLayer+1,col);


			particleStars[i].position = pos;
			particleStars[i].color = col ;
			particleStars[i].size = starSize;
			//stars[i].SetActive(false);

		}


		playerAvatar = (GameObject)Instantiate (playerPrefab, tx.transform.position, tx.transform.rotation);
		//playerAvatar.transform.parent = tx;
		playerAvatar.transform.position = new Vector3(0,0,0);
		PlayerGUI pg = playerAvatar.GetComponent<PlayerGUI> ();
		//pg.SetSelection (stars[0].transform);
		pb = playerAvatar.GetComponent<PlayerBehaviour> ();
		pb.Init (unitNumber);
		pb.SetTarget (stars [0].position);



		transform.Find("Galaxy Particles").GetComponent<ParticleSystem>().SetParticles (particleStars, particleStars.Length); 
		//universeOctree.InsertObject (playerAvatar);
	}

	Vector3 GenDisc(){
		Vector3 pos =new Vector3();
		
		
		pos = Random.insideUnitSphere * universeRadius + tx.position;
		
		pos = CartesianToPolar(pos);
		
		
		pos = PolarToCartesian(pos);
		pos.z *= 0.05f;
		return pos;
	}

	Vector3 GenSpiral(){
		Vector3 pos =new Vector3();
		
		pos = Random.insideUnitSphere * universeRadius + tx.position;
		pos.z *= 0.1f;
		pos.y *= 0.5f;
		pos = CartesianToPolar(pos);

		pos.z += spiralCoeff*1/Mathf.Log(pos.x);
		pos = PolarToCartesian(pos);
		
		return pos;
	}
	
	void checkPlayerVicinity(){


		float r2 = (universeRadius*2)*(universeRadius*2);//square of smallest detected distance; set to largest possible initially
		Vector3 playerPos = pb.GetPositionOnLayer (0) + pb.GetPositionOnLayer (1) * units [1].GetUScale ();
		SolarSystem script = solarSystem.GetComponent<SolarSystem>();
		List<SimulationPlaceHolder> neighbors = universeOctree.GetNeighbors (playerPos);

		foreach (SimulationPlaceHolder system in neighbors) {
			Vector3 rVec = system.position-playerPos;
			if(r2>rVec.sqrMagnitude){
				r2=rVec.sqrMagnitude;
				print ("found something closer");
				closestStar = system;
				pb.SetTarget(closestStar.position);
				if(script.IsRunning())script.Disable();
				script.Init(system.seed,100.0f,1,system.col);
			}
		}
	}



	void UpdateStarPosition(){
		if (closestStar != null) {
			print ("closest star: " + closestStar.position);
			solarSystem.transform.position = (closestStar.position - pb.GetPositionOnLayer (0)) * units [0].GetDScale ();
		}
	}

	// Update is called once per frame
	void Update () {
		checkPlayerVicinity ();
		UpdateStarPosition ();
		print ("star pos particle: " +stars[0].position*1000);
		print ("star pos: " +solarSystem.transform.position);

		//PlayerBehaviour pb = playerAvatar.GetComponent<PlayerBehaviour> ();

		UpdatePlayerToBounds (pb.GetLayer());//check if player position is out of bounds; correct all necessary coordinates

		//Update the particles functioning as upper layers in the background as well as their respective cameras
		transform.Find ("Galaxy Particles").transform.rotation = playerAvatar.transform.rotation;
		transform.Find ("Galaxy Particles").transform.position = pb.GetPositionOnLayer (0)+(pb.GetPositionOnLayer (1)*units[1].GetUScale());

		if (Input.GetKey (KeyCode.LeftControl))
						playerAvatar.transform.LookAt (stars [0].position); 

		fps = 1 / Time.deltaTime;
	}

	private void InitUnits(){
		units = new ScallableUnit[unitNumber];
		for(int i=0;i<units.Length;i++){
			units[i] = ScriptableObject.CreateInstance("ScallableUnit") as ScallableUnit;
		}

		units [0].Init ("kpc",0.001f,1000.0f);
		units [1].Init ("pc",0.001f,1000.0f);
	}


	void OnDrawGizmos(){
		Gizmos.color = new Color (0.0f, 1.0f, 0.0f, 0.1f);

		Gizmos.DrawSphere (tx.position,universeRadius);
	}


	void UpdatePlayerToBounds(int layer){//wrap player position to predefined bounds; update necessary higher levels
		//PlayerBehaviour pb = playerAvatar.GetComponent<PlayerBehaviour> ();

		Vector3 pos = pb.GetPositionOnLayer (layer);//get position on players current movement layer

		float x = pos.x;
		float y = pos.y;
		float z = pos.z;

		 //position change on higher levels
		float dx = 0.0f;
		float dy = 0.0f;
		float dz = 0.0f;
		
		if (x > maxPosition) {
			x -= 2 * maxPosition;
			dx += 2 * maxPosition * units[layer].GetUScale();
		}
		if (x < -maxPosition) {
			x += 2 * maxPosition;
			dx -= 2 * maxPosition * units[layer].GetUScale();
		}
		if (y > maxPosition) {
			y -= 2 * maxPosition;
			dy += 2 * maxPosition * units[layer].GetUScale();
		}
		if (y < -maxPosition) {
			y += 2 * maxPosition;
			dy -= 2 * maxPosition * units[layer].GetUScale();
		}
		if (z > maxPosition) {
			z -= 2 * maxPosition;
			dz += 2 * maxPosition * units[layer].GetUScale();
		}
		if (z < -maxPosition) {
			z += 2 * maxPosition;
			dz -= 2 * maxPosition * units[layer].GetUScale();
		}

		//update player coords
		pb.SetPositionOnLayer (new Vector3 (x, y, z), layer);
		if(pb.GetLayer()==layer)pb.SetPosition (new Vector3 (x, y, z));
		//update higher layer coords
		if (layer > 0) {
			Vector3 pos2 = new Vector3(dx,dy,dz);
			pos2 += pb.GetPositionOnLayer(layer-1);
			pb.SetPositionOnLayer(pos2,layer-1);
			UpdatePlayerToBounds(layer-1);
		}
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

	private void OnGUI(){
		GUI.TextArea (new Rect (10, 10, 40, 20), fps.ToString ());
	}



}

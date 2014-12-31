using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Octree : ScriptableObject {

	private Vector3 origin;				//origin point of node
	private Vector3 halfDimension;		//half the size of node in each direction
	private int maxDataCount;			//maximum amount of elements per node
	private Octree root;				//pointer to tree root; need this for updates
	private Octree[] children;
	/*private*/ GameObject[] dataTable;			//data table
	SimulationPlaceHolder[] sDataTable;



	// Use this for initialization
	void Start () {

	}

	public void Init(Vector3 size,Vector3 newOrigin,Octree newRoot){//use to initialize new octree node
		root = newRoot;
		maxDataCount = 2;
		dataTable = new GameObject[maxDataCount];//create and initialize data table
		sDataTable = new SimulationPlaceHolder[maxDataCount];
		for (int i=0; i<maxDataCount; i++) {
			dataTable [i] = null;
			sDataTable [i] = null;
		}

		children = new Octree[8];//create empty children so we can determine if node is leaf

		for (int i=0; i<8; i++)
			children [i] = null;

		halfDimension = size;
		origin = newOrigin;
	}

	public bool IsLeafNode(){
		return children [0] == null;
	}

	int GetOctantIndex(Vector3 point){//return index of octant containing a speciffic point
		int oct = 1;
		if ((point.x <= origin.x) && (point.y <= origin.y) && (point.z <= origin.z))
			oct = 0;

		if ((point.x <= origin.x) && (point.y <= origin.y) && (point.z > origin.z))
			oct = 1;

		if ((point.x <= origin.x) && (point.y > origin.y) && (point.z <= origin.z))
			oct = 2;

		if ((point.x <= origin.x) && (point.y > origin.y) && (point.z > origin.z))
			oct = 3;

		if ((point.x > origin.x) && (point.y <= origin.y) && (point.z <= origin.z))
			oct = 4;

		if ((point.x > origin.x) && (point.y <= origin.y) && (point.z > origin.z))
			oct = 5;

		if ((point.x > origin.x) && (point.y > origin.y) && (point.z <= origin.z))
			oct = 6;

		if ((point.x > origin.x) && (point.y > origin.y) && (point.z > origin.z))
			oct = 7;
				
		return oct;
	}

	public bool PointInOctant(Vector3 point){
		bool pointInOct = true;

		if((point.x > halfDimension.x+origin.x)||(point.x < origin.x-halfDimension.x))
			pointInOct = false;
		if((point.y > halfDimension.y+origin.y)||(point.y < origin.y-halfDimension.y))
			pointInOct = false;
		if((point.z > halfDimension.z+origin.z)||(point.z < origin.z-halfDimension.z))
			pointInOct = false;

		return pointInOct;
	}

	/*private*/ bool InsertData(GameObject newData){
		for (int i=0; i<maxDataCount; i++) {
			if(dataTable[i]==null){
				dataTable[i]=newData;
				return false;
			}
		}
		return true;
	}

	bool InsertData(SimulationPlaceHolder newData){
		for (int i=0; i<maxDataCount; i++) {
			if(sDataTable[i]==null){
				sDataTable[i]=newData;
				return false;
			}
		}
		return true;
	}
	
	public void InsertObject(GameObject newData){
		if (IsLeafNode()) {
			//print("    leaf node encountered; checking for data...");
			if (InsertData(newData)) {

				//print("      data present, subdividing node...");

				for (int i=0; i<8; i++) {
					//print("      creating octant "+i+" ...");
					Vector3 newOrigin = origin;
					
					switch(i){
					case 0:
						newOrigin.x += halfDimension.x * (-0.5f);
						newOrigin.y += halfDimension.y * (-0.5f);
						newOrigin.z += halfDimension.z * (-0.5f);
						break;
						
					case 1:
						newOrigin.x += halfDimension.x * (-0.5f);
						newOrigin.y += halfDimension.y * (-0.5f);
						newOrigin.z += halfDimension.z * (0.5f);
						break;
						
					case 2:
						newOrigin.x += halfDimension.x * (-0.5f);
						newOrigin.y += halfDimension.y * (0.5f);
						newOrigin.z += halfDimension.z * (-0.5f);
						break;
						
					case 3:
						newOrigin.x += halfDimension.x * (-0.5f);
						newOrigin.y += halfDimension.y * (0.5f);
						newOrigin.z += halfDimension.z * (0.5f);
						break;
						
					case 4:
						newOrigin.x += halfDimension.x * (0.5f);
						newOrigin.y += halfDimension.y * (-0.5f);
						newOrigin.z += halfDimension.z * (-0.5f);
						break;
						
					case 5:
						newOrigin.x += halfDimension.x * (0.5f);
						newOrigin.y += halfDimension.y * (-0.5f);
						newOrigin.z += halfDimension.z * (0.5f);
						break;
						
					case 6:
						newOrigin.x += halfDimension.x * (0.5f);
						newOrigin.y += halfDimension.y * (0.5f);
						newOrigin.z += halfDimension.z * (-0.5f);
						break;	
						
					case 7:
						newOrigin.x += halfDimension.x * (0.5f);
						newOrigin.y += halfDimension.y * (0.5f);
						newOrigin.z += halfDimension.z * (0.5f);
						break;
					}
					
					children[i] = ScriptableObject.CreateInstance("Octree") as Octree;
					children[i].Init(halfDimension * 0.5f, newOrigin,root);
					//print("      creating octant "+i+":done");
				}
				//print ("     subdivision done, inserting data to octants... ");
				//children [GetOctantIndex (oldData.transform.position)].InsertObject (oldData);
				//children [GetOctantIndex (newData.transform.position)].InsertObject (newData);
				for(int i=0;i<maxDataCount;i++){
					children [GetOctantIndex (dataTable[i].transform.position)].InsertObject (dataTable[i]);

				}
				dataTable=null;
				children [GetOctantIndex (newData.transform.position)].InsertObject (newData);
			} 
		} else {
			//print("    Interrior node encountered, inserting to octant: "+GetOctantIndex (newData.transform.position));
			children [GetOctantIndex (newData.transform.position)].InsertObject (newData);

		}
	}


	public void InsertObject(SimulationPlaceHolder newData){
		if (IsLeafNode()) {
			//Debug.Log("    leaf node encountered; checking for data...");

			if (InsertData(newData)) {
				
				//Debug.Log("      data present, subdividing node...");
				
				for (int i=0; i<8; i++) {
					//Debug.Log("      creating octant "+i+" ...");
					Vector3 newOrigin = origin;
					
					switch(i){
					case 0:
						newOrigin.x += halfDimension.x * (-0.5f);
						newOrigin.y += halfDimension.y * (-0.5f);
						newOrigin.z += halfDimension.z * (-0.5f);
						break;
						
					case 1:
						newOrigin.x += halfDimension.x * (-0.5f);
						newOrigin.y += halfDimension.y * (-0.5f);
						newOrigin.z += halfDimension.z * (0.5f);
						break;
						
					case 2:
						newOrigin.x += halfDimension.x * (-0.5f);
						newOrigin.y += halfDimension.y * (0.5f);
						newOrigin.z += halfDimension.z * (-0.5f);
						break;
						
					case 3:
						newOrigin.x += halfDimension.x * (-0.5f);
						newOrigin.y += halfDimension.y * (0.5f);
						newOrigin.z += halfDimension.z * (0.5f);
						break;
						
					case 4:
						newOrigin.x += halfDimension.x * (0.5f);
						newOrigin.y += halfDimension.y * (-0.5f);
						newOrigin.z += halfDimension.z * (-0.5f);
						break;
						
					case 5:
						newOrigin.x += halfDimension.x * (0.5f);
						newOrigin.y += halfDimension.y * (-0.5f);
						newOrigin.z += halfDimension.z * (0.5f);
						break;
						
					case 6:
						newOrigin.x += halfDimension.x * (0.5f);
						newOrigin.y += halfDimension.y * (0.5f);
						newOrigin.z += halfDimension.z * (-0.5f);
						break;	
						
					case 7:
						newOrigin.x += halfDimension.x * (0.5f);
						newOrigin.y += halfDimension.y * (0.5f);
						newOrigin.z += halfDimension.z * (0.5f);
						break;
					}
					
					children[i] = ScriptableObject.CreateInstance("Octree") as Octree;
					children[i].Init(halfDimension * 0.5f, newOrigin,root);
					//Debug.Log("      creating octant "+i+":done");
				}
				//Debug.Log ("     subdivision done, inserting data to octants... ");

				for(int i=0;i<maxDataCount;i++){
					children [GetOctantIndex (sDataTable[i].position)].InsertObject (sDataTable[i]);
					
				}
				sDataTable=null;
				children [GetOctantIndex (newData.position)].InsertObject (newData);
			} 
		} else {
			//Debug.Log("    Interrior node encountered, inserting to octant: "+GetOctantIndex (newData.position));
			children [GetOctantIndex (newData.position)].InsertObject (newData);
			
		}
	}




	private void CleanLeaves(){
		if (children != null) {
				bool childrenEmpty = true;
				for (int i =0; i<8; i++) {
					
					if(children[i]!=null){
						children [i].CleanLeaves ();
						if(children[i].IsLeafNode())childrenEmpty=false;
					}
				}
			if(childrenEmpty){
				//print ("Removing empty leaves...");
				for (int i=0; i<8; i++){
					Destroy(children[i]);
					//if(children[i]==null) print ("children["+i+"] is null");
				}
				//if(IsLeafNode()) print ("leaves removed");
			}
		} 
	}

	void OnDestroy(){
		//print ("   child removed");
	}
	
	// Update is called once per frame
	void Update () {
		if (dataTable != null) {
						for (int i=0; i<maxDataCount; i++)
								if (dataTable [i] != null) {
										if (!PointInOctant (dataTable[i].transform.position)) {
												//print ("object out of scope, updating Octree...");	
												GameObject oldData = dataTable[i];
												dataTable[i] = null;
												//if(data==null) print ("object removed, inserting to root...");
												//root.CleanLeaves();
												root.InsertObject (oldData);
										}
								}
				}
	}

	public Vector3 GetHalfDimension(){
		return halfDimension;
	}

	public Vector3 GetOrigin(){
		return origin;
	}

	public Octree GetChild(int i){
		return children[i];
	}

	public List<SimulationPlaceHolder> GetNeighbors(Vector3 point){ //recursively get neighbors surrounding a point in space
		List<SimulationPlaceHolder> neighbors = new List<SimulationPlaceHolder>();
		//Debug.Log ("checking for neighbors");
		if (IsLeafNode ()) {
			//Debug.Log ("leaf node found");
			for(int i=0;i<maxDataCount;i++){
				if(sDataTable[i]!=null){
					//Debug.Log ("neighbor found");
					neighbors.Add(sDataTable[i]);
				}
			}
		} else {
			neighbors = children[GetOctantIndex(point)].GetNeighbors(point);
		}
		return neighbors;
	}


	void OnDrawGizmos(){
		//if(IsLeafNode())
		Gizmos.color = new Color (0.0f, 1.0f, 0.0f, 0.1f);

			Gizmos.DrawWireCube(origin,halfDimension*2);
		Gizmos.DrawCube (origin, halfDimension * 2);
	}
}

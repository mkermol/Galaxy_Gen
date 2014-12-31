using UnityEngine;
using System.Collections;

public class PlayerBehaviour : MonoBehaviour {

	private Vector3[] universalPosition;
	private int currentUnitLayer;
	private Transform tx;
	private Vector3 target;




	void Start () {

	}

	public void Init(int unitNumber){// Use this for initialization
		tx = transform;
		universalPosition = new Vector3[unitNumber];//initialize complex universal position array
		for(int i=0;i<unitNumber;i++){
			universalPosition[i] = new Vector3(0,0,0);//assign starting position to each layer
		}
		currentUnitLayer = unitNumber-1;//define starting layer
	}


	
	// Update is called once per frame
	void Update () {
		universalPosition [currentUnitLayer] = tx.position;

		//print ("player pos: " + universalPosition [currentUnitLayer]);
	}

	public int GetLayer(){
		return currentUnitLayer;
	}



	public Vector3 GetPositionOnLayer(int i){
		return universalPosition [i];
	}

	public void SetPositionOnLayer(Vector3 pos, int layer){
		universalPosition [layer] = pos;
	}

	public void SetPosition(Vector3 pos){
		tx.position = pos;
	}

	public void SetTarget(Vector3 tPos){
		target = tPos;
		print ("Player target: " + target);
	}
}

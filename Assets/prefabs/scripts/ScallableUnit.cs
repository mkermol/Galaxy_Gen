using UnityEngine;
using System.Collections;

public class ScallableUnit : ScriptableObject {

	private string unitName;
	private float upScale;
	private float downScale;
	// Use this for initialization
	void Start () {
	
	}

	public void Init(string newName,float uScale,float dScale){
		unitName = newName;
		upScale = uScale;
		downScale = dScale;
	}

	public string GetString(){
		return unitName;
	}

	public float GetUScale(){
		return upScale;
	}

	public float GetDScale(){
		return downScale;
	}
}

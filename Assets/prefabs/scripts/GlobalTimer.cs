using UnityEngine;
using System.Collections;

public class GlobalTimer : MonoBehaviour {
	public static float timeScale;
	// Use this for initialization
	void Start () {
		timeScale = 1.0f;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI(){
		timeScale = GUI.HorizontalSlider (new Rect (60, 60, 350, 30), timeScale, 0.0f, 10.0f);
	}
}

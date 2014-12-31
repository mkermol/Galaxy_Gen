using UnityEngine;
using System.Collections;

public class PlayerGUI : MonoBehaviour {
	private Transform selection;

	public Texture targetTex;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetSelection(Transform target){
		selection = target;
	}

	void OnGUI(){
		//if (isSelected) {
		/*Vector3 o = transform.FindChild("Main Camera").camera.WorldToScreenPoint(selection.position);


		GUI.DrawTexture(new Rect(o.x-16,Screen.height-o.y-16,32,32),targetTex);*/
		//}
	}
}

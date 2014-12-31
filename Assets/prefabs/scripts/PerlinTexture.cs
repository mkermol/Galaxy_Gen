using UnityEngine;
using System.Collections;

public class PerlinTexture : MonoBehaviour {

	public int pixWidth;
	public int pixHeight;
	public float xOrg;
	public float yOrg;
	public float scale = 1.0F;
	public int nOctaves = 4;
	private Texture2D noiseTex;
	private Color[] pix;


	void Start() {
		noiseTex = new Texture2D(pixWidth, pixHeight);
		pix = new Color[noiseTex.width * noiseTex.height];
		renderer.material.mainTexture = noiseTex;
		renderer.material.SetTexture ("Heightmap", noiseTex);
		//renderer.material.SetTexture ("_Ilumin", noiseTex);
	}


	void CalcNoise() {
		float y = 0.0F;
		while (y < noiseTex.height) {
			float x = 0.0F;
			while (x < noiseTex.width) {
				float sample = 0.0f;
				float[] octaves = new float[nOctaves];
				for(int i=0;i<octaves.Length;i++){
					float xCoord = xOrg*i + x / noiseTex.width * (scale*i);
					float yCoord = yOrg*i + y / noiseTex.height * (scale*i);
					octaves[i] = Mathf.PerlinNoise(xCoord, yCoord);
					sample += octaves[i]*((nOctaves-i)*0.5f);
				}
				sample /= nOctaves;
				pix[(int)y * noiseTex.width + (int)x] = new Color(sample, sample, sample);
				x++;
			}
			y++;
		}
		noiseTex.SetPixels(pix);
		noiseTex.Apply();
	}


	void Update() {
		CalcNoise();
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drill : Block {

	const float time = 1f; //determines drill speed
	float timer = 0f;
	byte drilling;

	override public int memoryMapSize {
		get {
			return 1;
		}
	}

	override public byte Read(int offset) {
		return drilling;
	}

	override public void Write(int offset, byte value){}

	override public void Interact (char c) {}

	override public void InteractSlider (Player player, float value) {}

	void Update () {
		timer += Time.deltaTime;
		if (timer > time) {
			RaycastHit hit;
			if (Physics.Raycast (
				    new Ray (transform.position, -transform.up),
				    out hit,
				    1f
			    )) {
				if (hit.transform.CompareTag ("Surface")) {
					drilling = 0xff;
					Renderer renderer = hit.transform.GetComponent<Renderer> ();
					Texture2D tex = (Texture2D)renderer.material.mainTexture;
					Vector2 uvPos = hit.textureCoord;
					//print (uvPos);
					//print ((int)(uvPos [0] * tex.width)+","+(int)(uvPos [1] * tex.height));
					//print ((Color32)tex.GetPixel ((int)(uvPos[0] * tex.width), 0));
					while (timer > time) {
						timer -= time;
						Color32 pixel = tex.GetPixel ((int)(uvPos [0] * tex.width), 0);
						int[] color = new int[3] { (int)pixel.r, (int)pixel.g, (int)pixel.b };
						int sum = color [0] + color [1] + color [2];
						if (sum > 0) {
							int rand = Random.Range (1, sum);
							int primary;
							if (rand <= color [0]) {
								primary = 0;
							} else if (rand <= color [0] + color [1]) {
								primary = 1;
							} else {
								primary = 2;
							}
							//print(primary);
							color [primary]--;
							tex.SetPixel ((int)(uvPos [0] * tex.width), 0, new Color32 ((byte)color [0], (byte)color [1], (byte)color [2], byte.MaxValue));
							tex.Apply ();
							ship.AddColor (primary);
						}
					}
				} else {
					drilling = 0x0;
				}
			} else {
				drilling = 0x0;
			}
		}
	}
}

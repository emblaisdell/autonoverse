using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : Block {

	const float time = 0.01f;
	float timer = 1f; 
	public byte[] color = new byte[3] { 0, 0, 0 };
	Material insideMat;

	override public int memoryMapSize {
		get {
			return 3;
		}
	}

	void Start() {
		//color = new byte[3] {(byte)Random.Range(0,255),(byte)Random.Range(0,255),(byte)Random.Range(0,255)};
		insideMat = transform.GetChild (0).GetComponent<Renderer>().material;
		SetColor ();
	}

	override public byte Read(int offset) {
		return color[offset];
	}

	override public void Write(int offset, byte value){}

	override public void Interact (char c) {}

	override public void InteractSlider (Player player, float value) {
		timer += Time.fixedDeltaTime * Mathf.Abs(value);

		while (timer > time) {
			timer -= time;

			int sum = (int)color[0] + (int)color[1] + (int)color[2];
			int playerSum = (int)player.color[0] + (int)player.color[1] + (int)player.color[2];
			const int maxSum = 3 * (int)byte.MaxValue;
			if (value < 0f && sum > 0 && playerSum < maxSum ) {
				int rand = Random.Range (1, sum);
				//print (sum);
				int primary;
				if (rand <= (int)color [0]) {
					primary = 0;
				} else if (rand <= (int)color[0] + (int)color[1]) {
					primary = 1;
				} else {
					primary = 2;
				}
				if (player.color[primary] < byte.MaxValue){
					color[primary]--;
					player.color[primary]++;
					SetColor ();
				}
			}
			if (value > 0f && sum < maxSum && playerSum > 0) {
				int rand = Random.Range (1, playerSum);
				int primary;
				if (rand <= (int)player.color [0]) {
					primary = 0;
				} else if (rand <= (int)player.color [0] + (int)player.color [1]) {
					primary = 1;
				} else {
					primary = 2;
				}
				if (color [primary] < byte.MaxValue) {
					player.color [primary]--;
					color [primary]++;
					SetColor ();
				}
			}
		}
	}

	public void AddPrimary (int primary) {
		color [primary]++;
		SetColor ();
	}

	private void SetColor (){
		for (int i=0; i<3; i++){
			ship.Write (memoryMapPosition + i, color[i]);
		}
		insideMat.SetColor ("_EmissionColor", GetColor (color));
	}

	private static Color GetColor(byte[] color) {
		return new Color32 (color [0], color [1], color [2], byte.MaxValue);
	}
}

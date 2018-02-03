using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : Block {

	private byte ticks = 0;
	private float timer = 0f;
	private float tickTime = 1f;

	override public int memoryMapSize {
		get {
			return 1;
		}
	}

	override public byte Read(int offset) {
		return ticks;
	}

	override public void Write(int offset, byte value){
		ticks = value;
		timer = 0f;
	}

	override public void Interact (char c) {}

	override public void InteractSlider (Player player, float value) {}

	void Update () {
		timer += Time.deltaTime;
		while (timer >= tickTime) {
			if (ticks < byte.MaxValue) {
				ticks++;
			} else {
				ticks = 0;
			}
			ship.Write (memoryMapPosition, ticks);
			timer -= tickTime;
		}
	}
}

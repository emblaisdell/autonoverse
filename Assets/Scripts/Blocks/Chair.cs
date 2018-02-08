using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : Block, Sittable {

	float yaw = 0f;

	override public int memoryMapSize {
		get {
			return 0;
		}
	}

	override public byte Read(int offset) {
		return 0;
	}

	override public void Write(int offset, byte value){}

	public void SetAxes (float lookX, float lookY, float moveX, float moveY) {
		// TODO: Implement
	}
}

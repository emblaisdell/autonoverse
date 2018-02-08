using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : Block, Sittable {

	float yaw = 0f;
	const float angVel = 100f;

	const float maxByte = (float)byte.MaxValue;

	Transform seat;

	byte[] values = new byte[2];

	override public int memoryMapSize {
		get {
			return 0;
		}
	}

	void Start() {
		seat = transform.GetChild (0);
	}

	override public byte Read(int offset) {
		return 0;
	}

	override public void Write(int offset, byte value){}

	public void SetAxes (float lookX, float moveX, float moveY) {
		yaw += Time.fixedDeltaTime * angVel * lookX;
		seat.localRotation = Quaternion.Euler (0f, yaw, 0f);

		values [0] = (byte)(moveX / maxByte);
		values [1] = (byte)(moveY / maxByte);


	}
}

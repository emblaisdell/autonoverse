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
			return 2;
		}
	}

	void Start() {
		seat = transform.GetChild (0);
	}

	override public byte Read(int offset) {
		return values[offset];
	}

	override public void Write(int offset, byte value){}

	public void SetAxes (float lookX, float moveX, float moveY) {
		yaw += Time.fixedDeltaTime * angVel * lookX;
		seat.localRotation = Quaternion.Euler (0f, yaw, 0f);

		values [0] = (byte)(0.5f*(moveX+1f) * maxByte);
		values [1] = (byte)(0.5f*(moveY+1f) * maxByte);

	}
}

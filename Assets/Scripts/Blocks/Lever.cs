using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : Block, Slidable {

	const float speed = 1f;
	const float range = 0.7f;

	const float maxValue = (float)byte.MaxValue;

	Transform handle;
	float floatValue = 0f;
	byte value = 0;

	override public int memoryMapSize {
		get {
			return 1;
		}
	}

	void Start () {
		handle = transform.GetChild (0);
		handle.localPosition = (floatValue - 0.5f) * range * Vector3.up;
	}

	override public byte Read(int offset) {
		return value;
	}

	override public void Write(int offset, byte value){}

	public void Slide (Player player, float sliderValue) {
		floatValue = Mathf.Clamp01 (floatValue + speed * Time.fixedDeltaTime * sliderValue);
		value = (byte)(maxValue * floatValue);
		ship.Write (memoryMapPosition, value);

		handle.localPosition = (floatValue - 0.5f) * range * Vector3.up;
	}
}

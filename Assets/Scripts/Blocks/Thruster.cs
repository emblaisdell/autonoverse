using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : Block {

	override public int memoryMapSize {
		get {
			return 1;
		}
	}

	Transform shipPart;
	Rigidbody rb;
	float thrust = 0f;
	float maxThrust = 100f;
	byte value = 0;
	float maxValue = (float)byte.MaxValue;

	// Use this for initialization
	void Start () {
		shipPart = transform.parent;
		rb = shipPart.GetComponent <Rigidbody>();
	}

	override public byte Read (int offset) {
		return value;
	}

	override public void Write (int offset, byte value) {
		this.value = value;
		thrust = maxThrust * ((float)value) / maxValue;
	}

	void FixedUpdate () {
		rb.AddForceAtPosition (thrust * shipPart.up, transform.position);
	}

}

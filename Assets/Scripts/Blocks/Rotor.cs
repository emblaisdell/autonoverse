using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotor : Block {

	GameObject rotorDummy;
	public Transform ShipPart;
	public HingeJoint joint;

	byte motorSpeed = 127;
	float maxByte = (float)byte.MaxValue;
	float maxSpeed = 500f;

	override public int memoryMapSize {
		get {
			return 1;
		}
	}

	void Start () {
		Transform shipPart = transform.parent;

		rotorDummy = (GameObject)Resources.Load ("Blocks/RotorDummy");
		Transform newShipPart = Instantiate(ShipPart, transform.position, transform.rotation);
		Block newBlock = Instantiate (rotorDummy, transform.position, transform.rotation * Quaternion.Euler(180f,0f,0f), newShipPart).GetComponent<Block> ();
		Rigidbody rb = newShipPart.GetComponent<Rigidbody> ();
		Rigidbody oldRb = shipPart.GetComponent<Rigidbody> ();
		rb.velocity = oldRb.GetPointVelocity (transform.position);
		rb.angularVelocity = oldRb.angularVelocity;
		newBlock.ship = ship;
		ship.AddBlock (newBlock, memoryMapPosition);

		joint = shipPart.gameObject.AddComponent<HingeJoint> ();
		joint.connectedBody = rb;
		joint.anchor = transform.localPosition;
		joint.axis = transform.localRotation * Vector3.up;

		JointMotor motor = joint.motor;
		motor.force = 1000f;
		motor.targetVelocity = maxSpeed * (((float)motorSpeed - 127f) / maxByte);
		motor.freeSpin = false;
		joint.motor = motor;
		joint.useMotor = true;
	}

	override public byte Read(int offset) {
		return motorSpeed;
	}

	override public void Write(int offset, byte value){
		motorSpeed = value;

		JointMotor motor = joint.motor;
		motor.targetVelocity = maxSpeed * (((float)motorSpeed - 127f) / maxByte);
		joint.motor = motor;
	}

	override public void Interact (char c) {}

	override public void InteractSlider (Player player, float value) {}
}

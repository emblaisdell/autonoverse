using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piston : Block {

	GameObject pistonDummy;
	public Transform ShipPart;
	public ConfigurableJoint joint;

	Transform arm;
	Transform dummy;

	byte pos = 0;
	float maxByte = (float)byte.MaxValue;
	float farPos = 4f;

	override public int memoryMapSize {
		get {
			return 1;
		}
	}

	void Start () {
		arm = transform.GetChild (0);

		Transform shipPart = transform.parent;

		pistonDummy = (GameObject)Resources.Load ("Blocks/PistonDummy");
		Transform newShipPart = Instantiate(ShipPart, transform.position, transform.rotation);
		Block newBlock = Instantiate (pistonDummy, transform.position, transform.rotation * Quaternion.Euler(180f,0f,0f), newShipPart).GetComponent<Block> ();
		Rigidbody rb = newShipPart.GetComponent<Rigidbody> ();
		Rigidbody oldRb = shipPart.GetComponent<Rigidbody> ();
		rb.velocity = oldRb.GetPointVelocity (transform.position);
		rb.angularVelocity = oldRb.angularVelocity;
		newBlock.ship = ship;
		ship.AddBlock (newBlock, memoryMapPosition);

		joint = shipPart.gameObject.AddComponent<ConfigurableJoint> ();
		joint.autoConfigureConnectedAnchor = false;
		joint.connectedBody = rb;
		joint.anchor = transform.localPosition;
		joint.axis = transform.localRotation * Vector3.up;
		joint.connectedAnchor = Vector3.zero;

		JointDrive drive = joint.xDrive;
		//drive.mode = JointDriveMode.Position;
		drive.positionSpring = 1000f;
		drive.positionDamper = 10f;
		joint.xDrive = drive;


		
		joint.xMotion = ConfigurableJointMotion.Free;
		joint.yMotion = ConfigurableJointMotion.Locked;
		joint.zMotion = ConfigurableJointMotion.Locked;
		joint.angularXMotion = ConfigurableJointMotion.Locked;
		joint.angularYMotion = ConfigurableJointMotion.Locked;
		joint.angularZMotion = ConfigurableJointMotion.Locked;

		//joint.targetVelocity = Vector3.forward * 500f;

		dummy = newBlock.transform;
	}

	void Update(){
		float dist = (transform.position - dummy.position).magnitude;
		arm.localPosition = new Vector3(0f, 0.5f * dist, 0f);
		arm.localScale = new Vector3(1f, dist + 0.7f, 1f);
	}

	override public byte Read(int offset) {
		return pos;
	}

	override public void Write(int offset, byte value){
		pos = value;

		joint.targetPosition = farPos * (((float)pos) / maxByte) * Vector3.right;
	}
}

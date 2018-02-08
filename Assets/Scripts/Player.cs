using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	public Text HUD;
	public GameObject ShipPart;

	Rigidbody rb;
	Gravity grav;

	Transform head;

	//movement surface
	//public LayerMask playerSurface;
	//Transform lastTransform;
	//Vector3 lastNormal;

	//movement constants
	const float v = 10f;
	const float F = 1f;
	const float jumpV = 10f;
	const float angVel = 100f;
	const float stepSlope = 2f;
	const float minStandForce = 1.5f;
	const float maxStandVel = 10f;
	const float runVel = 5f;

	bool standing = false;
	Rigidbody standingOnRb = null;
	Vector3 standingNormal;

	Vector3 accel;
	Vector3 avgForceDifferential = Vector3.zero;
	const float ALPHA = 0.3f;
	Vector3 lastVel;
	const float legDrag = 1f; // how fast do legs slow you down
	const float standTorque = 2.5f;

	Vector3 movementForce = Vector3.zero;

	Vector3 lastLocalPosition;
	Quaternion lastLocalRotation;
	const float shakeVelocity = 0.5f;
	const float shakeAngularVelocity = 0.5f;
	bool moving;
	bool rotating;

	float pitch = 0f;
	const float maxPitch = 45f;
	const float minPitch = -45f;

	const float placeDist = 100f;

	int chosenBlock = 0;
	string[] blockNames = new string[]{ "ALU", "Chair", "Clock", "Drill", "Editor", "Glass", "Hull", "Keyboard", "Lever", "Memory", "Piston", "Processor", "Rotor", "Tank", "Tesseract", "Thruster", "VCPU", "Wheel" };
	GameObject[] blocks;

	public byte[] color = new byte[3] {0,0,0};

	Mesh[] ghostMeshes;
	Transform ghost;

	const float rotateTime = 0.2f;
	float rotateTimerX = 0f;
	float rotateTimerY = 0f;
	Quaternion blockRotation = Quaternion.identity;

	//const float blockChooserTime = 0.2f;
	//float blockChooserTimer = 0f;

	const float modeChangeTime = 0.2f;
	const float blockChangeTime = 0.2f;
	const float memoryMapChangeTime = 0.1f;
	float modeChangeTimer = 0f;

	bool placeDown = false;

	int memoryMapPosition = 0;

	int mode = 0;
	const int numModes = 2;

	bool sitDown = false; // is button down
	bool sitting = false;
	Sittable sittingIn;
	Transform sittingInTransform;
	Rigidbody sittingInRigidbody;

	// Use this for initialization
	void Start () {
		head = transform.GetChild (0);
		rb = GetComponent<Rigidbody> ();
		grav = GetComponent<Gravity> ();

		RaycastHit hit;
		if (Physics.Raycast (
			   	new Ray (transform.position, -transform.up),
			   	out hit
		   )) {
			//transform.position = hit.point;
			//transform.up = hit.normal;

			//transform.parent = hit.transform;
			//lastTransform = hit.transform;
			//lastNormal = hit.normal;

			Planet standingOn = hit.transform.GetComponent<Planet>();
			rb.velocity = standingOn.orbAngVel * standingOn.orthoStartR;
			lastVel = rb.velocity;
			rb.angularVelocity = standingOn.rotAngVel * Planet.POENormal;

			standingOnRb = standingOn.GetComponent<Rigidbody> ();

			lastLocalPosition = standingOn.transform.InverseTransformPoint (transform.position);
			lastLocalRotation = Quaternion.Inverse (standingOn.transform.rotation) * transform.rotation;

			print ("set player");
		}

		blocks = new GameObject[blockNames.Length];
		ghostMeshes = new Mesh[blockNames.Length];
		for (int i = 0; i < blockNames.Length; i++) {
			blocks [i] = (GameObject)Resources.Load ("Blocks/" + blockNames [i]);
			ghostMeshes[i] = ((GameObject)Resources.Load ("Ghosts/" + blockNames [i])).GetComponent<MeshFilter>().sharedMesh;
		}

		ghost = Instantiate((GameObject)Resources.Load ("Ghosts/" + blockNames [0])).transform;

		UpdateUI ();


	}
	
	// Update is called once per frame
	void Update () {
		InteractWithBlocks ();
		PlaceBlocks ();

		UpdateMode ();
		UpdateUI ();

		SitUpdate ();

		//AntiShake ();
	}

	void FixedUpdate () {
		Vector3 footPos = transform.position - 0.25f * transform.up;
		Vector3 curVel = rb.GetPointVelocity (footPos);
		accel = (curVel - lastVel) / Time.fixedDeltaTime;
		lastVel = curVel;

		Move ();
	}

	void UpdateMode () {
		modeChangeTimer -= Time.deltaTime;
		if (modeChangeTimer <= 0f) {
			float changeMode = Input.GetAxisRaw ("Mode Vertical");
			if (changeMode < 0f) {
				mode -= 1;
				if (mode < 0) { mode = numModes - 1; }
				modeChangeTimer = modeChangeTime;
			} else if (changeMode > 0f) {
				mode += 1;
				if (mode >= numModes) { mode = 0; }
				modeChangeTimer = modeChangeTime;
			}

			float modeChange = Input.GetAxisRaw ("Mode Horizontal");
			if (mode == 0) {
				if (modeChange < 0f) {
					chosenBlock -= 1;
					if (chosenBlock < 0) { chosenBlock = blockNames.Length - 1; }
					modeChangeTimer = blockChangeTime;
				}
				if (modeChange > 0f) {
					chosenBlock += 1;
					if (chosenBlock >= blockNames.Length) { chosenBlock = 0; }
					modeChangeTimer = blockChangeTime;
				}
				if (modeChange != 0f) {
					ghost.GetComponent<MeshFilter> ().mesh = ghostMeshes [chosenBlock];
				}
			} else if (mode == 1) {
				if (modeChange < 0f) {
					memoryMapPosition -= 1;
					if (memoryMapPosition < 0) { memoryMapPosition = Ship.memoryMapLength - 1; }
					modeChangeTimer = memoryMapChangeTime;
				}
				if (modeChange > 0f) {
					memoryMapPosition += 1;
					if (memoryMapPosition >= Ship.memoryMapLength) { memoryMapPosition = 0; }
					modeChangeTimer = memoryMapChangeTime;
				}
			}
		}
	}

	void Move () {

		moving = false;
		rotating = false;

		pitch -= Time.deltaTime * angVel * Input.GetAxis ("Look Vertical");
		pitch = Mathf.Clamp (pitch, minPitch, maxPitch);
		head.localEulerAngles = pitch * Vector3.right;

		if (!sitting) {

			float lookHorizontal = Input.GetAxis ("Look Horizontal");
			if (lookHorizontal != 0) {
				rotating = true;
			}
			transform.RotateAround (transform.position, transform.up, Time.deltaTime * angVel * lookHorizontal);

			float dx = 0f;
			float dy = 0f;

			if (Input.GetAxisRaw ("Rotate") == 0f) {
				dx = v * Input.GetAxis ("Move Horizontal");
				dy = v * Input.GetAxis ("Move Vertical");
			}

			Vector3 forceDifferential = rb.mass * accel - grav.net - movementForce;
			avgForceDifferential = ALPHA * forceDifferential + (1f - ALPHA) * avgForceDifferential;
			float forceDifferentialMag = avgForceDifferential.magnitude;
			Vector3 forceDifferentialNorm = avgForceDifferential / forceDifferentialMag;
			Debug.DrawRay (transform.position, forceDifferential, Color.blue);
			Debug.DrawRay (transform.position, avgForceDifferential, Color.cyan);
			Debug.DrawRay (transform.position, forceDifferentialNorm, Color.white);

			standing = false;

			//print ("start");
			if (forceDifferentialMag >= minStandForce) {
				//print ("sufficient gravity");
				RaycastHit hit;
				if (Physics.Raycast (
					    new Ray (transform.position + 0.1f * transform.up, -forceDifferentialNorm),
					    out hit
				    )) {
					//print ("down hit");
					standingOnRb = hit.rigidbody;
					standingNormal = hit.normal;

					Vector3 footPos = transform.position - 0.25f * transform.up;
					Vector3 velDifferential = rb.velocity - standingOnRb.GetPointVelocity (footPos);
					float velDifferentialMag = velDifferential.magnitude;
					//Vector3 velDifferentialNorm = velDifferential / velDifferentialMag;

					if (velDifferentialMag <= maxStandVel) {
					
						transform.rotation = Quaternion.FromToRotation (transform.up, standingNormal) * transform.rotation;
						//rb.AddForce (-legDrag * velDifferentialNorm); // slow down
						//Vector3 normal = forceDifferentialNorm;
						//Vector3 torque = standTorque * Vector3.ClampMagnitude(Vector3.Cross(transform.up, normal), 1f);
						rb.angularVelocity = standingOnRb.angularVelocity;
						//Debug.DrawRay (transform.position, torque);
						//rb.AddTorque (torque); // stand up
						//transform.up = normal;
						//print ("standing");

						if (Input.GetAxisRaw ("Jump") > 0f) {
							rb.velocity += jumpV * transform.up + dx * transform.right + dy * transform.forward;
						}
						if (dx != 0 || dy != 0) {
							moving = true;
							//rb.AddForce (transform.up);
							//rb.AddForce (dx * transform.forward + dy * transform.right);


							Vector3 movement = Time.fixedDeltaTime * (dx * transform.right + dy * transform.forward);
							float speed = movement.magnitude;
							movement += stepSlope * speed * transform.up;

							RaycastHit moveHit;
							if (Physics.Raycast (
								    new Ray (transform.position + movement, -transform.up),
								    out moveHit,
								    2f * stepSlope * speed
							    )) {
								Collider[] collisions = Physics.OverlapCapsule (
									                        moveHit.point + 0.25f * transform.up,
									                        moveHit.point + 0.75f * transform.up,
									                        0.25f
								                        );
								bool movable = true;
								foreach (Collider collision in collisions) {
									if (collision != GetComponent<Collider> () && collision.attachedRigidbody != standingOnRb) {
										movable = false;
										break;
									}
								}
								if (movable) {
									transform.position = moveHit.point;// + 0.1f * transform.up;
								}
							}
						}

						standing = true;
					}
				}
			} 
			if (!standing) {
				movementForce = F * (dx * transform.right + dy * transform.forward);
				rb.AddForce (movementForce);
			}
		}
	}

	void SitUpdate () {
		if (sitting) {
			print ("sitting");

			rb.MovePosition(sittingInTransform.position + Time.deltaTime * sittingInRigidbody.velocity);
			rb.MoveRotation(sittingInTransform.rotation);

			sittingIn.SetAxes (Input.GetAxis("Look Horizontal"), Input.GetAxis("Move Horizontal"), Input.GetAxis("Move Vertical"));
		}
	}

	void AntiShake () {
		if (standing) {
			Vector3 footPos = transform.position - 0.25f * transform.up;
			//print ("v: " + (rb.velocity - standingOnRb.GetPointVelocity (footPos)).magnitude);
			if (!moving && (rb.velocity - standingOnRb.GetPointVelocity (footPos)).magnitude <= shakeVelocity) {
				transform.position = standingOnRb.transform.TransformPoint (lastLocalPosition);
			} else {
				lastLocalPosition = standingOnRb.transform.InverseTransformPoint (transform.position);
			}

			//print ("omega: " + (rb.angularVelocity - standingOnRb.angularVelocity).magnitude);
			if (!rotating && (rb.angularVelocity - standingOnRb.angularVelocity).magnitude <= shakeAngularVelocity) {
				transform.rotation = standingOnRb.transform.rotation * lastLocalRotation;
				transform.rotation = Quaternion.FromToRotation(transform.up, standingNormal) * transform.rotation;
			} else {
				lastLocalRotation = Quaternion.Inverse (standingOnRb.transform.rotation) * transform.rotation;
			}
		}
	}

	void PlaceBlocks () {
		RaycastHit hit;
		if (Physics.Raycast (
			    new Ray (head.position, head.forward),
			    out hit,
			    placeDist
		    )) {
			Transform hitTransform = hit.collider.transform;
			if (!placeDown && Input.GetAxisRaw ("Place") > 0f) {
				placeDown = true;
				if (hitTransform.CompareTag ("Surface")) {
					GameObject block = blocks [chosenBlock];
					GameObject newShipPart = (GameObject)Instantiate (ShipPart, hit.point + 0.5f * hit.normal, Quaternion.FromToRotation (Vector3.up, hit.normal) * blockRotation);
					Block newBlock = Instantiate (block, newShipPart.transform).GetComponent<Block> ();
					Rigidbody rb = newShipPart.GetComponent<Rigidbody> ();
					Rigidbody surfRb = hitTransform.GetComponent<Rigidbody> ();
					rb.velocity = surfRb.GetPointVelocity (hit.point);
					rb.angularVelocity = surfRb.angularVelocity;
					newBlock.ship = Ship.NewShip ();
					newBlock.ship.AddBlock (newBlock, memoryMapPosition);
				}
				if (hitTransform.CompareTag ("Block")) {
					Transform hitShipPart = hitTransform.parent;
					hitShipPart.GetComponent<Rigidbody> ().mass += 1f;
					GameObject block = blocks [chosenBlock];
					Block newBlock = Instantiate (block, hitTransform.position + 1f * hit.normal, hitShipPart.rotation * blockRotation, hitShipPart).GetComponent<Block> ();
					newBlock.ship = hitTransform.GetComponent<Block> ().ship;
					newBlock.ship.AddBlock (newBlock, memoryMapPosition);
				}
			}
			if (Input.GetAxisRaw ("Rotate") > 0f) {
				ghost.gameObject.SetActive (true);

				Vector3 ghostPosition;
				Quaternion ghostRotation;

				if (hitTransform.CompareTag ("Surface")) {
					ghostPosition = hit.point + 0.5f * hit.normal;
					ghostRotation = Quaternion.FromToRotation (Vector3.up, hit.normal);
				}
				if (hitTransform.CompareTag ("Block")) {
					ghostPosition = hitTransform.position + 1f * hit.normal;
					Transform hitShipPart = hitTransform.parent;
					ghostRotation = hitShipPart.rotation;
				}

				rotateTimerX += Time.deltaTime * Input.GetAxis ("Move Horizontal");
				rotateTimerY += Time.deltaTime * Input.GetAxis ("Move Vertical");
				if (rotateTimerX <= rotateTime) {
					rotateTimerX += rotateTime;
					blockRotation = Quaternion.AngleAxis(90f, VectorUtil.ClosestDirection(transform.up, ghostRotation)) * blockRotation;
				}
				if (rotateTimerX >= rotateTime) {
					rotateTimerX -= rotateTime;
					blockRotation = Quaternion.AngleAxis(-90f, VectorUtil.ClosestDirection(transform.up, ghostRotation)) * blockRotation;
				}
				if (rotateTimerY <= rotateTime) {
					rotateTimerY += rotateTime;
					blockRotation = Quaternion.AngleAxis(-90f, VectorUtil.ClosestDirection(transform.right, ghostRotation)) * blockRotation;
				}
				if (rotateTimerY >= rotateTime) {
					rotateTimerY -= rotateTime;
					blockRotation = Quaternion.AngleAxis(90f, VectorUtil.ClosestDirection(transform.right, ghostRotation)) * blockRotation;
				}

				ghost.position = ghostPosition;
				ghost.rotation = ghostRotation * blockRotation;

			} else {
				ghost.gameObject.SetActive (false);
			}
		} else {
			ghost.gameObject.SetActive (false);
		}
		if (Input.GetAxis("Place") <= 0f) {
			placeDown = false;
		}
	}

	void InteractWithBlocks(){
		if (Input.anyKeyDown) {
			RaycastHit hit;
			if (Physics.Raycast (
				   new Ray (head.position, head.forward),
				   out hit,
				   placeDist
			   )) {
				Block hitBlock = hit.collider.transform.GetComponent<Block>();
				if (hitBlock != null && hitBlock is Typable) {
					foreach (char c in Input.inputString) {
						((Typable)hitBlock).Type (c);
					}
				}
			}
		}
		float slider = Input.GetAxis ("Slider");
		if (slider != 0) {
			RaycastHit hit;
			if (Physics.Raycast (
				new Ray (head.position, head.forward),
				out hit,
				placeDist
			)) {
				Block hitBlock = hit.collider.transform.GetComponent<Block>();
				if (hitBlock != null && hitBlock is Slidable) {
					((Slidable)hitBlock).Slide (this, slider);
				}
			}
		}
		if (Input.GetAxisRaw ("Sit") > 0f) {
			if (!sitDown) {
			sitDown = true;
				if (sitting) {
					sitting = false;
					rb.isKinematic = false;
					rb.detectCollisions = true;
				} else {
					RaycastHit hit;
					if (Physics.Raycast (
						   new Ray (head.position, head.forward),
						   out hit,
						   placeDist
					   )) {
						Transform hitTransform = hit.collider.transform;
						Block hitBlock = hitTransform.GetComponent<Block> ();
						if (hitBlock != null && hitBlock is Sittable) {
							sitting = true;
							sittingIn = (Sittable)hitBlock;
							sittingInTransform = hitTransform.GetChild (0);
							sittingInRigidbody = hit.rigidbody;
							rb.isKinematic = true;
							rb.detectCollisions = false;
							transform.position = sittingInTransform.position;
							transform.rotation = sittingInTransform.rotation;
						}
					}
				}
			}
		} else {
			sitDown = false;
		}
	}

	void UpdateUI () {
		string text = "";
		text += mode == 0 ? "> " : "  ";
		text += blockNames[chosenBlock].ToUpper() + "\n\n";
		text += mode == 1 ? "> " : "  ";
		text += Convert(memoryMapPosition) + "\n\n";
		text += "  #" + Convert(color[0]) + Convert(color[1]) + Convert(color[2]) + "\n\n";
		HUD.text = text;
	}

	private static string Convert(int num) {
		return System.Convert.ToString (num, 16).PadLeft (2, '0');
	}
}

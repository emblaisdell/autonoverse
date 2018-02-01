using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour {

	public const float G = 0.3f;
	public Vector3 net;
	Rigidbody rb;

	void Awake() {
		rb = GetComponent<Rigidbody> ();
	}
	
	void FixedUpdate () {
		net = Vector3.zero;
		Vector3 r;
		foreach (GravitySource source in GravitySource.sources) {
			r = source.transform.position - transform.position;
			net += (G * rb.mass * source.mass / Mathf.Pow (r.sqrMagnitude, 1.5f)) * r;
		}
		//Debug.DrawRay (transform.position, net);
		rb.AddForceAtPosition (net, rb.worldCenterOfMass);
	}
}

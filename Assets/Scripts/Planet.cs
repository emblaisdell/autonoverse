using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour {

	public Transform orbiting;
	public static Vector3 POENormal = Vector3.up; //plane of elliptic normal

	float rotAngPos = 0;
	public float rotAngVel;

	float orbAngPos = 0;
	public float orbAngVel;

	public Vector3 startR;
	public Vector3 orthoStartR;

	private Rigidbody rb;
	private float M;

	Vector3 r;

	int[][] triColors;
	Texture2D texture;

	// Use this for initialization
	void Awake () {
		rb = GetComponent<Rigidbody> ();
		SetColors ();
		SetOrbit ();
	}

	void SetColors() {
		Mesh mesh = GetComponent<MeshFilter> ().mesh;
		int length = mesh.vertices.Length / 3;

		Vector2[] uv = new Vector2[3 * length];
		for (int i = 0; i < length; i++) {
			uv [3 * i + 0] = new Vector2 ((i + 0.5f) / length, 0.5f);
			uv [3 * i + 1] = new Vector2 ((i + 0.5f) / length, 0.51f);
			uv [3 * i + 2] = new Vector2 ((i + 0.51f) / length, 0.5f);
		}

		mesh.uv = uv;

		triColors = new int[length][];
		for (int i = 0; i < length; i++) {
			triColors [i] = new int[3] {
				Random.Range (0, 255),
				Random.Range (0, 255),
				Random.Range (0, 255)
			};
		}

		texture = new Texture2D (length, 1);
		for (int i = 0; i < length; i++) {
			texture.SetPixel (i, 0, new Color (
				triColors [i] [0] / 255f,
				triColors [i] [1] / 255f,
				triColors [i] [2] / 255f
			));
		}
		texture.Apply ();
		Renderer renderer = GetComponent<MeshRenderer> ();
		renderer.material.mainTexture = texture;
	}

	void SetOrbit() {
		startR = transform.position - orbiting.position;
		orthoStartR = Vector3.Cross (startR, POENormal);

		rotAngVel = Random.Range(-0.25f, 0.25f);
		orbAngVel = Mathf.Sqrt (
			Gravity.G * orbiting.GetComponent<GravitySource> ().mass / Mathf.Pow(startR.sqrMagnitude, 1.5f)
		);
	}

	void FixedUpdate(){
		orbAngPos += orbAngVel * Time.fixedDeltaTime;
		rotAngPos += rotAngVel * Time.fixedDeltaTime;
		Vector3 r = Mathf.Cos (orbAngPos) * startR + Mathf.Sin (orbAngPos) * orthoStartR;
		rb.MovePosition (orbiting.position + r);
		rb.MoveRotation (Quaternion.AngleAxis(Mathf.Rad2Deg * rotAngPos, POENormal));
		//print (rb.velocity.magnitude);
		//print (rb.angularVelocity.magnitude);
		//rb.position = orbiting.position + r;
		//rb.velocity = Vector3.Cross (r, orbAngVel * POENormal);
		//Debug.DrawRay (transform.position, rb.velocity);
	}

}

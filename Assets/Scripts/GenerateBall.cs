using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateBall : MonoBehaviour {

	//public Transform Face;

	//int[] defTriangle = new int[] { 0, 1, 2/*, 0, 2, 1*/ };

	public static int NUM_FACES = 128;
	public float RADIUS;

	List<Vector3> verts = new List<Vector3>();
	List<int[]> tris = new List<int[]> ();

	List<Vector3> projs;
	List<int> horizon;

	void Awake(){
		
		for (int i = 0; i < NUM_FACES; i++) {
			verts.Add (Random.onUnitSphere);
		}

		for (int i = 0; i < NUM_FACES; i++) {
			Vector3 v = verts [i];
			/*List<Vector3>*/ projs = new List<Vector3> ();
			float r;
			Vector3 newVect;
			for (int j = 0; j < NUM_FACES; j++) {
				if (i == j) {projs.Add (Vector3.zero);}
				else{
					r = -1 / Vector3.Dot(verts[j] - v,v); //projection
					//print(r);
					newVect = v + r*(verts[j] - v);
					//print (newVect);
					projs.Add (newVect);
				}
			}

			Vector3 right = Vector3.Cross (Vector3.up, v).normalized;
			Vector3 up = Vector3.Cross (v, right).normalized;

			// find maximal x
			int startIndex = 0;
			float maxDot = -RADIUS;
			for (int j = 0; j < projs.Count; j++) {
				//print (j);
				float dot = Vector3.Dot(projs[j], right);
				if (dot > maxDot) {
					startIndex = j;
					maxDot = dot;
				}
			}

			//print ("startIndex");
			//print (startIndex);

			/*List<int>*/ horizon = new List<int> ();
			int nextIndex = startIndex;
			int lastIndex;
			Vector3 direction = up;

			horizon.Add (nextIndex);
			lastIndex = nextIndex;
			nextIndex = GetNextIndex (lastIndex, i, direction, projs);
			direction = (projs[nextIndex] - projs[lastIndex]).normalized;

			int count = 0;

			while(nextIndex != startIndex && count<25){
				//print ("nextIndex");
				//print (nextIndex);
				horizon.Add (nextIndex);
				lastIndex = nextIndex;
				nextIndex = GetNextIndex (lastIndex, i, direction, projs);
				direction = (projs[nextIndex] - projs[lastIndex]).normalized;

				count++;
			}

			/*print("Horizon");
			for (int j = 0; j < horizon.Count; j++) {
				print (horizon[j]);
			}*/

			for (int j = 0; j < horizon.Count; j++) {
				int jnext = (j + 1) % horizon.Count;
				//print ("j");
				//print (j);
				//print (jnext);
				if (i > horizon [j] && i > horizon [jnext]) {
					tris.Add (new int[] { 
						i, 
						horizon[j], 
						horizon[jnext]
					});
				}
			}
		}

		Vector3[] vertices = new Vector3[3 * tris.Count];
		int[] triangles = new int[3 * tris.Count];
		for (int i = 0; i < tris.Count; i++) {
			for(int j=0; j<3; j++){
				vertices [3 * i + j] = RADIUS * verts [tris [i] [j]];
				triangles [3 * i + j] = 3 * i + j;
			}
		}
		Mesh mesh = GetComponent<MeshFilter> ().mesh;
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.RecalculateNormals ();
		mesh.RecalculateTangents ();
		mesh.RecalculateBounds ();
		MeshCollider meshCollider = GetComponent<MeshCollider> ();
		meshCollider.sharedMesh = null;
		meshCollider.sharedMesh = mesh;

		/*for (int j = 0; j < tris.Count; j++) {
			GameObject face = Instantiate (Face, transform).gameObject;
			Mesh mesh = face.GetComponent<MeshFilter> ().mesh;
			mesh.vertices = tris [j];
			//print ("--------");
			//print (tris [j] [0]);
			//print (tris [j] [1]);
			//print (tris [j] [2]);
			mesh.triangles = defTriangle;
			mesh.RecalculateNormals ();

			face.GetComponent<MeshCollider> ().sharedMesh = mesh;
		}*/

	}

	int GetNextIndex(int index, int ignore, Vector3 direction, List<Vector3> projs) {
		//print (index);
		int next = 0;
		float maxDot = -1f; //antiparralell minimum
		for (int i = 0; i < projs.Count; i++) {
			//print ("==== GetNextIndex ====");
			//print (i);
			//print (index);
			//print (ignore);
			if (i != ignore && i != index) {
				//print (direction);
				//print (projs[i]);
				//print (projs [index]);
				//print ((projs[i]-projs[index]).normalized);
				float newDot = Vector3.Dot(direction, (projs[i]-projs[index]).normalized);
				//print (newDot);
				if (newDot>maxDot) {
					next = i;
					//print ("new next");
					//print (next);
					maxDot = newDot;
				}
			}
		}
		return next;
	}

	/*void Awake() {
		List<Vector3> verts = new List<Vector3> ();
		List<Vector3[]> faces = new List<Vector3[]>();

		for (int t = 0; t < 3; t++) {
			verts.Add (Random.onUnitSphere);
		}

		faces.Add(new Vector3[]{ verts[0], verts[1], verts[2] });
		faces.Add(new Vector3[]{ verts[0], verts[2], verts[1] });

		Vector3 x;

		for(int num=0; num<NUM_FACES-3; num++) {
			// ADD vertex
			x = Random.onUnitSphere;

			int oldFaces = faces.Count;
			for(int i=0; i<oldFaces; i++){
				if (!InsideFace (faces[i][0], faces[i][1], faces[i][2], x)) {
					for (int t = 0; t < 3; t++) {
						bool works = true;
						for (int v = 0; v < verts.Count; v++) {
							if (!InsideFace (faces [i] [t], faces [i] [(t+1)%3], x, verts [v])) {
								//works = false;
							}
						}
						print (t);
						print ((t + 1) % 3);
						print (works);
						if (works) {
							faces.Add(new Vector3[]{ faces [i] [t], faces [i] [(t+1)%3], x });
						}
					}
					faces.RemoveAt (i);
					oldFaces--;
				}
			}

			verts.Add (x);
		}

		for (int i = 0; i < faces.Count; i++) {
			GameObject face = Instantiate (Face, transform).gameObject;
			Mesh mesh = face.GetComponent<MeshFilter> ().mesh;
			mesh.Clear ();
			mesh.vertices = faces [i];
			mesh.triangles = defTriangle;
			MeshRenderer renderer = face.GetComponent<MeshRenderer> ();
			renderer.material.color = 
				new Color (Random.Range(0,255)/256f,
					Random.Range(0,255)/256f,
					Random.Range(0,255)/256f
				);
		}
	}

	public bool InsideFace (Vector3 a, Vector3 b, Vector3 c, Vector3 x) {
		return Vector3.Dot (Vector3.Cross (b - a, c - a), x - a) <= 0;
	}*/

	/*public static float JAGGEDNESS = 0.75f;

	void Awake() {
		Vector3 a = RADIUS * Random.onUnitSphere;
		Vector3 b = RADIUS * Random.onUnitSphere;
		Vector3 c = -(a + b).normalized;

		Vector3 a = new Vector3 (1, 0, -1 / Mathf.Sqrt (2));
		Vector3 b = new Vector3 (-1, 0, -1 / Mathf.Sqrt (2));
		Vector3 c = new Vector3 (0, 1, 1 / Mathf.Sqrt (2));
		Vector3 d = new Vector3 (0, -1, 1 / Mathf.Sqrt (2));

		List<Vector3[]> tris = new List<Vector3[]> () {
			new Vector3[] { a, b, c },
			new Vector3[] { b, a, d },
			new Vector3[] { c, b, d },
			new Vector3[] { a, c, d }
		};

		for (int i = 0; i < NUM_FACES - 4; i++) {
			int index = Random.Range (0, tris.Count - 1);
			Vector3[] tri = tris [index];
			tris.RemoveAt (index);
			float u = Random.value;
			float v = Random.value;
			if (u + v > 1f) {
				u = 1 - u;
				v = 1 - v;
			}
			// this is the secret sauce
			//Vector3 x = JAGGEDNESS * 0.5f * Vector3.Cross (tri [1] - tri [0], tri [2] - tri [0])
			//            + tri [0] + u * (tri [1] - tri [0]) + v * (tri [2] - tri [0]);
			Vector3 x = (tri [0] + u * (tri [1] - tri [0]) + v * (tri [2] - tri [0])).normalized;
			tris.Add (new Vector3[]{ tri[0], tri[1], x });
			tris.Add (new Vector3[]{ tri [1], tri [2], x });
			tris.Add (new Vector3[]{ tri [2], tri [0], x });
		}

		for (int i = 0; i < tris.Count; i++) {
			GameObject face = Instantiate (Face, transform).gameObject;
			Mesh mesh = face.GetComponent<MeshFilter> ().mesh;
			mesh.Clear ();
			mesh.vertices = tris [i];
			mesh.triangles = defTriangle;
			MeshRenderer renderer = face.GetComponent<MeshRenderer> ();
			renderer.material.color = 
				new Color (Random.Range(0,255)/256f,
					Random.Range(0,255)/256f,
					Random.Range(0,255)/256f);
		}
	}*/

	/*void Awake () {
		Vector3 triA = RADIUS*Random.onUnitSphere;
		Vector3 triB = RADIUS*Random.onUnitSphere;
		Vector3 triC = RADIUS*Random.onUnitSphere;

		List<Vector3> top = new List<Vector3> ();
		List<Vector3> bottom = new List<Vector3> ();

		for (int i = 0; i < NUM_FACES - 3; i++) {
			Vector3 vert = RADIUS*Random.onUnitSphere;
			if (InTriangle (triA, triB, triC, vert)) {
				top.Add (vert);
			} else {
				bottom.Add (vert);
			}
		}

		List<Vector3[]> tris = GenerateTriangles (triA, triB, triC, top);
		tris.AddRange (GenerateTriangles(triC, triB, triA, bottom));

		for (int i = 0; i < tris.Count; i++) {
			GameObject face = Instantiate (Face, transform).gameObject;
			Mesh mesh = face.GetComponent<MeshFilter> ().mesh;
			mesh.Clear ();
			mesh.vertices = tris [i];
			mesh.triangles = defTriangle;
			MeshRenderer renderer = face.GetComponent<MeshRenderer> ();
			renderer.material.color = 
				new Color (Random.Range(0,255)/256f,
				Random.Range(0,255)/256f,
				Random.Range(0,255)/256f);
		}
	}

	List<Vector3[]> GenerateTriangles(Vector3 a, Vector3 b, Vector3 c, List<Vector3> verts) {
		if (verts.Count == 0) {
			return new List<Vector3[]> { new Vector3[] { a, b, c } };
		}

		//int index = Random.Range (0, verts.Count - 1); //used to do this randomly
		int index = 0;
		float maxDot = 0f;
		float curDot;
		Vector3 cross = Vector3.Cross (b - a, c - a);
		for (int i = 0; i < verts.Count; i++) {
			curDot = Vector3.Dot (cross, verts [i]);
			//print (curDot);
			if (curDot > maxDot) {
				//print ("New max");
				index = i;
				maxDot = curDot;
			}
		}

		print (index);

		Vector3 x = verts[index];
		verts.RemoveAt (index);

		List<Vector3> vertsABX = new List<Vector3>();
		List<Vector3> vertsBCX = new List<Vector3>();
		List<Vector3> vertsCAX = new List<Vector3>();

		for (int i = 0; i < verts.Count; i++) {
			if (InTriangle (a, b, x, verts [i])) {
				vertsABX.Add (verts [i]);
			} else if(InTriangle (b, c, x, verts [i])) {
				vertsBCX.Add (verts [i]);
			} else {
				vertsCAX.Add (verts [i]);
			}
		}

		List<Vector3[]> tris = GenerateTriangles (a, b, x, vertsABX);
		tris.AddRange (GenerateTriangles (b, c, x, vertsBCX));
		tris.AddRange (GenerateTriangles (c, a, x, vertsCAX));

		return tris;
	}

		static bool InHalfPlane(Vector3 a, Vector3 b, Vector3 x) {
		return Vector3.Dot (Vector3.Cross (a, b), x) >= 0;
	}

	static bool InTriangle(Vector3 a, Vector3 b, Vector3 c, Vector3 x) {
		if (InHalfPlane (a, b, c)) {
			return InHalfPlane (a, b, x) && InHalfPlane (b, c, x) && InHalfPlane (c, a, x);
		} else {
			return InHalfPlane (a, b, x) || InHalfPlane (b, c, x) || InHalfPlane (c, a, x);
		}
	}*/
}

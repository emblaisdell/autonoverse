using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySource : MonoBehaviour {



	public static List<GravitySource> sources = new List<GravitySource>();
	public float mass;

	void Awake(){
		sources.Add (this);
	}
}

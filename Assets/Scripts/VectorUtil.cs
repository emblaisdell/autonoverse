using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorUtil {

	public static Vector3 ClosestDirection(Vector3 direction, Quaternion rotation) {
		// Return the principle direction wrt rotation that is closest to direction

		Vector3[] principleDirs = new Vector3[3] { Vector3.right, Vector3.up, Vector3.forward };

		float maxDot = 0;
		Vector3 outDir = Vector3.zero;

		foreach (Vector3 principleDir in principleDirs) {
			float dot = Vector3.Dot( rotation * principleDir, direction );
			if (dot > maxDot) {
				outDir = principleDir;
				maxDot = dot;
			} else if (-dot > maxDot) {
				outDir = (-principleDir);
				maxDot = -dot;
			}
		}

		return outDir;

	}

}

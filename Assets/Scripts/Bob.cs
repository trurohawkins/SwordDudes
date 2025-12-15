using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bob : MonoBehaviour {
	public float speed = 0.1f;
	float t = 0;

	void FixedUpdate () {
		Vector3 pos = transform.parent.position;
		pos.y += Mathf.Sin(t);
		transform.position = pos;
		t = (t + speed) % Mathf.PI;
	}
}

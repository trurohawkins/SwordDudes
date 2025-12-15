using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dum : MonoBehaviour {

	float speed = 10;
	Vector3 move;

	void Awake() {
		move = new Vector3 (0, 0, 0);
	}

	void Update () {
		if (Input.GetKey ("up")) {
			move.y = 1;
		} else if (Input.GetKey ("down")) {
			move.y = -1;
		} else if (Input.GetKey ("left")) {
			move.x = -1;
		} else if (Input.GetKey ("right")) {
			move.x = 1;
		} else {
			move = Vector3.zero;
		}
		transform.Translate (move * speed * Time.smoothDeltaTime);
		if (Input.GetKeyDown ("v")) {
			Debug.Log ("poo");
		}
	}
}

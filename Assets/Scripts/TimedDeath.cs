using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDeath : MonoBehaviour {

	public int lifeTime;

	// not good for a lot of stuff, cuz it doesnt deal with removeing the form
	void FixedUpdate () {
		if (lifeTime > 0) {
			lifeTime--;
		} else {
			Destroy (gameObject);
		}
	}
}

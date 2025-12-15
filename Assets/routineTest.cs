using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class routineTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		StartCoroutine (go ());
	}
	float t = 0;
	IEnumerator go() {
		while (true) {
			t = Time.time;
			yield return new WaitForSecondsRealtime (0.01f);
			//yield return new WaitForFixedUpdate ();
			Debug.Log (t - Time.time);
		//	
		}
	}
}

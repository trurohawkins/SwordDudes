using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class releaseFire : Purpose {

	fire myFlame;

	 protected override void Awake() {
		myFlame = gameObject.GetComponent<fire> ();
	}

	public override void callAction(Form col, int state, int x, int y) {
		//if (myFlame.burning.Contains (col)) {
		//Debug.Log("released");
			myFlame.burning.Remove (col);
		//}	
	}
}

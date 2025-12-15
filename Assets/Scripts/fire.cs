using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fire : Purpose {

	public List<Form> burning;

	 protected override void Awake() {
		base.Awake ();
		self.addAction (this);
		burning = new List<Form> ();
	}

	public override IEnumerator callAction(int delay) {
		for (int i = 0; i < burning.Count; i++) {
			defense d = burning[i].GetComponent<defense>();
			if (d) {
				//Debug.Log (self.name);
				d.callAction (self, 0, 0, 0);
			}
		}
		yield return new WaitForSeconds (1);
	}

	public override void callAction(Form col, int state, int x, int y) {
		//Debug.Log ("enetering fire");
		if (!burning.Contains(col)) {
			burning.Add(col);
		}
	}

}

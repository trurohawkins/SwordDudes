using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attackChunk : Purpose {
	
	circleAttack myAttack;
	bool inside;
	public int blade = -1;
	public int segment = -1;

	void Start() {
		/*
		if (transform.parent && transform.parent.gameObject) {
			myAttack = transform.parent.gameObject.GetComponent<circleAttack> ();
		}
		*/
	}

	public circleAttack getAttack() {
		return myAttack;
	}

	public void setAttack(circleAttack attack) {
		myAttack = attack;
	}

	public void setIndex(int n_blade, int n_segment) {
		blade= n_blade;
		segment = n_segment;
	}

	public override void callAction(Form enemy, int state, int x, int y) {
		//Debug.Log(name + " " + enemy.name + " " + state);
		if (!enemy || blade == -1 || segment == -1) {
			return;
		}
		defense d = enemy.gameObject.GetComponent<defense> ();
		if (d && d.swordStuck) {
			//Debug.Log("in defense + " + myAttack);
			if (myAttack) {
				float friction = 0.5f;//Mathf.Min (0.05f + ((float)(myAttack.self.hyperSpeed - myAttack.minSpeed) / (float)(myAttack.maxSpeed - myAttack.minSpeed)), 1);
				//Debug.Log (enemy.name + " curSpeed: " + myAttack.self.hyperSpeed + " friction: " + friction + " " + myAttack.maxSpeed);
				//Debug.Log(name + " is inside " + enemy.name);
				if (state == 1) {
					if (!inside) {
						//Debug.Log(name + " is inside, fricion applied");
						myAttack.hitPlayer ();
						inside = true;
						myAttack.applyFriction (friction);
					}
				} else if (state == 2) {
					if (inside) {
						//Debug.Log(name + " friction cleared");
						inside = false;
						myAttack.clearFriction ();
					}
				} 
			}
		} 
	}

}

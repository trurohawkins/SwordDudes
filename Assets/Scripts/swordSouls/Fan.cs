using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : SwordSoul {
	public Color[] colors;
	public int spread;
	bool spreading;
	bool folding;
	public int[] openSpeeds;
	public int[] normSpeeds;
	public int fanSpeed = 3;

	public override bool startBurn() {
		if (!base.startBurn ()) {
			//will.multiAngleSpacing = 50;
			//StartCoroutine(spread());
			//use other method
			//life.setPoise(2f);
			spreading = true;

			for (int k = 0; k < will.bladeMulti; k++) {
				for (int i = 0; i < will.blade [k].Count; i++) {
					will.blade [k][i].color = colors [k];
					will.blade [k] [i].transform.GetChild (0).GetChild (0).GetChild (0).GetComponent<SpriteRenderer> ().color = colors [k];
				}
			}
		}
		return true;
	}

	public override IEnumerator callAction(int delay) {
		if (burning) {
			if (spreading) {
				if (will.multiAngleSpacing < spread) {
					will.multiAngleSpacing += fanSpeed;
					if (will.checkBody (will.curPos, false) == false) {
						//Debug.Log ("oh no bad spread");
						will.multiAngleSpacing -= fanSpeed;
					}
				} else {
					will.bladeSpacing = 2;
					if (will.checkBody (will.curPos, false) == false) {
						will.bladeSpacing = 1;
					} else {
						will.minSpeed = chargedSpeed.x;//openSpeeds [0];
						will.maxSpeed = chargedSpeed.y;//openSpeeds [1];
						spreading = false;
					}
				}
			} 
		} else if (folding) {
			if (will.multiAngleSpacing > 0) {
				will.multiAngleSpacing -= fanSpeed;
				/*
				for (int k = 0; k < will.bladeMulti; k++) {
					for (int i = 0; i < will.blade [k].Count; i++) {
						will.blade [k][i].color = colors [k];
					}
				}
				
				bursting = 1;*/
			} else {
				will.bladeSpacing = 1;
				will.minSpeed = attackSpeed.x;//normSpeeds [0];
				will.maxSpeed = attackSpeed.y;//normSpeeds [1];
				folding = false;
			}
			will.calcPos ();
		} 
		//if (!folding && !spreading) {
			StartCoroutine (base.callAction (delay));
		//}
		yield return new WaitForSeconds (0.01f);
	}

	public override bool stopBurn() {
		if (base.stopBurn ()) {
			folding = true;
			//use other method
			//life.setPoise (1f);
		}
		return true;
	}

	public override void heatUp(float heat, float max) {
		//if (!spreading) {
			base.heatUp (heat, max);
		//}
	}

	public override void coolDown(float loss, float min) {
		if (!spreading) {
			if (energy - loss > min) {
				energy -= loss;
			} else {
				energy = min;
			}
			if (energy < burningPoint) {
				stopBurn ();
				if (!folding) {
					//Debug.Log ("cooling down fan");
					fade = (energy - climate) / (burningPoint - climate);
					if (fade >= 0) {
						Color cola = Color.Lerp (soulColorA, heatColorA, fade);
						Color colb = Color.Lerp (soulColorB, heatColorB, fade);
						for (int k = 0; k < will.bladeMulti; k++) {
							for (int i = 0; i < will.blade[k].Count; i++) {
								will.blade [k][i].changeColorN (cola);
								will.blade [k] [i].transform.GetChild (0).GetChild (0).GetChild (0).GetComponent<SpriteRenderer> ().color = cola;//Color.Lerp (soulColor, heatColor, fade);
								will.blade [k] [i].transform.GetChild (0).GetChild (0).GetChild (1).GetComponent<SpriteRenderer> ().color = colb;
							}
						}
						if (pAnim) {
							pAnim.lerpHeat(fade);
						}
					}
				}
			}
			if (energy <= freezingPoint) {
				startFreeze ();
			}
		}
	}

	public override void meetBody(GameObject player) {
		base.meetBody (player);
		//normSpeeds = new int[2];
		//normSpeeds [0] = will.minSpeed;
		//normSpeeds [1] = will.maxSpeed;
	}

	public override void dodge() {
		reset ();
	}

	public override void reset() {
		wielder.gameObject.GetComponent<Player> ().slowUp (0);
		will.multiAngleSpacing = 0;
		will.bladeSpacing = 1;
		will.calcPos ();
		base.reset ();
		//will.minSpeed = attackSpeed.x;//normSpeeds [0];
		//will.maxSpeed = attackSpeed.y;//normSpeeds [1];
	}

}

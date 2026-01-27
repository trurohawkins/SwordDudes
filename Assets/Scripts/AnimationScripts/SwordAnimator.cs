using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAnimator : MonoBehaviour {
	public Animator[] anims;
	protected SpriteRenderer[] sr;
	public SpriteRenderer[] heatSR;
	public string soul;
	string[] subNames;

	string[] states;
	string[] actions;
	protected string state;
	protected string action;

	protected circleAttack will;
	protected SwordSoul spirit;
	
	// used for glancing char screen
	public List<GameObject> extraGraphics;

	 void Awake() {
		states = new string[2];
		states[0] = "Blade";
		states[1] = "Burn";
		actions = new string[3];
		actions[0] = "Idle";
		actions[1] = "Swing";
		actions[2] = "Knock";
		state = states[0];
		action = actions[0];
		sr = new SpriteRenderer[anims.Length];
		for (int i = 0; i < anims.Length; i++) {
			sr[i] = anims[i].GetComponent<SpriteRenderer>();
		}
		subNames = new string[4];
		subNames[0] = "";
		subNames[1] = "Deet";
		Debug.LogWarning(name + " awake");
	}

	public virtual void initiate() { }

	protected virtual void newAnimation() {
		//Debug.Log(soul+state+action);
		anims[0].Play(soul+state+action);
		anims[1].Play(soul+state+"Deet"+action);
		/*
		if (state == "Blade") {
			invisColor(1, false);
			//Debug.Log(soul+state+"Deet"+action);
			anims[1].Play(soul+state+"Deet"+action);
		} else {
			invisColor(1, true);
		}
		*/
	}

	public void playAnim(int sprite, string action) {
		anims[sprite].Play(action);
	}
	
	// 0 - normal, 1 - burning
	public void setState(int s) {
		if (states != null) {
			string newState = states[s % states.Length];
			if (newState != state) {
				state = newState;
				newAnimation();
			}
		} else {
			Debug.LogError(name + " has no states");
		}
	}

	public void setAction(int a) {
		if (actions != null) {
			string newAction = actions[a % actions.Length];
			if (newAction != action) {
				action = newAction;
				newAnimation();
			}
		} else {
			Debug.LogWarning("no actions on " + name);
		}
	}

	public void setFlip(bool on) {
		for (int i = 0; i < sr.Length; i++) {
			sr[i].flipX = on;
		}
	}

	public float getRoto() {
		return transform.eulerAngles.z + 90;
	}

	public bool checkRotation(float pos) {
		return transform.eulerAngles.z == pos -90;
	}

	public virtual void setRotation(float pos) {
		//Debug.Log("setting roto " + pos);
		transform.eulerAngles = new Vector3(0, 0, pos - 90);
		if (pos > 0 && pos < 180) {
			setLayer(-2);
		} else {
			setLayer(5);
		}
	}

	public virtual void setLayer(int l) {
		if (sr != null) {
			for (int i = 0; i < sr.Length; i++) {
				if (i % 2 == 0) {
					sr[i].sortingOrder = l;
				} else {
					sr[i].sortingOrder = l + 1;
				}
				// hilt over sword
				if (i == 1) {
					l+=2;
				}
			}
		} else {
			Debug.LogWarning("no sr on " + name);
		}
	}

	public virtual void setHeatColor(Color c) {
		//Debug.Log(name + " heat color " + c);
		for (int i = 0; i < heatSR.Length; i++) {
			heatSR[i].color = c;
		}
	}

	public virtual void setColor(Color c, int sprite) {
		if (!will || !will.isHidden()) {
			if (sr != null && sprite < sr.Length) {
				sr[sprite].color = c;
			} else {
				Debug.LogError("sprite " + sprite + " is out of bounds for sword colors on " + name);
			}
		} else {
			//Debug.Log("couldn;t set the color");
		}
	}

	public void invisColor(int sprite, bool invisible) {
		//Debug.Log(name + " invis: " + invisible + " " + sprite);
		if (sr != null) {
			if (sprite >= 0 && sprite < sr.Length) {
				Color c = sr[sprite].color;
				if (invisible) {
					//Debug.Log("setting sprite " + sprite + " invisible");
					c.a = 0;
				} else {
					c.a = 1;
				}
				setColor(c, sprite);
			} else {
				Debug.LogError("sprite " + sprite + " is out of bounds for sword colors on " + name);
			}
		}
	}

	public virtual void setAlpha(float val) {
		//Debug.LogError(name + " set alhpa " + val);
		if (sr != null) {
			//Debug.Log("setting alpha " + val);
			for (int i = 0; i < sr.Length; i++) {
				Color c = sr[i].color;
				c.a = val;
				setColor(c, i);
			}
		} else {
			Debug.LogWarning("no sr on " + name);
		}
	}

	public void setWill(circleAttack w) {
		will = w;
	}

	public void setSoul(SwordSoul s) {
		spirit = s;
	}

	public virtual void charSelectGlance() {}

	void OnDisable() {
		Debug.LogWarning(name + " disabled");
	}

	void OnEnable() {
		Debug.LogWarning(name + " enabled");
	}
}

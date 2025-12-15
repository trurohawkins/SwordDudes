using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keyControl : MonoBehaviour {

	string[] arrows;
	string[] sword;
	int[] swordMouse;
	string dodge;
	int dodgeMouse;
	string ignition;
	string careful;
	string[] accept;
	string[] decline;

	void Awake() {
		arrows = new string[4];
		sword = new string[2];
		swordMouse = new int[2];
		accept = new string[2];
		accept[0] = "return";
		accept[1] = "right shift";
		decline = new string[1];
		decline[0] = "escape";
		DontDestroyOnLoad(gameObject);
	}

	public bool getMove() {
		for (int i = 0; i < 4; i++) {
			if (Input.GetKey (arrows [i])) {
				return true;
			}
		}
		return false;
	}

	public int getYmove() {
		if (Input.GetKey(arrows[0])) {
			return 1;
		}
		if (Input.GetKey(arrows[2])) {
			return -1;
		}
		return 0;
	}

	public int getXmove() {
		if (Input.GetKey(arrows[3])) {
			return 1;
		}
		if (Input.GetKey(arrows[1])) {
			return -1;
		}
		return 0;
	}

	public bool setArrow(string val, int dir) {
		bool goodSet = true;
		for (int i = 0; i < 4; i++) {
			if (i != dir) {
				if (arrows [i] == val) {
					goodSet = false;
				}
			}
		}
		if (goodSet) {
			arrows [dir] = val;
		}
		//Debug.Log("arrow " + dir + " set to " + val + " " + goodSet);
		return goodSet;
	}

	public string getArrow(int dir) {
		return arrows [dir];
	}

	public bool setSword(string val, int dir) {
		bool goodSet = true;
		if (val != "") {
			for (int i = 0; i < 2; i++) {
				if (i != dir) {
					if (sword [i] == val) {
						goodSet = false;
					}
				}
			}
		}
		if (goodSet) {
			if (val != "") {
				swordMouse[dir] = -1;
			}
			sword [dir] = val;
		}
		return goodSet;
	}

	public bool setSword(int val, int dir) {
		bool goodSet = true;
		if (val != -1) {
			for (int i = 0; i < 2; i++) {
				if (i != dir) {
					if (swordMouse [i] == val) {
						//goodSet = false;
					}
				}
			}
		}
		if (goodSet) {
			if (val != -1) {
				sword [dir] = "";
			}
			swordMouse [dir] = val;
		}
		return goodSet;
	}

	public string getSword(int dir) {
		return sword [dir];
	}

	public int getSwordM(int dir) {
		return swordMouse [dir];
	}

	public bool setDodge(string val) {
		dodge = val;
		dodgeMouse = -1;
		return true;
	}

	public bool setDodge(int val) {
		dodgeMouse = val;
		dodge = "";
		return true;
	}

	public string getDodge() {
		return dodge;
	}

	public int getDodgeM() {
		return dodgeMouse;
	}

	public string getAccept(int val) {
		return accept[val];
	}

	public string getDecline(int val) {
		if (val < 1) {
			return decline[val];
		} else {
			return "";
		}
	}
}

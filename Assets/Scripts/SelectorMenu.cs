using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectorMenu : MonoBehaviour {

	public bool level;
	public string[,] moKeys;
	string[] aKey;
	int cur = 0;
	public string[] options;
	public Text selection;
	public int numOptions;

	public int controlNum = -1;
	bool input;

	void Awake(){
		moKeys = new string[2, 2];
		moKeys [0, 0] = "w";
		moKeys [0, 1] = "s";
		moKeys [1, 0] = "up";
		moKeys [1, 1] = "down";
		aKey = new string[2];
		aKey [0] = "space";
		aKey [1] = "return";
	}

	public virtual void Update(){
		//float moveX = Input.GetAxis ("LeftJoystickX"+controlNum);
		float moveY = getMove();//Input.GetAxis ("LeftJoystickY"+controlNum);
		if (moveY != 0) {
			if (!input) {
				//butts [cur].image.color = colors [0];
				if (moveY > 0.5f) {
					goUp ();
				} else if (moveY < -0.5f) {
					goDown ();
				}
			}
			input = true;
			//butts [cur].image.color = colors [1];
		} else {
			input = false;
		}
		accept ();
	}

	void sendInfo(int pNum) {
		boomBox.S.yesPress (pNum);
		if (!level) {
			if (pNum == 0) {
				GameInfo.S.p1Song = cur;
			} else {
				GameInfo.S.p2Song = cur;
			}
		} else {
			GameInfo.S.level = cur;
		}
	}

	public float getMove() {
		if (controlNum >= 0) {
			float move = Input.GetAxis ("LeftJoystickY" + controlNum);
			if (move != 0) {
				return move;
			} else {
				if (Input.GetKeyDown(moKeys[controlNum, 0])) {
					return 1;
				} else if (Input.GetKeyDown(moKeys[controlNum, 1])) {
					return -1;
				}
			}
		} else {
			for (int i = 0; i < 2; i++) {
				float move = Input.GetAxis ("LeftJoystickY" + i);
				if (move != 0) {
					return move;
				} else {
					if (Input.GetKeyDown(moKeys[i, 0])) {
						return 1;
					} else if (Input.GetKeyDown(moKeys[i, 1])) {
						return -1;
					}
				}
			}
		}
		return 0;
	}

	public bool accept() {
		if (controlNum > -1) {
			if (Input.GetButtonDown ("aButton" + controlNum) || Input.GetKeyDown (aKey [controlNum])) {
				sendInfo (controlNum);
			} 
		} else {
			for (int i = 0; i < 2; i++) {
				if (Input.GetButtonDown ("aButton" + i) || Input.GetKeyDown (aKey [i])) {
					sendInfo(i);
				}
			}
		}
		return false;
	}

	void goUp(){
		boomBox.S.nextPress (controlNum);
		if (cur < options.Length - 1) {
			cur++;
		} else {
			cur = 0;
		}
		updateSel ();
	}

	void goDown(){
		boomBox.S.nextPress (controlNum);
		if (cur > 0) {
			cur--;
		} else {
			cur = options.Length - 1;
		}
		updateSel ();
	}

	void updateSel() {
		selection.text = options [cur];
	}
}

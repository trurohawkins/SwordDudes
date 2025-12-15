using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetControlsMenu : UIMenu {
	keyControl[] controllers;
	public int curController = 0;
	public Text[] moveTexts;
	public Text[] swordTexts;
	public Text dodgeText;
	public Text controlText;
	public string[] otherKeys;

	public override void Start() {
		base.Start ();
		controllers = GameInfo.S.getControls ();
		setText ();
	}

	public void changeControl() {
		curController = (curController + 1) % controllers.Length;
		setText ();
	}

	void setText() {
		string temp = "";
		for (int i = 0; i < 4; i++) {
			moveTexts [i].text = controllers [curController].getArrow (i).ToUpper();
		}
		for (int i = 0; i < 2; i++) {
			temp = controllers [curController].getSword (i).ToUpper();
			if (temp == "") {
				temp = controllers [curController].getSwordM (i) + "";
			}
			swordTexts [i].text = temp;
		}
		temp = controllers [curController].getDodge ().ToUpper();
		if (temp == "") {
			temp = controllers [curController].getDodgeM () + "";
		}
		dodgeText.text = temp;
		controlText.text = "#" + curController;
	}

	public void setMove(int dir) {
		if (!waiting) {
			moveTexts [dir].text = "---";
			waiting = true;
			moveVal = dir;
			setInput (false);
		}
	}

	public void setSword(int dir) {
		if (!waiting) {
			swordTexts [dir].text = "---";
			waiting = true;
			swordVal = dir;
			setInput (false);

		}
	}

	public void setDodge() {
		if (!waiting) {
			dodgeText.text = "---";
			waiting = true;
			dodgeVal = true;
			setInput (false);
		}
	}

	bool waiting = false;
	int moveVal = -1;
	int swordVal = -1;
	bool dodgeVal = false;

	protected override void Update() {
		if (!waiting) {
			base.Update ();
		} else {
			for (int i = 0; i < otherKeys.Length; i++) {
				if (Input.GetKeyDown (otherKeys [i])) {
					string val = otherKeys [i];
					Debug.Log (val);
					if (moveVal != -1) {
						if (controllers [curController].setArrow (val, moveVal)) {
							moveTexts [moveVal].text = val.ToUpper();
							moveVal = -1;
							waiting = false;
							inputOn ();
							return;
						}
					} else if (swordVal != -1) {
						if (controllers [curController].setSword (val, swordVal)) {
							swordTexts[swordVal].text = val.ToUpper();
							swordVal = -1;
							waiting = false;
							inputOn ();
							return;
						}
					} else if (dodgeVal) {
						if (controllers [curController].setDodge (val)) {
							dodgeText.text = val.ToUpper();
							dodgeVal = false;
							waiting = false;
							inputOn ();
							return;
						}
					}
				}
			}
			string inp = Input.inputString;
			if (inp != "" && inp != " " && inp != "\n" && inp != "\b") {
				char[] c = inp.ToCharArray ();
				string val = c [0] + "";
				if (moveVal != -1) {
					if (controllers [curController].setArrow (val, moveVal)) {
						moveTexts [moveVal].text = val.ToUpper();
						moveVal = -1;
						waiting = false;
						inputOn ();
					}
				} else if (swordVal != -1) {
					if (controllers [curController].setSword (val, swordVal)) {
						swordTexts[swordVal].text = val.ToUpper();
						swordVal = -1;
						waiting = false;
						inputOn ();
					}
				} else if (dodgeVal) {
					if (controllers [curController].setDodge (val)) {
						dodgeText.text = val.ToUpper();
						dodgeVal = false;
						waiting = false;
						inputOn ();
					}
				}

			} else {
				for (int i = 0; i < 3; i++) {
					if (Input.GetMouseButtonDown (i)) {
						if (swordVal != -1) {
							if (controllers [curController].setSword (i, swordVal)) {
								swordTexts[swordVal].text = "mouse"+i;
								swordVal = -1;
								waiting = false;
								inputOn ();
								break;
							}
						}
						if (dodgeVal) {
							if (controllers [curController].setDodge (i)) {
								dodgeText.text = "mouse" + i;
								dodgeVal = false;
								waiting = false;
								inputOn ();
								break;
							}
						}
					}
				}
			}
		}
	}

	void inputOn() {
		StartCoroutine (turnOnInput ());
	}

	IEnumerator turnOnInput() {
		yield return new WaitForSeconds (0.5f);
		setInput (true);
	}
}

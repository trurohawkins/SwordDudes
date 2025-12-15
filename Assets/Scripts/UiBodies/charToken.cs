using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class charToken : UIBody {

	public GameObject sprite;
	int playerNum;
	GridSelector master;
	SongBodyUI myButt;
	public charFunction myScreen;

	public override void Awake() {
		base.Awake();
		GameObject tmp = Instantiate (sprite, transform.parent);//parent is canvas
		visual = tmp.GetComponent<RectTransform> ();
	}

	public void setPlayerNumber(int pNum) {
		playerNum = pNum;
		Color p = GameInfo.S.playerColors [pNum];
		p.a = 0.75f;
		visual.gameObject.GetComponent<Image> ().color = p;
		visual.transform.GetChild(0).GetComponent<Image> ().sprite = GameInfo.S.pSprites[pNum];
		visual.transform.GetChild(0).GetComponent<Image>().color = p;
	}

	public override void Update() {
		if (master) {
			transform.position = master.transform.position;
		}
		/*
		Vector3 screenPos;
		if (!master) {
			screenPos = Camera.main.WorldToScreenPoint (transform.position);
		} else {
			screenPos = Camera.main.WorldToScreenPoint (master.transform.position);
		}
		visual.anchoredPosition = new Vector2 (screenPos.x - Screen.width/2, screenPos.y - Screen.height/2);//screenPos.y);
		*/
		base.Update ();
	}

	public override void Action(GridSelector gs) {
		if (!active || !gs) {
			if (!gs) {
				Debug.Log("passed null gridselector");
			}
			return;
		}
		Debug.Log("char token action " + gs.holding);
		if (master == gs && gs != null) {//place token
			if (myButt) {
				myButt.select(true, -1);
				GameInfo.S.setSong (playerNum, myButt.getSong ());//myButt.song);
				GameInfo.S.changeColor (GameInfo.S.colors[playerNum], playerNum);
				//master.removeButt (this);
			}
			master.holding = false;
			master = null;
		} else if (!gs.holding) {//pick up token
			if (!MainMenu.S.checkCharacterScreen (playerNum)) {
				MainMenu.S.addCharacterScreen ();
			}
						//if (myButt) {
			

			//}

			if (myButt) {
				GameInfo.S.setSong (playerNum, -1);
				myButt.select(false, -1);
				if (myScreen) {
					myScreen.glanceChar(myButt.getSong(), playerNum);
				}
			}

			master = gs;
			if (master) {
				Debug.Log("gs: " + gs.playerNum + " picked up token " + playerNum);
				master.holding = true;
				if (master.playerNum == -1) {
					//master.playerNum = playerNum;
					MainMenu.S.setPlayer (playerNum, master);
				} else {
					if (master.playerNum != playerNum) {
						MainMenu.S.setAI (playerNum);
					}
				}
			}

		}
	}

	void OnTriggerStay(Collider col) {
			if (myButt == null) {
				SongBodyUI butt = col.gameObject.GetComponent<SongBodyUI> ();
				if (butt && !butt.isLocked()) {
					myButt = butt;
					if (myScreen) {
						myScreen.glanceChar(butt.getSong(), playerNum);
					}
				}
			}
	}

	void OnTriggerExit(Collider col) {
			SongBodyUI butt = col.gameObject.GetComponent<SongBodyUI> ();
			if (butt == myButt) {
				if (myScreen) {
					myScreen.glanceChar(-1, playerNum);
				}
				myButt = null;
			}
	}

	public void removeMasterAndDestroy() {
		if (master) {
			master.holding = false;
			master = null;
		}
		if (myButt) {
			myButt.select(false, -1);
		}
		Destroy (visual.gameObject);
		Destroy (gameObject);
	}
}

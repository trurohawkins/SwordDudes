using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridSelector : MonoBehaviour {
	string up;
	string down;
	string left;
	string right;
	string accept;
	int acceptI = -1;
	public int controlType = 0;
	public int controlNum = -1;
	public int playerNum = -1;
	public float speed;
	Vector3 move;
	public	UIBody myButt;
	public List<UIBody> myButts;

	public bool holding;
	Image[] cursorSprites;
	public Sprite[] cursors;

	void Awake() {
		/*
		up = new string[2];
		up [0] = "w";
		up [1] = "i";
		down = new string[2];
		down [0] = "s";
		down [1] = "k";
		left = new string[2];
		left [0] = "a";
		left [1] = "j";
		right = new string[2];
		right [0] = "d";
		right [1] = "l";

		accept = new string[2];
		accept [0] = "space";
		accept [1] = "return";
		*/
		cursorSprites = new Image[2];
		cursorSprites[0] = transform.GetChild(0).GetComponent<Image>();
		cursorSprites[1] = transform.GetChild(0).GetChild(0).GetComponent<Image>();
		move = new Vector3 (0, 0, 0);
		myButts = new List<UIBody> ();
	}

	public void setColor(Color c) {
		cursorSprites[1].color = c;
	}
		
	GameObject myCursor;

	void Start() {
		UIBody cursor = gameObject.GetComponent<UIBody> ();
		myCursor = cursor.visual.gameObject;
		myCursor.transform.parent.SetParent (transform.parent);
		if (controlType == 2) {
			keyControl mine = GameInfo.S.getControl (controlNum);
			up = mine.getArrow (0);
			left = mine.getArrow (1);
			down = mine.getArrow (2);
			right = mine.getArrow (3);
			accept = mine.getDodge ();
			if (accept == "") {
				acceptI = mine.getDodgeM ();
			}
		}
	}

	void FixedUpdate() {
		transform.Translate (move * Time.smoothDeltaTime);
	}

	void Update () {
		if (controlType == 2) {
			keyControls ();
		} else if (controlType == 1) {
			padControls ();
		}
	}

	void keyControls() {
		if (Input.GetKeyDown (up)) {
			move.y = speed;
		}
		if (Input.GetKeyDown (down)) {
			move.y = -speed;
		}

		if (Input.GetKeyUp (up)) {
			if (Input.GetKey (down)) {
				move.y = -speed;
			} else {
				move.y = 0;
			}
		}
		if (Input.GetKeyUp (down)) {
			if (Input.GetKey (up)) {
				move.y = speed;
			} else {
				move.y = 0;
			}
		}

		if (Input.GetKeyDown (right)) {
			move.x = speed;
		}
		if (Input.GetKeyDown (left)) {
			move.x = -speed;
		}

		if (Input.GetKeyUp (right)) {
			if (Input.GetKey (left)) {
				move.x = -speed;
			} else {
				move.x = 0;
			}
		}
		if (Input.GetKeyUp (left)) {
			if (Input.GetKey (right)) {
				move.x = speed;
			} else {
				move.x = 0;
			}
		}

		if ((acceptI < 0 && Input.GetKeyDown (accept)) || (acceptI > -1 && Input.GetMouseButtonDown(acceptI))) {
			makeSelection ();
		}
	}

	void padControls() {
		float xInput = Input.GetAxis ("LeftJoystickX" + controlNum);
		float yInput = Input.GetAxis ("LeftJoystickY" + controlNum);
		move = new Vector3 (xInput * speed, yInput * speed);

		if (Input.GetButtonDown ("aButton" + controlNum)) {
			makeSelection ();
		}		
		if (Input.GetButtonDown ("bButton" + controlNum)) {
			MainMenu.S.dropToken (playerNum);//setNull (playerNum);
		}

	}

	void makeSelection() {
		if (myButts.Count > 0) {
			/*
			if (myButt.song >= 0) {
				GameInfo.S.setSong (playerNum, 7);//myButt.song);
			} else if (myButt.level != 0) {
				GameInfo.S.incrementLevel (myButt.level);
			}
			*/
			for (int i = 0; i < myButts.Count; i++) {
				myButts[i].Action (this);
			}
			//Debug.Log ("action pressed");
			if (boomBox.S) {
				boomBox.S.yesPress (playerNum);
			}
		}
	}

	void OnTriggerEnter(Collider col) {
		if (!holding) {
			//if (myButt == null) {
			UIBody butt = col.gameObject.GetComponent<UIBody> ();
			if (butt && butt.getShowing()) {
				butt.hover(true);
				addButt (butt);//myButt = butt;
			}
			//}
		}
	}

	void OnTriggerExit(Collider col) {
		//if (!holding) {
			UIBody butt = col.gameObject.GetComponent<UIBody> ();
			if (butt) {
				butt.hover(false);
				removeButt (butt);//myButt = null;
			}
		//}
	}

	public void addButt(UIBody newButt) {
		if (!myButts.Contains (newButt)) {
			myButts.Add (newButt);
		}
	}

	public void removeButt(UIBody oldButt) {
		if (myButts.Contains(oldButt)) {
			myButts.Remove(oldButt);
		}
	}

	public void DebugClick() {
		
	}

	public void destroySelf() {
		Debug.Log ("destroyyoyo grid selector");
		Destroy (myCursor);
		Destroy (gameObject);
	}
}

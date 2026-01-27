using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : UIMenu {

	public GameObject controls;
	public GameObject credits;
	GameObject curWindow;
	public int controlPresser;

	protected override void Update() {
		//if (curWindow == null) {
			base.Update ();
		//}
		/*
		if (Input.GetKeyDown ("escape") || Input.GetButtonDown ("bButton0") || Input.GetButtonDown ("bButton1")) {
			if (boomBox.S) {
				boomBox.S.noPress (controlNum);
			}
			if (curWindow == null) {
				goBack ();
			} else {
				closeWindow ();
			}
		}
		*/
	}

    public override void holdBack(int player) {
        base.holdBack(player);
    }

    public override void buttClick(int player) {
		controlPresser = player;
        base.buttClick(player);
    }

    public override void pressBack(int player) {
        if (curWindow) {
			closeWindow();
		}
    }

    public void goBack() {
		MainMenu.S.StartCoroutine (MainMenu.S.restart ());
		resetCursor ();
		gameObject.SetActive (false);
	}

	public void play(int val) {
		MainMenu.S.letsPlay (val);
		gameObject.SetActive (false);
	}

	public void map() {
		MainMenu.S.mapSelect();
		gameObject.SetActive(false);
	}

	public void controlsUp() {
		windowUp (controls);
		//gameObject.SetActive (false);
	}

	public void creditsUp() {
		windowUp (credits);
	}

	void windowUp(GameObject win) {
		win.SetActive (true);
		for(int i = 0; i < blocked.Length; i++) {
			blocked[i] = 1;
		}
		//MainMenu.S.gameObject.SetActive (false);
		curWindow = win;
	}

	public void closeWindow() {
		//MainMenu.S.gameObject.SetActive (true);
		for(int i = 0; i < blocked.Length; i++) {
			blocked[i] = -1;
		}
		curWindow.SetActive (false);
		curWindow = null;
	}

	public void exit() {
		Application.Quit();
	}

	public void backToStart() {
		Debug.Log ("back to start");
		checkControlScheme ();
		closeWindow ();
		gameObject.SetActive (true);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseBoy : MonoBehaviour {
	public GameObject menu;
	GameObject curMenu;

	void Update () {
		if (!curMenu) {
			if (Input.GetKeyDown("escape")) {
				pauseGame(2);
			} else {
				for (int i = 0; i < 4; i++) {
					if (Input.GetButtonDown("bButton" + i) || Input.GetButtonDown("startButton" + i)) {
						pauseGame(1);
					}
				}
			}
		}
	}

	public void pauseGame(int control) {
		curMenu = Instantiate (menu);//.GetComponent<PauseMenu>();
		PauseMenu pm = curMenu.GetComponent<PauseMenu>();
	}
}

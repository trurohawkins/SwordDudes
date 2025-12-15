using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : UIMenu {

    public override void Start() {
        base.Start();
		prePause = Map.S.flowing;
		GM.S.pausePlayers(true, true);
    }

    public void reset(){
		//StartCoroutine (GM.S.restart ());
		exitMenu ();
		GM.S.letsRestart(true);
		
		//StartCoroutine (restarting ());
	}

	//IEnumerator restarting() {
		
	//	if (GM.S.gameOver) {
	//		boomBox.S.restartGame ();
	//	}
	//	yield return new WaitForSeconds (0.3f);
	//	SceneManager.LoadScene (SceneManager.GetActiveScene().name);

	//}
	protected override void Update() {
		base.Update();
		if (Input.GetKeyDown("escape")) {
			exitMenu();
		} else if (controlNum > -1) {
			if (Input.GetButtonDown("bButton" + controlNum) || Input.GetButtonDown("startButton" + controlNum)) {
				exitMenu();
			}
		} else {
			for (int i = 0; i < 4; i++) {
				if (Input.GetButtonDown("bButton" + i) || Input.GetButtonDown("startButton" + i)) {
					exitMenu();
				}
			}
		}
	}

    public void goToMenu() {
		Time.timeScale = 1;
		GM.S.returnToMenu();
	}

	public void exitMenu() {
		if (boomBox.S) {
			boomBox.S.noPress (controlNum);
		}
		//controller.enabled = true;
		GM.S.pausePlayers(false, prePause);// prePause);
		Destroy (gameObject);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlBodyUI : UIBody {

	public int playerNumber;

	public override void Action(GridSelector gs) {
 	/*	if (playerNumber == gs.playerNum) {
			MainMenu.S.setAI (playerNumber);
		} else {*/
		/*
		if (gs.playerNum == -1 && playerNumber != gs.playerNum && MainMenu.S.getPlayer(playerNumber) == null) {
			MainMenu.S.setPlayer (playerNumber, gs);
		} else {
			Debug.Log (GameInfo.S.controls [playerNumber]);
			if (GameInfo.S.controls [playerNumber] == 0) {
				MainMenu.S.setAI (playerNumber);
			} else {
				MainMenu.S.setNull (playerNumber);
			}
		}
		*/
		if (MainMenu.S.getPlayer (playerNumber) == null) {
			if (GameInfo.S.controls [playerNumber] < 0) {
				MainMenu.S.setNull (playerNumber);
			} else {
				if (gs.playerNum == -1 && playerNumber != gs.playerNum) {
					MainMenu.S.setPlayer (playerNumber, gs);
				} else {
					MainMenu.S.setAI (playerNumber);
				}
			}
		} else {
			if (MainMenu.S.getTotalPlayers () != 1) {
				MainMenu.S.setAI (playerNumber);
			} else {
				if (gs.playerNum == -1 && playerNumber != gs.playerNum) {
					MainMenu.S.setPlayer (playerNumber, gs);
				}
			}
		}
		//}
	}
}

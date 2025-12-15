using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colorBodyUI : UIBody {

	public int playerNum;
	public int colorNum;
	public int dir;

	public override void Action(GridSelector gs) {
		colorNum = GameInfo.S.colors [playerNum];
		do {
			Debug.Log(colorNum);
			if (dir > 0) {
				colorNum = (colorNum + 1) % 4;
			} else {
				if (colorNum > 0) {
					colorNum--;
				} else {
					colorNum = 3;
				}
			}
		} while (GameInfo.S.checkColorCombo (colorNum, playerNum) == false);
		GameInfo.S.changeColor (colorNum, playerNum);
		//if (GameInfo.S.songs[playerNum] != GameInfo.S.songs[(playerNum + 1) % GameInfo.S.numPlayers] || ) {
			
		//}
	}
}

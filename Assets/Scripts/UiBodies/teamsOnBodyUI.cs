using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class teamsOnBodyUI : UIBody {

	bool teamsOn = false;

	public override void Action(GridSelector gs) {
		teamsOn = !teamsOn;
		string text = "OFF";
		if (teamsOn) {
			text = "ON";
		}
		visual.gameObject.GetComponentInChildren<Text> ().text = "Teams - " + text;
		MainMenu.S.setTeams (teamsOn);
	}

}

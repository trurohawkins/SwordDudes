using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teamBodyUI : UIBody {

	public int playerNumber;

	public override void Action(GridSelector gs) {
		GameInfo.S.cycleTeam (playerNumber);
	}

}

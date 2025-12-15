using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AILevelBodyUI : UIBody {

	public int playerNum;
	public int dir;

	public override void Action(GridSelector gs) {
		GameInfo.S.incrementAILevel (dir, playerNum);
	}

}

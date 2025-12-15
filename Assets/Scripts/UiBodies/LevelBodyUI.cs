using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBodyUI : UIBody {

	public int levelDir;

	public override void Action(GridSelector gs) {
		GameInfo.S.incrementLevel (levelDir);
	}
}

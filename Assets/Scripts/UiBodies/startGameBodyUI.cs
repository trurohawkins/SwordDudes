using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class startGameBodyUI : UIBody {

	public override void Action(GridSelector gs) {
		SceneManager.LoadScene ("arena");
	}
}

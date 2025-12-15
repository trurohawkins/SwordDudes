using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBackButton : UIBody {
    public override void Action(GridSelector gs) {
        base.Action(gs);
        StartCoroutine(MainMenu.S.restart());
    }
}

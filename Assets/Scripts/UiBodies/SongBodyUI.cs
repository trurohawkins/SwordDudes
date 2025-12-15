using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongBodyUI : UIBody {

	bool locked = true;
	public int mySong;
    selectButton butt;

    public override void Awake() {
        locked = GameInfo.S.locked[mySong];
        butt = visual.GetComponent<selectButton>();
        if (locked) {
            //visual.transform.GetChild(0).GetComponent<Text>().text = "LOCKED";
        }
        
    }

    public bool isLocked() {
        return locked;
    }

    public int getSong() {
		return mySong;
	}

    public void select(bool val, int player) {
        if (butt) {
            butt.activate(val, player);
        }
    }
}

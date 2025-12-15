using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roar : SwordSoul {

	public GameObject ghost;
	GameObject myGhost;
	public float ghostSpeed;
	PlayerAnimator ghostAnim;
	int num;
	bool stagged;
	 
	public override void setColors(int pNum) {
		base.setColors (pNum);
		myGhost = Instantiate (ghost, transform.position, transform.rotation);
		ghostAnim = myGhost.transform.GetChild (0).gameObject.GetComponent<PlayerAnimator> ();

		ghostAnim.setColors (this, pNum);
		ghostAnim.setDir(4);
		ghostAnim.setDodge (2);

		ghostAnim.setAlpha (0);
		num = pNum;
	}

	public override bool startBurn() {
		if (!base.startBurn()) {
			myPlayer.stopped = true;
			if (myPlayer.going) {
				myPlayer.going = false;
			}
			if (GM.S.drawSprites) {
				ghostAnim.setAlpha (0.65f);
				myGhost.transform.position = wielder.transform.position;
				ghostAnim.
				setMainColors(subColor[num]);
				if (pAnim) {
					pAnim.disembodiedState(true, myPlayer.dead);
				}
			}
			myPlayer.body.height = 11;
			will.revealSelf();
			//Debug.Log (wielder.transform.position + " " + wielder.name);
			will.mobileCenter = wielder.centerPoint;

			if (myPlayer.stagged) {
				getStaggered();
			}
		}
		return false;
	}

	public override bool stopBurn() {
		if (base.stopBurn()) {
			killGhost ();
			//Destroy (myGhost);
		}
		return false;
	}

	void killGhost() {
		myPlayer.stopped = false;
		will.mobileCenter = new Vector2Int (-2, -2);
		will.erasePresence (false);
		will.revealSelf ();
		ghostAnim.setAlpha (0f);
		if (pAnim) {
			pAnim.disembodiedState(false, myPlayer.dead);
		}
		myPlayer.body.height = 11;
	}

	public override void burn() {
		if (myPlayer.going) {
			//Debug.LogError ("player shouldnt be going");
		}
		if (myPlayer.moveInput && !myPlayer.stagged) {
			//if (speedCounter > speed) {
				Vector2 dir = myPlayer.mDest - will.getCenter();// - myPlayer.body.centerPoint;
				if (dir.x == 0 && dir.y == 0) {
					if (ic) {
						ic.newDest ();
					} else if (myCPU) {
						myCPU.newDest ();
					}
					dir = myPlayer.mDest - will.getCenter();
				}
				dir.Normalize ();
				//Debug.Log (dir);
				Vector2Int d = new Vector2Int ((int)(dir.x * ghostSpeed), (int)(dir.y * ghostSpeed));
				Vector2Int dest = will.mobileCenter + d;
				//Debug.Log ("trying to move to " + dest);
				if (dest.x > -1 && dest.y > -1 && dest.x < Map.S.worldSizeX && dest.y < Map.S.worldSizeY) {
					//if (will.checkMove (d, false) && wielder.checkBody (dest, false, false)) {
						//Debug.Log ("moving success");
						will.mobileCenter += d;
						myGhost.transform.Translate (d.x, d.y, 0);//poistion = new Vector3 (will.mobileCenter.x, will.mobileCenter.y, 0);
						if (will.adjustToPlayerMove (d)) {
				
						}
					//}
				}
				//speedCounter = 0;
			//} else {
				//speedCounter++;
			//}

		}
	}

	public override void reset() {
		base.reset ();
		killGhost ();
	//	Destroy (myGhost);
	}

    public override void getStaggered() {
		if (burning) {
			Debug.Log("ROAR STAGGERED");
			ghostAnim.setMainColors(detColor[num]);
			will.erasePresence(false);
		}
    }

    public override void staggerOver() {
        if (burning) {
			ghostAnim.setMainColors(subColor[num]);
			will.revealSelf();
		}
    }
}

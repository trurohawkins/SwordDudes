using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grow : Purpose {
	public int maxWidth;
	public int maxLength;

	public override IEnumerator callAction(int delay) {

		yield return new WaitForFixedUpdate();
	}
	public void Update() {
		if (Input.GetKeyDown ("g")) {
			//growing ();
		}
	}

	public void growing() {
		int oldWidth = self.width;
		int oldLength = self.length;
		bool growing = false;
		Vector2Int move = Vector2Int.zero;
		if (self.width < maxWidth) {
			if (self.width % 2 == 1) {
				move.x = -1;
			} else {
				move.x = 1;
			}
			self.width++;
			growing = true;
		}
		if (self.length < maxLength) {
			if (self.length % 2 == 1) {
				move.y = -1;
			} else {
				move.y = 1;
			}
			self.length++;
			growing = true;
		}
		//Debug.Log (move);
		if (growing) {
			self.erasePresence (false);
			self.squareBody ();
			if (self.checkBody ()) {
				self.drawBody ();
			} else {
				move *= -1;
				self.checkBody (self.centerPoint + move, true, false);
				if (self.curCollided.Count == 0) {
					self.move (self.centerPoint + move, false);
				} else {
					self.width = oldWidth;
					self.length = oldLength;
				}
			}
		}
	}

}

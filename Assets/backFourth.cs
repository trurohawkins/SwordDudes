using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class backFourth : Purpose {
	public int interval;
	public int[] dir;
	int curD = 0;
	int mi = 0;

	protected override void Awake() {
		base.Awake ();
		dir = new int[2];
		dir [0] = Random.Range (0, 8);
		dir [1] = (dir [0] + Random.Range (1, 7)) % 8;
	}

	public override IEnumerator callAction(int delay){
		if (self.speedCounter > self.speed) {
			for(int i = 0; i < self.hyperSpeed; i++) {
				self.move (dir[curD]);
				if (mi < interval && self.curCollided.Count == 0) {
					mi++;
				} else {
					mi = 0;
					curD = (curD + 1) % 2;
				}
			}
			self.speedCounter = 0;
		} else {
			self.speedCounter++;
		}
		yield return new WaitForFixedUpdate();
	}
}

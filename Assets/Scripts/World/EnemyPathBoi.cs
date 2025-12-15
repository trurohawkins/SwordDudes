using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathBoi : PathBoi {
    public override void moveTowards(Vector2Int start, Vector2Int destination) {
		Vector2Int dest = canIGo (start, destination, true);
		for (int i = 0; i < self.curCollided.Count; i++) {
			if (self.curCollided[i].id == 4 && Random.value > walkIntoDestructible) {
				blockedByDestructible = true;
			} else if (self.curCollided[i].id != 4 && self.curCollided [i] == target || self.curCollided[i].parent == target) {
				blockedByTarget = true;
			}
		}
		self.curCollided.Clear ();
		if (dest.x >= 0 && dest.y >= 0) {
			Vector2Int dir = GM.S.getClosestVec(self.centerPoint, dest);// new Vector2Int(0, -1);//target.centerPoint - self.centerPoint;
            self.move(self.centerPoint + dir, false);
		} else if (!blockedByTarget) {

		}
		if (Vector2Int.Distance(self.centerPoint, dest) > minDist) {
			Debug.Log("close enough");
		}
    }
}

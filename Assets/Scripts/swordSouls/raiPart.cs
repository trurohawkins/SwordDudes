using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class raiPart : Particle {

	public Color coreColor;
	public Color tip;
	List<int[]> volumeD;

	 protected override void Awake() {
		base.Awake ();
		volumeD = new List<int[]> ();
	}

	public override bool addParticle (Vector2Int pos) {

		if (!volume.Contains(pos)) {
			if (pos.x > 0 && pos.y > 0 && pos.x < Map.S.worldSizeX && pos.y < Map.S.worldSizeY) {
				int[] tmp = new int[4];
				tmp[0] = pos.x;
				tmp[1] = pos.y;
				do {
					tmp [2] = Random.Range (-1, 2);
					tmp [3] = Random.Range (-1, 2);
				} while (tmp [2] == 0 && tmp [3] == 0);
				volumeD.Add (tmp);
				volume.Add (pos);
			//	dir.Add (new Vector2Int (Random.Range (-1, 2), Random.Range (-1, 2)));

				Map.S.world [pos.x, pos.y].addParticle (spri);
				Map.S.world [pos.x, pos.y].changePartColor (color);
				return true;
			}
		}
		return false;
	}

	public override void spread() {
		for (int i = 0; i < volumeD.Count; i++) {
			if (Random.value < spreadSpeed) {
				int x = volumeD [i][0] + volumeD [i][2];
				int y = volumeD [i][1] + volumeD [i][3];

				Vector2Int pos = new Vector2Int (x, y);
				//Debug.Log (x + " " + y);
				addParticle (pos);
			}
		}
	}

	public override void burn() {
		List<int[]> dead = new List<int[]> ();
		//List<Vector2Int> deadP = new List<Vector2Int> ();
		for (int i = 0; i < volumeD.Count; i++) {
			Vector2Int pos = new Vector2Int(volumeD [i][0], volumeD[i][1]);
			float dist = Vector2Int.Distance (pos, origin);
			Cell home = Map.S.world [pos.x, pos.y];
			Color c = color;//home.getPartColor ();
			int yellRange = (int)(curRange * Random.Range(0.3f, 0.7f));
			if (dist < yellRange) {
				float perc = dist / yellRange;// * (1 + Random.value));
				c = Color.Lerp(color, coreColor, perc);
				//c.a = 0.5f;
			} else {
				float perc = (dist - yellRange) / (curRange - yellRange);
				c = Color.Lerp(coreColor, tip, perc);
				//c.a = Mathf.Lerp (1, 0, perc);
			}

			//dist = Vector2Int.Distance (volume [i], body.centerPoint);
			//float p = dist / (curRange * 2);// * (1 + Random.value));
			//c.a = Mathf.Lerp(1, 0, p);
			//c = Color.Lerp(Color.white, Color.yellow, perc);
			//c.a = 1 - perc;
			home.changePartColor (c);
			if (home.topFloor.sprite == null) { // to prevent particle from being overwritten by other particles
				home.addParticle (spri);
				//home.changePartColor (c);
			}
			if (Vector2Int.Distance (pos, origin) > range) {
				dead.Add (volumeD[i]);
			}
		}

		while (dead.Count > 0) {
			destroyPart (dead [0]);
			//deadP.RemoveAt (0);
			dead.RemoveAt (0);
		}
	}

	void destroyPart(int[] p) {
		Vector2Int pos = new Vector2Int (p [0], p [1]);
		volumeD.Remove (p);
		volume.Remove (pos);
		removeParticle (pos);
	}

	public override void disperse() {
		if (curRange > 0) {
			curRange -= 0.5f;
		} else {
			removeParticle (origin);
			while (volumeD.Count > 0) {
				destroyPart (volumeD [0]);
			}
		}
	}


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swellPart : Particle {

	public List<float> volTimes;
	public Color fadeOut;

	 protected override void Awake() {
		base.Awake ();
		volTimes = new List<float> ();
	}

	public override bool addParticle(Vector2Int pos) {
		if (base.addParticle (pos)) {
			volTimes.Add (Random.Range (0.8f, 1));
			return true;
		} else {
			return false;
		}
	}

	public override void burn() {
		if (burning) {
			List<Vector3> dead = new List<Vector3> ();
			for (int i = 0; i < volume.Count; i++) {
				/*
			float dist = Vector2Int.Distance (volume [i], origin);
			Cell home = Map.S.world [volume[i].x, volume[i].y];
			Color c = color;//home.getPartColor ();
			float perc = dist / (curRange * (1 + Random.value));
			c.a = 0.9f - perc;
			c.g = 0.8f - perc;
			home.changePartColor (c);
			if (home.topFloor.sprite == null) { // to prevent particle from being overwritten by other particles
				home.addParticle (spri);
				//home.changePartColor (c);
			}
			*/
				if (volTimes [i] <= 0) {
					dead.Add (new Vector3 (volume [i].x, volume [i].y, volTimes [i]));
				} else {
					Cell home = Map.S.world [volume [i].x, volume [i].y];
					Color c = color;//home.getPartColor ();
					c = Color.Lerp (color, fadeOut, volTimes [i]);
					home.changePartColor (c);
					volTimes [i] -= 0.1f;
				}
			}

			while (dead.Count > 0) {
				destroyP (dead [0]);
				dead.RemoveAt (0);
			}
		}
	}

	public override void spread() {
		if (burning) {
			for (int i = 0; i < volume.Count; i++) {
				if (volume.Count < 200) {
					if (Vector2Int.Distance (volume [i], origin) < range) {
						if (Random.value < spreadSpeed) {
							int x = volume [i].x + Random.Range (-1, 2);
							int y = volume [i].y + Random.Range (-1, 2);
							Vector2Int pos = new Vector2Int (x, y);
							//Debug.Log (x + " " + y);
							addParticle (pos);
						}
					}
				} else {
					break;
				}
			}
		}
	}
	public override void disperse() {
		List<Vector3> dead = new List<Vector3> ();
		for (int i = 0; i < volume.Count; i++) {
			if (volTimes [i] <= 0) {
				dead.Add (new Vector3 (volume [i].x, volume [i].y, volTimes [i]));
			} else {
				Cell home = Map.S.world [volume[i].x, volume[i].y];
				Color c = color;//home.getPartColor ();
				c = Color.Lerp(color, new Color(0,0,0,0), volTimes[i]);
				home.changePartColor (c);
				volTimes [i] -= 0.1f;
			}
		}
		while (dead.Count > 0) {
			destroyP (dead [0]);
			dead.RemoveAt (0);
		}
	}

	public void setColors(Color main, Color fade) {
		float alpha = color.a;
		color = main;
		color.a = alpha;
		alpha = fadeOut.a;
		fadeOut = fade;
		fadeOut.a = alpha;
	}


	 void destroyP(Vector3 pos) {
		volTimes.Remove (pos.z);
		Vector2Int p = new Vector2Int ((int)pos.x, (int)pos.y);
		removeParticle (p);
		volume.Remove (p);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dodgeParticle : Particle {
    
    public float fade = 0.01f;

    public override void spread() {
		if (burning) {
			for (int i = volume.Count - 1; i >= Mathf.Max(0, volume.Count - curRange); i--) {

				if (Random.value < spreadSpeed) {
					int x = volume[i].x + Random.Range (-1, 2);
					int y = volume[i].y + Random.Range (-1, 2);
					Vector2Int pos = new Vector2Int (x, y);
					//if (Vector2Int.Distance(pos, origin) <= curRange) {
						//Debug.Log (x + " " + y);
						addParticle (pos);
					//}
				}
			}
		}
    }
    public override void burn() {
        List<Vector2Int> dead = new List<Vector2Int> ();
		for (int i = 0; i < volume.Count; i++) {
            Cell home = Map.S.world [volume[i].x, volume[i].y];
            Color c = home.getPartColor ();
            c.a -= fade;
            home.changePartColor (c);
			if (home.topFloor.sprite == null) { // to prevent particle from being overwritten by other particles
				home.addParticle (spri);
				//home.changePartColor (c);
			}
			if (c.a <= fade) {
				dead.Add (volume[i]);
			}
        }
        while (dead.Count > 0) {
			destroyPart (dead [0]);
			dead.RemoveAt (0);
		}
    }

    public override void disperse() {
        
    }
}

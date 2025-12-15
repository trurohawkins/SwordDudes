using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : Purpose {

	public Vector2Int origin;
//	public int lifeTime;
	public float spreadSpeed;
	public Sprite spri;
	public Color color;
	public Color endColor;
	public bool startBurn;
//	[HideInInspector]
	public float curRange;
	//[HideInInspector]
	public bool burning = true;
//	[HideInInspector]
	public List<Vector2Int> volume;
	public bool sheathed = false;
	public float baseAlpha = 0.9f;

	 protected override void Awake() {
		base.Awake ();
		volume = new List<Vector2Int> ();
		curRange = range;
		if (Map.S) {
			Map.S.logParticle(this);
		}
	}

	public override IEnumerator callAction(int delay) {
		if (!sheathed) { 
			if (speedCounter >= speed) {
				if (!burning) {
					disperse ();
				} else if (volume.Count == 0) {
					addParticle (origin);
				}
				spread ();
				burn ();
				speedCounter = 0;
			} else {
				speedCounter++;
			}
		}
		yield return new WaitForFixedUpdate();
	}

	public virtual void setUp(Form bod, Form parent) {
		if (bod) {
			active = true;
			bod.addParticle (this);
		}
		if (origin.x < 0 || origin.y < 0) {
			if (bod) {
				origin = bod.centerPoint;
			} else {
				origin = new Vector2Int ((int)transform.position.x, (int)transform.position.y);
			}
		}
		if (startBurn) {
			kindle ();
		} else {
			burning = false;
		}
	}

	public virtual bool addParticle (Vector2Int pos) {
		if (!volume.Contains(pos)) {
			if (pos.x > 0 && pos.y > 0 && pos.x < Map.S.worldSizeX && pos.y < Map.S.worldSizeY) {
				volume.Add (pos);
				Map.S.world [pos.x, pos.y].addParticle (spri);
				Map.S.world [pos.x, pos.y].changePartColor (color);
				return true;
			}
		}
		return false;
	}

	public virtual bool removeParticle (Vector2Int pos) {
		//volume.Remove (pos);
		if (pos.x > 0 && pos.y > 0 && pos.x < Map.S.worldSizeX && pos.y < Map.S.worldSizeY) {
			Map.S.world [pos.x, pos.y].removeParticle();
			//Map.S.world [pos.x, pos.y].changePartColor (Color.white);
			return true;
		}
		return false;
	}

	public void move(Vector2Int pos) {
		origin = pos;
		if (burning && !sheathed) {
			addParticle (pos);
		}
	}

	public virtual void spread() {
		for (int i = 0; i < volume.Count; i++) {
			if (Random.value < spreadSpeed) {
				int x = volume[i].x + Random.Range (-1, 2);
				int y = volume[i].y + Random.Range (-1, 2);
				Vector2Int pos = new Vector2Int (x, y);
				//Debug.Log (x + " " + y);
				addParticle (pos);
			}
		}
	}

	public virtual void burn() {
		List<Vector2Int> dead = new List<Vector2Int> ();
		for (int i = 0; i < volume.Count; i++) {
			float dist = Vector2Int.Distance (volume [i], origin);
			Cell home = Map.S.world [volume[i].x, volume[i].y];
			Color c = color;//home.getPartColor ();
			float perc = dist / (curRange * (1 + Random.value));
			c.g = 0.8f - perc;
			c = Color.Lerp(color, endColor, perc);
			c.a = baseAlpha - perc;
			home.changePartColor (c);
			if (home.topFloor.sprite == null) { // to prevent particle from being overwritten by other particles
				home.addParticle (spri);
				//home.changePartColor (c);
			}
			if (c.a <= 0.1f) {
				dead.Add (volume[i]);
			}
		}

		while (dead.Count > 0) {
			destroyPart (dead [0]);
			dead.RemoveAt (0);
		}
	}

	public virtual void disperse() {
		if (curRange > 0) {
			curRange -= 0.5f;
			//yield return new WaitForFixedUpdate ();
		} else if (volume.Count > 0) {
			//removeParticle (origin);
			//burning = false;
			while (volume.Count > 0) {
				destroyPart (volume [0]);
				//	Vector2Int tmp = volume [0];
				//	volume.RemoveAt (0);
				//	removeParticle (tmp);
			}
		}

	}

	public void removeAll() {
		removeParticle (origin);
		//burning = false;
		while (volume.Count > 0) {
			destroyPart (volume [0]);
			//	Vector2Int tmp = volume [0];
			//	volume.RemoveAt (0);
			//	removeParticle (tmp);
		}
	}

	public void kindle() {
		burning = true;
		curRange = range;
		if (!sheathed) {
			addParticle (origin);
		}
	}

	public void smother() {
		burning = false;
	}

	public virtual void destroyPart(Vector2Int pos) {
		volume.Remove (pos);
		removeParticle (pos);
	}

	public void erasePresence() {
		for (int i = 0; i < volume.Count; i++) {
			removeParticle (volume [i]);
		}
	}

	// Update is called once per frame
	void Update () {


		//move(new Vector2Int ((int)transform.position.x, (int)transform.position.y));
		/*
		if (Input.GetKey("p")) {
			move(origin + new Vector2Int(0, 1));
		}
		if (Input.GetKey("l")) {
			move(origin + new Vector2Int(0, -1));
		}

		if (Input.GetKeyDown ("space")) {
			if (volume.Count > 0) {
				burning = false;
			} else {
				kindle ();
			}
		}

		if (Input.GetKeyDown ("space")) {
			StartCoroutine (flare (1));
		}
				*/
	}

	public IEnumerator flare(float time, float multi) {
		bool startBurn = burning;
		if (!startBurn) {
			kindle();
		}
		bool noRange = curRange == 0;
		if (noRange) {
			curRange = multi;
			range = multi;
		} else {
			curRange *= multi;
			range *= multi;
		}
		yield return new WaitForSeconds (time);
		if (noRange) {
			//smother ();
			range = 0;
			curRange = 0;
		} else {
			curRange /= multi;
			range /= multi;
		}
		if (!startBurn) {
			smother();
		}
	}

	public void setRange(float newR) {
		curRange = newR;
		range = newR;
	}
}

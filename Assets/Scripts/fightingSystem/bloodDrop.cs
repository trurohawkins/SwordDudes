using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bloodDrop : MonoBehaviour {

	public Sprite[] drops;
	public float[] amounts;
	public Sprite onFloor;
	int type = 0;
	public int[] lifeTimes;
	int lifeTime;
	public Vector2 speeds;
	float speed;
	public Vector2 arcs;
	float arc;
	int dir = 1;

	float step = 0;
	float cur = 0;
	float yPos;
	public float ySpread = 4;
	Form puddle;
	LargeMass mass;
	int amount;

	public float getInfo(Color c, float max, Form pudd) {
		SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
		if (sr && max > amounts[0]) {
			puddle = pudd;
			mass = pudd.GetComponent<LargeMass>();
			if (!GM.S.curDM) {
				mass.spawnIn();
			}
			for (int i = amounts.Length-1; i > 0; i--) {
				if (max >= amounts[i]) {// && Random.value < 0.75f) {
					type = i;
					break;
				}
			}
			amount = (int)max;
			sr.sprite = drops[type];
			arc = Random.Range(arcs[0], arcs[1]);
			speed = Random.Range(speeds[0], speeds[1]);
			lifeTime = Random.Range(lifeTimes[0], lifeTimes[1]);
			//lifeTime += (;
			step = Mathf.PI / lifeTime;
			//Debug.LogError(step);
			yPos = transform.position.y + Random.Range(-ySpread, ySpread);
			sr.color = c;
			if (Random.value > 0.5) {
				sr.flipX = true;
				dir = -1;
			}

			return amounts[type];
		}
		Destroy(gameObject);
		return 0;
	}

	void FixedUpdate() {
		if (lifeTime > 0) {
			Vector3 pos = transform.position;
			pos.x += speed * dir;
			cur += step;
			pos.y = yPos + (Mathf.Sin(cur) * arc);
			transform.position = pos;
			lifeTime--;
			/*
			if (transform.position.x < 0 || transform.position.x > Map.S.worldSizeX - 1 || transform.position.y < 0 || transform.position.y > Map.S.worldSizeY - 1) {
				
				Destroy(gameObject);
			}
			*/
		} else {
			land();
		}
	}

	void land() {
		//if (type > 0) {
			//Debug.Log(transform.position + " " + Map.S.worldSizeX + ", " + Map.S.worldSizeY);

			//Debug.LogError(transform.position);
		if (Map.S) {
			//Debug.Log("landed " + transform.position);
			if (mass) {
				mass.setPhysical(new Vector2Int((int)transform.position.x, (int)transform.position.y), amount);
			} else {
				Debug.LogError(name + " doesnt have connection to a large mass");
			}
			/*
			int r = 1;
			for (int i = -r; i <= r; i++) {
				for (int j = -r; j <= r; j++) {
					int x = (int)transform.position.x + i;
					int y = (int)transform.position.y + j;
					if (x > 0 && x < Map.S.worldSizeX && y > 0 && y < Map.S.worldSizeY) {
						Cell c = Map.S.world[x, y];
						//Debug.Log(c.name);
						//dont want to put blood in walls
						if (c.bottomFloor.sprite == null) {
							//c.formEnter(puddle);
							mass.spawnBody(new Vector2Int(x, y), true);
						}
					}
				}
			}
			*/
		}
		//}
		Destroy(gameObject);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vault : SwordSoul {

	public GameObject piece;
	List<GameObject> fallen;
	List<GameObject> broken;
	public int fallAmount = 2;
	public int shotPow = 30;
	public int shotMinSpd;
	public int shotMaxSpd;
	public int shotFlyAmnt;
	public int shotHyp;
	float shotInertia;

	 protected override void Awake() {
		base.Awake ();
		fallen = new List<GameObject> ();
		broken = new List<GameObject> ();
		shotInertia = (float)(shotMaxSpd + shotMinSpd) / shotFlyAmnt;
	}

	public override bool startBurn() {
		if (will.blade [0].Count >= fallAmount) {
			for (int i = 0; i < fallAmount; i++) {
				Form f = will.blade [0] [will.blade [0].Count - 1];
				f.erasePresence (false);
				GameObject tmp = Instantiate (piece, transform.position, transform.rotation);
				Form f2 = tmp.GetComponent<Form> ();
				f2.squareBody ();
				Vector3 tipDir =  f.transform.position - myPlayer.transform.position;
				attackChunk ac = tmp.GetComponent<attackChunk>();
				if (ac) {
					ac.setAttack(will);
				} else {
					Debug.Log("broken vault has no attackChunk");
				}
				if (will.team != -1) {
					Value v = f2.GetComponent<Value>();
					//v.setTeam(will.team);
				}
				Map.S.spawnForm (tmp, f.centerPoint.x + ((int)Mathf.Sign(tipDir.x) * 3), f.centerPoint.y + ((int)Mathf.Sign(tipDir.y) * 3));
				if (f2.spawned) {
					f.changeColorN (soulColorA);

					if (GM.S.drawSprites) {//turn off sowrd sprite && reset color
						if (f.transform.childCount > 0) {
							if (f.transform.GetChild (0).childCount > 0) {
								f.transform.GetChild (0).GetChild (0).GetChild (0).GetComponent<SpriteRenderer> ().color = soulColorA;//Color.Lerp (soulColor, heatColor, fade);
								f.transform.GetChild (0).GetChild (0).GetChild (1).GetComponent<SpriteRenderer> ().color = soulColorB;
								f.transform.GetChild (0).GetChild (0).gameObject.active = false;
							}
						}
					}
					fallen.Add (f.gameObject);

					broken.Add (tmp);

					//f2.parent = body;
					//f2.changeColorN (soulColorA);
					if (GM.S.drawSprites) {
						tmp.transform.GetChild (0).GetComponent<SpriteRenderer> ().color = heatColorA;//soulColorB;
					} else {
						f2.color = soulColorB;
						tmp.transform.GetChild (0).gameObject.SetActive(false);
					}
					Knockable k = f2.GetComponent<Knockable> ();
					//k.power = shotPow;
					//k.flyDirection = tipDir.normalized;
					//k.spot = new Vector2 (f2.centerPoint.x, f2.centerPoint.y);
					//Debug.Log (k.spot);
					k.setStats(shotMinSpd, shotMaxSpd, shotFlyAmnt, new Vector2 (f2.centerPoint.x, f2.centerPoint.y), tipDir.normalized);//, shotHyp, shotPow);

					//Debug.LogError ("chunk shot out " + tipDir + " " + (((int)Mathf.Sign(tipDir.x) * 3)) + " sp: " + will.curPos);
					Form drop = will.blade[0][will.blade[0].Count - 1];
					will.blade [0].RemoveAt (will.blade [0].Count - 1);
					drop.removeForm();
					if (will.blade [0].Count == 0) {
						will.active = false;
						will.clearPath (4);
						burning = true;
					} else {
						will.range--;
						if (will.range > 0) {
							will.calculateSwordRange ();
						}
						if (myCPU) {
							myCPU.updateMySwordInfo ();
						}
					}
				} else {
					Destroy(tmp);
					f2.die();
				}
			}

		} 
		energy = climate;
		return true;
	}

	public int burnTime = 30;
	int burnCounter = 0;

	public override void burn() {
		if (burnCounter < burnTime) {
			burnCounter++;
		} else {
			life.takeDamage (200, false);
		}
	}

	public override void reset() {
		base.reset ();
		for (int i = fallen.Count - 1; i >= 0; i--) {
			if (GM.S.drawSprites) {
				fallen [i].transform.GetChild (0).GetChild (0).gameObject.active = true;
			}
			will.blade [0].Add (fallen [i].GetComponent<Form>());

			broken [i].GetComponent<Form>().die ();
			broken.RemoveAt (i);
		}
		will.range = range;
		will.calculateSwordRange ();
		if (myCPU) {
			myCPU.updateMySwordInfo ();
		}
		fallen.Clear ();
		will.active = true;
	}

	public int calcDistOfShot(int frames) {
		int power = shotPow;
		int speed = 0;
		int hs = shotHyp;
		int dist = 0;
		int sc = 0;
		float hyp = shotHyp;
		float spd = 0;
		while (frames > 0) {
			if (power > 0) {
				if (sc >= speed) {
					for (int i = 0; i < hs; i++) {
						power -= hs;
						if (hs > 1) {
							hyp -= shotInertia;
							if (hyp >= 1) {
								hs = (int)hyp;
							}
						} else if (speed < shotMinSpd) {
							spd += shotInertia;
							speed = (int)spd;
						}
						//Vector2Int move = new Vector2Int((int)(self.centerPoint.x + (flyDirection.x * self.hyperSpeed)), (int)(self.centerPoint.y + (flyDirection.y * self.hyperSpeed)));
						//spot += flyDirection;
						dist++;
					}
					sc = 0;
				} else {
					sc++;
				}
			}
			frames--;
		}
		return dist;
	}

	public int calcFrameOfShot(int dist) {
		int power = shotPow;
		int speed = 0;
		int hs = shotHyp;
		int frame = 0;
		int sc = 0;
		float hyp = shotHyp;
		float spd = 0;
		while (dist > 0) {
			if (power > 0) {
				if (sc >= speed) {
					for (int i = 0; i < hs; i++) {
						power -= hs;
						if (hs > 1) {
							hyp -= shotInertia;
							if (hyp >= 1) {
								hs = (int)hyp;
							}
						} else if (speed < shotMinSpd) {
							spd += shotInertia;
							speed = (int)spd;
						}
						//Vector2Int move = new Vector2Int((int)(self.centerPoint.x + (flyDirection.x * self.hyperSpeed)), (int)(self.centerPoint.y + (flyDirection.y * self.hyperSpeed)));
						//spot += flyDirection;
						dist--;
						if (dist == 0) {
							frame++;
							break;
						}
					}
					sc = 0;
				} else {
					sc++;
				}
				frame++;
			}
		}
		return frame;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shade : SwordSoul {

	public float shadeDamage;
	public float shadeBodyDamage;
	public GameObject piece;
	public GameObject shadeWall;

	public float shadeAlpha = 0.7f;
	public int shadeTime;
	int sRange;
	//List<int> miasma;
	LargeMass miasma;
	Tiler mTiles;
	List<int> miasmaTimers;
	Form shadeForm;

    void produceShade() {
		if (life && burning) {
			Vector3 tipDir = myBlade [0] [sRange].transform.position - myPlayer.transform.position;
			for (int i = sRange; i >= 0; i--) {
				Vector2Int spawnPos = myBlade [0] [i].centerPoint;
				//Vector2Int spawnPos = new Vector2Int (sp.x + ((int)Mathf.Sign (tipDir.x) * 2), sp.y + ((int)Mathf.Sign (tipDir.y) * 2));
				Cell check = Map.S.world [spawnPos.x, spawnPos.y];
				Tiler t = check.getTiler();
				if (t) {
					if (mTiles.priority < t.priority) {
						continue;
					}
				}
				//if (check.within.Count == 0 || (check.within.Count == 1 && check.within[0].parent == wielder)) {
				if (!check.checkID(1) && !check.within.Contains (shadeForm)) {
					if (life.takeDamage (shadeDamage, false)) {
						/*
						Map.S.world [spawnPos.x, spawnPos.y].formEnter (shadeForm);
						Map.S.world [spawnPos.x, spawnPos.y].changeDrawOrder (10);
						//miasma.Add(new Vector3Int(spawnPos.x ,spawnPos.y, shadeTime));
						miasma.Add (spawnPos.x);
						miasma.Add (spawnPos.y);
						miasma.Add (shadeTime);
						*/
						miasma.spawnBody(spawnPos, false);
						miasmaTimers.Add (spawnPos.x);
						miasmaTimers.Add (spawnPos.y);
						miasmaTimers.Add(shadeTime);
					} else {
						break;
					}
						/*
					GameObject tmp = Instantiate (piece, transform.position, transform.rotation);
					Form f = tmp.GetComponent<Form> ();
					f.squareBody ();
					f.changeColorN (heatColorB);
					Map.S.spawnForm (tmp, spawnPos.x, spawnPos.y);
					*/
					//}
				} 
			}
		}
	}

    public override void swing() {
        base.swing();
		produceShade();
    }

    public override void move() {
        produceShade();
    }

    public override IEnumerator callAction(int delay) {
		if (wielder && !wielder.dead) {
			for (int i = 0; i < miasmaTimers.Count; i += 3) {
				if (miasmaTimers [i+2] > 0) {
					miasmaTimers [i + 2]--;
				} else {
					disperseMiasma (i);
					i-=3;
				}
			}
		}
		yield return base.callAction (delay);
	}

	void disperseMiasma(int i) {
		/*
		Map.S.world [miasma[i], miasma[i+1]].formLeave (shadeForm);
		Map.S.world [miasma[i], miasma[i+1]].changeDrawOrder (0);
		*/
		miasma.removeBody(miasmaTimers[i], miasmaTimers[i+1]);
		miasmaTimers.RemoveAt (i);
		miasmaTimers.RemoveAt (i);
		miasmaTimers.RemoveAt (i);
		
	}

	public override void meetBody(GameObject player) {
		base.meetBody (player);
		sRange = (int)will.range - 1;
		//miasma = new List<int> ();
		/*
		GameObject sf = Instantiate(piece);
		shadeForm = sf.GetComponent<Form> ();
		shadeForm.parent = wielder;
		shadeForm.color = heatColorA;
		shadeForm.color.a = shadeAlpha;
		ShadeBlock sb = sf.GetComponent<ShadeBlock>();
		if (myPlayer.team != -1) {
			Debug.Log("set shade team");
			Value v = sf.GetComponent<Value>();
			v.setTeam(myPlayer.team);
		}
		attackChunk ac = sf.GetComponent<attackChunk>();
		ac.setAttack(will);
		sb.spirit = this;
		TileSet ts = shadeForm.gameObject.GetComponent<TileSet> ();
		ts.tileColor = soulColorA;
		ts.tileColor.a = shadeAlpha;
		*/
		GameObject murk = Instantiate(shadeWall);
		mTiles = murk.GetComponent<Tiler>();
		shadeForm = murk.GetComponent<Form>();
		shadeForm.parent = wielder;
		ShadeBlock block = murk.GetComponent<ShadeBlock>();
		block.spirit = this;
		if (myPlayer.team != -1) {
			Value v = murk.GetComponent<Value>();
			v.setTeam(myPlayer.team);
		}
		attackChunk ac = murk.GetComponent<attackChunk>();
		ac.setAttack(will);
		Tiler tile = murk.GetComponent<Tiler>();
		tile.color = soulColorA;
		tile.color.a = shadeAlpha;
		miasma = murk.GetComponent<LargeMass>();
		if (!GM.S.curDM) {
			miasma.spawnIn();
		}
		miasmaTimers = new List<int>();
	}

	public void shadeBodyHit(Form col, int x, int y) {
		/*
		Value v = col.gameObject.GetComponent<Value>();
		if (v) {
			float damage = v.getValue("damage");
			if (damage > -1) {
				life.takeDamage(damage, false);
			}
			life.takeStagger(v.getValue ("stagger"), v.getValue ("stagTime"));
		*/
		//float oldWeight = life.weight;
		//life.weight = -1;
		life.takeDamage(shadeBodyDamage, false);//callAction(col, 0, x, y);
		//life.weight = oldWeight;
		//}
	}

	public override void reset() {
		base.reset ();
		while (miasmaTimers.Count > 0) {
			disperseMiasma (0);
		}
	}
}

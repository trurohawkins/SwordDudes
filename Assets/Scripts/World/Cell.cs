using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour {

	public int heightMod;
	public int height;
	int x;
	int y;
	public List<Form> within;
	public Sprite square;
	//public Card curTile;
	public SpriteRenderer sr;
	TileSet myTile;
	Tiler curTile;

	public SpriteRenderer topFloor;
	public SpriteRenderer bottomFloor;
	List<Form> entered;

	void Awake () {
		within = new List<Form> ();
		entered = new List<Form>();
		bottomFloor.enabled = true;
		//sr = gameObject.GetComponent<SpriteRenderer> ();
	}

	public bool checkID(int id) {
		for (int i = 0; i < within.Count; i++) {
			if (within[i].id == id) {
				return true;
			}
		}
		return false;
	}

	public void formEnter(Form f) {
		if (!f) {
			Debug.Log("no f");
			return;
		}
		if (f.id == 1 && x == 44 && y == 65) {
				Debug.LogWarning("player entering");
			}
		if (within.Contains(f) && f.id == 1) {
			//Debug.Log(name + " already contains " + f.name);
			return;
		}
		if (within.Count > 0) {
			entered.Clear();
			bool swordEntering = f.id == 1;
	

			for (int i = 0; i < within.Count; i++) {
				if (swordEntering && within [i].id == 1) {
					//Debug.LogError (within[i].name + " possible error! trying to add " + f.name + "at " + gameObject.transform.position);
				}
				if (swordEntering && within [i].id == -1) {
					//Debug.LogError (within [i].name + " possible error! trying to add " + f.name + "at " + gameObject.transform.position);
				}
				Form f2 = within[i];
				if (!f.etheral && !f2.etheral && f.checkCol(f2)) {
					if (!f.curEntered.Contains (f2)) {
						f.curEntered.Add (f2);
						entered.Add(f2);
					} else if (f2.manyBodied) {
						entered.Add(f2);
					}
				}
			}
			for (int i = 0; i < entered.Count; i++) {
				Form e = entered[i];
				if (!e.dead) {
					e.enter (f, x, y);
					f.enter (e, x, y);
				}
			}
		} else {
			setToFormColor(f);
		}
		TileSet ts = f.gameObject.GetComponent<TileSet>();
		if (ts) {
			addTile (ts);
		}
		addTiler(f.GetComponent<Tiler>());
		within.Add (f);
		int preHeight = height;
		if (!f.etheral && !f.gaseous) {
			height = f.height;
		}
		if (preHeight > height) {
			//Debug.Log(name + " form entered " + height + " was " + preHeight);
		}
		//sr.color = f.color;
	}

	public bool formLeave(Form f){
		if (within.Remove (f)) {
			//Debug.Log (within.Count);
			Color col = Color.white;
			col.a = 0;
			TileSet ts = f.gameObject.GetComponent<TileSet> ();
			if (ts) {
				if (ts == myTile) {
					Debug.Log("we both have the same tile");
					myTile = null;
				} else {
					sr.sprite = null;
				}
			} 
			Tiler t = f.GetComponent<Tiler>();
			if (t && curTile) {
				if (t == curTile) {
					curTile = null;
					sr.sprite = null;
					sr.color = col;
					splashProcessTile();
				}
			}
			if (within.Count > 0) {
				bool onlyTheGhosts = true;
				//Debug.Log ("there is still someone here");
				height = -10;
				Sprite cur = null;

				for (int i = 0; i < within.Count; i++) {
					if (within[i]) {
						if (!within [i].etheral) {
							Form w = within[i];
							if (!f.etheral) {
								w.exit (f, x, y);
								f.exit (w, x, y);
							} 
							if (!within[i].gaseous && height < within [i].height) {
								height = within [i].height;
							}
							onlyTheGhosts = false;
						}
						if (!within[i].invisible && ((!within[i].GetComponent<Tiler>() && !within [i].skin) || !GM.S.drawSprites)) {
							cur = square;
							col = within [i].color;
						} 
					}
				}
				if (!myTile || !curTile || !GM.S.drawSprites) {
					if (bottomFloor.sprite == null && cur != null) {
						bottomFloor.enabled = true;
						bottomFloor.sprite = cur;
						bottomFloor.transform.localScale = new Vector3 (1, 1, 1f);
						bottomFloor.color = col;
					}
				}
				if (onlyTheGhosts) {
					height = 0;
					//sr.color = Color.black;
				}
			} else {
				//Debug.Log("nothing left " + name);
				//remove entirely for cool effect while in debug draw
				if (!GM.S.drawSprites || !f.skin || f.debugDraw) {
					bottomFloor.sprite = null; //took out because blood is on bottom floor and we dont want to remove it when we walk over it
				}
				height = 0;
			}
			return true;
		} else {
			//Debug.Log (" I already left! ");
			return false;
		}
	}
	
	public void setToFormColor(Form f) {
		if (!f.invisible && (!(f.skin || f.GetComponent<TileSet>() || f.GetComponent<Tiler>()) || !GM.S.drawSprites || f.debugDraw)) {//if (!f.skin || ((GM.S.drawPlayerSprites && (f.id != 1 && f.id != 2)) || !GM.S.drawPlayerSprites)) {
			bottomFloor.enabled = true;
			bottomFloor.sprite = square;
			bottomFloor.transform.localScale = new Vector3 (1f, 1f, 1f);
			bottomFloor.color = f.color;
		}
	}

	public bool canEnter(Form f) {
		if (within.Count == 0 || height == 0 || height < f.height) {
			return true;
		} else {
			for (int i = 0; i < within.Count; i++) {
				if (!within[i].etheral && !within[i].gaseous) {
					return false;
				}
			}
		}
		return true;
	}

	public void addTile(TileSet ts) {//Form tileHolder) {
		if (GM.S.drawSprites) {
			//TileSet ts = tileHolder.gameObject.GetComponent<TileSet> ();
			if (ts) {
				//myTile = ts;
				if (ts.dynamic) {//Map.S.flowing) {
					sr.sprite = ts.getTile (0);
					float sprSize = 100f / ts.size;
					sr.transform.localScale = new Vector3 (sprSize, sprSize, 1f);
					sr.color = ts.tileColor;//for shade blocks
				} else {
					myTile = ts;
				}
			}
		}
	}

	public void addTiler(Tiler t) {
		if (GM.S.drawSprites && t) {
			sr.sprite = t.getTile(0);
			sr.sortingOrder = t.sorting;//sr.sortingOrder + t.sorting;
			sr.transform.localScale = new Vector3(t.size.x, t.size.y, 1);
			sr.color = t.color;
			curTile = t;
			splashProcessTile();
        }
    }

    int tileNum = 1;

	public void processTile(int x, int y) {
		if (myTile) {
			int start = -1;
			int maxCount = -1;
			for (int i = 0; i < 8; i += 2) {
				Vector2Int d = GM.S.dirs [i] * tileNum;
				if (x + d.x >= 0 && x + d.x < Map.S.worldSizeX && y + d.y >= 0 && y + d.y < Map.S.worldSizeY) {
					if (Map.S.world [x + d.x, y + d.y].within.Count == 0) {
						int cnt = 1;
						for (int j = 1; j <= 3; j++) {
							int nextD = (i + (j * 2)) % 8; 
							Vector2Int nd = GM.S.dirs [nextD] * tileNum;
							if (x + nd.x >= 0 && x + nd.x < Map.S.worldSizeX && y + nd.y >= 0 && y + nd.y < Map.S.worldSizeY) {
								if (Map.S.world [x + nd.x, y + nd.y].within.Count != 0) {
									break;
								} else {
									cnt++;
								}
							}
						}
						if (cnt > maxCount) {
							start = i;
							maxCount = cnt;
						}
					}
				}
			}
			if (!bottomFloor.enabled) {
				bottomFloor.enabled = true;
			}
			if (start == -1) {
				bottomFloor.sprite = myTile.getTile (0);
			} else {
				if (maxCount < 4) {
					bottomFloor.sprite = myTile.getTile (maxCount);
				} else {
					bottomFloor.sprite = myTile.getTile (0);
				}
				transform.eulerAngles = new Vector3 (0, 0, (start / 2) * 90);
				if (bottomFloor.sprite == square) {
					Debug.Log ("start: " + start + " " + ((start / 2) * 90) + " count: " + maxCount);
				}
				if (myTile.edgePriority) {
					bottomFloor.sortingOrder = 0;
				}
			}
			float sprSize = 100f / myTile.size;
			bottomFloor.transform.localScale = new Vector3 (sprSize, sprSize, 1f);
			bottomFloor.color = myTile.tileColor;
		} else if (within.Count > 0) {
			bottomFloor.sprite = square;
		}
	}

	void splashProcessTile() {
		if (Map.S.flowing) {
			for (int i = -1; i <= 1; i++) {
				for (int j = -1; j <= 1; j++) {
					if (x + i >= 0 && x + i < Map.S.worldSizeX && y + j >= 0 && y + j < Map.S.worldSizeY) {
						Cell c = Map.S.world[x + i, y + j];
						c.processTiler();
					}
				}
			}
		}
	}

	public void processTiler() {
		if (curTile) {
			int sides = curTile.calcTile(x, y, sr);
			if (curTile.cornerPriority && sides >= 6) {
				sr.sortingOrder = /*sr.sortingOrder + */curTile.sorting + 1;
			}
		}
	}

	public TileSet getTile() {
		return myTile;
	}

	public void setTileSize(int size) {
		tileNum = size;
	}

	public void fill(){
		//if (!within.Contains(Map.S.terrain)) {
			formEnter(Map.S.terrain);
		//}
	}

	public void floor(Form f) {
		//bottomFloor.color = f.color;
	}

	public void empty(){
			height = 0;
			bottomFloor.sprite = null;
			sr.sprite = null;
			within = new List<Form> ();
			myTile = null;
			curTile = null;
	}

	public void removeAllID(int id) {
		for (int i = 0; i < within.Count; i++) {
			if (within[i].id == id) {
				formLeave(within[i]);
				i--;
			}
		}
	}

	public void hide(){
		Color tmp = sr.color;
		tmp.a = 0;
		sr.color = tmp;
	}

	public void reveal(){
		Color tmp = sr.color;
		tmp.a = 1;
		sr.color = tmp;
	}

	public Color getPartColor() {
		return topFloor.color;
	}

	public void addParticle(Sprite p) {//float time) {
		topFloor.sprite = p;
		topFloor.enabled = true;
		//StartCoroutine (particlePass (time));
	}

	public void changePartColor(Color c) {
		topFloor.color = c;
	}

	public void removeParticle() {
		topFloor.sprite = null;
		topFloor.enabled = false;
		//topFloor.color = Color.white;
	}

	 IEnumerator particlePass(float time) {
		yield return new WaitForSeconds (time);
		topFloor.sprite = null;
	}

	public void changeDrawOrder(int newOrder) {
		sr.sortingOrder = newOrder;
	}

	public void setPos(int n_x, int n_y) {
		x = n_x;
		y = n_y;
		name = "cell " + x + ", " + y;
	}

	public Tiler getTiler() {
		return curTile;
	}
}

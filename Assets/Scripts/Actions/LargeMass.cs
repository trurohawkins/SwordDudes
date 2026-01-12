using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeMass : DungeonObject {
    public float spreadChance;
    
    public List<Vector2Int> body;
    Tiler tile;

    protected override void Awake() {
        base.Awake();
        tile = gameObject.GetComponent<Tiler>();
    }

    public void setPhysical(Vector2Int pos, int x, int y, bool circle) {
        setPhysical(pos, new Vector2Int(x, y), circle);
    }

    public void setPhysical(Vector2Int pos, int spread) {
        //center = pos;
        //int spread = size.x * size.y;
        Vector2Int cur = pos;
        Vector2Int dir = Vector2Int.zero;//GM.S.dirs[];
        int d = Random.Range(0, GM.S.dirs.Length);
        if (d % 2 != 0) {
            //d = (d + 1) % 8;
        }
        int noLoop = 0;
        int maxLoop = spread * 6;
        while (spread > 0 && noLoop < maxLoop) {
            bool s = false;
            int x = cur.x + dir.x;
            int y = cur.y + dir.y;
            if (x >= 0 && x < Map.S.worldSizeX && y >= 0 && y < Map.S.worldSizeY) {
                if (Random.value < spreadChance) {
                    if (checkAndAdd(x, y)) {
                        spread--;
                        s = true;
                    }
                }
            }
            if (s) {
                cur += dir;
                d = Random.Range(0, GM.S.dirs.Length);
                if (d % 2 != 0) {
                    //d = (d + 1) % GM.S.dirs.Length;
                }
            } else {
                d = (d + 1) % GM.S.dirs.Length;
            }
            dir = GM.S.dirs[d];
            //spread--;
            noLoop++;
        }
        if (noLoop >= maxLoop) {
            //Debug.Log("no loop exit of debris psread");
        }
    }

    public void setPhysical(Vector2Int center, Vector2Int size, bool circle) {
        for (int x = center.x - size.x / 2; x < center.x + size.x / 2; x++) {
			for (int y = center.y - size.y / 2; y < center.y + size.y / 2; y++){
				if (x >= 0 && x < Map.S.worldSizeX && y >= 0 && y < Map.S.worldSizeY) {
                    Cell c = Map.S.world[x, y];
                    if (Random.value < spreadChance && (!circle || Vector2Int.Distance(center, new Vector2Int(x, y)) < Mathf.Min(size.x, size.y)*0.33)) {
                        checkAndAdd(x, y);
                    }
				}
			}
		}
        //Debug.Log(name + " " + body.Count);
    }

    public void setPhysicalRing(Vector2Int center, Vector2Int size, int cenSize) {
        for (int x = center.x - size.x / 2; x < center.x + size.x / 2; x++) {
			for (int y = center.y - size.y / 2; y < center.y + size.y / 2; y++){
				if (x >= 0 && x < Map.S.worldSizeX && y >= 0 && y < Map.S.worldSizeY) {
                    Cell c = Map.S.world[x, y];
                    //
                    float dist = Vector2Int.Distance(center, new Vector2Int(x, y));
                    float distX = Mathf.Abs(center.x - x);
                    float distY = Mathf.Abs(center.y - y);
                    float val = 0.16f;
                   
                    //if (distX < size.x*val && distY < size.y*val && dist > cenSize) {
                    if (dist < Mathf.Min(size.x, size.y)*0.33 && dist > cenSize) {
                        if (Random.value < spreadChance) {
                            checkAndAdd(x, y);
                        }
                    }
				}
			}
		}
    }

    public bool checkAndAdd(int x, int y) {
        Cell c = Map.S.world[x, y];
        Tiler t = c.getTiler();
        if (t) {
            if (tile.priority < t.priority) {
                return false;
            }
        }
        if (!c.within.Contains(self) && !body.Contains(new Vector2Int(x, y)) && !c.checkID(-1)) {
            if (master && !master.readyToGo) {
                addBody(new Vector2Int(x, y));
            } else {
                spawnBody(new Vector2Int(x, y), true);
            }
            return true;
        }
        return false;
    }

    public override bool spawnIn() {
        spawned = true;
        int goodSpawn = 0;
        for (int i = 0; i < body.Count; i++) {
            Cell c = Map.S.world[body[i].x, body[i].y];
            if (!c.checkID(-1) && ((master && !master.readyToGo) || c.canEnter(self))) {//height == 0 || c.height < self.height) {
                c.formEnter(self);
                goodSpawn++;
            } else {
                //Debug.Log(master.name + "'s " + name + " removing self at " + c.name);
                for (int j = 0; j < c.within.Count; j++) {
                    //Debug.Log(c.within[j].name);
                }
                body.RemoveAt(i);
                i--;
            }
        }
        //Debug.Log(name + " had a good spawn: " + goodSpawn);
        //master.addDebris(goodSpawn);
        return body.Count > 0;
    }

    public override void callAction(Form enemy, int state, int x, int y) { }

    public override void cleanUp() {
        spawned = false;
        for (int i = 0; i < body.Count; i++) {
            Cell c = Map.S.world[body[i].x, body[i].y];
            c.formLeave(self);
        }
    }

    public void addBody(Vector2Int p) {
        body.Add(p);
    }

    public void removeBody(Vector2Int p) {
        removeBody(p.x, p.y);
    }

    public void removeBody(int x, int y) {
        if(!Map.S.world[x,y].formLeave(self)) {
            //Debug.Log(name + " wasnt in " + x + ", " + y);
        }
        body.Remove(new Vector2Int(x, y));
    }


    public void spawnBody(Vector2Int p, bool check) {
        Cell c = Map.S.world[p.x, p.y];
        //Debug.Log("within: " + c.within.Count + " height
        if (!check || c.canEnter(self)) {//height < self.height) {
            c.formEnter(self);
            addBody(p);
        }
    }


    public override void receiveMaster(DungeonRoom n_master) {
        base.receiveMaster(n_master);
        if (isTarget) {// && body.Count > 0) {
            master.targetCount++;
        }
    }
}

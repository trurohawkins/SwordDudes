using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shot : MonoBehaviour {

    public bool room;
    public bool pause = true;
    public int speaker = 0;
    public int doorSpace = 5;
    public float leadUp;
    public Sprite image;
    public string[] text;
    public string[] analog;
    public string[] key;
    public string[] waveText;
    public string[] afterText;
    public Sprite afterImage;
    public Shot pre;
    public GameObject[] next;
    public GameObject[] storyPoint;
    public DungeonRoom myRoom;
    protected List<StoryPoint> stories;
    public bool read = false;
    public Storybo narrator;
    public int roomType = 1;
    //public bool noRoomClear = true;

    void Awake() {
        stories = new List<StoryPoint>();
    }

    public virtual void beforeShot() {
        if (GameInfo.S.controls[0] == 1) {
            appendText(analog);
        } else {
            appendText(key);
        }
    }

    public virtual IEnumerator readyingShot(Dialogue d) {
        yield return new WaitForSeconds(leadUp);
        d.startWriting();
        //StartCoroutine(d.writeOutWords());
    }

    public virtual bool readyForShot() {
        return !read;
    }

    int cur = 0;
    public virtual GameObject nextShot(int dir) {
        if (cur < next.Length) {
            GameObject shot = next[cur];
            cur++;
            return shot;
        }
        return null;
    }

    public bool played() {
        return true;
    }

    public bool randomRoom;

    public virtual void shapeRoom(DungeonRoom room, mapTerrain creator, int dir) {
        myRoom = room;
        room.setDoorSpace(doorSpace);
        Vector2Int center = new Vector2Int(Map.S.worldSizeX / 2, Map.S.worldSizeY / 2);
        creator.clearCircle(center, Map.S.worldSizeX - doorSpace*2);
        creator.clearRect(center, Map.S.worldSizeX - doorSpace * 2, Map.S.worldSizeY - doorSpace * 2);
        for (int i = 0; i < 4; i++) {
            room.doors[i] = true;
        }
        /*
        Enemy baddy = makeEnemy(20, 30, room);
        if (baddy) {
            baddy.setStats(0.3f);
            baddy.longRange(0.2f);
            baddy.moveAndShoot = true;
            baddy.setSize(3);
            baddy.setWeight(20);
            baddy.setStaggerDef(15, 25);
            baddy.setKnockback(0.3f);
            baddy.attackDieOnImpact = false;
            baddy.senseRange = 25;
        }
        Debug.LogError("making baad");
        */
        if (!narrator.getBroom()) {
            //Debug.Log("poo poo broom two");
            //creator.clearRect((int)(Map.S.worldSizeX * 0.666f), (int)(Map.S.worldSizeY * 0.2f), 20, 10);
            Vector2Int p = new Vector2Int(45, 55);
            //creator.fillRect(p, 25, 25);
            room.dm.spawnSword(10, p, room);
            //room.dm.spawnSword(7, new Vector2Int(45, 35), room);
            //room.dm.spawnSword(10, new Vector2Int(55, 55), room);
            //room.dm.makeEnemy(45, 60, room);
            //room.dm.spawnKnocker(42, 10, room);
            //room.dm.spawnHeal(new Vector2Int(21, 21), room);
            //room.dm.spawnWalk(new Vector2Int(45, 60), 30, 30, room);
        }
        //room.dm.spawnKnocker(60, 45, room);
        //room.dm.spawnHeal(new Vector2Int(35, 21), room);
        if (dir == -1) {
            room.beatRoom(false);
        }
        room.setUpRoom(-1);
    }

    public virtual void cleanUpRoom(DungeonRoom room, mapTerrain creator) {
        // stop dialogue from ta;king
        Debug.Log("ckeaning up room");
        if (narrator.dia.isWriting()) {
            Debug.LogError("stop writing");
            narrator.dia.stopWriting();
        }
    }

        public void getNarrator(Storybo nar) {
        narrator = nar;
    }

    public virtual void roomBeat(DungeonRoom room) {
        if (afterText.Length > 0) {
            narrator.dia.curPause = 0;
            narrator.dia.dumpShit();
            narrator.dia.shotArt(this, true);
            narrator.dia.readText(afterText);
        }
    }

    public void appendText(string[] t) {
        if (t.Length == 0) {
            return;
        }
        string[] full = new string[t.Length + text.Length];
        for (int i = 0; i < t.Length + text.Length; i++) {
            if (i < text.Length) {
                full[i] = text[i];
            } else {
                full[i] = t[i-text.Length];
            }
        }
        text = full;
    }

    protected StoryPoint spawnStoryPoint(GameObject sp, int xp, int yp) {
        TargetRoom tr = myRoom.GetComponent<TargetRoom>();
        DungeonObject dun = tr.logObj(sp);
        dun.self.squareBody();
        //creator.clearRect(xp, yp, dun.self.width + 1, dun.self.height + 1);
        dun.pos = new Vector2Int(xp, yp);// new Vector2Int(50, 45);
        StoryPoint stry = dun.GetComponent<StoryPoint>();
        stry.getDialogue(narrator.dia);
        stories.Add(stry);
        return stry;
    }

    protected Enemy makeEnemy(int xp, int yp, DungeonRoom room) {
        DungeonObject dun = room.dm.makeEnemy(xp, yp, room);
        if (dun) {
            return dun.GetComponent<Enemy>();
        }
        return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstSteps : Shot {
    TargetRoom tr;
    mapTerrain creator;
    public string[] altKey;
    public string[] altAnalog;
    bool readBroom = false;
    DungeonRoom curDun;

    public override bool readyForShot() {
        if (!read) {
            return true;
        } else {
            return narrator.getBroom() && !readBroom && (curDun && !curDun.beat);
        }
    }

    public override void beforeShot() {
        if (!narrator.getBroom()) {
            base.beforeShot();
        } else if (!readBroom && !curDun.beat) {
            readBroom = true;
            read = false;
            pause = true;
            if (GameInfo.S.controls[0] == 1) {
                text = altKey;
            } else {
                text = altAnalog;
            }
            stories[0].deleteSelf();
            stories.RemoveAt(0);
            for (int i = 0; i < stories.Count; i++) {
                stories[i].alterText();
            }
        }
    }

    public override void shapeRoom(DungeonRoom room, mapTerrain crea, int dir) {
        curDun = room;
        stories = new List<StoryPoint>();
        creator = crea;
        Vector2Int center = new Vector2Int(Map.S.worldSizeX/2, Map.S.worldSizeY/2);
        //creator.clearCircle(center, 30);
        tr = room.GetComponent<TargetRoom>();
        if (!tr) {
            Debug.Log("no tr  " + room.name);
        }

        DungeonObject dun = room.logObj(storyPoint[1]);
        StoryPoint stry = dun.GetComponent<StoryPoint>();
        stry.getDialogue(narrator.dia);
        stories.Add(stry);
        room.setHallSize(8, 12);
        CreepDebris creepy = null;
       // followed the Narrator's instructions
        if (dir == 1) {
            room.doors[0] = true;
            room.doors[3] = true;
            room.connectRoom(new Vector2Int(75, center.y), new Vector2Int(75, 75));
            creator.clearCircle(new Vector2Int(75, 75), 20);
            creator.clearCircle(new Vector2Int(15, 75), 20);
            room.connectRoom(new Vector2Int(75, 75), new Vector2Int(15, 75));
            room.connectRoom(new Vector2Int(15, 80),  new Vector2Int(15, 15));
            room.connectRoom(new Vector2Int(15, 15), new Vector2Int(40, 15));
            creator.clearRect(new Vector2Int(45, 33), 35, 45);
            // 1st room switch & 2nd
            spawnSwitchClear(75, 75).spawnIn();
            spawnSwitchClear(15, 75).spawnIn();

            spawnSwitchClear(Map.S.worldSizeX/2, 45).spawnIn();

            spawnStoryPoint(storyPoint[2], 60, 75);
            spawnStoryPoint(storyPoint[0], 15, 60);

            if (!narrator.getBroom()) {
                DungeonObject d = room.dm.spawnSword(10, new Vector2Int(Map.S.worldSizeX/2, 45), room).GetComponent<DungeonObject>();
                d.clearSpace = -1;
            }
            // 1st hall of debris, past the exit
            creepy = tr.dm.spawnDebris(new Vector2Int(25, 75), 200, tr);
            tr.dm.spawnDebris(new Vector2Int(35, 75), 200, tr);
            tr.dm.spawnDebris(new Vector2Int(45, 75), 200, tr);
            tr.dm.spawnDebris(new Vector2Int(35, 75), 25, 20, tr);
            // bottom corner of debris
            tr.dm.spawnDebris(new Vector2Int(20, 15), 300, tr);
            tr.dm.spawnDebris(new Vector2Int(25, 15), 300, tr);
            tr.dm.spawnDebris(new Vector2Int(25, 15), 300, tr);
            tr.dm.spawnDebris(new Vector2Int(15, 20), 300, tr);
            // big room of debris
            tr.dm.spawnDebris(new Vector2Int(45, 33), 300, tr);
            tr.dm.spawnDebris(new Vector2Int(48, 26), 300, tr);
            tr.dm.spawnDebris(new Vector2Int(40, 20), 300, tr);
            tr.dm.spawnDebris(new Vector2Int(55, 22), 300, tr);
            tr.dm.spawnDebris(new Vector2Int(45, 15), 35, 30, tr);
        } else if (dir == 3) {
            room.doors[1] = true;
            room.doors[2] = true;
            room.connectRoom(new Vector2Int(15, center.y), new Vector2Int(15, 15));
            creator.clearCircle(new Vector2Int(15, 15), 20);
            creator.clearCircle(new Vector2Int(75, 15), 20);
            room.connectRoom(new Vector2Int(15, 15), new Vector2Int(75, 15));
            room.connectRoom(new Vector2Int(75, 10),  new Vector2Int(75, 75));
            room.connectRoom(new Vector2Int(75, 75), new Vector2Int(40, 75));
            creator.clearRect(new Vector2Int(45, 57), 35, 45);
            // 1st room switch & 2nd
            spawnSwitchClear(15, 15).spawnIn();
            spawnSwitchClear(75, 15).spawnIn();

            spawnSwitchClear(Map.S.worldSizeX/2, 45).spawnIn();

            spawnStoryPoint(storyPoint[2], 30, 15);
            spawnStoryPoint(storyPoint[0], 75, 35);
            if (!narrator.getBroom()) {
                DungeonObject d = room.dm.spawnSword(10, new Vector2Int(Map.S.worldSizeX/2, 45), room).GetComponent<DungeonObject>();
                d.clearSpace = -1;
            }
            // 1st hall of debris, past the exit
            creepy = tr.dm.spawnDebris(new Vector2Int(65, 15), 200, tr);
            tr.dm.spawnDebris(new Vector2Int(55, 15), 200, tr);
            tr.dm.spawnDebris(new Vector2Int(45, 15), 200, tr);
            tr.dm.spawnDebris(new Vector2Int(55, 15), 25, 20, tr);
            // bottom corner of debris
            tr.dm.spawnDebris(new Vector2Int(70, 75), 300, tr);
            tr.dm.spawnDebris(new Vector2Int(65, 75), 300, tr);
            tr.dm.spawnDebris(new Vector2Int(65, 75), 300, tr);
            tr.dm.spawnDebris(new Vector2Int(75, 70), 300, tr);
            // big room of debris
            tr.dm.spawnDebris(new Vector2Int(45, 57), 300, tr);
            tr.dm.spawnDebris(new Vector2Int(48, 64), 300, tr);
            tr.dm.spawnDebris(new Vector2Int(40, 70), 300, tr);
            tr.dm.spawnDebris(new Vector2Int(55, 68), 300, tr);
            tr.dm.spawnDebris(new Vector2Int(45, 75), 35, 30, tr);
        }
        if (creepy) {
            creepy.cleanPercent = 0.65f;
        }
        room.wallsAndDoors();
        room.setUp = true;
    }

    void spawnSwitchClear(float x, float y) {
        spawnSwitchClear((int)x, (int)y);
    }

    LargeMass spawnSwitchClear(int x, int y) {
        Vector2Int sw = new Vector2Int(x, y);
        creator.clearCircle(sw, 15);
        LargeMass lm = null;
        //Debug.Log("spawn switch " + x + ", " + y);
        if (!tr) {
            Debug.LogError("no training room");
        } else {
            lm = tr.dm.spawnWalk(new Vector2Int(x, y), 10, 10, tr);
            //tr.spawnSwitch(x, y, -1, false);
        }
        return lm;
    }

    void oldPath(TargetRoom room, int dir) {
        Vector2Int center = new Vector2Int(Map.S.worldSizeX/2, Map.S.worldSizeY/2);
        if (dir == 3) {
            room.connectRoom(new Vector2Int(12, 46), new Vector2Int(45, 14));
            room.connectRoom(new Vector2Int(23, 45), new Vector2Int(45, 12));
            room.connectRoom(new Vector2Int(44, 12), new Vector2Int(86, 76));
            room.connectRoom(new Vector2Int(86, 76), new Vector2Int(44, 81));

            spawnSwitchClear(Map.S.worldSizeX/5 - 5, center.y);
            spawnSwitchClear(Map.S.worldSizeX * 0.85f, Map.S.worldSizeY * 0.85f);            
            //spawnSwitchClear(Map.S.worldSizeX * 0.5f, Map.S.worldSizeY * 0.15f);

            tr.dm.spawnDebris(new Vector2Int(64, 50), 30, 30, tr);//.spawnIn();
            tr.dm.spawnDebris(new Vector2Int(60, 40), 20, 20, tr);//.spawnIn();
            tr.dm.spawnDebris(new Vector2Int(53, 27), 20, 20, tr);//.spawnIn();
        } else {
            room.connectRoom(new Vector2Int(78, 46), new Vector2Int(67, 45));
            room.connectRoom(new Vector2Int(67, 45), new Vector2Int(45, 12));
            room.connectRoom(new Vector2Int(46, 12), new Vector2Int(14, 76));
            room.connectRoom(new Vector2Int(14, 76), new Vector2Int(44, 81));

            spawnSwitchClear(Map.S.worldSizeX * 0.8f - 5, center.y);
            spawnSwitchClear(Map.S.worldSizeX * 0.15f, Map.S.worldSizeY * 0.85f);
            //spawnSwitchClear(44, 13);
            tr.dm.spawnDebris(new Vector2Int((int)(Map.S.worldSizeX * 0.17f), center.y), 10, 50, tr);//.spawnIn();
            tr.dm.spawnDebris(new Vector2Int(26, 50), 30, 30, tr);//.spawnIn();
            tr.dm.spawnDebris(new Vector2Int(30, 40), 20, 20, tr);//.spawnIn();
            tr.dm.spawnDebris(new Vector2Int(37, 27), 20, 20, tr);//.spawnIn();
        }

        Vector2Int sp = new Vector2Int(center.x, (int)(Map.S.worldSizeY * 0.15f));
        creator.clearCircle(sp, 20);

        spawnSwitchClear(sp.x, sp.y);
        DungeonObject dun = tr.logObj(storyPoint[0]);
        dun.self.squareBody();
        dun.pos = new Vector2Int(sp.x, sp.y);// new Vector2Int(50, 45);
        StoryPoint stry = dun.GetComponent<StoryPoint>();
        stry.getDialogue(narrator.dia);
        if (GameInfo.S.controls[0] == 1) {
            stry.appendText(analog);
        } else {
            stry.appendText(key);
        }
        dun.spawnIn();
        //for debris
        Debug.Log("target count " + room.targetCount);
        room.targetCount--;
    }
}

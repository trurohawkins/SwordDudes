using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoom : DungeonRoom {
    int bossSong = 7;
    
    protected override void setUpDoors(int door) {

    }

    public override bool setUpRoom(int dir) {
        if (base.setUpRoom(dir)) {
            //creator.fullWorld(true);
            //creator.fullWorld(false);
            challengeType = -1;
            bossSong = dm.getBossSword();
            dm.spawnBoss(bossSong, Mathf.Max(1, dm.progress), this);
            /*
            int np = GM.S.getNumPlayers() + 1;
            Vector2Int pos = new Vector2Int((int)(Map.S.worldSizeX*0.15f), Map.S.worldSizeY/2);
            creator.clearCircle(pos, 10);
            Vector3Int spawnPoint = new Vector3Int(pos.x, pos.y, -90);
            DungeonObject d = setUpObj(guy, spawnPoint.x, spawnPoint.y);
            InputComputer ic = d.GetComponent<InputComputer>();
            ic.difficultyLevel = 1;
            Player pp = d.GetComponent<Player>();
            BossEnemy b = d.GetComponent<BossEnemy>();
            b.setSong(bossSong);
            pp.soulType = GameInfo.S.souls[bossSong];
            //need change pNum?
            Debug.Log("gameibfo[" + (np-1) + "] = " + GameInfo.S.colors[np-1]);
		    pp.setInfo(np - 1, GameInfo.S.colors[np-1], GameInfo.S.playerColors [np-1], -1);
		    pp.setSpawnInfo (spawnPoint);
		    //players [i].hasSword = hasSwords [i];
		    GM.S.spawnPlayer (pp.gameObject, spawnPoint.x, spawnPoint.y);
            pp.dead = false;
            spawnObj(d);
            d.receiveMaster(this);
            */
        }
        return true;
    }

    public override void shapeRoom(int dir) {
        //circleRoom();
        for (int i = 0; i < 4; i++) {
            //doors[i] = false;
        }
        Vector2Int center = new Vector2Int(Map.S.worldSizeX / 2, Map.S.worldSizeY / 2);
        creator.clearCircle(center, Map.S.worldSizeX - doorSpace*2);
        //creator.clearRect(center, Map.S.worldSizeX - doorSpace*2, Map.S.worldSizeY - doorSpace*2);
        //creator.fillRect(new Vector2Int(60, 45), 4, 30);

        creator.pathStart();
        pathNode cen = creator.makePathNode(center);
        pathNode a = creator.makePathNode(60, 70);
        creator.setNeighbors(cen, a, 7);
        
    }

    public override void beatRoom(bool paused) {
        base.beatRoom(paused);
        StartCoroutine(waitForNoTalk());
    }

    IEnumerator waitForNoTalk() {
        while(dm.storybo.dia.isWriting()) {
            yield return new WaitForEndOfFrame();
        }
        dm.beatBoss();
        dm.clearRooms(0);
        dm.loadData();
        dm.curRoom = null;
        //room.dm.removeRoom(room);
        //room.cleanUpRoom();
        GM.S.destroyWorld(-1);
        if (boomBox.S) {
            boomBox.S.restartGame();
        }
        //room.dm.curRoom.pos = room.pos;
        // attach neighbors to new room
        /*
        for (int i = 0; i < 4; i++) {
            if (room.neighbors[i]) {
                room.neighbors[i].neighbors[(i+2)%4] = room.dm.curRoom;
            }
        }
        */
    }

    public override void cleanUpRoom() {
        base.cleanUpRoom();
        if (boomBox.S) {
            boomBox.S.rightSong(GameInfo.S.songs[GM.S.getNumPlayers()-1]);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemy : DungeonObject {
    Player p;
    InputComputer ic;
    defense d;
    int song;

    protected override void Awake() {
        base.Awake();
        p = gameObject.GetComponent<Player>();
        ic = gameObject.GetComponent<InputComputer>();
    }

    public override void cleanUp() {
        spawned = false;
        self.active = false;
        ic.enabled = false;
        p.removeBody(false);
        defense def = gameObject.GetComponent<defense>();
        def.hideUI();

        if (boomBox.S) {
            boomBox.S.rightSong(GameInfo.S.songs[GM.S.getNumPlayers()-1]);
        }
        GM.S.removePlayer(p);
        for (int i = 0; i < GM.S.players.Count; i++) {
            def = GM.S.players[i].GetComponent<defense>();
            def.placeHealthUI(i, GM.S.getNumPlayers());
        }
    }

    public override bool spawnIn() {
        ic.enabled = true;
        self.active = true;
        Vector3Int spawnPoint = new Vector3Int(pos.x, pos.y, -90);

        p.dead = false;
        //master.spawnObj(this);

        spawned = true;

        GM.S.spawnPlayer (p.gameObject, spawnPoint.x, spawnPoint.y);
		p.setSwordStart(spawnPoint.z);
        if (p.fodder) {
            p.doStart();
        }
        p.respawn(spawnPoint, true, false);
        int np = GM.S.getNumPlayers() + 1;
        defense def = p.GetComponent<defense>();
        def.heal(100, false);
        def.stopStagger();
        master.addObj(bloodPuddle.GetComponent<DungeonObject>());
        if (!p.fodder) {
            placeHealthUI(np - 1, np);
            setLivesUI(1);
            for (int i = 0; i < GM.S.players.Count; i++) {
                def = GM.S.players[i].GetComponent<defense>();
                def.placeHealthUI(i, np);
            }
            if (np == 2) {
                if (boomBox.S) {
                    boomBox.S.rightSong(song);
                }
            }
            GM.S.addPlayer(p);
        }
        return true;
    }

    public override void dying(bool burn) {
        if (!self.dead) {
            SwordSoul mySoul = p.myAttack.getSoul();
            int pNum = p.colorNum;
            /*
            GameObject tmp = Instantiate (corpse, transform.parent);
			Form poo = tmp.GetComponent<Form> ();
			poo.squareBody ();
			poo.etheral = true;
			Map.S.spawnForm (tmp, self.centerPoint.x, self.centerPoint.y);
			if (!poo.spawned) {
				poo.die ();
			} else {
				poo.etheral = false;
                FighterAnimator anim = poo.GetComponentInChildren<FighterAnimator>();
                anim.setColors(mySoul, pNum);
                anim.setDeath(true);
                //curCorpse = poo;
			}
            */
            isTarget = p.fodder;
            master.targetMet(this);

            base.dying(burn);
            //master.removeDenizen(this);
           //master.beatRoom(false);
            /*
            defense def = gameObject.GetComponent<defense>();
            def.hideUI();
            */


            p.deleteSelf();
            if (!p.fodder) {
                // added later, then postcedidng lines,
                GM.S.removePlayer(p);
                for (int i = 0; i < GM.S.players.Count; i++) {
                    defense def = GM.S.players[i].GetComponent<defense>();
                    def.placeHealthUI(i, GM.S.getNumPlayers());
                }
                GameObject sword = master.dm.spawnSword(song, new Vector2Int(self.centerPoint.x, self.centerPoint.y), master);
                if (sword) {
                    SwordPickup sp = sword.GetComponent<SwordPickup>();
                    sp.important = true;
                    sp.isTarget = true;
                } else {
                    Debug.LogError("sword didn;t spawn");
                }
                //adjust for co op
                if (boomBox.S) {
                    boomBox.S.weHaveWinner(0);
                }
            }
        }
    }

    public void setSong(int s) {
        song = s;
    }

}

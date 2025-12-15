using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordPickup : DungeonObject {
    public SpriteRenderer symbol;
    public SpriteRenderer glow;
    public Sprite[] symbols;
    public Sprite broom;
    int swordType = 7;
    bool pickedUp = false;
    public bool important;

    public override bool spawnIn() {

        return base.spawnIn();
    }

    public override void callAction(Form poo, int state, int x, int y) {
        if (!poo.dead && poo.id == 1 && !pickedUp) {
            //Debug.Log("picked up " + important + " " + poo.name + " " + isTarget + " at " + x + ", " + y);
            pickedUp = true;
            //if (important) {
                master.targetMet(this);
            //}
            Player p = poo.GetComponent<Player>();
            int oldSword = -1;//p.myAttack;
            GameInfo.S.songs[p.playerNum] = swordType;
            if (p.myAttack) {
                p.myAttack.deleteSword();
                oldSword = GameInfo.S.songs[p.playerNum];
            } else {
                Debug.Log("this is th eplayers first sword");

            }
            if (boomBox.S) {
                boomBox.S.startSwordSongs(false);
            }
            p.soulType = GameInfo.S.souls[swordType];

            p.hasSword = GM.S.hasSwords[p.playerNum] = true;
            p.setUpSword();
            //p.getDef().setUIColor();
            Vector2Int pos = self.centerPoint;
            self.die();
            master.removeDenizen(this);
            if (swordType == 10) {
                master.dm.storybo.setBroom(true);
            }
            /*
            if (oldSword >= 0) {
                dm.spawnSword(oldSword, pos);
            }
            */
        }
    }

    //public overr

    public void setType(int t) {
        swordType = t;
        if (t < 10) {
            symbol.sprite = symbols[t];
        } else {
            symbol.sprite = broom;
            Color c = glow.color;
            c.a = 0;
            glow.color = c;
            self.width = 1;
            self.height = 4;
            self.squareBody();
            glow.transform.localScale = new Vector3(12, 12, 1);
        }
        /*
        GameObject tmp = Instantiate(GameInfo.S.souls[t]);
        SwordSoul ss = tmp.GetComponent<SwordSoul>();
        if (ss.getAnim()) {
            Transform tr = ss.getAnim().transform;
            Vector3 pos = transform.position;
            pos.y -= 3.55f;
            tr.position = pos;
            tr.parent = transform;
        } else {
            self.color = ss.mainColor[0];
        }
        
        Destroy(tmp);
        */
    }
}

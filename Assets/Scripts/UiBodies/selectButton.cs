using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class selectButton : icon {

    Image symbol;
    Image active;
    public int soulNumber = 0;
    public Sprite[] symbols;
    Color invis;

    public override void Awake() {
        active = transform.GetChild(3).GetComponent<Image>();
        symbol = transform.GetChild(2).GetComponent<Image>();
        active.gameObject.SetActive(false);
        invis = Color.white;
        invis.a = 0;
        if (!locked()) {
            active.gameObject.SetActive(true);
        }
        base.Awake();
    }

    public override void activate(bool val, int player) {
        //active.gameObject.SetActive(val);
        if (!GameInfo.S.locked[soulNumber]) {
            if (val) {
                if (player < 0) {
                    selected.color = Color.white;
                } else if (selections != null) {
                    selections[player].color = GameInfo.S.playerColors[player];
                }
            } else {
                if (player < 0) {
                    selected.color = invis;
                } else if (selections != null) {
                    selections[player].color = invis;//GameInfo.S.playerColors[player];
                }
            }
            /*
            if (val) {
                symbol.sprite = symbols[1];
            } else {
                symbol.sprite = symbols[0];
            }
            */
        } else {
            //symbol.sprite = null;
            //symbol.color = new Color(0, 0, 0, 0);
        }
        base.activate(val, player);
    }

    public void selectChar(int player) {
        GameInfo.S.setSong (player, soulNumber);//myButt.song);
		GameInfo.S.changeColor (GameInfo.S.colors[player], player);
    }

    public bool locked() {
        return GameInfo.S.locked[soulNumber];
    }
}

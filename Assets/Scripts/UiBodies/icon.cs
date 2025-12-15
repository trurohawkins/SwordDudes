using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class icon : MonoBehaviour {

    //public Sprite[] selections;
    protected Image[] selections;
    Sprite basic;
    protected Image selected;
    bool chosen;
    public int func = -1;
    bool locked = false;

    public virtual void Awake() {
        selected = gameObject.GetComponent<Image>();
        basic = selected.sprite;

        Transform sel = transform.GetChild(0);
        if (sel.childCount > 3) {
            selections = new Image[4];
            for(int i = 0; i < 4; i++) {
                selections[i] = sel.GetChild(i).GetComponent<Image>();
                //unChoose(i);
            }
        }
        activate(false, -1);
        // turn off basic selection
        unChoose(-2);
    }

    public virtual void activate(bool val, int player) { }

    public virtual void choose(int num) {
        if (!locked) {
            if (num < 0) {
                selected.color = Color.white;
            } else if (selections != null) {
                //selections[num].color = GameInfo.S.playerColors[num];
            }
        }
        
    }

    // -1 for basic, -2 for all, and >= 0 for specfic
    public virtual void unChoose(int num) {
        //Debug.Log(name + " unchoose " + num);
        Color c = selected.color;
        c.a = 0;
        if (num == -1) {
            selected.color = c;
        } else if (num == -2) {
            selected.color = c;
            if (selections != null) {
                for (int i = 0; i < selections.Length; i++) {
                    selections[i].color = c;
                }
            }
        } else  if (selections != null) {
            // was turned off for some reason
            // turned back on so that when we hold b out of character, cursor also goes away
            selections[num].color = c;
        }
    }

    public void setLock(bool val) {
        locked = val;
        Button butt = gameObject.GetComponent<Button>();
        butt.interactable = !val;
    }

    public bool isLocked() {
        return locked;
    }
}

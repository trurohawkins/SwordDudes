using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : Purpose {

    public int thinkInterval = 0;
    int thinkTimer;
    
    public virtual bool think() { 
        if (thinkTimer >= thinkInterval) {
            thinkTimer = 0;
            return true;
        } else {
            thinkTimer++;
            return false;
        }
    }
	
    public virtual void die() {}

    public virtual void move(Vector2Int direction) { }
}

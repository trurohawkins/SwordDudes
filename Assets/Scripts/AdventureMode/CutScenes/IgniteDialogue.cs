using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgniteDialogue : StoryPoint {
    
    public Obstacle check;
    public int noIgniteTime = 100;
    int igniteTimer = 0;

    public override IEnumerator callAction(int delay) {
        if (check) {
            if (speedCounter > speed) {
                if (check.isHit()) {
                    startText();
                    active = false;
                }
                speedCounter = 0;
            } else {
                speedCounter++;
            }
            if (igniteTimer < noIgniteTime) {
                igniteTimer++;
            } else {
                alterText();
                startText();
                active = false;
            }
        }
        yield return new WaitForFixedUpdate();
    }
}

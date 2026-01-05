using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaggerStoryPoint : StoryPoint {
    
    public DungeonObject mon;
    public int noStaggerTime = 100;
    int staggerTimer = 0;

    public override IEnumerator callAction(int delay) {
        if (mon) {
            if (speedCounter > speed) {
                if (mon.staggered()) {
                    startText();
                    active = false;
                }
                speedCounter = 0;
            } else {
                speedCounter++;
            }
            if (staggerTimer < noStaggerTime) {
                staggerTimer++;
            } else {
                alterText();
                startText();
               // active = false;
               staggerTimer = 0;
            }
        }
        yield return new WaitForFixedUpdate();
    }
}

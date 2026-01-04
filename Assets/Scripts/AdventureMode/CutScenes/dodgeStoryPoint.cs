using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dodgeStoryPoint : StoryPoint {
    Grader judge;
    Grade curRoom;
    int numDodges = 0;

    public void Start() {
        if (master.dm) {
            judge = master.dm.judge;
            if (judge) {
                curRoom = judge.getCurRoom();
                Debug.Log("got judge " + curRoom);
                numDodges = curRoom.thruWalls;
            }

        } else {
            Debug.Log("no master or dm");
        }
    }

    public override IEnumerator callAction(int delay) {
        if (dia && !dia.isWriting()) {
            if (speedCounter > speed) {
                    if (curRoom) {
                        if (curRoom.thruWalls > numDodges) {
                            active = false;
                        } else {
                            formText();
                            startText();
                        }
                    }
                speedCounter = 0;
            } else {
                speedCounter++;
            }
        }
        yield return new WaitForFixedUpdate();
    }
}

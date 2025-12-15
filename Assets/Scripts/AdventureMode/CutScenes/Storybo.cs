using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storybo : MonoBehaviour {
    public GameObject dialogueObj;
    public Dialogue dia;
    DungeonMaster dm;
    public GameObject[] board;
    public GameObject[] bossScenes;

    Shot curShot;
    int curBoard = 0;
    public bool mute;

    void Awake() {
        GameObject canvas = GameObject.FindGameObjectWithTag ("canvas");
        dia = Instantiate(dialogueObj, canvas.transform).GetComponent<Dialogue>();
        dm = gameObject.GetComponent<DungeonMaster>();
        dia.storybo = this;
        //finishShot();
    }

    public Shot startNextShot (int dir, int progress) {
        finishShot(dir, progress);
        if (curShot) {
            //startShot();
            return curShot;
        }
        return null;
    }

   public void startShot() {
        if (curShot) {
            if (curShot.readyForShot()) {
                curShot.read = true;
                //StartCoroutine(dia.writeOutWords());
                dia.startWriting();
            }
        }
    }

    public void startWriting() {
        dia.startWriting();
    }

    public void finishShot(int dir, int progress) {
        //Debug.Log("finishing shot " + progress);
        GameObject nextShot = null;//curShot.nextShot();
        if (curShot) {
            Debug.Log(curShot.name);
        }
        if (curShot) {
            nextShot = curShot.nextShot(dir);
            Debug.Log(curShot.name + " ggetting next shot:" + nextShot);
            //Destroy(curShot.gameObject);
        }
        if (!nextShot) {
            if (curBoard < board.Length) {
                nextShot = board[curBoard];
                curBoard++;
            }
        }
        if (nextShot) {
            spawnShot(nextShot);
        } else {
            curShot = null;
        }
    }

    public void setBoard(int board) {
        curBoard = board;
    }

    public int curBoss = 0;
    public Shot getBossShot(int progress) {
        if (progress < bossScenes.Length) {
            spawnShot(bossScenes[progress]);
            startShot();
            //curBoss++;
            return curShot;
        }
        return null;
    }

    public int getBoss() {
        return curBoss;
    }

    public void beatBoss() {
        curBoss++;
    }

    public void spawnShot(GameObject shot) {
        Shot newShot  = Instantiate(shot, transform).GetComponent<Shot>();
        newShot.getNarrator(this);
        // I think we dont need this anymore, it can get in the way of backtracking to old rooms by having their shot rewritten
        // if the new shot doesnt have a room, then its to be played in current room
        if (!newShot.room) {
            if (curShot && curShot.myRoom) {
                // here we change ownership by acessing the current shot's room and pointing it towards the new shot
                //curShot.myRoom.myShot = newShot;
            }
        }
        //Debug.Log(newShot.name + " is now the current shot");
        curShot = newShot;
        dia.receiveShot(curShot);
    }

    public void setCurShot(Shot s) {
        curShot = s;
        if (s) {
            dia.receiveShot(curShot);
            startShot();
        }
    }

    bool hasBroom;
    public void setBroom(bool v) { hasBroom = v; }
    public bool getBroom() { return hasBroom; }
}

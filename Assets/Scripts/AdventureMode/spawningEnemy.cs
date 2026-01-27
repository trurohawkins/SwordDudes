using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawningEnemy : MonoBehaviour {

    public DungeonObject whatToSpawn;
    public Player playerToSpawn;
    int toReset = -1;
    Vector3Int spawnPos;
    Enemy myGuy;

    public void spawnIt() {
        if (whatToSpawn) {
            whatToSpawn.spawnIn();
            myGuy = whatToSpawn.GetComponent<Enemy>();
            if (myGuy) {
                myGuy.paused = true;
                whatToSpawn.invulnerable = true;
            }
        } else if (playerToSpawn) {
            if (toReset == -1) {
                Form f = playerToSpawn.gameObject.GetComponent<Form> ();
		        f.squareBody ();
		        Map.S.spawnForm(playerToSpawn.gameObject, (int)transform.position.x, (int)transform.position.y);
                playerToSpawn.turnOnSprites();
                Controller ic = playerToSpawn.GetComponent<InputController>();
                if (ic) {
                    ic.enabled = true;
                }
            } else {
                playerToSpawn.turnOnSprites();
                Controller ic = playerToSpawn.GetComponent<InputController>();
                if (ic) {
                    ic.enabled = true;
                }
                if (toReset == 0) {
                    playerToSpawn.respawn(spawnPos, false, true);
                } else {
                    playerToSpawn.respawn(spawnPos, true, true);
                }
            }
        }
    }

    public void receivePlayer(GameObject player) {
        playerToSpawn = player.GetComponent<Player>();
        Form f = playerToSpawn.GetComponent<Form>();
        //so sword spawning goes from right place
        f.centerPoint = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        playerToSpawn.turnOffSprites();
        Controller ic = playerToSpawn.GetComponent<InputController>();
        if (ic) {
            ic.enabled = false;
        }

        getColor();
    }

    public void receivePlayer(Player p, Vector3Int spawn, bool reset) {
        if (reset) {
            toReset = 1;
        } else {
            toReset = 0;
        }
        playerToSpawn = p;
        spawnPos = spawn;
        playerToSpawn.turnOffSprites();
        Controller ic = playerToSpawn.GetComponent<InputController>();
        if (ic) {
            ic.enabled = false;
        }
        getColor();
    }

    void getColor() {
        if (playerToSpawn) {
            circleAttack tmp = Instantiate (playerToSpawn.soulType, new Vector3(1000, 1000, 0), transform.rotation).GetComponent<circleAttack> ();
		    tmp.gameObject.GetComponent<SwordSoul> ().setColors (playerToSpawn.colorNum);
            SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
            sr.color = tmp.GetComponent<SwordSoul> ().heatColorA;
            Destroy(tmp.gameObject);
        }
    }

    public void animEnd() {
        if (myGuy) {
            myGuy.paused = false;
            whatToSpawn.invulnerable = false;
        }
        Destroy(gameObject);
    }
}

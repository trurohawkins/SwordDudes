using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawningEnemy : MonoBehaviour {

    public DungeonObject whatToSpawn;
    Enemy myGuy;

    public void spawnIt() {
        whatToSpawn.spawnIn();
        myGuy = whatToSpawn.GetComponent<Enemy>();
        if (myGuy) {
            myGuy.paused = true;
            whatToSpawn.invulnerable = true;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawningEnemy : MonoBehaviour {

    public DungeonObject whatToSpawn;
    public Player playerToSpawn;
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
            Form f = playerToSpawn.gameObject.GetComponent<Form> ();
		    f.squareBody ();
		    Map.S.spawnForm(playerToSpawn.gameObject, (int)transform.position.x, (int)transform.position.y);
            playerToSpawn.turnOnSprites();
        }
    }

    public void receivePlayer(GameObject player) {
        playerToSpawn = player.GetComponent<Player>();
        Form f = playerToSpawn.GetComponent<Form>();
        //so sword spawning goes from right place
        f.centerPoint = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        playerToSpawn.turnOffSprites();
        circleAttack tmp = Instantiate (playerToSpawn.soulType, new Vector3(1000, 1000, 0), transform.rotation).GetComponent<circleAttack> ();
		tmp.gameObject.GetComponent<SwordSoul> ().setColors (playerToSpawn.colorNum);
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        sr.color = tmp.GetComponent<SwordSoul> ().heatColorA;
        Destroy(tmp.gameObject);
    }

    public void animEnd() {
        if (myGuy) {
            myGuy.paused = false;
            whatToSpawn.invulnerable = false;
        }
        Destroy(gameObject);
    }
}

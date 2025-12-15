using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class demiMaster : MonoBehaviour {
	public GameObject poo;
	public GameObject player;
	public GameObject obie;
	public Vector2Int playerSpawn;
	public Vector2Int pooSpawn;
	public Vector2Int obSpawn;
	public int amountOfObie;

	// Use this for initialization
	void Start () {


		GameObject p1 = Instantiate (player);
		p1.GetComponent<InputController> ().gamePad = GameInfo.S.p1Pad;
		p1.GetComponent<Controller> ().controlNum = GameInfo.S.p1ControlNum;
		Form f = p1.GetComponent<Form> ();
		f.squareBody ();
		Map.S.spawnForm(p1, playerSpawn.x, playerSpawn.y);
		p1.GetComponent<Player> ().dead = false;
		GameObject tmp = Instantiate (poo);
		tmp.GetComponent<Form> ().squareBody ();
		Map.S.spawnForm (tmp, pooSpawn.x, pooSpawn.y);
		tmp.GetComponent<path> ().objOfDesire = f;
		tmp.GetComponent<path> ().updateObj ();
		/*
		p1 = Instantiate (player);
		p1.GetComponent<InputController> ().gamePad = GameInfo.S.p2Pad;
		p1.GetComponent<Controller> ().controlNum = GameInfo.S.p2ControlNum;
		f = p1.GetComponent<Form> ();
		f.squareBody ();
		Map.S.spawnForm(p1, 100, 70);
		p1.GetComponent<Player> ().dead = false;
		*/
		spawnObstacles ();
	}
	
	void spawnObstacles () {
		//for (int i = 0; i < amountOfObie; i++) {
			//Vector2Int pos = new Vector2Int (Random.Range (1, Map.S.worldSizeX), Random.Range (1, Map.S.worldSizeY));
		Vector2Int pos = obSpawn;
			GameObject tmp = Instantiate (obie);
			tmp.GetComponent<Form> ().squareBody ();
			Map.S.spawnForm (tmp, pos.x, pos.y);
		//}
	}
}

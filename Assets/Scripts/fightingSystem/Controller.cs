using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : Brain {
	public int controlNum = 0;
	public Player me;
	//public Form self;
	public circleAttack attack;
	public GameObject menu;

	public virtual void Awake(){
		me = gameObject.GetComponent<Player> ();
		//self = gameObject.GetComponent<Form> ();
		base.Awake();
	}

	public void stopMove() {
		//Debug.Log ("stop Move");
	//	me.move.x = 0;
	//	me.move.y = 0;
	//	me.moveX = 0;
	//	me.moveY = 0;
		me.mDest = new Vector2Int(-1, -1);
	}

	public virtual void meetSword(circleAttack blade){
		attack = blade;
	}

	public void pause() {
		//me.moveInput = false;
		//me.move.x = 0;
		//me.move.y = 0;
		if (menu) {
			//Instantiate (menu).GetComponent<PauseMenu>().getController(this);
		}
	}

	public virtual void iMoved(int state) {
		if (state == 1 && controlNum == 0) {
			//Debug.Log (name + " moved " + me.gameObject.GetComponent<Form>().centerPoint + " " + me.mDest);
		}
	}

	public virtual void newDest() {
		//Debug.Log ("someone has requested a new destination");
	}

	public int findShortestDist(float start, float end){
		return findShortestDist ((int)start, (int)end);
	}

	public int findShortestDist(int start, int end){
		return Mathf.Min(saneDifference (start, end, -1), saneDifference(start, end, 1));//saneDifference(start, end, findShortestDir(start, end));
	}

	public int findShortestDir(float start, float angle) {
		return findShortestDir ((int)start, (int)angle);
	}

	public int findShortestDir(int start, int angle) {
		//Debug.Log(start + " -> " + angle + " angles: " + Mathf.DeltaAngle(start, angle) + " " + Mathf.DeltaAngle(angle, start));

		int neg = saneDifference (start, angle, -1);
		int pos = saneDifference(start, angle, 1);
		if (pos < neg) {
			return 1;
		} else {
			return -1;
		}
	}

		
	public int saneAddition(float start, float increase, float dir) {
		return Map.S.saneAddition ((int)start, (int)increase, (int)dir);
	}

	public int saneAddition(int start, int increase, int dir) {//so dumb, do better
		return Map.S.saneAddition(start, increase, dir);
	}

	public int saneDifference(float start, float end, float dir) {
		return Map.S.saneDifference ((int)start, (int)end, (int)dir);
	}

	public int saneDifference(int start, int end, int dir) {
		return Map.S.saneDifference(start, end, dir);
	}

	public virtual Vector2Int calculateDestination() {
		Debug.Log ("basic controller calculate");
		return new Vector2Int (-1, -1);
	}

	public virtual void die() {}

	public virtual bool getStrikeInput(){
		return false;
	}

	public virtual void stopSwingInput() { }
}

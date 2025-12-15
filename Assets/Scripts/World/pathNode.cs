using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pathNode : MonoBehaviour {

	Vector2Int pos;
	public pathNode[] neighbors;
	public bool[] dodgeTo;
	pathNode[] myPar;
	public bool[] dPar;
	float[] distAi;
	float[] distTarg;

	void Awake() {
		neighbors = new pathNode [8];
		dodgeTo = new bool [8];
		for (int i = 0; i < 8; i++) {
			dodgeTo [i] = false;
		}
		pos = new Vector2Int ((int)transform.position.x, (int)transform.position.y);
		distAi = new float[4];
		distTarg = new float[4];
		myPar = new pathNode[4];
		dPar = new bool[4];
		for (int i = 0; i < 4; i++) {
			distAi[i] = Mathf.Infinity;
			distTarg[i] = Mathf.Infinity;
			myPar [i] = null;
		}

	}

	public void setNeighbor(pathNode neigh, int dir, bool dodgeVal) {
		if (neighbors [dir] != null) {
			Debug.LogWarning(name + " is already neighbors with " + neighbors[dir].name + " at " + dir + " cant add " + neigh.name);
		}
		neighbors [dir] = neigh;
		dodgeTo[dir] = dodgeVal;
	}

	public void getNeighbors(bool dodgeOk, pathNode[] myNeighbs) {
		for (int i = 0; i < 8; i++) {
			if (neighbors [i] != null && (dodgeOk || !dodgeTo [i])) {
				myNeighbs [i] = neighbors [i];
			} else {
				myNeighbs [i] = null;
			}
		}
		//return myNeighbs;
	}

	public bool getDodgeVal(int i) {
		return dodgeTo [i];
	}

	public Vector2Int getPos() {
		return pos;
	}

	public void setPos(Vector2Int p) {
		pos = p;
	}

	public bool isDeadEnd(pathNode cur, pathNode dest) {
		if (dest == this) {
			return false;
		}
		int others = 0;
		for (int i = 0; i < neighbors.Length; i++) {
			if (neighbors [i] != cur && neighbors[i] != null) {
				others++;
			}
		}
		if (others == 0) {
			return true;
		} else {
			return false;
		}
	}

	public bool calcScore(int p, Vector2Int ai, Vector2Int targ, pathNode par) {
		float tAi = Vector2Int.Distance (ai, pos);
		float tTar = Vector2Int.Distance (targ, pos);
		if (tAi + tTar < distAi[p] + distTarg[p]) {
			distAi[p] = tAi;
			distTarg[p] = tTar;// + blocked;
			myPar[p] = par;
			for (int i = 0; i < 8; i++) {
				if (par == neighbors [i]) {
					dPar [p] = dodgeTo [i];
				}
			}
			return true;
		} else {
			return false;
		}
	}

	public void clearScore(int p) {
		distAi[p] = Mathf.Infinity;
		distTarg[p] = Mathf.Infinity;
	}

	public float getScore(int p) {
		return distAi[p] + distTarg[p];
	}

	public pathNode getParent(int p) {
		return myPar[p];
	}
}

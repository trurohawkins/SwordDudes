using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulStats : MonoBehaviour {

	public int level;
	int maxLevel = 5;
	//[0] - minVal [1] - maxVal [2] - minLvl [3] - maxLvl
	public float[] combatDodgeP;//0-1
	public float[] inPlaceDodgeP; //0-1
	public float[] dodgePathP;
	public float[] moveDodgeP; //0-1
	public float[] moveDodgeCheckP;
	public float[] circleDodgeP; //0-1
	public float[] wallDodgeStick; //0-1, 0.98f still functional lol
	public float[] dodgeAccuracy;

	public float[] creeping;
	public float[] misBurnP; //1-0
	public float[] runFromBurnP;//0-1
	public float[] overrideRanges;
	public float[] misRangeAccuracy;
	public float[] swingDirectionAccuracy;
	public float[] swingInterval;
	public float[] swingLengthAccuracy;
	public float[] swingInvulWait; //0-1
	public float[] walkIntoDes; //1-0

	public int[] adjOrder;
	public int[] distOrder;
	public int wantToBurn = 0; //-1: no, 0: dont care, 1: yes
	public bool runAndBurn = false;
	public float[] rangedAttack;
	public bool runIfNotPowered = false;
	public bool dontAdjust = false;

	//public int updateInterval = 1;
	public float[] pathingInterval;
	//public int checkBodyInterval = 2;
	public float[] bodyInterval;
	public int burnAdj = -1;
	public float heatUnderBurn = -1;
	public bool burnNoMatter;//doesnt tke into account health when deciding to burn
	public bool ignoreBurning;

	public void setLevel(int newLevel) {
		level = Mathf.Clamp (newLevel, 1, maxLevel);
	}

	public int raiseLevel(int amount) {
		if (level + amount < maxLevel) {
			level += amount;
		} else {
			level = maxLevel;
		}
		return level;
	}

	public int lowerLevel(int amount) {
		if (level - amount > 1) {
			level -= amount;
		} else {
			level = 1;
		}
		return level;
	}

	public void updateAI(InputComputer ic) {
		ic.dodgeCombatChance = calcStat (combatDodgeP);
		ic.inPlaceDodgeChance = calcStat (inPlaceDodgeP);
		ic.dodgePathChance = calcStat (dodgePathP);
		ic.dodgeMoveChance = calcStat (moveDodgeP);
		ic.dodgeMoveCheck = calcStat (moveDodgeCheckP);
		ic.dodgeCircleChance = calcStat (circleDodgeP);
		ic.dodgeWallStuck = calcStat (wallDodgeStick);
		ic.dodgeAngleMiss = (int)calcStat (dodgeAccuracy);

		ic.creepChance = calcStat (creeping);
		ic.burnMisFire = calcStat (misBurnP);
		ic.burnRunAway = calcStat (runFromBurnP);

		ic.hitRngAccuracy = calcStat (misRangeAccuracy);//HitAccuracy ();
		ic.swingDirAcc = calcStat(swingDirectionAccuracy);
		ic.setSwingInterval((int)calcStat(swingInterval));
		//Debug.Log ("psi: " + ic.postSwingInterval);
		ic.swingLenAcc = calcStat (swingLengthAccuracy);
		ic.swingWaitChance = calcStat (swingInvulWait);

		if (overrideRanges [0] != 0) {
			ic.getAttack ().setRanges (overrideRanges);
		}
		if (adjOrder.Length != 0) {
			ic.getAdjOrder (adjOrder);
		}
		if (distOrder.Length != 0) {
			ic.getDistOrder (distOrder);
		}
		ic.setBurnDesire (wantToBurn);//wantToBurn = wantToBurn;
		ic.runAndBurn = runAndBurn;
		ic.rangeAttack = calcStat(rangedAttack);
		//Debug.Log (ic.rangeAttack + " " + calcStat(rangedAttack));
		ic.runIfNotFullPowered = runIfNotPowered;
		ic.dontAdjust = dontAdjust;
		if (burnAdj != -1) {
			ic.setBurnAdj (burnAdj);
		}
		if (heatUnderBurn != -1) {
			ic.setHeatUnderBurn (heatUnderBurn);
		}
		ic.burnNoMatter = burnNoMatter;
		ic.ignoreBurn = ignoreBurning;
		ic.setPathStats( (int)calcStat(pathingInterval), (int)calcStat (bodyInterval), calcStat(walkIntoDes));
	}

	float calcHitAccuracy() {
		float amount = calcStat (misRangeAccuracy);//Mathf.Lerp(rangeAccuracy[0], rangAccuracy[1], level
		if (Random.value > 0.5f) {
			return 1 + amount;
		} else {
			return 1 - amount;
		}
	}

	float calcStat(float[] stat) {
		return stat [level-1];
		/*
		if (level >= stat [2]) {
			return Mathf.Lerp (stat [0], stat [1], (level - stat [2]) / (maxLevel - stat [2]));
		} else {
			return stat [0];
		}
		*/
	}
	/*
	int calcStat(float[] stat) {
		return (int) (Mathf.Lerp (stat [0], stat [1], level / maxLevel));
	}
	*/
}

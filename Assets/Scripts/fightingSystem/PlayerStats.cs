using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour {

	public float health;
	public float staggerThreshold;
	public float recoverySpeed;
	public float weight;
	public float relaxedPoise;
	public float movingPoise;
	public float attackingPoise;
	public float backStabDefense;

	public int hiSpeed;
	public int loSpeed;
	public int maxHyperSpeed;
	public int maxExtraSpeed;
	public int turnCost;
	public int accelSpeed;
	public int decelSpeed;

	public float dodgeCost;
	public float dodgeLength = 25;
	public int dodgeSpeed = 2;
	public int dodgeRecover = 15;
	public int dodgeStartUp = 15;

	public float stamina;
	public float staminaRecovery;
	public int attackWindow;

	public int percievedPower;
	public int aggression;
	public int outerSelf;
	public int charisma;

	void Awake(){
		giveStats ();
	}

	void giveStats(){
		defense d = gameObject.GetComponent<defense> ();
		if (d) {
			d.setStats (health, staggerThreshold, recoverySpeed, weight, relaxedPoise, movingPoise, attackingPoise, backStabDefense);
		}

		Player p = gameObject.GetComponent<Player> ();
		if (p) {
			p.setStats (hiSpeed, loSpeed, maxHyperSpeed, turnCost, accelSpeed, decelSpeed, stamina, staminaRecovery, dodgeCost, dodgeLength, dodgeSpeed, dodgeRecover, dodgeStartUp, maxExtraSpeed);//, swordWeight);
		}

		InputController i = gameObject.GetComponent<InputController> ();
		if (i) {
			i.setStats (attackWindow);
		}
	}

}

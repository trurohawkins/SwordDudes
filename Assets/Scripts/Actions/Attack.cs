using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : Purpose {

    protected Enemy creator;
    protected Value val;

    protected override void Awake() {
        base.Awake();
        val = gameObject.GetComponent<Value>();
    }

    public void getCreator(Enemy n_creator) {
        creator = n_creator;
    }

    protected virtual void endAttack() {
        if (creator) {
            creator.releaseAttack();
        }
    }
    public void setDamages(float newDam, float newKnock, float newStag) {
        //Debug.Log("setting damages dam:" + newDam + "knock: " + newKnock + " stg: " + newStag);
		val.setValue("damage", newDam, newDam);
		val.setValue("knockBack", newKnock, newKnock);
		val.setValue("stagger", newStag, newStag);
	}

	public void setTeam(int team) {
		val.setTeam(team);
	}

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : FighterAnimator {
	public Color mainColor;
	public Color subColor;
	public Color detColor;
	public Color powerColor;
	public Animator body;
	public Animator muzzle;
	public Animator mouth;
	public Animator tongue;
	public Animator eyesOpen;
	public Animator eyesClosed;
	public Animator face;
	public Animator effect;

	SpriteRenderer[] mainSprites;
	int mainNum = 2;
	SpriteRenderer[] subSprites;
	int subNum = 2;
	SpriteRenderer[] detailSprites;
	int detNum = 3;
	SpriteRenderer[] powSprites;
	int powNum = 1;
	SpriteRenderer[] allSprites;
	int allNum;
	int[] renderOrders;

	public float setAlphaVal;
	public int eyes;
	public int eyeState = -1;
	public int mouthPos;
	int mouthState = -1;
	public int dodging = 0;

	public int die = 0;
	public bool debug = false;
	public int spinSpeed = 0;
	int spinCounter = 0;

	void Awake() {
		allNum = mainNum + subNum + detNum + powNum;
		allSprites = new SpriteRenderer[allNum];
		renderOrders = new int[allNum];
		mainSprites = new SpriteRenderer[mainNum];
		mainSprites [0] = body.GetComponent<SpriteRenderer> ();
		allSprites [0] = mainSprites [0];
		mainSprites [1] = muzzle.GetComponent<SpriteRenderer> ();
		allSprites [1] = mainSprites [1];

		subSprites = new SpriteRenderer[subNum];
		subSprites [0] = eyesOpen.GetComponent<SpriteRenderer> ();
		allSprites [2] = subSprites [0];
		subSprites [1] = tongue.GetComponent<SpriteRenderer> ();
		allSprites [3] = subSprites [1];
		//subSprites [2] = effect.GetComponent<SpriteRenderer> ();

		detailSprites = new SpriteRenderer[detNum];
		detailSprites [0] = eyesClosed.GetComponent<SpriteRenderer> ();
		allSprites [4] = detailSprites [0];
		detailSprites [1] = mouth.GetComponent<SpriteRenderer> ();
		allSprites [5] = detailSprites [1];
		detailSprites [2] = face.GetComponent<SpriteRenderer> ();
		allSprites [6] = detailSprites [2];

		powSprites = new SpriteRenderer[powNum];
		powSprites [0] = effect.GetComponent<SpriteRenderer> ();
		allSprites [7] = powSprites [0];
		for (int i = 0; i < allNum; i++) {
			renderOrders[i] = allSprites [i].sortingOrder;
		}
	}

	public void setColors() {
		for (int i = 0; i < mainNum; i++) {
			mainSprites [i].color = mainColor;
		}
		for (int i = 0; i < subNum; i++) {
			subSprites [i].color = subColor;
		}
		for (int i = 0; i < detNum; i++) {
			detailSprites [i].color = detColor;
		}
		for (int i = 0; i < powNum; i++) {
			powSprites [i].color = powerColor;
		}
	}

	void OnEnable() {
		respawn();
	}

	float scaleFactor = 9.3333f;
	void Start () {
		if (transform.parent) {
			Form par = transform.parent.gameObject.GetComponent<Form> ();
			if (par) {
				int wid = par.width;
				//transform.localScale = new Vector3 (wid * scaleFactor, wid * scaleFactor, 1f);
				//setAlpha (0.5f);
			}
		}
	}

	public override void setColors(SwordSoul mySoul, int pNum) {
		//Debug.Log("player " + pNum + " setting colors for soul: " + mySoul.description);
		mainColor = mySoul.mainColor [pNum];
		//Debug.Log(mainNum);
		for (int i = 0; i < mainNum; i++) {
			mainSprites [i].color = mainColor;
		}
		subColor = mySoul.subColor [pNum];
		for (int i = 0; i < subNum; i++) {
			subSprites [i].color = subColor;;
		}
		detColor = mySoul.detColor [pNum];
		for (int i = 0; i < detNum; i++) {
			detailSprites [i].color = detColor;
		}
		powerColor = mySoul.powerColor [pNum];
		for (int i = 0; i < powNum; i++) {
			powSprites [i].color = powerColor;
		}
	}

	public void setColors(Color main, Color sub, Color det, Color pow) {
		mainColor = main;
		for (int i = 0; i < mainNum; i++) {
			mainSprites [i].color = mainColor;
		}
		subColor = sub;
		for (int i = 0; i < subNum; i++) {
			subSprites [i].color = subColor;;
		}
		detColor = det;
		for (int i = 0; i < detNum; i++) {
			detailSprites [i].color = detColor;
		}
		powerColor = pow;
		for (int i = 0; i < powNum; i++) {
			powSprites [i].color = powerColor;
		}
	}

	public void giveColors(PlayerAnimator pa) {
		pa.setColors(mainColor, subColor, detColor, powerColor);
	}

	public void setMainColors(Color col) {
		for (int i = 0; i < mainNum; i++) {
			mainSprites [i].color = col;
		}
	}

    public override void spawn() {
        setEyes (2);
		setDir (4);
    }

    public override void respawn() {
		setDeath (0);
		setEyes (2);
		lerpEyeColor(0);
		setDodge (0);
        setMouth (0);
    }

    public override void swingInput(bool begin) {
        if (begin) {
			if (getMouthState () != 3) {
				//Debug.Log ("starting swing anim");
				setMouth (1);
			} else {
				//Debug.Log ("dont stop Im stil swinging");
			}
		} else {
			if (getMouthState () == 1) {
				setMouth (0);
			} else {
				//Debug.LogError ("mout hstate is " + pGraphics.getMouthState ());
			}
		}
    }

    public override void swingBegin() {
        setMouth (3);
    }

    public override void lerpHeat(float fade) {
        lerpEyeColor(fade);
    }

    public override void disembodiedState(bool begin, bool dead) {
		if (begin) {
			setWalking (false);
			setEyes (-1);
			setMouth (0);
			mouthLock = true;
			eyeLock = true;
			setDodge (3);
		} else {
			mouthLock = false;
			eyeLock = false;
			if (!dead) {
				setEyes (2);
				setMouth (0);
				setDodge(0);
			}
		}
    }

    public override void swingOver(bool gettingInput, bool swingDetached) {
        if (!swingDetached) {
			if (!gettingInput) {
				if (getMouthState () == 3) {
					setMouth (0);
				}
			} else {
				setMouth (1);
			}
			setEyes (2);
		}
    }

    public override void cantSwing(bool begin) {
        if (begin) {
			setMouth(2);
		} else {
			setMouth(0);
		}
    }

    public override void hurt(bool begin) {
        if (begin) {
			if (!staggered) {
				setEyes(1);
			} else {
				setEyes(0);
			}
		} else {
			setEyes(2);
		}
    }

    public override void stagger(bool begin) {
        if (begin) {
			setMouth (2);
			mouthLock = true;
		} else {
			mouthLock = false;
			setMouth (0);
			setEyes(2);
		}
		staggered = begin;
    }

    void Update () {
		if (debug) {
			if (spinSpeed > 0) {
				if (spinCounter > spinSpeed) {
					direction = (direction + 1) % 8;
					spinCounter = 0;
				} else {
					spinCounter++;
				}
			}
			if (body.GetBool ("walking") != walking) {
				setWalking (walking);
			}
			if (body.GetInteger ("dir") != direction) {
				setDir (direction);
			}
			if (eyes != eyeState) {
				setEyes (eyes);
			}
			if (mouthPos != mouthState) {
				setMouth (mouthPos);
			}

			if (dodging != dodgeState) {
				setDodge (dodging);
			}

			if (die != deathState) {
				setDeath (die);
			}
			if (mainSprites [0].color.a != setAlphaVal) {
				setAlpha (setAlphaVal);
			}
		}
	}

	public override void setWalking(bool walk) {
		if (walking != walk) {
			if (walk == false || (dodgeState == 0 && deathState == 0)) {
				//Debug.Log (transform.parent.name + " walking: " + walk);
				body.SetBool ("walking", walk);
				walking = walk;
			}
		}
	}

	public override void setWalkingSpeed(float n_speed) {
		body.speed = Mathf.Clamp(n_speed, 0.3f, 1);
	}

	public override void setDir(int dir) {
		body.SetInteger ("dir", dir);
		//face.SetInteger("dir", dir);
		eyesClosed.SetInteger("dir", dir);
		eyesOpen.SetInteger("dir", dir);
		tongue.SetInteger ("dir", dir);
		mouth.SetBool ("showing", true);//why??
		if (dir == 3 || dir == 5) {
			muzzle.SetBool ("facingSide", true);
			mouth.SetBool ("facingSide", true);
		} else {
			muzzle.SetBool ("facingSide", false);
			mouth.SetBool ("facingSide", false);
		}
		if (dir == 1 || dir == 7) {
			muzzle.SetBool ("facingBack", true);
			mouth.SetBool ("facingBack", true);
		} else {
			muzzle.SetBool ("facingBack", false);
			mouth.SetBool ("facingBack", false);
		}
		if (dir == 2 || dir == 6) {
			muzzle.SetBool ("facingFullSide", true);
			mouth.SetBool ("facingFullSide", true);
		} else {
			muzzle.SetBool ("facingFullSide", false);
			mouth.SetBool ("facingFullSide", false);
		}
		if (dir == 0) {
			face.SetBool("back", true);
			face.SetBool("backside", false);
			face.SetBool("forward", false);
			face.SetBool("side", false);
			face.SetBool("fullside", false);
		} else if (dir == 1 || dir == 7) {
			face.SetBool("back", false);
			face.SetBool("backside", true);
			face.SetBool("forward", false);
			face.SetBool("side", false);
			face.SetBool("fullside", false);
		} else if (dir == 4) {// || dir == 3 || dir == 5){
			face.SetBool("back", false);
			face.SetBool("backside", false);
			face.SetBool("forward", true);
			face.SetBool("side", false);
			face.SetBool("fullside", false);
		} else if (dir == 2 || dir == 6) {
			face.SetBool("back", false);
			face.SetBool("backside", false);
			face.SetBool("forward", false);
			face.SetBool("side", false);
			face.SetBool("fullside", true);
		} else {
			face.SetBool("back", false);
			face.SetBool("backside", false);
			face.SetBool("forward", false);
			face.SetBool("side", true);
			face.SetBool("fullside", false);
		}
		if (dir == 2 || dir == 1 || dir == 3) {
			for (int i = 0; i < mainNum; i++) {
				mainSprites [i].flipX = true;
			}
			for (int i = 0; i < subNum; i++) {
				subSprites [i].flipX = true;
			}
			for (int i = 0; i < detNum; i++) {
				detailSprites [i].flipX = true;
			}
			for (int i = 0; i < powNum; i++) {
				powSprites [i].flipX = true;
			}
		} else if (dir == 6 || dir == 5 || dir == 7) {
			for (int i = 0; i < mainNum; i++) {
				mainSprites [i].flipX = false;
			}
			for (int i = 0; i < subNum; i++) {
				subSprites [i].flipX = false;
			}
			for (int i = 0; i < detNum; i++) {
				detailSprites [i].flipX = false;
			}
			for (int i = 0; i < powNum; i++) {
				powSprites [i].flipX = false;
			}
		}
		direction = dir;
	}

	public bool eyeLock = false;

	public void setEyes(int eye) {
		if (!eyeLock && (eye == -1 || (deathState == 0 && dodgeState == 0))) {
			if (eye == -1) {
				eyesOpen.SetBool ("showing", false);
				eyesClosed.SetBool ("showing", false);

			} else if (eye == 0) {
				eyesOpen.SetBool ("showing", false);
				eyesClosed.SetBool ("showing", true);
				eyesOpen.SetBool ("half", false);
				eyesClosed.SetBool ("half", false);
			} else if (eye == 1) {
				//Debug.Log ("half eyes");
				eyesOpen.SetBool ("showing", true);
				eyesClosed.SetBool ("showing", true);
				eyesOpen.SetBool ("half", true);
				eyesClosed.SetBool ("half", true);
			} else if (eye >= 2) {//== 2 || eye == 3) {
				eyesOpen.SetBool ("showing", true);
				eyesClosed.SetBool ("showing", false);
				eyesOpen.SetBool ("half", false);
				eyesClosed.SetBool ("half", false);
			}
			/*
			if (eye == 3) {
				subSprites [0].color = powerColor;
			} else {
				subSprites [0].color = subColor;
			}
			*/
			eyeState = eye;
		}
	}

	public void lerpEyeColor(float i) {
		subSprites [0].color = Color.Lerp(subColor, powerColor, i);
	}

    public override void attack(int state) {
		if (state < 2) {
			setMouth(state);
		} else {
			setMouth(state+1);
		}
    }

    public bool mouthLock = false;

	public void setMouth(int m) {
		if (!mouthLock && (dodgeState == 0 && deathState == 0)) {
			//Debug.Log(transform.parent.name + " setting motuh to " + m);
			if (m == 0) {//closed
				muzzle.SetBool ("droop", false);
				muzzle.SetBool ("open", false);
				mouth.SetBool ("droop", false);
				mouth.SetBool ("open", false);
				tongue.SetBool ("showing", false);
			} else if (m == 1) {//open
				muzzle.SetBool ("droop", false);
				muzzle.SetBool ("open", true);
				mouth.SetBool ("droop", false);
				mouth.SetBool ("open", true);
				tongue.SetBool ("showing", false);
			} else if (m == 2) {//drooping
				muzzle.SetBool ("droop", true);
				muzzle.SetBool ("open", false);
				mouth.SetBool ("droop", true);
				mouth.SetBool ("open", false);
				tongue.SetBool ("showing", false);
			} else if (m == 3) {//open tongue
				muzzle.SetBool ("droop", false);
				muzzle.SetBool ("open", true);
				mouth.SetBool ("droop", false);
				mouth.SetBool ("open", true);
				tongue.SetBool ("showing", true);
			}
			mouthState = m;
		} else {
			//Debug.Log ("cant set mouth state" + deathState + " " + dodgeState);
		}
	}


	public int getMouthState() {
		return mouthState;
	}

	public override void setDodge(int d) {
		//Debug.Log("dodge set to " + d);
		dodgeState = d;
		if (d == 2 || d == 1 || d == 3) {
			setWalking (false);
			tongue.SetBool ("showing", false);

			setEyes (-1);
			face.SetBool ("showing", true);
			face.SetBool ("dodge", true);
			muzzle.SetBool ("showing", false);
			mouth.SetBool ("showing", false);
			body.SetBool ("dodging", true);
			if (d != 3) {
				for (int i = 0; i < mainNum; i++) {
					mainSprites [i].color = subColor;
				}
			}
			if (d == 2) {
				effect.SetBool ("dodging", true);
			} else {
				effect.SetBool ("dodging", false);
			}
		} else if (d == 0) {
			body.SetBool ("dodging", false);
			setEyes (2);
			face.SetBool ("showing", false);
			face.SetBool ("dodge", false);
			effect.SetBool ("dodging", false);
			muzzle.SetBool ("showing", true);
			mouth.SetBool ("showing", true);
			setMouth (0);
			for (int i = 0; i < mainNum; i++) {
				mainSprites [i].color = mainColor;
			}
		}
	}

	public float deathModColor;

	public override void setDeath(int d) {
		if (d != 0) {
			setDodge (0);
			body.SetBool ("walking", false);
			face.SetBool ("dodge", false);
			tongue.SetBool ("showing", false);
			body.SetBool ("dead", true);
			setEyes (-1);
			face.SetBool ("showing", true);
			face.SetBool ("dead", true);
			effect.SetBool ("dead", true);
			muzzle.SetBool ("showing", false);
			mouth.SetBool ("showing", false);
			modRenderOrder (-2);
			modColors(-deathModColor);
		} else {
			body.SetBool ("dead", false);
			setEyes (2);
			face.SetBool ("showing", false);
			face.SetBool ("dead", false);
			effect.SetBool ("dead", false);
			muzzle.SetBool ("showing", true);
			mouth.SetBool ("showing", true);
			setMouth (mouthState);
			resetRenderOrder ();
			modColors(deathModColor);
		}
		deathState = d;
	}

	public override void setAlpha(float val) {
		for (int i = 0; i < mainNum; i++) {
			Color c = mainSprites [i].color;
			c.a = val;
			mainSprites [i].color = c;
		}
		for (int i = 0; i < subNum; i++) {
			Color c = subSprites [i].color;
			c.a = val;
			subSprites [i].color = c;
		}
		for (int i = 0; i < detNum; i++) {
			Color c = detailSprites [i].color;
			c.a = val;
			detailSprites [i].color = c;
		}
		for (int i = 0; i < powNum; i++) {
			Color c = powSprites [i].color;
			c.a = val;
			powSprites [i].color = c;
		}
	}

	void modColors(float val) {
		for (int i = 0; i < mainNum; i++) {
			Color c = mainSprites [i].color;
			c.r += val;
			c.g += val;
			c.b += val;
			mainSprites [i].color = c;
		}
	}

	void modRenderOrder(int orderMod) {
		for (int i = 0; i < allNum; i++) {
			allSprites [i].sortingOrder += orderMod;
		}
	}

	void resetRenderOrder() {
		for (int i = 0; i < allNum; i++) {
			allSprites [i].sortingOrder = renderOrders[i];
		}
	}
}


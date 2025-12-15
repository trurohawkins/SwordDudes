using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadeOld : SwordSoul {
	public int shadeTime;

	public override bool startBurn() {
		if (!burning) {
			will.erasePresence (false);
			if (boomBox.S) {
				boomBox.S.setBurst (pNum, 1);
			}
			burning = true;
			energy = burningPoint + shadeTime;
			for (int k = 0; k < will.bladeMulti; k++) {
				for (int i = 0; i < will.blade [k].Count; i++) {
					Form f = will.blade [k] [i];
					for (int j = 0; j < f.effects.Count; j++) {
						f.effects [j].kindle ();
					}
				}
			}
		}
		return true;
	}

	public override bool stopBurn() {
		will.revealSelf ();
		if (boomBox.S) {
			boomBox.S.resetBurst (pNum);
		}
		for (int k = 0; k < will.bladeMulti; k++) {
			for (int i = 0; i < will.blade [k].Count; i++) {
				Form f = will.blade [k] [i];
				for (int j = 0; j < f.effects.Count; j++) {
					f.effects [j].smother ();
				}
			}
		}
		burning = false;
		return true;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootboxSlider : MonoBehaviour {

    // Use this for initialization

    private Transform active_;

	void Start () {

        active_ = transform.GetChild(0);
        // TODO: Make slider to lerp from leftmost position to rightmost based on user's touch (horizontal)
        // right swipe should focus on next lootbox, and swipe left should focus on prev lootbox
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

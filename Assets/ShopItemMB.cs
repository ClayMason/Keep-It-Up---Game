using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopItemMB : MonoBehaviour {

    public int id;
    public BackgroundData bg_data;
    public int price;

	// Update is called once per frame
	void Update () {
		
	}

    public void purchase ()
    {
        //Debug.Log("Coin value before: " + PlayerPrefs.GetInt("coin_count"));

        if (bg_data.ball_skins[id].unlocked)
        {
            //Debug.Log("ball is unlocked");
            if (PlayerPrefs.GetInt("ball_skin") != id)
            {
                int old_active = PlayerPrefs.GetInt("ball_skin");
                PlayerPrefs.SetInt("ball_skin", id);
                bg_data.updateShopItemView(id, this.gameObject, old_active);
            } // otherwise do nothing

        } else
        {
            //Debug.Log("ball is locked");
            if (PlayerPrefs.GetInt("coin_count") >= price)
            {
                //Debug.Log("Can purchase");
                int old_active = PlayerPrefs.GetInt("ball_skin");
                bg_data.purchaseBall(id);
                bg_data.updateShopItemView(id, this.gameObject, old_active);
                //Debug.Log("Coin value after: " + PlayerPrefs.GetInt("coin_count"));
            }
            //else Debug.Log("cannot purchase (" + PlayerPrefs.GetInt("coin_count") + ")");

            // update coin values
            GameObject.Find("Game Controller").GetComponent<GameController>().updateCoinValue();
        }



        //Debug.Log("Coin value after: " + PlayerPrefs.GetInt("coin_count"));
    }
    
}

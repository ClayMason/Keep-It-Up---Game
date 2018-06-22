using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour {

    private Vector2 init_pos;
    private Vector2 end_pos;
    private const float LERP_DISPLACEMENT = 0.15f;

    private float coin_lerp_value = 0f;
    private int dir = 1;
    private const float LERP_SPEED = 2f;

	// Use this for initialization
	void Start () {
        //Debug.Log("Coin exists");

        changeInitPosition(transform.position);
	}
	
	// Update is called once per frame
	void Update () {

        coin_lerp_value += LERP_SPEED * Time.deltaTime * dir;
        if (coin_lerp_value > 1f) dir = -1;
        if (coin_lerp_value < 0f) dir = 1;
        transform.position = Vector2.Lerp(init_pos, end_pos, Mathf.Clamp(coin_lerp_value, 0f, 1f));

	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ball")
        {
            GameObject.Find("Game Controller").GetComponent<GameController>().coins_collected++;
            gameObject.SetActive(false);
        }
    }

    public void changeInitPosition ( Vector2 new_init_pos )
    {
        gameObject.SetActive(true);
        init_pos = new_init_pos;
        end_pos = new Vector2(init_pos.x, init_pos.y + LERP_DISPLACEMENT);
    }

    public void offset ( int val, int max )
    {
        // offset lerp by val / max 
        coin_lerp_value = (float) val / max;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCtrl : MonoBehaviour {

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ( collision.gameObject.tag == "Ground" )
        {
            // end game when ball hits the ground
            GameObject.Find("Game Controller").GetComponent<GameController>().gameOver();
            this.gameObject.SetActive(false);
        }
    }

}

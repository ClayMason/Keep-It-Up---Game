using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    public Text score_text;

    public GameObject pause_menu;
        public GameObject sound_ui;
        public GameObject music_ui;
        public Sprite sound_on;
        public Sprite sound_off;
        public Sprite music_on;
        public Sprite music_off;
        public bool music_enabled;
        public bool sound_enabled;
        public bool paused = false;
    
    public GameObject game_over_screen;
        private bool game_over = false;
        private float game_over_lerp = 0f;
        public GameObject player_score;
        private Vector2 current_score_start_position = new Vector2(400f, 79.3f);
        private Vector2 current_score_end_position = new Vector2(400f, 729.5f);

    public GameObject location_indicator;
    public GameObject location_indicator_current;
    //private Vector2 location_ui_range = new Vector2(-149f, 147f);
    private Vector2 location_ui_range = new Vector2(242f, 538f);
    private Vector2 player_ui_range = new Vector2(-15.5f, 15.5f);

    public GameObject ball_instance;
    // Use this for initialization
    void Start () {
        pause_menu.SetActive(false);
        music_enabled = true;
        sound_enabled = true;
        game_over = false;
        //Debug.Log(player_score.transform.position);
    }
	
	// Update is called once per frame
	void Update () {

        //Debug.Log(location_indicator_current.transform.position);
        Debug.Assert(ball_instance != null);
        location_indicator_current.transform.position = new Vector2(
                Mathf.Lerp(location_ui_range.x, location_ui_range.y, (ball_instance.transform.position.x - player_ui_range.x)/(player_ui_range.y - player_ui_range.x)),
                location_indicator_current.transform.position.y
            );

        if (game_over)
        {
            game_over_screen.SetActive(true);
            player_score.transform.position = Vector2.Lerp(current_score_start_position, current_score_end_position, game_over_lerp);
            //Debug.Log(player_score.transform.position);

            player_score.GetComponent<Text>().fontSize = (int) ( 100f + (100f * game_over_lerp));
            game_over_lerp = Mathf.Clamp(game_over_lerp + 3f * Time.deltaTime, 0f, 1f);
        }
        else
        {
            game_over_screen.SetActive(false);
        }
	}

    public void PauseUI ( bool _pause )
    {

        paused = _pause;

        // if paused
        if (_pause)
        {
            pause_menu.SetActive(true);
        }
        else
        {
            pause_menu.SetActive(false);
        }
    }

    public void changeMusicStatus ( bool status )
    {
        music_enabled = status;
        if ( status ) music_ui.GetComponent<Image>().sprite = music_on;
        else music_ui.GetComponent<Image>().sprite = music_off;
    }

    public void changeSoundStatus ( bool status )
    {
        sound_enabled = status;
        if (status)
        {
            sound_ui.GetComponent<Image>().sprite = sound_on;
        }
        else sound_ui.GetComponent<Image>().sprite = sound_off;
    }

    public void RestartGame ()
    {
        Debug.Log("Restart Game");
        PauseUI(false);
        game_over_lerp = 0f;
        FindObjectOfType<GameController>().RestartGame();
    }

    public void gameOver ()
    {
        game_over = true;
    }

    public void setBallInstance (GameObject _instance)
    {
        Debug.Log("Ball instance set!");
        ball_instance = _instance;
        Debug.Assert(ball_instance != null);
    }

    public void setScore ( int new_score )
    {
        score_text.text = "" + new_score;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public GameObject player_prefab;
    public Camera scene_cam;
    public GameObject ball_prefab;
    public GameObject offscreen_indicator;
    public GameObject coin_prefab;
    public GameObject destination_prefab;
    public GameObject destination_pointer_prefab;

    private GameObject destination_pointer_instance;
    private int hits_before_reaching_destination = 0;
    private int point_incrementer = 1;
    private int score = 0;
    private GameObject destination_instance;
    private Vector2 destination_position;
    public int coins_collected = 0;
    private bool reset_coins;
    private GameObject[] coin_jar;
    private const int MAX_COIN_COUNT = 5;
    private Vector2 coin_displacement = new Vector2(0.8f, 0f);
    private UIController ui_ctrl;
    private bool paused = false;
    private const float BALL_OFFSCREEN_HEIGHT = 5.85f;
    private const float BALL_OFFSCREEN_MAX_DISPLACEMENT_SCALE = 4.15f;
    private GameObject offscreen_indicator_instance;
    private Rigidbody2D ball_rb;
    private GameObject ball_instance;
    private PlayerController player_ctrl_instance;
    private GameObject player_scale_point_indicator;
    private GameObject player_scale_point;
    private SpriteRenderer power_scale_arrow_sprite;
    private SpriteRenderer power_scale_sprite;
    private GameObject player_instance;
    private GameObject player_power_scale;
    private bool pressed = false;
    private Vector2 touch_pos;
    private Vector2 end_pos;
    private const float MAX_DISTANCE = 200;
    private const float MAX_POWER_Y_SCALE = 70;
    private const float SHOT_METER_SPEED = 10f;
    private float shot_meter_lerp = 0f;
    public float strength = 0f;
    private float rad;
    private bool shot_active = false;
    private const float BALL_GRAV_SCALE = 0.7f;
    private Vector2 position_before_pause;
    private Vector3 ball_spawn_point = new Vector3(0f, 2f, 0f);

    private Color MAX_COLOR = new Color(0.84f, 0.412f, 0.412f, 0.5f);
    private Color MIN_COLOR = new Color(0.58f, 0.835f, 0.592f, 0.5f);

    // Use this for initialization
    private void Awake()
    {
        ball_instance = Instantiate(ball_prefab, ball_spawn_point, Quaternion.identity);
        ui_ctrl = FindObjectOfType<UIController>();
        Debug.Assert(ui_ctrl != null);
        ui_ctrl.ball_instance = ball_instance;
        //ui_ctrl.setBallInstance(ball_instance);
        destination_instance = Instantiate(destination_prefab, transform.position, Quaternion.identity);
        Debug.Assert(destination_instance != null);
    }
    void Start () {
        destination_pointer_instance = Instantiate(destination_pointer_prefab, transform.position, Quaternion.identity);
        destination_pointer_instance.transform.SetParent(scene_cam.transform);
        destination_pointer_instance.SetActive(false);
        offscreen_indicator_instance = Instantiate(offscreen_indicator, new Vector3(0f, 4.55f, 0f), Quaternion.identity);
        offscreen_indicator_instance.SetActive(false);
        ball_rb = ball_instance.GetComponent<Rigidbody2D>();
        player_instance = Instantiate(player_prefab, transform.position, Quaternion.identity);
        position_before_pause = player_instance.transform.position;
        player_instance.SetActive(false);
        player_power_scale = player_instance.transform.Find("power_scale").gameObject;
        power_scale_sprite = player_power_scale.GetComponent<SpriteRenderer>();
        player_scale_point_indicator = player_power_scale.transform.Find("power_arrow_indicator").gameObject;
        player_scale_point = player_instance.transform.Find("power_arrow_point").gameObject;
        power_scale_arrow_sprite = player_scale_point.GetComponent<SpriteRenderer>();
        player_ctrl_instance = player_instance.GetComponent<PlayerController>();
        Debug.Assert(player_ctrl_instance);
        player_instance.transform.SetParent(scene_cam.transform);

        Debug.Assert(scene_cam != null);
        setScore(0);
        generateDestination ();

        coin_jar = new GameObject[MAX_COIN_COUNT];
        for (int i = 0; i < MAX_COIN_COUNT; i++)
        {
            coin_jar[i] = Instantiate(coin_prefab, transform.position, Quaternion.identity);
            coin_jar[i].GetComponent<CoinScript>().offset(i, MAX_COIN_COUNT);
            coin_jar[i].SetActive(false);
        }
        reset_coins = true;
	}

    // Update is called once per frame
    void Update () {

        // beginning test
        controlDestinationPointer();

        //if (destination_renderer.isVisible)
        //{
        //    Debug.Log("Destination seen in Camera!");
        //}
        //else Debug.Log("Destination out of Camera!");

        // end of testing

        if (coins_collected == MAX_COIN_COUNT) reset_coins = true;
        if (reset_coins) resetCoins();

        if (!ui_ctrl.paused)
        {
            ball_rb.gravityScale = BALL_GRAV_SCALE;
            checkBallOffscreen();
            cameraControl(ball_instance);

            player_controls();
            position_before_pause = ball_instance.transform.position;

            if (ball_instance.transform.position.x < -16.6f || ball_instance.transform.position.x > 16.6f)
            {
                gameOver();
                ball_instance.SetActive(false);
            }

        } else
        {
            ball_instance.transform.position = position_before_pause;
            ball_rb.gravityScale = 0.0f;
        }

	}

    private void controlDestinationPointer ()
    {
        //visibleInCam(destination_instance)
        Debug.Assert(destination_instance != null);
        if (!visibleInCam(destination_instance))
        {
            destination_pointer_instance.SetActive(true);

            // angle between the destination and the ball
            float delta_x = destination_instance.transform.position.x - ball_instance.transform.position.x;
            float delta_y = destination_instance.transform.position.y - ball_instance.transform.position.y;
            destination_pointer_instance.transform.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan2(delta_y, delta_x));

            if (relativeDirection(scene_cam.transform.position, destination_instance.transform.position) == 1)
            {
                // Right 2.4
                destination_pointer_instance.transform.localPosition = new Vector2(
                    2.4f,
                    destination_pointer_instance.transform.localPosition.y);
            } else
            {
                // Left -2.4
                destination_pointer_instance.transform.localPosition = new Vector2(
                    -2.4f,
                    destination_pointer_instance.transform.localPosition.y);
            }

            // follow the y value
            destination_pointer_instance.transform.position = new Vector2(
                    destination_pointer_instance.transform.position.x,
                    destination_instance.transform.position.y
                );

        }
        else destination_pointer_instance.SetActive(false);
    }

    public void generateDestination (bool scored = false) {
        if (scored)
        {
            if (hits_before_reaching_destination == 1)
            {
                // if it only took me 1 try to hit, add 1 to multiplier
                point_incrementer += 1;
            }
            else point_incrementer = 1;
            // increase score count
            increaseScore();
        }

        hits_before_reaching_destination = 0;
       
        // generate a destination that the ball must reach
        destination_position = new Vector2( Random.Range(-8.9f, 8.9f), Random.Range(0.5f, 3.5f) );
        destination_instance.transform.position = destination_position;

    }

    private void resetCoins()
    {
        //Debug.Log("Resetting Coins");
        coins_collected = 0;
        reset_coins = false;

        // choose a start position
        Vector2 new_start_position = new Vector2(Random.Range(-10f, 5f), Random.Range(0.6f, 4f));
        for ( int i = 0; i < MAX_COIN_COUNT; i++ )
        {
            Vector2 specific_start_position = new_start_position + (i * coin_displacement);
            //Debug.Log("Init Start Pos [" + i + "] => " + specific_start_position);
            coin_jar[i].transform.position = specific_start_position;
            coin_jar[i].GetComponent<CoinScript>().changeInitPosition( specific_start_position );

        }
    }

    private void player_controls ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            player_power_scale.SetActive(true);
            player_scale_point.SetActive(true);
            shot_meter_lerp = 0f;
            pressed = true;
            player_instance.SetActive(true);
            player_instance.transform.position = scene_cam.ScreenToWorldPoint(Input.mousePosition);
            player_instance.transform.position = new Vector3(player_instance.transform.position.x, player_instance.transform.position.y,
                0);
            touch_pos = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 displacement_vec = new Vector2(Mathf.Cos(rad + Mathf.PI / 2), Mathf.Sin(rad + Mathf.PI / 2)) * (2.6f * strength);
            end_pos = (Vector2)player_instance.transform.position + displacement_vec;
            pressed = false;
            shot_active = true;
            player_power_scale.SetActive(false);
            player_scale_point.SetActive(false);
        }
        if (shot_active) shoot();

        if (pressed)
        {
            if ((Vector2)Input.mousePosition == touch_pos)
            {
                player_instance.transform.eulerAngles = new Vector3(0f, 0f, 0f);
            }
            else
            {
                rad = Mathf.Atan2(Input.mousePosition.y - touch_pos.y, Input.mousePosition.x - touch_pos.x) + (Mathf.PI / 2);
                player_instance.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Rad2Deg * rad);
            }

            // find the distance from the touch position
            float distance = Vector2.Distance(touch_pos, (Vector2)Input.mousePosition);

            strength = distance / MAX_DISTANCE;
            strength = strength > 1 ? 1 : strength;
            player_ctrl_instance.strength = strength;
            float y_scale_value = MAX_POWER_Y_SCALE * strength;
            player_power_scale.transform.localScale = new Vector3(
                    player_power_scale.transform.localScale.x,
                    y_scale_value,
                    player_power_scale.transform.localScale.z
                );


            power_scale_sprite.color = Color.Lerp(MIN_COLOR, MAX_COLOR, strength);
            // set the position of the arrow to the indicator
            player_scale_point.transform.position = player_scale_point_indicator.transform.position;
            power_scale_arrow_sprite.color = power_scale_sprite.color;

        }
    }

    private void checkBallOffscreen ()
    {
        // 5.85

        if ( ball_instance.transform.position.y >= BALL_OFFSCREEN_HEIGHT )
        {
            offscreen_indicator_instance.SetActive(true);
            offscreen_indicator_instance.transform.position = new Vector2(
                ball_instance.transform.position.x,
                offscreen_indicator_instance.transform.position.y);

            float offscreen_scale = 0.5f + ( 0.5f * (ball_instance.transform.position.y - BALL_OFFSCREEN_HEIGHT) / BALL_OFFSCREEN_MAX_DISPLACEMENT_SCALE);
            offscreen_scale = Mathf.Clamp(offscreen_scale, 0f, 1f);
            offscreen_indicator_instance.transform.localScale = new Vector3(offscreen_scale, offscreen_scale, 0f);
        }
        else offscreen_indicator_instance.SetActive(false);
    }

    private void cameraControl (GameObject _obj)
    {
        scene_cam.transform.position = new Vector3(Mathf.Clamp(_obj.transform.position.x, -12.87f, 12.85f), scene_cam.transform.position.y, scene_cam.transform.position.z);
    }

    private void shoot ()
    {

        if (shot_meter_lerp == 0f) hits_before_reaching_destination++;

        player_ctrl_instance.shooting = true;
        // based on the shot strength and touch position, find the end position
        //Debug.Log(shot_meter_lerp + " From: " + ((Vector2)scene_cam.ScreenToWorldPoint(touch_pos)) + " to : " + end_pos);
        shot_meter_lerp += SHOT_METER_SPEED * Time.deltaTime;
        player_instance.transform.position = Vector2.Lerp((Vector2) scene_cam.ScreenToWorldPoint(touch_pos), end_pos, shot_meter_lerp);

        if (shot_meter_lerp >= 1f)
        {
            shot_active = false;
            player_instance.SetActive(false);
            player_ctrl_instance.shooting = false;
        }
    }

    public void RestartGame ()
    {
        Debug.Log("Restart game triggered");
        ball_instance.SetActive(true);
        ball_instance.transform.position = ball_spawn_point;

        ResetScore ();
    }

    private void gameOver ()
    {
        //Debug.Log("Game Over");

        ui_ctrl.gameOver();
    }

    private void ResetScore ()
    {
        score = 0;
    }

    private void increaseScore ()
    {
        score += point_incrementer;
        ui_ctrl.setScore(score);
    }

    private void setScore ( int new_score )
    {
        score = new_score;
        ui_ctrl.setScore(new_score);
    }

    public bool visibleInCam ( GameObject obj )
    {
        // 3.6 to -3.8
        float delta_x = scene_cam.transform.position.x - obj.transform.position.x;
        if (delta_x < 3.6f && delta_x > -3.8f) return true;
        return false;
    }

    private int relativeDirection (Vector2 from, Vector2 to)
    {
        //return 1 or -1 based on the direction of to relative to from
        float delta_x = from.x - to.x;
        return delta_x < 0 ? 1 : -1;
    }
}

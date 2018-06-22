using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentalController : MonoBehaviour {

    public GameObject clouds_close;
    public GameObject clouds_far;
    public GameObject sun;

    private Vector3 clouds_close_init_pos;
    private Vector3 clouds_far_init_pos;
    private const float CLOSE_CLOUDS_SPEED = 1.5f;
    private const float FAR_CLOUDS_SPEED = 2.5f;
    private const float CLOSE_SCROLLING_LOOP_RESET_POSITION = -30f;
    private const float FAR_SCROLLING_LOOP_RESET_POSITION = -33.7f;

    private int sun_lerp_dir = 1;
    private float sun_lerp_control = 0f;
    private Vector3 sun_init_pos;
    private Vector3 sun_end_pos;

    // Use this for initialization
    void Start () {
        clouds_close_init_pos = clouds_close.transform.position;
        clouds_far_init_pos = clouds_far.transform.position;
        sun_init_pos = sun.transform.position;
        sun_end_pos = sun_init_pos + new Vector3(0f, 0.4f);
    }

	// Update is called once per frame
	void Update () {
        clouds_controller();
        sun_controller();
	}

    private void sun_controller()
    {
        sun_lerp_control += sun_lerp_dir * 0.34f * Time.deltaTime;
        if (sun_lerp_control > 1f) sun_lerp_dir = -1;
        if (sun_lerp_control < 0f) sun_lerp_dir = 1;
        sun.transform.localPosition = Vector2.Lerp(sun_init_pos, sun_end_pos, sun_lerp_control);

    }
    private void clouds_controller ()
    {
        // scroll the clouds
        clouds_close.transform.position -= new Vector3(CLOSE_CLOUDS_SPEED * Time.deltaTime, 0f);
        clouds_far.transform.position -= new Vector3(FAR_CLOUDS_SPEED * Time.deltaTime, 0f);

        // reset the clouds
        if (clouds_close.transform.position.x < CLOSE_SCROLLING_LOOP_RESET_POSITION)
        {
            clouds_close.transform.position = clouds_close_init_pos;
        }
        if (clouds_far.transform.position.x < FAR_SCROLLING_LOOP_RESET_POSITION)
        {
            clouds_far.transform.position = clouds_far_init_pos;
        }
    }
}

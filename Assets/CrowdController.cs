using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdController : MonoBehaviour {

    private ArrayList group_leader = new ArrayList();
    private GameObject[] group_1;
    private GameObject[] group_2;
    private GameObject[] group_3;

    private int active_group = 0;
    private float group_lerp = 0f;
    private float v_disp = 0.05f;
    private int lerp_dir = 1;
    private const float UP_LERP_SPEED = 1.8f;
    private const float DOWN_LERP_SPEED = 2.2f;

    private float front_y_begin;
    private float front_y_end;
    private float middle_y_begin;
    private float middle_y_end;
    private float back_y_begin;
    private float back_y_end;

	// Use this for initialization
	void Start () {

        group_1 = new GameObject[3];
        group_2 = new GameObject[3];
        group_3 = new GameObject[3];

        int group_1_ind = 0;
        int group_2_ind = 0;
        int group_3_ind = 0;
        for ( int i = 0; i < gameObject.transform.childCount; i++ )
        {
            Transform curr_child = gameObject.transform.GetChild(i);
            int curr_group = getGroup(curr_child.name);
            if (curr_group == 1)
            {
                group_1[group_1_ind] = curr_child.gameObject;
                group_1_ind++;
            } else if (curr_group == 2)
            {
                group_2[group_2_ind] = curr_child.gameObject;
                group_2_ind++;
            }else if (curr_group == 3)
            {
                group_3[group_3_ind] = curr_child.gameObject;
                group_3_ind++;
            }

            string crowd_pos = getPosition(curr_child.name);
            if (crowd_pos == "middle")
            {
                middle_y_begin = curr_child.position.y;
                middle_y_end = middle_y_begin + v_disp;
            } else if (crowd_pos == "back")
            {
                back_y_begin = curr_child.position.y;
                back_y_end = back_y_begin + v_disp;
                
            } else if (crowd_pos == "front")
            {
                front_y_begin = curr_child.position.y;
                front_y_end = front_y_begin + v_disp;
            }
        }


        group_leader.Add(group_1);
        group_leader.Add(group_2);
        group_leader.Add(group_3);

    }
	
	// Update is called once per frame
	void Update () {
        //Debug.Log(group_lerp + " " + lerp_dir);

        control_crowd();
	}

    private void control_crowd ()
    {
        if ( group_lerp < 0 )
        {
            // reset and move to next group
            group_lerp = 0f;
            lerp_dir = 1;
            active_group = (active_group+1)%3;
        }
        if (group_lerp > 1) lerp_dir = -1;

        if (lerp_dir == 1) group_lerp += UP_LERP_SPEED * Time.deltaTime;
        else group_lerp -= DOWN_LERP_SPEED * Time.deltaTime;

        for ( int i = 0; i < 3; i++ )
        {
            Debug.Assert(i < 3);
            Debug.Assert(i >= 0);

            float current_y_start;
            float current_y_end;

            Debug.Assert(group_leader != null);
            GameObject curr_crowd = ( (GameObject[]) group_leader[active_group] )[i];
            string pos = getPosition(curr_crowd.name);
            if (pos == "front")
            {
                current_y_start = front_y_begin;
                current_y_end = front_y_end;
            } else if (pos == "middle")
            {
                current_y_start = middle_y_begin;
                current_y_end = middle_y_end;
            } else
            {
                current_y_start = back_y_begin;
                current_y_end = back_y_end;
            }

            curr_crowd.transform.position = Vector2.Lerp( new Vector2(curr_crowd.transform.position.x,current_y_start ), 
                new Vector2(curr_crowd.transform.position.x, current_y_end), group_lerp );
        }
    }

    private int getGroup (string identifier)
    {
        int to_return;
        Int32.TryParse (identifier.Substring( "crowd_group_".Length, 1 ), out to_return);
        return to_return;
    }
    private string getPosition (string identifier)
    {
        return identifier.Substring("crowd_group_1_".Length);
    }
}

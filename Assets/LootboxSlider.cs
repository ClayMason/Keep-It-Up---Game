using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootboxSlider : MonoBehaviour {

    // Use this for initialization

    public Text cost_btn_text;
    public Button cost_btn;

    private const float TOUCH_DISTANCE_SLIDE = 150f;
    private const float SLIDE_SPEED = 10f;
    private const float PRE_SLIDE_DISP = 1.4f;
    private const float PRE_SLIDE_SPEED = 4.3f;
    private const float POST_SLIDE_DISP = 1f;
    private const float POST_SLIDE_SPEED = 4f;

    private int active_ind;
    private Vector2 touch_begin;
    private bool recording;
    private bool sliding;
    private bool pre_slide;
    private bool post_slide;
    private int slide_dir;
    private Vector2 from_pre_;
    private Vector2 from_post_;
    private Vector2 to_pre_;
    private Vector2 to_post_;
    private float slide_lerp;
    private float pre_slide_lerp;
    private float post_slide_lerp;
    private LootboxScript[] all_lootbox_scripts;

    public bool lootbox_focused = false;

	void Start () {

        all_lootbox_scripts = new LootboxScript[transform.childCount];
        for ( int i = 0; i < transform.childCount; ++i )
        {
            all_lootbox_scripts[i] = transform.GetChild(i).gameObject.GetComponent<LootboxScript>();
        }

        active_ind = 0;
        checkLootboxStatus();
        cost_btn.onClick.AddListener(OpenLootbox);
        recording = false;
        sliding = false;
        setCameraPos(active_ind);

        pre_slide = false;
        post_slide = false;
	}
	
	// Update is called once per frame
	void Update () {
        if ( !post_slide && !pre_slide && !sliding && !anyFocus() && Input.GetMouseButtonDown(0) )
        {
            // mouse down
            touch_begin = Input.mousePosition;
            if (!lootbox_focused) recording = true;
            slide_lerp = 0f;
            pre_slide_lerp = 0f;
            post_slide_lerp = 0f;
        }

        if ( Input.GetMouseButtonUp(0) )
        {
            // mouse up
            recording = false;

        }

        if ( recording )
        {
            if ( Vector2.Distance(touch_begin, Input.mousePosition) >  TOUCH_DISTANCE_SLIDE )
            {
                float slope_ = Mathf.Abs((touch_begin.y-Input.mousePosition.y)/(touch_begin.x-Input.mousePosition.x));
                if ( slope_ <= 0.67f )
                {
                    slide(Input.mousePosition.x - touch_begin.x);
                }
            }
        }

        if ( pre_slide )
        {

            // move opposite to direction to slide to create feel of momentum
            Vector2 pre_pos = Vector2.Lerp(from_pre_, from_post_, pre_slide_lerp);
            pre_slide_lerp = Mathf.Clamp(pre_slide_lerp + (PRE_SLIDE_SPEED * Time.deltaTime), 0f, 1f );
            Camera.main.transform.position = new Vector3 (pre_pos.x, pre_pos.y, Camera.main.transform.position.z);

            if ( pre_slide_lerp == 1f )
            {
                Camera.main.transform.position = new Vector3(from_post_.x, from_post_.y, Camera.main.transform.position.z);
                pre_slide = false;
                sliding = true;
            }
        }

        if ( sliding )
        {
            Vector2 new_pos = Vector2.Lerp(from_post_, to_pre_, slide_lerp);
            Camera.main.transform.position = new Vector3(new_pos.x, new_pos.y, Camera.main.transform.position.z);
            slide_lerp = Mathf.Clamp(slide_lerp + (SLIDE_SPEED * Time.deltaTime), 0f, 1f);
            //Debug.Log(slide_lerp);

            if ( slide_lerp == 1f )
            {
                Debug.Log("End Sliding");
                Camera.main.transform.position = new Vector3(to_pre_.x, to_pre_.y, Camera.main.transform.position.z);
                sliding = false;
                post_slide = true;
            }
        }

        if ( post_slide )
        {
            // Vector2 new_pos = Vector2.Lerp(to_pre_, to_post_, post_slide_lerp);
            Vector3 new_pos = Vector3.Slerp(to_pre_, to_post_, post_slide_lerp);
            Camera.main.transform.position = new Vector3(new_pos.x, new_pos.y, Camera.main.transform.position.z);
            post_slide_lerp = Mathf.Clamp(post_slide_lerp + (POST_SLIDE_SPEED * Time.deltaTime), 0f, 1f);

            if ( post_slide_lerp == 1f )
            {
                Camera.main.transform.position = new Vector3(to_post_.x, to_post_.y, Camera.main.transform.position.z);
                post_slide = false;
                checkLootboxStatus();
            }
        }
	}

    void OpenLootbox ()
    {
        if ( !isSliding() && currentLootbox().canGet () )
        {
            currentLootbox().Open();
        }
    }

    LootboxScript currentLootbox ()
    {
        return all_lootbox_scripts[active_ind];
    }

    void checkLootboxStatus()
    {
        if (currentLootbox().getLootboxCount() > 0)
        {
            cost_btn_text.text = "OPEN " + currentLootbox().getLootboxCount();
        }
        else
        {
            cost_btn_text.text = "BUY " + currentLootbox().getPrice();
        }
    }

    void slide (float val)
    {
        pre_slide = false;
        sliding = false;
        post_slide = false;
        if ( val > 0 && active_ind > 0 )
        {
            // go left
            Debug.Log("Go left");
            recording = false;
            pre_slide = true;
            slide_dir = -1;

            from_pre_ = Camera.main.transform.position;
            from_post_ = from_pre_ + new Vector2(PRE_SLIDE_DISP, 0f);
            to_post_ = transform.GetChild(active_ind - 1).position;
            to_pre_ = to_post_ + new Vector2(POST_SLIDE_DISP * -1, 0f);
            --active_ind;
        }

        if ( val < 0 && active_ind < transform.childCount - 1 )
        {
            // go right
            Debug.Log("Go right");
            recording = false;
            pre_slide = true;
            slide_dir = 1;

            from_pre_ = Camera.main.transform.position;
            from_post_ = from_pre_ + new Vector2(PRE_SLIDE_DISP * -1, 0f);
            to_post_ = transform.GetChild(active_ind + 1).position;
            to_pre_ = to_post_ + new Vector2(POST_SLIDE_DISP, 0f);
            ++active_ind;
        }
    }

    private bool anyFocus ()
    {
        for ( int i = 0; i < all_lootbox_scripts.Length; ++i )
        {
            if (all_lootbox_scripts[i].hasFocus()) return true;
        }
        return false;
    }

    void setCameraPos (int child_ind)
    {
        if (child_ind >= 0 && child_ind < transform.childCount) setCameraPos(transform.GetChild(child_ind).position);
    }

    void setCameraPos (Vector2 pos)
    {
        Camera.main.transform.position = new Vector3(pos.x, pos.y, Camera.main.transform.position.z);
    }

    public bool isSliding ()
    {
        return pre_slide || sliding || post_slide;
    }
}

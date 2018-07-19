using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootboxScript : MonoBehaviour {

    /*
     * LOOTBOX
     * Coin Rewards:
     *      25 coins    50 coins    100 coins
     *      250 coins   500 coins   1000 coins
     *      2000 coins  5000 coins  10,000 coins
     *      
     * Ball Skin Reward:
     * 
     * Ball Trail Reward:
     *      TODO
     */

    private int price = 15;
    public string type;
    private int lootbox_count = 0;

    public float coin_chance = 0.4f;
    public float ball_skin_chance = 0.3f;
    public float ball_trail_chance = 0.3f;

    public float common_chance = 0.6f;
    public float rare_chance = 0.3f;
    public float epic_chance = 0.02f;
    public float legendary_chance = 0.0f;

    private float[] coin_reward_chance;
    private int[] coin_reward_values;
    private GameObject lower_;
    private GameObject upper_;
    private GameObject lock_;

    private bool pre_scaling = false;
    private bool scaling = false;
    private bool pausing_scale = false;
    private bool open_slide = false;
    private const float UPPER_DISP = 1.5f;
    private const float LOWER_DISP = -1f;
    private const float PRE_SCALE = 0.65f;
    private const float POST_SCALE = 1.25f;
    private const float PRE_SCALE_SPEED = 3.5f;
    private const float SCALE_SPEED = 8f;
    private const float PAUSE_SCALE_TIME = 0.6f;
    private const float OPEN_SPEED = 5f;
    private float scale_lerp;
    private Vector2 init_scale_vec;
    private Vector2 init_upper_pos;
    private Vector2 init_lower_pos;
    private LootboxSlider slider_;

    private void Start()
    {
        if ( type == "bronze" )
        {
            price = 15;
            lootbox_count = PlayerPrefs.GetInt("lootbox_bronze");
        } else if ( type == "emerald" )
        {
            price = 40;
            lootbox_count = PlayerPrefs.GetInt("lootbox_emerald");
        } else if ( type == "diamond" )
        {
            price = 100;
            lootbox_count = PlayerPrefs.GetInt("lootbox_diamond");
        } else
        {
            Debug.Log ("Invalid lootbox type.");
        }

        slider_ = GameObject.Find("LootBox Slider").GetComponent<LootboxSlider>();
        init_scale_vec = transform.localScale;
        Debug.Log("Init scale: " + init_scale_vec);
        scale_lerp = 0f;

        // these indexes match with the coin values in the comments above, respectively
        coin_reward_chance = new float [9] {50f, 35f, 20f, 
                                            15f, 10f, 5f, 
                                            3f, 2f, 1f};
        coin_reward_values = new int[9] {25, 50, 100,
                                         250, 500, 1000,
                                         2000, 5000, 10000};

        upper_ = transform.Find("Lootbox Upper").gameObject;
        lower_ = transform.Find("Lootbox Lower").gameObject;
        lock_ = transform.Find("Lootbox Lower").Find("Lootbox Lock").gameObject;
        
        init_upper_pos = upper_.transform.position;
        init_lower_pos = lower_.transform.position;
    }

    private void Update()
    {
        scaleAnimation();
    }

    private void scaleAnimation ()
    {
        if (pre_scaling)
        {
            if (slider_.lootbox_focused == false) slider_.lootbox_focused = true;

            transform.localScale = new Vector3(
                    init_scale_vec.x + ((PRE_SCALE - init_scale_vec.x) * (scale_lerp)),
                    init_scale_vec.y + ((PRE_SCALE - init_scale_vec.y) * (scale_lerp)),
                    transform.localScale.z
                );
            scale_lerp = Mathf.Clamp(scale_lerp + (PRE_SCALE_SPEED * Time.deltaTime), 0f, 1f);
            if (scale_lerp == 1f)
            {
                transform.localScale = new Vector3(PRE_SCALE, PRE_SCALE, transform.localScale.z);
                pre_scaling = false;
                pausing_scale = true;
                scale_lerp = 0f;
            }
        }
        else if (pausing_scale)
        {
            if (slider_.lootbox_focused == false) slider_.lootbox_focused = true;

            scale_lerp = Mathf.Clamp(scale_lerp + Time.deltaTime, 0f, PAUSE_SCALE_TIME);
            if (scale_lerp == PAUSE_SCALE_TIME)
            {
                pausing_scale = false;
                scaling = true;
                scale_lerp = 0f;
            }
        }
        else if (scaling)
        {
            if (slider_.lootbox_focused == false) slider_.lootbox_focused = true;

            transform.localScale = new Vector3(
                    PRE_SCALE + ((POST_SCALE - PRE_SCALE) * (scale_lerp)),
                    PRE_SCALE + ((POST_SCALE - PRE_SCALE) * (scale_lerp)),
                    transform.localScale.z
                );
            scale_lerp = Mathf.Clamp(scale_lerp + (SCALE_SPEED * Time.deltaTime), 0f, 1f);
            if (scale_lerp == 1f)
            {
                transform.localScale = new Vector3(POST_SCALE, POST_SCALE, transform.localScale.z);
                scaling = false;
                open_slide = true;
                scale_lerp = 0f;
            }
        }
        else if ( open_slide )
        {
            if (slider_.lootbox_focused == false) slider_.lootbox_focused = true;
            upper_.transform.position = new Vector3(
                    init_upper_pos.x,
                    init_upper_pos.y + (UPPER_DISP * scale_lerp),
                    upper_.transform.position.z
                );
            lower_.transform.position = new Vector3(
                    init_lower_pos.x,
                    init_lower_pos.y + (LOWER_DISP * scale_lerp),
                    lower_.transform.position.z
                );

            scale_lerp = Mathf.Clamp(scale_lerp + (OPEN_SPEED * Time.deltaTime), 0f, 1f);

            if ( scale_lerp == 1f )
            {
                upper_.transform.position = new Vector3(
                        init_upper_pos.x,
                        init_upper_pos.y + UPPER_DISP,
                        upper_.transform.position.z
                    );
                lower_.transform.position = new Vector3(
                        init_lower_pos.x,
                        init_lower_pos.y + LOWER_DISP,
                        lower_.transform.position.z
                    );
                scale_lerp = 0f;
                open_slide = false;
            }
            
        }
        else
        {
            if (slider_.lootbox_focused == true) slider_.lootbox_focused = false;
        }
    }

    private void grantReward ()
    {

    }

    private string randomReward (out int val)
    {
        // Roll chance for type of reward first
        float[] reward_type = { coin_chance, ball_skin_chance, ball_trail_chance };
        int reward_output = chanceRoll(reward_type);

        if (reward_output == 0)
        {
            // Coin Reward
            int coin_reward = chanceRoll(coin_reward_chance);
            val = coin_reward_values[coin_reward];
            return "coin";
        } else
        {
            BackgroundData bg_data = GameObject.Find("BackgroundDataCenter").GetComponent<BackgroundData>();

            if ( reward_output == 1)
            {
                // Ball Skin Reward

                float[] skin_chance = {common_chance, rare_chance, epic_chance, legendary_chance};
                int skin_reward = chanceRoll(skin_chance);
                string[] skin_type = { "common", "rare", "epic", "legendary" };

                Dictionary<string, ArrayList> ball_categories = bg_data.getCollectables()["ball_skins"];
                val = (int) Random.Range(0, ( ball_categories[skin_type[skin_reward]].Count - 0.01f) );
                return "ball_skin";

            }
            /*
             else if (reward_output == 2)
            {
                // Ball Trail Reward
                Debug.Log("TODO: Ball Trail");
            }
            */
            else Debug.Log("Reward Output out of range of index");
        }



        // TODO - remove
        val = -1;
        return "null";
    }

    private int chanceRoll (float[] chances)
    {
        /*
         * Given an array of floats, randomly choose an index of chances arr, given its float value, out of the aggregate
         */

        float sum = 0f;
        for (int i = 0; i < chances.Length; ++i) sum += chances[i];
        float rand_val = Random.Range(0f, sum);

        float chance_sum = 0f;
        int index = 0;
        while ( chance_sum < rand_val && index < chances.Length )
        {
            chance_sum += chances[index];
            ++index;
        }

        if ( chance_sum < rand_val )
        {
            Debug.Log("Did not find in chance roll");
        }
        return index;
    }

    public void Open ()
    {
        Debug.Log("OPEN LOOTBOX - " + type);
        pre_scaling = true;
    }

    public int getPrice ()
    {
        return price;
    }
    public int getLootboxCount ()
    {
        return lootbox_count;
    }
    
    public bool canGet ()
    {
        // can get if has any left or have enough to buy
        // TODO

        return true;
    }

    public bool hasFocus ()
    {
        // has focus if, either being opened or scaling
        return pre_scaling || scaling || pausing_scale || open_slide; // TODO add opening case
    }
}

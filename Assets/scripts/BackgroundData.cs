using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BackgroundData : MonoBehaviour {

    public BallScript[] ball_skins;
    public GameObject shop_ui_content;
    public GameObject shop_item_view_prefab;
    public GameObject popup_prefab;

    private Dictionary<string, Dictionary<string, ArrayList>> collectables;
    private GameObject popup_instance;
    private GameObject[] ball_shop_ui;

    public void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        // any item group should be stored in an array (derived from Scriptable Object) and added to collectables dictionary with a string as an identifier.
        Dictionary<string, ArrayList> ball_item_category = generateItemCategory(ball_skins);

        collectables = new Dictionary<string, Dictionary<string, ArrayList>>();
        collectables.Add("ball_skins", ball_item_category);

        ball_shop_ui = new GameObject[ball_skins.Length];
        populateShopContentPage();
    }

    private Dictionary<string, ArrayList> generateItemCategory(ScriptableObject[] obj_items)
    {
        Dictionary<string, ArrayList> coll = new Dictionary<string, ArrayList>();

        mergeSort<BallScript>(ref ball_skins);

        for (int i = 0; i < ball_skins.Length; ++i)
        {
            string[] item_info = parseItemInfo(ball_skins[i].name);
            if (item_info.Length < 2) Debug.Log(ball_skins[i].name + " gives an error.");
            if (coll.ContainsKey(item_info[1])) coll[item_info[1]].Add(ball_skins[i]);
            else
            {
                ArrayList new_list = new ArrayList();
                new_list.Add(ball_skins[i]);
                coll.Add(item_info[1], new_list);
            }
        }

        return coll;
    }

    private string[] parseItemInfo (string val)
    {
        return val.Split(' ');
    }

    public void purchaseBall (int ball_id)
    {
        ball_skins[ball_id].unlocked = true;
        PlayerPrefs.SetInt("ball_skin", ball_id);
        PlayerPrefs.SetInt("coin_count", PlayerPrefs.GetInt("coin_count") - ball_skins[ball_id].ball_price);
    }

    public Sprite getActiveBallSprite ()
    {
        return ball_skins[PlayerPrefs.GetInt("ball_skin")].ball_sprite;
    }

    private void populateShopContentPage()
    {
        // populate shop content page with the shop items
        int rows = (int)(((float)ball_skins.Length / 3) + 0.5f) + 1;

        int x_offset = -185;// 155; //-190
        int y_offset = 85 * rows;// 760;//-70;//  1000; //-80
        for ( int i = 0; i < ball_skins.Length; i++ )
        {
            GameObject current_obj = getShopItemPreviewGameobject(i);
            current_obj.transform.position = new Vector3(x_offset + ( (i % 3) * 225 ), y_offset - ((int) (i/3) * 230), 0.49218f);
            ball_shop_ui[i] = current_obj;
        }
        
        Debug.Log("Rows: " + rows);
        RectTransform shop_content_transform = shop_ui_content.GetComponent<RectTransform>();
        shop_content_transform.sizeDelta = new Vector2(683, 234 * rows);

    }

    private GameObject getShopItemPreviewGameobject (int ball_id)
    {
        // create an instance of the shop_item_view game object
        // and fill it with the information for the given ball_id

        GameObject item_preview = Instantiate(shop_item_view_prefab, transform.position, Quaternion.identity);
        item_preview.transform.SetParent( shop_ui_content.transform );

        // fill the game object with necessary information
        BallScript current_script = ball_skins[ball_id];
        item_preview.transform.Find("_name").GetComponent<Text>().text = current_script.ball_name;
        item_preview.transform.Find("_preview").GetComponent<Image>().sprite = current_script.ball_sprite;
        item_preview.name = "SHOP ITEM: " + current_script.ball_name;

        ShopItemMB shop_item_script = item_preview.GetComponent<ShopItemMB>();
        shop_item_script.bg_data = this;
        shop_item_script.id = ball_id;
        shop_item_script.price = ball_skins[ball_id].ball_price;

        if ( ball_id == PlayerPrefs.GetInt("ball_skin") )
        {
            // set as active ball
            //Debug.Log("id " + ball_id + "is active");
            item_preview.transform.Find("_price").gameObject.SetActive(false);
            item_preview.transform.Find("_coin").gameObject.SetActive(false);
            item_preview.transform.Find("_owned_active_text").gameObject.SetActive(true);

            item_preview.transform.Find("_owned_active_text").GetComponent<Text>().text = "ACTIVE";
        } else if ( current_script.unlocked )
        {
            //Debug.Log("id " + ball_id + " is unlocked but not active");
            // set text as OWNED
            item_preview.transform.Find("_price").gameObject.SetActive(false);
            item_preview.transform.Find("_coin").gameObject.SetActive(false);
            item_preview.transform.Find("_owned_active_text").gameObject.SetActive(true);

            item_preview.transform.Find("_owned_active_text").GetComponent<Text>().text = "OWNED";
        } else
        {
            //Debug.Log("id " + ball_id + " is locked.");
            // unbaught item
            item_preview.transform.Find("_price").gameObject.SetActive(true);
            item_preview.transform.Find("_coin").gameObject.SetActive(true);
            item_preview.transform.Find("_owned_active_text").gameObject.SetActive(false);

            item_preview.transform.Find("_price").GetComponent<Text>().text = current_script.ball_price + "";
        }

        return item_preview;
    }

    public void updateShopItemView ( int _id, GameObject shop_item_go, int old_active = -1 )
    {

        //Debug.Log("Item id (" + _id + ") vs Active Ball Id (" + PlayerPrefs.GetInt("ball_skin") + ")" );
        // check the status
        if ( _id == PlayerPrefs.GetInt("ball_skin") )
        {
            //Debug.Log("entered if");
            updateShopItemView(old_active, ball_shop_ui[old_active]);
            // set as active
            shop_item_go.transform.Find("_price").gameObject.SetActive(false);
            shop_item_go.transform.Find("_coin").gameObject.SetActive(false);
            shop_item_go.transform.Find("_owned_active_text").gameObject.SetActive(true);

            shop_item_go.transform.Find("_owned_active_text").GetComponent<Text>().text = "ACTIVE";
        } else if ( ball_skins[_id].unlocked )
        {
            //Debug.Log("entered else");
            shop_item_go.transform.Find("_price").gameObject.SetActive(false);
            shop_item_go.transform.Find("_coin").gameObject.SetActive(false);
            shop_item_go.transform.Find("_owned_active_text").gameObject.SetActive(true);

            shop_item_go.transform.Find("_owned_active_text").GetComponent<Text>().text = "OWNED";
        }
    }

    public void CreatePopup (string title, string desc, string btn_txt, Color btn_color, Color sprite_color, Color bg_color, Sprite sprite_, bool has_color = true)
    {

        Debug.Log("Creating popup with title: " + title);

        // Instantiate a popup game object (prefab) and set the properties.
        if ( popup_instance == null )
        {
            popup_instance = Instantiate(popup_prefab, popup_prefab.transform.position, popup_prefab.transform.rotation);
            popup_instance.transform.SetParent( GameObject.Find("Canvas").transform );
        }

        // TODO: Set the properties for popup_instance
        PopupScript popup_ = popup_instance.GetComponent<PopupScript>();
        popup_.setTitle(title);
        popup_.setMessage(desc);
        popup_.setButton(btn_txt, btn_color);
        popup_.setImage(sprite_, sprite_color, has_color);
        popup_.setBackground(bg_color);

        // Show the popup
        popup_instance.SetActive(true);
    }

    public BallScript FindRandomPurchasableItem ()
    {
        BallScript[] filtered_skins = new BallScript[ball_skins.Length];
        int size_ = 0;
        for ( int i = 0; i < ball_skins.Length; ++i )
        {
            if ( ball_skins[i].ball_price <= PlayerPrefs.GetInt("coin_count") )
            {
                filtered_skins[size_] = ball_skins[i];
                ++size_;
            }
        }

        if (size_ == 0) return null;
        else return filtered_skins[(int)(Random.Range(0, size_))];
    }

    public Dictionary<string, Dictionary<string, ArrayList>> getCollectables () { return collectables; }

    public void GetMoreCurrenct (int val)
    {
        if ( val == 0 )
        {
            Debug.Log("Get more coins");
        } else
        {
            Debug.Log("Get more paid currency");
        }
    }
    
}

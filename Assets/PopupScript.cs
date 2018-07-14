using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupScript : MonoBehaviour {

    public Color sprite_color;
    public Color bg_color;
    public Color btn_color;
    public Sprite sprite_img;
    public string title;
    public string desc;
    public string button_msg;

	// Use this for initialization
	void Start () {
        setTitle(title);
        setMessage(desc);

        setImage(sprite_img, sprite_color);
        setBackground(bg_color);

        setButton(button_msg, btn_color);
    }

    public void setTitle (string _title )
    {
        transform.Find("Popup Title").GetComponent<Text>().text = _title;
    }

    public void setMessage (string msg)
    {
        transform.Find("Popup Message").GetComponent<Text>().text = msg;
    }

    public void setImage (Sprite sprite_, Color clr_, bool has_color = true)
    {
        clr_ = clr_ == null ? new Color(1f, 1f, 1f) : clr_;
        Image sprite_img_component = transform.Find("Popup Image").GetComponent<Image>();
        sprite_img_component.sprite = sprite_;
        if (has_color) sprite_img_component.color = clr_;
    }

    public void setBackground (Color bg_clr)
    {
        transform.Find("Popup Background").GetComponent<Image>().color = bg_clr == null ? new Color(0f, 0f, 0f) : bg_clr;
    }

    public void setButton (string msg, Color clr_)
    {
        Transform btn_transform = transform.Find("Popup Button");
        btn_transform.Find("Text").GetComponent<Text>().text = msg;
        btn_transform.GetComponent<Image>().color = clr_;
    }
}

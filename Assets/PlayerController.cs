using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    
    public float strength;
    private float power = 7;
    public bool shooting = false;
    private float raycast_hit_radius = 1f;//0.7f;

    private GameObject raycast_start_location;
	// Use this for initialization
	void Start () {
        raycast_start_location = gameObject.transform.Find("raycast_start_location").gameObject;
	}
	
	// Update is called once per frame
	void Update () {

	}

    private void FixedUpdate()
    {
        Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.up) * 10, Color.white);
        if ( shooting )
        {
            //Debug.Log( Vector2.Distance( (Vector2) transform.position, (Vector2)GameObject.Find("Ball Object").transform.position));

            // move backwards by 1.3 units
            RaycastHit2D hit = Physics2D.Raycast(raycast_start_location.transform.position, transform.TransformDirection(Vector2.up), 2.5f);
            //Debug.Log(Vector2.Distance((Vector2) transform.position, (Vector2) hit.transform.position));
            Debug.Log(hit.collider.name);
            if ( hit != null && hit.transform != null && Vector2.Distance((Vector2)transform.position, (Vector2)hit.transform.position) <= raycast_hit_radius )
            {
                // shoot the object
                hit.rigidbody.velocity = transform.TransformDirection(Vector2.up) * strength * power;
                shooting = false;
            }
        }
    }
}

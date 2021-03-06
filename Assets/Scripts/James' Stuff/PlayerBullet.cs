﻿using UnityEngine;
using System.Collections;

public class PlayerBullet : MonoBehaviour {

	float speed;
	float dmg;

	float lifeTimer;

	SpriteRenderer sr;

	// Use this for initialization
	void Start () {
		speed = 2000;
		lifeTimer = 0;
		dmg = 5;
		sr = GetComponentInChildren<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		transform.position += new Vector3 (0, Time.deltaTime * speed, 0);
		lifeTimer += Time.deltaTime;
		if (!sr.isVisible) {
			Destroy (gameObject);
		}
	}

	void FixedUpdate(){

	}

	void OnTriggerEnter2D (Collider2D other){
		if (other.tag == "EnemyHit") {
			other.gameObject.GetComponentInParent<Movement> ().health -= dmg;
			other.gameObject.GetComponentInParent<Movement> ().Blink();
			Destroy (gameObject);
		} else if (other.tag == "WormPart") {
			other.gameObject.GetComponent<WormBod> ().mov.health -= dmg;
			other.gameObject.GetComponentInParent<WormBod> ().Blink();
			Destroy (gameObject);
        }
        else if (other.tag == "Boss")
        {
            other.gameObject.GetComponent<Boss>().DealDamage(dmg);
            Destroy(gameObject);
        }
        Debug.Log (other.tag);
	}
}

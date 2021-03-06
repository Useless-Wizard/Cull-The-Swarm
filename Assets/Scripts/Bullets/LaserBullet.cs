﻿using UnityEngine;
using System.Collections;

public class LaserBullet : MonoBehaviour {

	float dmg;
	public bool front;
	public Transform spriteObject;
	float scaleInc;
	float maxScale;
	BoxCollider2D bc;

	bool cool;

	SpriteRenderer sr;

	// Use this for initialization
	void Start () {
		cool = false;
		maxScale = 1000;
		dmg = 10;
		sr = GetComponentInChildren<SpriteRenderer> ();
		scaleInc = 1f;
		bc = GetComponent<BoxCollider2D> ();
	}

	// Update is called once per frame
	void Update () {
		if(Time.timeScale != 1f) return;

		if (Input.GetButton ("Precision")) {
			dmg = 30;
            transform.localScale = new Vector3(2f, transform.localScale.y, 0f);
        } else {
            transform.localScale = new Vector3(1f, transform.localScale.y, 0f);
            dmg = 20;
		}

		if (Input.GetButton ("Primary") || Input.GetButton("XBOX_A") || Input.GetButton("XBOX_RB")) {
			if (transform.localScale.y < maxScale) {
				if (scaleInc < 100f) {
					scaleInc += scaleInc;
				}
				if (scaleInc > 100) {
					scaleInc = 100;
				}
				transform.localScale += new Vector3 (0f, scaleInc, 0f);
				if (front) {
					transform.position += new Vector3 (0f, scaleInc / 2, 0f);
				} else {
					transform.position -= new Vector3 (0f, scaleInc / 2, 0f);
				}
			} else {
				transform.localScale = new Vector3 (transform.localScale.x, maxScale, 0f);
				if (front) {
					transform.position = new Vector3 (transform.position.x, transform.parent.position.y + maxScale/2, 0f);
				} else {
					transform.position = new Vector3 (transform.position.x, transform.parent.position.y - maxScale/2, 0f);
				}
			}
		} else {
			Destroy (gameObject);
		}

		transform.position = new Vector3 (transform.position.x, transform.position.y, 50);

	}

	void OnTriggerEnter2D (Collider2D other){
		if (other.tag == "EnemyHit" && !cool) {
			other.gameObject.GetComponentInParent<Movement> ().health -= dmg;
			other.gameObject.GetComponentInParent<Movement> ().Blink();
			StartCoroutine ("Cooldown");
		} else if (other.tag == "WormPart" && !cool) {
			other.gameObject.GetComponent<WormBod> ().mov.health -= dmg;
			other.gameObject.GetComponentInParent<WormBod> ().Blink();
			StartCoroutine ("Cooldown");
		}
		else if (other.tag == "Boss"  && !cool)
		{
			other.gameObject.GetComponent<Boss>().DealDamage(dmg);
			StartCoroutine ("Cooldown");
		}
	}
		
	void OnTriggerStay2D (Collider2D other){
		if (other.tag == "EnemyHit" && !cool) {
			other.gameObject.GetComponentInParent<Movement> ().health -= dmg;
			other.gameObject.GetComponentInParent<Movement> ().Blink();
			StartCoroutine ("Cooldown");
		} else if (other.tag == "WormPart" && !cool) {
			other.gameObject.GetComponent<WormBod> ().mov.health -= dmg;
			other.gameObject.GetComponentInParent<WormBod> ().Blink();
			StartCoroutine ("Cooldown");
		}
		else if (other.tag == "Boss"  && !cool)
		{
			other.gameObject.GetComponent<Boss>().DealDamage(dmg);
			StartCoroutine ("Cooldown");
		}
	}

	IEnumerator Cooldown(){
		cool = true;
		yield return new WaitForSeconds (.25f);
		cool = false;
		yield break;
	}
}

﻿using UnityEngine;
using System.Collections;

public class FlamethrowerPrimary : MonoBehaviour {

	public Transform gunF;
	float shootCool;
	float shootTimer;
	bool cooling;
	public GameObject bullet;
	private Player player;
	float degrees;
	bool rotRight;

	// Use this for initialization
	void Start () {
		shootCool = .015f;
		shootTimer = 0;
		cooling = false;
		player = GetComponent<Player> ();
		bullet = Resources.Load ("PlayerBullets/FireTestBullet") as GameObject;
		gunF = transform.Find ("GunF");

		degrees = 0;
	}

	// Update is called once per frame
	void Update () {
		if(Time.timeScale != 1f) return;

		if (Input.GetButtonDown ("Primary") && !cooling) {
			StartCoroutine ("Firing");
		}
		if(Input.GetButtonUp("Primary") && !cooling){
			cooling = true;
		}
		if (cooling) {
			if (shootTimer < shootCool) {
				shootTimer += Time.deltaTime;
			} else {
				shootTimer = 0;
				cooling = false;
			}
		}
	}

	void Shoot(){

		GameObject temp = Instantiate (bullet, gunF.position, Quaternion.identity) as GameObject;
		temp.GetComponent<FlamethrowerBullet>().degrees = degrees;
		degrees = (degrees + 15) % 360;


	}

	IEnumerator Firing(){
		while(Input.GetButton("Primary") && !player.dead){
			Shoot();
			yield return new WaitForSeconds(shootCool);
		}
		yield break;
	}
}

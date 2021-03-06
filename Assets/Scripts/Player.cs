﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

	public GameObject mesh;
	public GameObject playerObj; 
	private CircleCollider2D shrinkCollider;

	public float TimeScale = 1f;

	public bool chassisBooster;
	public bool chassisExterminator;
	public bool chassisShrink;
	public bool chassisQuick;
	public bool chassisFinal;

    // Make these into constants of another class later...
    float stageWidth = 1580f;
    float stageHeight = 830f;
    public float moveSpeed;
	public float precisionSpeed;
	float shipTilt = 0f;

	public float cooldownBoost = 1.2f;

	public ParticleSystem ps;
    public ParticleSystem ps2;
	public ParticleSystem ps3;

	private PhaseManager phaseOn;
	bool phaseEquipped = false;
	bool hitCool;
	public bool dead;
	public int health;
	HealthBorder hBorder;
	ScreenFade sf;

	Loadout loadout;

	Score scoreHandle;	//JUSTIN
	public GameObject retryDialog;	//JUSTIN

	//JUSTIN
	public PauseMenu pauseMenu;
	//JUSTIN

	// Use this for initialization
	void Start()
	{
		scoreHandle = GameObject.Find("Score").GetComponent<Score>();	//JUSTIN

		loadout = GameObject.FindGameObjectWithTag ("SaveManager").GetComponent<SavedGameManager> ().getCurrentGame ().getCurrentLoadout ();
		shrinkCollider = GetComponent<CircleCollider2D>(); 
		setLoadout ();

		hitCool = false;
		dead = false;

		//determines chassis health type
		if (chassisExterminator || chassisBooster || chassisShrink) {
			health = 3;
		} 
		else if (chassisQuick)
		{
			health = 2;
		} 
		else if (chassisFinal)
		{
			if((SceneIndex)SceneManager.GetActiveScene().buildIndex == SceneIndex.GAMEPLAY_4_2)
			{
				health = 5;
			}
			else
			{
				health = 1;
			}
		}

		//determines chassis movement speed type
		if (chassisExterminator || chassisFinal)
		{
			moveSpeed = 10f;
			precisionSpeed = 6f;
		} 
		else if (chassisBooster)
		{
			moveSpeed = 8f;
			precisionSpeed = 14f;
		} 
		else if (chassisShrink) 
		{
			moveSpeed = 6f;
			precisionSpeed = 4f;
		} 
		else if (chassisQuick)
		{
			moveSpeed = 12f;
			precisionSpeed = 8f;
		}



		hBorder = GameObject.FindGameObjectWithTag ("Border").GetComponent<HealthBorder> ();
		sf = GameObject.Find ("ScreenFade").GetComponent<ScreenFade> ();	
		hBorder.health = health;
		if (health == 5) {
			hBorder.fiveHealth = true;
		}
	}

	// Update is called once per frame
	void Update()
	{
		//JUSTIN
		if((Input.GetButtonDown("Pause") && pauseMenu != null) || Input.GetButtonDown("XBOX_Start") && pauseMenu != null)
		{
			pauseMenu.togglePaused();
			return;
		}
		if(Time.timeScale != 1f || LevelCompleteHandler.isLevelComplete) return;
		//Time.timeScale = TimeScale;
		//JUSTIN

		// Get input from controller.

		float hSpeed = 0;
		float vSpeed = 0;

		if (Input.GetAxisRaw ("Horizontal") != 0) {
			hSpeed = Mathf.Round (Input.GetAxisRaw ("Horizontal"));
		} else if(Input.GetAxisRaw("XBOX_LS_X") != 0){
			hSpeed = Mathf.Round (Input.GetAxisRaw ("XBOX_LS_X"));
		} else if(Input.GetAxisRaw("XBOX_DP_X") != 0){
			hSpeed = Mathf.Round (Input.GetAxisRaw ("XBOX_DP_X"));
		}

		if (Input.GetAxisRaw ("Vertical") != 0) {
			vSpeed = Mathf.Round(Input.GetAxisRaw("Vertical"));
		} else if(Input.GetAxisRaw("XBOX_LS_Y") != 0){
			vSpeed = Mathf.Round(Input.GetAxisRaw("XBOX_LS_Y"));
		} else if(Input.GetAxisRaw("XBOX_DP_Y") != 0){
			vSpeed = Mathf.Round(Input.GetAxisRaw("XBOX_DP_Y"));
		}

		// Slightly slower when moving diagonally.
		if (hSpeed != 0 && vSpeed != 0){
			hSpeed *= 0.7f;
			vSpeed *= 0.7f;
		}
		

		// Adjust for precision mode.
		if (Input.GetButton("Precision") || Input.GetButton("XBOX_LB"))
		{
			hSpeed *= precisionSpeed;
			vSpeed *= precisionSpeed;

			//adjusts scale during precision for shrink chassis
			if (chassisShrink)
			{
				mesh.transform.localScale =  new Vector3(.5f, .5f, .5f);
				shrinkCollider.radius = 6f;
			}
		}
		else
		{
			hSpeed *= moveSpeed;
			vSpeed *= moveSpeed;

			//adjusts scale during precision for shrink chassis
			if (chassisShrink)
			{
				mesh.transform.localScale = new Vector3(1f, 1f, 1f);
				shrinkCollider.radius = 12f;
			}
		}

		shipTilt = shipTilt * 0.8f - (2 * hSpeed) * 0.2f;

		transform.position = new Vector3(
			Mathf.Clamp(transform.position.x + hSpeed, -stageWidth / 2, stageWidth / 2),
			Mathf.Clamp(transform.position.y + vSpeed, -stageHeight / 2, stageHeight / 2),
			0
		);

		if (!hitCool && !mesh.activeSelf) {
			mesh.SetActive (true);
		}

		mesh.transform.rotation = Quaternion.Euler(0f, shipTilt, 0f);

		if (sf.finished && dead) {
			//JUSTIN
			//SceneManager.LoadScene ((int)SceneIndex.WORLD_MAP);
			retryDialog.SetActive(true);
			//JUSTIN
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if ((other.tag == "Bullet" || other.tag == "EnemyHit" || other.tag == "WormPart") && !hitCool)
		{
			if (phaseEquipped)
			{
				if (phaseOn.isActive)
				{
					return;
				}
				else
				{
					GetComponent<AudioSource> ().Play ();
                    ps2.GetComponent<ParticleSystem>().Play();
                    OnDamage ();
				}
			} 
			else 
			{
				GetComponent<AudioSource> ().Play ();
                ps2.GetComponent<ParticleSystem>().Play();
                OnDamage ();
			}
		}
	}

	void OnDamage(){

		scoreHandle.handlePlayerHit();	//JUSTIN

		hitCool = true;
		health--;
		hBorder.health = health;
		if (health == 1) {
			ParticleSystem.EmissionModule em = ps.emission;
			em.enabled = false;
			em = ps3.emission;
			em.enabled = true;
			ps3.Play ();
		}
        if (health <= 0) {
			OnDeath ();
		} else {
			StartCoroutine ("HitCool");
			StartCoroutine ("Blink");
		}
	}

	IEnumerator HitCool(){
		yield return new WaitForSeconds (1f);
		hitCool = false;
		yield break;
	}

	IEnumerator Blink(){
		ParticleSystem.EmissionModule em;
		if (health > 1) {
			em = ps.emission;
		} else {
			em = ps3.emission;
		}

		while (hitCool) {
			em.enabled = false;
			mesh.SetActive (!mesh.activeSelf);
			yield return new WaitForSeconds (.05f);
		}

		em.enabled = true;
		
		mesh.SetActive (true);
		yield break;
	}

	void OnDeath(){
		ParticleSystem.EmissionModule em = ps3.emission;
		em.enabled = false;
		mesh.SetActive (false);
		dead = true;
		sf.Fade ();

	}
	void setLoadout(){
		setPrimary ();
		setSecondary ();
		setChasis ();
	}
	void setPrimary(){
		switch(loadout.getPrimary()){
			case Loadout.LoadoutPrimary.NULL:
				Debug.Log ("Primary NULL");
				break;
		case Loadout.LoadoutPrimary.REPEL:
				Debug.Log ("Primary Set Bug Repellant");
				gameObject.AddComponent <StandardPrimary>();
				break;
			case Loadout.LoadoutPrimary.SWATTER:
				Debug.Log ("Primary set No-Miss Swatter");
				gameObject.AddComponent <SwatterPrimary>();
				break;
			case Loadout.LoadoutPrimary.FLAME:
				Debug.Log ("Primary set Flamethrower");
				gameObject.AddComponent <FlamethrowerPrimary>();
				break;
			case Loadout.LoadoutPrimary.VOLT:
				Debug.Log ("Primary set Volt Lantern");
				gameObject.AddComponent <LaserPrimary>();
				break;
			case Loadout.LoadoutPrimary.BUGSPRAY:
				Debug.Log ("Primary set Bug Spray");
				gameObject.AddComponent <BugSprayPrimary>();
				break;
			default:
				Debug.Log ("Primary not in range");
				break;
		}
	}
	void setSecondary(){
		switch (loadout.getSecondary ())
		{
		case Loadout.LoadoutSecondary.NULL:
			Debug.Log ("Secondary NULL");
			break;
		case Loadout.LoadoutSecondary.EMP:
			Debug.Log ("Secondary set EMP Counter");
			gameObject.AddComponent<EMPManager>();
			break;
		case Loadout.LoadoutSecondary.PHASING:
			Debug.Log ("Secondary set Phasing System");
			gameObject.AddComponent<PhaseManager> ();
			phaseOn = GetComponent<PhaseManager> ();
			phaseEquipped = true;
			break;
		case Loadout.LoadoutSecondary.TESLA:
			Debug.Log ("CSecondary set Mosquito Tesla Coil");
			gameObject.AddComponent<TeslaManager>();
			break;
		case Loadout.LoadoutSecondary.FREEZE:
			Debug.Log ("Secondary set Freeze Ray");
			gameObject.AddComponent<FreezeManager>();
			break;
		case Loadout.LoadoutSecondary.HOLOGRAM:
			Debug.Log ("Secondary set Holo Duplicate");
			gameObject.AddComponent<HoloManager>();
			break;
		default:
			Debug.Log ("Secondary not in range");
			break;
		}
	}
	void setChasis(){
		switch (loadout.getChasis ()) {
			case Loadout.LoadoutChasis.NULL:
				Debug.Log ("Armor NULL");
				chassisExterminator = false;
				chassisBooster = false;
				chassisShrink = false;
				chassisQuick = false;
				chassisFinal = false;
				break;
		case Loadout.LoadoutChasis.EXTERMINATOR:
				Debug.Log ("Armor Set Exterminator Chasis");
				chassisExterminator = true;
				chassisBooster = false;
				chassisShrink = false;
				chassisQuick = false;
				chassisFinal = false;
				break;
			case Loadout.LoadoutChasis.BOOSTER:
				Debug.Log ("Armor set Booster Chasis");
				chassisExterminator = false;
				chassisBooster = true;
				chassisShrink = false;
				chassisQuick = false;
				chassisFinal = false;
				break;
			case Loadout.LoadoutChasis.SHRINK:
				Debug.Log ("Armor set Shrink Chasis");
				chassisExterminator = false;
				chassisBooster = false;
				chassisShrink = true;
				chassisQuick = false;
				chassisFinal = false;
				break;
			case Loadout.LoadoutChasis.QUICK:
				Debug.Log ("Armor set Quick Chasis");
				chassisExterminator = false;
				chassisBooster = false;
				chassisShrink = false;
				chassisQuick = true;
				chassisFinal = false;
				break;
			case Loadout.LoadoutChasis.FINAL:
				Debug.Log ("Armor set Final Chasis");
				chassisExterminator = false;
				chassisBooster = false;
				chassisShrink = false;
				chassisQuick = false;
				chassisFinal = true;
				break;
			default:
				Debug.Log ("Armor not in range");
				chassisExterminator = false;
				chassisBooster = false;
				chassisShrink = false;
				chassisQuick = false;
				chassisFinal = false;
				break;
		}
	}
}

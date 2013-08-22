using UnityEngine;
using System.Collections;

// This script is part of the tutorial series "Making a 2D game with Unity3D using only free tools"
// http://www.rocket5studios.com/tutorials/make-a-2d-game-in-unity3d-using-only-free-tools-part-1

public class Player : Character {

	// shoot objects
	private Transform shootParent;
	private Renderer shootRenderer;
	private OTAnimatingSprite shootSprite;
	
	private Vector3 spawnPoint;
	
	public override void Start()
    {
		base.Start();
		
		spawnPoint = thisTransform.position; // player will respawn at initial starting point
		
		// connect external objects
		shootParent = transform.Find("shoot parent");
		shootRenderer = GameObject.Find("shoot").renderer;
		shootSprite = GameObject.Find("shoot").GetComponent<OTAnimatingSprite>();
    }
	
	/* ============================== CONTROLS ============================== */
	
	
	public void Update () {
		
		// these are false unless one of keys is pressed
		isLeft = false;
		isRight = false;
		isUp = false;
		isDown = false;
		isShoot = false;

		// keyboard input
		if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) 
		{ isLeft = true; }
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) 
		{ isRight = true; }

		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) 
		{ isUp = true; }
		if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) 
		{ isDown = true; }

		if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.E)) 
		{ isShoot = true; }	
		
		if(Input.GetKey(KeyCode.LeftControl))
		{ 
			reverse = !reverse;
		}
		
		UpdateMovement();
		
		// shoot
		if (isShoot && !shooting && !onRope && !falling && !shotBlockedLeft && !shotBlockedRight) 
		{
			StartCoroutine(Shoot());
		}
	}
	
	
	/* ============================== SHOOT ====================================================================== */
	
	IEnumerator Shoot()
	{
		shooting = true;
		
		// show the shoot sprite and play the animation
		shootRenderer.enabled = true;
		shootSprite.Play("shoot");
		
		// check facing direction and flip the shoot parent to the correct side
		if(facingDir == 1)
		{
			shootParent.localScale = new Vector3(1,1,1); // left side
		}
		if(facingDir == 2)
		{
			shootParent.localScale = new Vector3(-1,1,1); // right side
		}
		
		// fire rate, wait this long before allowing the player to shoot again
		yield return new WaitForSeconds(0.4f);
		
		// hide the sprite
		shootRenderer.enabled = false;
		shooting = false;
	}
	
	/* ============================== DEATH AND RESPAWN ====================================================================== */
	
	void RespawnPlayer()
	{
		// respawn the player at her initial start point
		thisTransform.position = spawnPoint;
		alive = true;
	}
	
	/* ============================== TRIGGER EVENTS ====================================================================== */
	
	void OnTriggerEnter(Collider other)
	{
		// did the player collide with a pickup?
		// pickups and scoring will be added in an upcomming tutorial
		if (other.gameObject.CompareTag("Pickup"))
		{
			if (other.GetComponent<Pickup>())
			{
				other.GetComponent<Pickup>().PickMeUp();
				xa.sc.Pickup(); // tell Scoring.cs that we collected a pickup
			}
		}
	}
	
}

using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {
	
	public bool blockedRight = false;
	public bool blockedLeft = false;
	public bool blockedUp = false;
	public bool blockedDown = false;
	public bool reverse = false;
	
	public bool isLeft;
	public bool isRight;
	public bool isUp;
	public bool isDown;
	public bool isShoot;

	public bool alive;
	public bool onLadder;
	public bool onRope;
	public bool falling;
	public bool shooting;

	public int facingDir = 1; // 1 = left, 2 = right, 3 = up, 4 = down
	
	

	public float playerHitboxX = 0.225f; // player x = 0.45
	public  float playerHitboxY = 0.5f; // 0.5 is correct for ladders while player actual y = 0.6
	public float reverseplayerHitboxY = 0.0f;
	
	public  Vector3 glx;
	
	protected Transform thisTransform;
	
	

	// movement
	public float gravitySpeed = 5;
	public float moveSpeed = 5;
	private int moveDirX;
	private int moveDirY;
	private Vector3 movement;
			
	// raycasts
	private float rayBlockedDistX = 0.6f;
	private RaycastHit hit;
	
	// layer masks	
	//protected int groundMask = 1 << 8; // layer = Ground
	protected int shootMask = 1 << 8 | 1 << 9; // layers = Ground, Ladder
		
	protected bool dropFromRope = false;
	
	protected bool shotBlockedLeft;
	protected bool shotBlockedRight;
	
	protected Vector3 ladderHitbox;
	
	
	
	// Use this for initialization
	
	public virtual void Awake() 
	{
		thisTransform = transform;
	}
	
	public virtual void Start () {
		alive = true;
	}
	
	// Update is called once per frame
	public virtual void UpdateMovement () {
		
		UpdateRaycasts();
		
		moveDirX = 0;
		moveDirY = 0;
		
		// move left
		if(isLeft && !blockedLeft && !shooting) 
		{
			moveDirX = -1;
			facingDir = 1;
		}
		
		// move right
		if(isRight && !blockedRight && !shooting) 
		{
			moveDirX = 1;
			facingDir = 2;
		}
		
		// move up on ladder
		if(isUp && !blockedUp && onLadder)
		{
			moveDirY = 1;
			facingDir = 3;
		}
		
		// move down on ladder
		if(isDown && !blockedDown && onLadder) 
		{
			moveDirY = -1;
			facingDir = 4;
		}
		
		// drop from rope
		if(isDown && onRope) 
		{
			onRope = false;
			dropFromRope = true;
		}
		
		
		// Do the actual movement
		
		
		// player is not falling so move normally
		if(!falling || onLadder) 
		{
			movement = new Vector3(moveDirX, moveDirY,0f);
			movement *= Time.deltaTime*moveSpeed;
			thisTransform.Translate(movement.x,movement.y, 0f);
		}
		
		// player is falling so apply gravity
		else 
		{
			float dir;
			if(reverse){dir = 1f;}
			else{dir = -1f;}
			movement = new Vector3(0f,dir,0f);
			movement *= Time.deltaTime*gravitySpeed;
			thisTransform.Translate(0f,movement.y, 0f);
		}
	}
	
	/* ============================== RAYCASTS ============================== */
	
	void UpdateRaycasts() 
	{
		// set these to false unless a condition below is met
		blockedRight = false;
		blockedLeft = false;
		shotBlockedLeft = false;
		shotBlockedRight = false;
		
		// is the player is standing on the ground?
		// cast 2 rays, one on each side of the character
		if(!reverse){
		if (Physics.Raycast(new Vector3(thisTransform.position.x-0.3f,thisTransform.position.y,thisTransform.position.z+1f), -Vector3.up, out hit, 0.7f, xa.groundMask | xa.enemyMask) || Physics.Raycast(new Vector3(thisTransform.position.x+0.3f,thisTransform.position.y,thisTransform.position.z+1f), -Vector3.up, out hit, 0.7f, xa.groundMask | xa.enemyMask))
		{	
			falling = false;
			
			// snap the player to the top of a ground tile if she's not on a ladder
			if(!onLadder)
			{
				thisTransform.position = new Vector3(thisTransform.position.x, hit.point.y + playerHitboxY, thisTransform.position.z);
			}
				
		}
		
		// then maybe she's falling
		else
		{
			if(!onRope && !falling && !onLadder) {
				falling = true;
			}
		}
		}
		else{
		if (Physics.Raycast(new Vector3(thisTransform.position.x-0.3f,thisTransform.position.y,thisTransform.position.z+1f), Vector3.up, out hit, 0.7f, xa.groundMask | xa.enemyMask) || Physics.Raycast(new Vector3(thisTransform.position.x+0.3f,thisTransform.position.y,thisTransform.position.z+1f), Vector3.up, out hit, 0.7f, xa.groundMask | xa.enemyMask))
		{	
			falling = false;
			
			// snap the player to the top of a ground tile if she's not on a ladder
			if(!onLadder)
			{
				//if(!reverse){
					//thisTransform.position = new Vector3(thisTransform.position.x, hit.point.y + playerHitboxY, thisTransform.position.z);
				//}
				//else{
					//thisTransform.position = new Vector3(thisTransform.position.x, hit.point.y + reverseplayerHitboxY, thisTransform.position.z);
				//}
			}
				
		}
		
		// then maybe she's falling
		else
		{
			if(!onRope && !falling && !onLadder) {
				falling = true;
			}
		}
		}
		
		// player is blocked by something on the right
		// cast out 2 rays, one from the head and one from the feet
		if (Physics.Raycast(new Vector3(thisTransform.position.x,thisTransform.position.y+0.3f,thisTransform.position.z+1f), Vector3.right, rayBlockedDistX, xa.groundMask) || Physics.Raycast(new Vector3(thisTransform.position.x,thisTransform.position.y-0.4f,thisTransform.position.z+1f), Vector3.right, rayBlockedDistX, xa.groundMask))
		{
			blockedRight = true;
		}
		
		// player is blocked by something on the left
		// cast out 2 rays, one from the head and one from the feet
		if (Physics.Raycast(new Vector3(thisTransform.position.x,thisTransform.position.y+0.3f,thisTransform.position.z+1f), -Vector3.right, rayBlockedDistX, xa.groundMask) || Physics.Raycast(new Vector3(thisTransform.position.x,thisTransform.position.y-0.4f,thisTransform.position.z+1f), -Vector3.right, rayBlockedDistX, xa.groundMask))
		{
			blockedLeft = true;
		}
		
		// is there something blocking our shot to the right?
		if (Physics.Raycast(new Vector3(thisTransform.position.x,thisTransform.position.y,thisTransform.position.z+1f), Vector3.right, 1f, shootMask))
		{
			shotBlockedRight = true;
		}
		
		// is there something blocking our shot to the left?
		if (Physics.Raycast(new Vector3(thisTransform.position.x,thisTransform.position.y,thisTransform.position.z+1f), -Vector3.right, 1f, shootMask))
		{
			shotBlockedLeft = true;
		}
		
		// did the shot hit a brick tile to the left?
		if (Physics.Raycast(new Vector3(thisTransform.position.x-1f,thisTransform.position.y,thisTransform.position.z+1f), -Vector3.up, out hit, 0.6f, xa.groundMask))
		{
			if(!shotBlockedLeft && isShoot && facingDir == 1) {
				// breaking bricks will be added in an upcomming tutorial
				/*if (hit.transform.GetComponent<Brick>())
				{
					StartCoroutine(hit.transform.GetComponent<Brick>().PlayBreakAnim());
				}*/
			}
		}
		
		// did the shot hit a brick tile to the right?
		if(Physics.Raycast(new Vector3(thisTransform.position.x+1f,thisTransform.position.y,thisTransform.position.z+1f), -Vector3.up, out hit, 0.6f, xa.groundMask))
		{
			if(!shotBlockedRight && isShoot && facingDir == 2) {
				// breaking bricks will be added in an upcomming tutorial
				/*if (hit.transform.GetComponent<Brick>())
				{
					StartCoroutine(hit.transform.GetComponent<Brick>().PlayBreakAnim());
				}*/
			}
		}
		
		// is the player on the far right edge of the screen?
		if (thisTransform.position.x + playerHitboxX > (Camera.mainCamera.transform.position.x + xa.orthSizeX)) 
		{
			blockedRight = true;
		}
		
		// is the player on the far left edge of the screen?
		if (thisTransform.position.x - playerHitboxX < (Camera.mainCamera.transform.position.x - xa.orthSizeX)) 
		{
			blockedLeft = true;
		}
	}
	
	void OnTriggerStay(Collider other)
	{
		// has the player been crushed by a block?
		// this will be added in an upcomming tutorial
		/*if (other.gameObject.CompareTag("Crusher"))
		{
			if(alive)
			{
				alive = false;
				RespawnPlayer();
				sc.LifeSubtract();
			}
		}*/
		
		// is the player overlapping a ladder?
		if(other.gameObject.CompareTag("Ladder"))
		{
			onLadder = false;
			blockedUp = false;
			blockedDown = false;
			
			ladderHitbox.y = other.transform.localScale.y * 0.5f; // get half the ladders Y height
			
			// is the player overlapping the ladder?
			// if player is landing on top of ladder from a fall, let him pass by
			if ((thisTransform.position.y + playerHitboxY) < ((ladderHitbox.y + 0.1f) + other.transform.position.y))
			{
				onLadder = true;
				falling = false;
			}
			
			// if the player is at the top of the ladder, then snap her to the top
			if ((thisTransform.position.y + playerHitboxY) >= (ladderHitbox.y + other.transform.position.y) && isUp)
			{
				blockedUp = true;
				glx = thisTransform.position;
                glx.y = (ladderHitbox.y + other.transform.position.y) - playerHitboxY;
                thisTransform.position = glx;
			}
			
			// if the player is at the bottom of the ladder, then snap her to the bottom
			if ((thisTransform.position.y - playerHitboxY) <= (-ladderHitbox.y + other.transform.position.y))
			{
				blockedDown = true;
				glx = thisTransform.position;
				glx.y = (-ladderHitbox.y + other.transform.position.y) + playerHitboxY;
                thisTransform.position = glx;
			}
		}
		
		// is the player overlapping a rope?
		if(other.gameObject.CompareTag("Rope"))
		{
			onRope = false;
			
			if(!onRope && !dropFromRope) 
			{
				// snap player to center of the rope
				if (thisTransform.position.y < (other.transform.position.y + 0.2f) && thisTransform.position.y > (other.transform.position.y - 0.2f))
                {
					onRope = true;
					falling = false;
					glx = thisTransform.position;
                    glx.y = other.transform.position.y;
                    thisTransform.position = glx;
                }
			}
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		// did the player exit a rope trigger?
		if (other.gameObject.CompareTag("Rope"))
		{
			onRope = false;
			dropFromRope = false;
		}
		
		// did the player exit a ladder trigger?
		if (other.gameObject.CompareTag("Ladder")) 
		{
			onLadder = false;
		}
	}
}

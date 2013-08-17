using UnityEngine;
using System.Collections;

// This script is part of the tutorial series "Making a 2D game with Unity3D using only free tools"
// http://www.rocket5studios.com/tutorials/make-a-2d-game-in-unity3d-using-only-free-tools-part-1

public class PlayerAnims : MonoBehaviour {
	public enum anim { None, WalkLeft, WalkRight, RopeLeft, RopeRight, Climb, ClimbStop, StandLeft, StandRight, HangLeft, HangRight, FallLeft, FallRight , ShootLeft, ShootRight }

	OTAnimatingSprite mySprite;
	anim currentAnim;
	private Character character;
	// Use this for initialization
	void Start () 
	{
		mySprite = GetComponent<OTAnimatingSprite>();
		character = GetComponent<Character>();
	}
	
	void Update() 
	{
		// run left
		if(character.isLeft && !character.onRope && !character.onLadder && !character.falling && currentAnim != anim.WalkLeft)
		{
			currentAnim = anim.WalkLeft;
			mySprite.Play("runLeft");
		}
		if(!character.isLeft && !character.onRope && !character.falling && currentAnim != anim.StandLeft && character.facingDir == 1)
		{
			currentAnim = anim.StandLeft;
			mySprite.ShowFrame(13); // stand left
		}
		
		// run right
		if(character.isRight && !character.onRope && !character.onLadder && !character.falling && currentAnim != anim.WalkRight)
		{
			currentAnim = anim.WalkRight;
			mySprite.Play("runRight");
		}
		if(!character.isRight && !character.onRope && !character.falling && currentAnim != anim.StandRight && character.facingDir == 2)
		{
			currentAnim = anim.StandRight;
			mySprite.ShowFrame(16); // stand left
		}
		
		// climb
		if(character.isUp && character.onLadder && currentAnim != anim.Climb)
		{
			currentAnim = anim.Climb;
			mySprite.Play("climb");
		}
		if(!character.isUp && character.onLadder && currentAnim != anim.ClimbStop && character.facingDir == 3)
		{
			currentAnim = anim.ClimbStop;
			mySprite.ShowFrame(1); // climb left
		}
		
		if(character.isDown && character.onLadder && currentAnim != anim.Climb)
		{
			currentAnim = anim.Climb;
			mySprite.Play("climb");
		}
		if(!character.isDown && character.onLadder && currentAnim != anim.ClimbStop && character.facingDir == 4)
		{
			currentAnim = anim.ClimbStop;
			mySprite.ShowFrame(1); // climb left
		}
		
		// rope
		if(character.isLeft && character.onRope && currentAnim != anim.RopeLeft)
		{
			currentAnim = anim.RopeLeft;
			mySprite.Play("ropeLeft");
		}
		if(!character.isLeft && character.onRope && currentAnim != anim.HangLeft && character.facingDir == 1)
		{
			currentAnim = anim.HangLeft;
			mySprite.ShowFrame(6); // hang left
		}
		
		if(character.isRight && character.onRope && currentAnim != anim.RopeRight)
		{
			currentAnim = anim.RopeRight;
			mySprite.Play("ropeRight");
		}
		if(!character.isRight && character.onRope && currentAnim != anim.HangRight && character.facingDir == 2)
		{
			currentAnim = anim.HangRight;
			mySprite.ShowFrame(9); // hang right
		}
		
		// falling
		if(character.falling && currentAnim != anim.FallLeft && character.facingDir == 1)
		{
			currentAnim = anim.FallLeft;
			mySprite.ShowFrame(2); // fall left
		}
		if(character.falling && currentAnim != anim.FallRight && character.facingDir == 2)
		{
			currentAnim = anim.FallRight;
			mySprite.ShowFrame(3); // fall right
		}
		
		// shooting
		if(character.shooting && currentAnim != anim.ShootLeft && character.facingDir == 1)
		{
			currentAnim = anim.ShootLeft;
			mySprite.ShowFrame(10); // shoot left
		}
		if(character.shooting && currentAnim != anim.ShootRight && character.facingDir == 2)
		{
			currentAnim = anim.ShootRight;
			mySprite.ShowFrame(11); // shoot right
		}
	}
}

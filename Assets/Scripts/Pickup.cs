using UnityEngine;
using System.Collections;

// This script is part of the tutorial series "Making a 2D game with Unity3D using only free tools"
// http://www.rocket5studios.com/tutorials/make-a-2d-game-in-unity3d-using-only-free-tools-part-1

public class Pickup : MonoBehaviour 
{
	
	private GameObject thisGameObject;
	private Renderer thisRenderer;
	
	void Awake() 
	{
		thisGameObject = gameObject;
		thisRenderer = renderer;
	}
	
	// Called from Player.cs when the player enteres the pickup
	public void PickMeUp()
	{
		thisRenderer.enabled = false; // hide the pickup
		thisGameObject.tag = "Untagged"; // untag the pickup so it won't get triggered again
	}
}

using UnityEngine;
using System.Collections;
using Pathfinding;

// static class courtesy of Michael Todd http://twitter.com/thegamedesigner
// This script is part of the tutorial series "Making a 2D game with Unity3D using only free tools"
// http://www.rocket5studios.com/tutorials/make-a-2d-game-in-unity3d-using-only-free-tools-part-1

public class xa : MonoBehaviour {

	public  LayerMask theGroundMask;
	public  LayerMask theLadderAndRopeMask;
	public  LayerMask theEnemyMask;

	public static Scoring sc; // scoring will be added in an upcomming tutorial

	public static float orthSize;
	public static float orthSizeX;
	public static float orthSizeY;
	public static float camRatio;
	
	public static LayerMask groundMask;
	public static LayerMask ladderAndRopeMask;
	public static LayerMask enemyMask;
	public void Awake()
	{
		groundMask = theGroundMask;
		ladderAndRopeMask = theLadderAndRopeMask;
		enemyMask = theEnemyMask;
	}
	public void Start()
	{
		sc = (Scoring)(this.gameObject.GetComponent("Scoring")); // scoring will be added in an upcomming tutorial

		// gather information from the camera to find the screen size
		xa.camRatio = 1.333f; // 4:3 is 1.333f (800x600) 
		xa.orthSize = Camera.mainCamera.camera.orthographicSize;
		xa.orthSizeX = xa.orthSize * xa.camRatio;
	//	UpdateGridGraph();
	}
/*	public static void UpdateGridGraph()
	{
		GridGraph gridGraph = AstarPath.active.graphs[0] as GridGraph;
		
		MyGUO myGraphUpdateObject = new MyGUO(new Bounds(Vector3.zero, new Vector3(gridGraph.width,gridGraph.depth,0)), groundMask | ladderAndRopeMask);
		AstarPath.active.UpdateGraphs (myGraphUpdateObject);
	}*/
	public static bool IsGroundBetween(Vector3 pos1, Vector3 pos2)
	{
		pos1.z = 0; pos2.z = 0;
		return Physics.Linecast(pos1, pos2, groundMask.value);
	}
	
	public void OnDrawGizmosSelected()
	{
		if(AstarPath.active && AstarPath.active.graphs != null && AstarPath.active.graphs.Length > 0)
		{
			MyGridGraph gg = (AstarPath.active.graphs[0] as MyGridGraph);
			
			if(gg.graphNodes.Length > 0)
			{
				for (int y = gg.depth-1; y >= 0; y--) 
				{
					for (int x = 0; x < gg.width; x++)
					{	
						MyGridNode node = gg.graphNodes[y*gg.width+x] as MyGridNode;
						
						if(node.isTrueWalkable && node.isFallLane)
						{
							Gizmos.color = new Color(1, 1, 0, 0.5F);
							
							Gizmos.DrawSphere((Vector3)node.position, 0.3f);
						}
						
						else if(node.isTrueWalkable)
						{
							Gizmos.color = new Color(0, 1, 0, 0.5F);
							
							Gizmos.DrawSphere((Vector3)node.position, 0.3f);
						}
						
						else if(node.isFallLane)
						{
							Gizmos.color = new Color(0, 0, 1, 0.5F);
							
							Gizmos.DrawSphere((Vector3)node.position,  0.3f);
						}
					}
				}
			}
		}
	}
}
/*
class MyGUO : GraphUpdateObject {
	private LayerMask raycastMask;
 	
	public MyGUO(Bounds b, LayerMask mask) : base(b) {
		raycastMask = mask;
		updatePhysics = false;
		modifyWalkability = true;
	}
	
    public override void Apply (Node node) {
		
        //Keep the base functionality
        base.Apply (node);
		
		// make node non-walkable by default
        node.walkable = false;
		
		RaycastHit hit;
		
		//The position of a node is an Int3, convert it to world coordinate
		Vector3 pos = new Vector3((float)node.position.x / 100f, (float)node.position.y / 100f, (float)node.position.z / 100f);
		pos.z -= 1; // translate backward so we can raycast forward onto a tile
		
		// if hit a tile, check it's tag
		if(Physics.Raycast(pos, Vector3.forward, out hit, 5f, raycastMask))
		{
			if(hit.transform.gameObject.tag == "Ground") // this node is ground. can't travel in it
			{
//				Debug.Log("Found direct ground at " + (node as GridNode).GetIndex());
				node.walkable = false;
			}
			else if(hit.transform.gameObject.tag == "Ladder")
			{
				node.walkable = true;
			}
			else if(hit.transform.gameObject.tag == "Rope")
			{
				node.walkable = true;
			}
		}
		else // didn't hit anything, so must be air.  Check if we can walk on the ground under this row
		{
			// check tile 1 row down.
			pos.y -= 1;
			if(Physics.Raycast(pos, Vector3.forward, out hit, 5f, raycastMask))
			{
				if(hit.transform.gameObject.tag == "Ground")
				{
					node.walkable = true;
				}
			}
			pos.y += 1; // undo
			
		}
    }
}
*/